'use strict';
const $ = id => document.getElementById(id);
const money = amount => new Intl.NumberFormat('en-GB', { style: 'currency', currency: 'GBP' }).format(amount);
const date = (value, withTime = false) => value ? new Intl.DateTimeFormat('en-GB', { day: '2-digit', month: 'short', year: 'numeric', ...(withTime ? { hour: '2-digit', minute: '2-digit' } : {}) }).format(new Date(value)) : '—';
const label = value => ({ CityCar: 'City car', FamilyCar: 'Family car', SportsCar: 'Sports car', SUV: 'SUV', FullElectric: 'Electric' }[value] || value);
const state = {
  staff: null, csrf: '', epoch: 0, quote: null, estimate: null, carId: null, details: null,
  fleetPage: 1, historyPage: 1, fleetStatus: 'all', fleetTotal: 0, historyTotal: 0, catalog: null,
  custCatalog: null, custEstimate: null, custCarId: null, custTab: 'book', custDetails: null
};
const versions = { estimate: 0, fleet: 0, history: 0, overview: 0, details: 0, custEstimate: 0, custFleet: 0 };
let estimateTimer, fleetTimer, historyTimer, custEstimateTimer;
const dialogOpeners = new WeakMap();

function showDialog(id, opener = document.activeElement) {
  const dialog = $(id);
  dialogOpeners.set(dialog, opener);
  if (!dialog.open) dialog.showModal();
}

function element(tag, text, className) {
  const node = document.createElement(tag);
  if (text !== undefined && text !== null) node.textContent = String(text);
  if (className) node.className = className;
  return node;
}

function statusBadge(status) {
  return element('span', status === 'Active' ? 'On hire' : status === 'available' ? 'Available' : status === 'rented' ? 'On hire' : status, 'badge ' + status.toLowerCase());
}

function actionButton(text, callback, kind = '') {
  const button = element('button', text, 'button small ' + kind);
  button.type = 'button';
  button.addEventListener('click', callback);
  return button;
}

function notice(message, kind = '') {
  $('pageNotice').textContent = message;
  $('pageNotice').className = 'notice ' + kind;
  $('pageNotice').hidden = !message;
}

function custNotice(message, kind = '') {
  $('custNotice').textContent = message;
  $('custNotice').className = 'notice ' + kind;
  $('custNotice').hidden = !message;
}

function errorIn(id, error) {
  $(id).textContent = error.message || error;
  $(id).hidden = false;
}

async function api(url, method = 'GET', data) {
  const epoch = state.epoch;
  let response;
  try {
    response = await fetch(url, {
      method,
      credentials: 'same-origin',
      headers: {
        ...(data ? { 'Content-Type': 'application/json' } : {}),
        ...(method !== 'GET' && state.csrf ? { 'X-CSRF-Token': state.csrf } : {})
      },
      body: data ? JSON.stringify(data) : undefined,
      signal: AbortSignal.timeout(35000)
    });
  } catch {
    throw new Error('The server did not respond. Your booking was not confirmed here; retry the same review or check booking history.');
  }
  const result = await response.json();
  if (epoch !== state.epoch && !url.startsWith('/api/public/')) throw new Error('Session changed.');
  if (!response.ok) {
    if (response.status === 401 && url !== '/api/login' && url !== '/api/session' && !url.startsWith('/api/public/')) showLogin('Your session has ended. Sign in again.');
    const error = new Error(result.message || 'The request could not be completed.');
    error.status = response.status;
    throw error;
  }
  return result;
}

function resetBooking() {
  $('bookingForm').reset();
  state.quote = null;
  state.estimate = null;
  state.carId = null;
  versions.estimate++;
  for (const field of ['firstName', 'surname', 'address', 'age', 'licence', 'category', 'fuel', 'days']) {
    $(field + 'Error').textContent = '';
    $(field).removeAttribute('aria-invalid');
  }
  renderEstimate();
}

function showCustomerView() {
  $('appShell').hidden = true;
  $('loginView').hidden = true;
  $('customerView').hidden = false;
  document.title = 'WeAreCars · Car rental Sunderland';
  initCustomerPortal();
}

function showLogin(message = '') {
  state.epoch++;
  state.staff = null;
  state.csrf = '';
  state.details = null;
  state.catalog = null;
  for (const key of Object.keys(versions)) versions[key]++;
  clearTimeout(estimateTimer);
  clearTimeout(fleetTimer);
  clearTimeout(historyTimer);
  for (const dialog of document.querySelectorAll('dialog')) if (dialog.open) dialog.close();
  resetBooking();
  for (const id of ['recentRows', 'overviewFleetRows', 'fleetRows', 'historyRows', 'reviewDetails', 'bookingDetails', 'printArea']) $(id).replaceChildren();
  $('fleetSearch').value = '';
  $('historySearch').value = '';
  $('password').value = '';
  $('password').type = 'password';
  $('togglePassword').textContent = 'Show';
  $('togglePassword').setAttribute('aria-label', 'Show password');
  $('customerView').hidden = true;
  $('appShell').hidden = true;
  $('loginView').hidden = false;
  $('loginError').hidden = !message;
  $('loginError').textContent = message;
  document.title = 'Staff sign-in · WeAreCars';
  notice('');
}

async function startSession(staff) {
  state.staff = staff.username;
  state.csrf = staff.csrf;
  $('staffName').textContent = staff.username;
  $('staffRole').textContent = staff.role === 'admin' ? 'Administrator' : 'Rental desk';
  $('customerView').hidden = true;
  $('loginView').hidden = true;
  $('appShell').hidden = false;
  $('password').value = '';
  const epoch = state.epoch;
  try {
    const catalog = await api('/api/catalog');
    if (epoch !== state.epoch) return;
    state.catalog = catalog;
    for (const [id, options] of [['category', catalog.categories], ['fuel', catalog.fuels]]) {
      const select = $(id);
      const current = select.value;
      while (select.options.length > 1) select.remove(1);
      for (const item of options) {
        const option = element('option', label(item.value) + (item.surcharge ? ' · +' + money(item.surcharge) : ''));
        option.value = item.value;
        select.append(option);
      }
      if (current) select.value = current;
    }
    if ($('category').value && $('fuel').value) scheduleEstimate();
    $('mileageRate').textContent = money(catalog.mileageDaily) + ' per day';
    $('coverRate').textContent = money(catalog.coverDaily) + ' per day';
    await route();
    health();
  } catch (error) {
    if (state.staff) notice(error.message, 'error');
  }
}

async function health() {
  if (!state.staff) return;
  try {
    const h = await api('/api/health');
    $('connectionStatus').textContent = h.database ? (h.mode === 'demo' ? 'Demo · Database connected' : 'Database connected') : 'Database unavailable';
    $('connectionStatus').className = 'connection ' + (h.database ? 'online' : 'offline');
  } catch {
    $('connectionStatus').textContent = 'Connection unavailable';
    $('connectionStatus').className = 'connection offline';
  }
}

async function route() {
  const hash = location.hash.slice(1).split('?')[0];
  if (state.staff) {
    if (hash === 'customer' || hash.startsWith('customer-')) {
      $('appShell').hidden = true;
      $('loginView').hidden = true;
      $('customerView').hidden = false;
      const custTab = hash === 'customer-fleet' ? 'fleet' : hash === 'customer-lookup' ? 'lookup' : 'book';
      switchCustTab(custTab);
      return;
    }
    const name = ['overview', 'booking', 'fleet', 'history'].includes(hash) ? hash : 'overview';
    if (name !== hash) { location.hash = name; return; }
    $('customerView').hidden = true;
    $('loginView').hidden = true;
    $('appShell').hidden = false;
    for (const section of ['overview', 'booking', 'fleet', 'history']) $('view-' + section).hidden = section !== name;
    for (const link of document.querySelectorAll('[data-route]')) {
      if (link.dataset.route === name) link.setAttribute('aria-current', 'page');
      else link.removeAttribute('aria-current');
    }
    $('breadcrumb').textContent = ({ overview: 'Overview', booking: 'New booking', fleet: 'Fleet inventory', history: 'Booking history' })[name];
    document.title = $('breadcrumb').textContent + ' · WeAreCars';
    notice('');
    if (name === 'overview') await loadOverview();
    if (name === 'fleet') await loadFleet();
    if (name === 'history') await loadHistory();
    if (name === 'booking') scheduleEstimate();
    return;
  }
  if (hash === 'staff' || ['overview', 'booking', 'fleet', 'history'].includes(hash)) {
    showLogin();
  } else {
    showCustomerView();
    const custTab = hash === 'customer-fleet' ? 'fleet' : hash === 'customer-lookup' ? 'lookup' : 'book';
    switchCustTab(custTab);
  }
}

function emptyRow(tbody, columns, heading, detail = '') {
  tbody.replaceChildren();
  const row = element('tr'), cell = element('td', null, 'empty');
  cell.colSpan = columns;
  cell.append(element('strong', heading), element('span', detail));
  row.append(cell);
  tbody.append(row);
}
function loadingRow(tbody, columns) { emptyRow(tbody, columns, 'Loading records…'); }
function customerCell(booking) {
  const cell = element('td');
  cell.append(element('div', booking.firstName + ' ' + booking.surname, 'cell-main'));
  return cell;
}

function bookingRow(booking, recent = false) {
  const row = element('tr'), reference = element('td');
  reference.append(element('div', booking.reference, 'cell-main'), element('div', date(booking.createdAt, true), 'cell-sub'));
  row.append(reference, customerCell(booking));
  const vehicle = element('td');
  vehicle.append(element('div', booking.car.registration, 'registration'), element('div', label(booking.car.category) + ' · ' + label(booking.car.fuel), 'cell-sub'));
  row.append(vehicle);
  row.append(element('td', recent ? date(booking.dueAt) : booking.days + (booking.days === 1 ? ' day' : ' days')));
  const status = element('td');
  status.append(statusBadge(booking.status));
  row.append(status, element('td', money(booking.total), 'amount'));
  const actions = element('td', null, 'actions');
  actions.append(actionButton('View', () => openBooking(booking.id)));
  row.append(actions);
  return row;
}

function fleetRow(car) {
  const row = element('tr');
  row.append(element('td', car.registration, 'registration'), element('td', label(car.category)), element('td', label(car.fuel)), element('td', money(car.dailyRate) + ' / day'));
  const status = element('td');
  status.append(statusBadge(car.rented ? 'rented' : 'available'));
  row.append(status);
  const actions = element('td', null, 'actions');
  if (car.rented) actions.append(element('span', 'Manage in booking history', 'hint'));
  else actions.append(actionButton('Book vehicle', () => {
    resetBooking();
    state.carId = car.id;
    $('category').value = car.category;
    $('fuel').value = car.fuel;
    location.hash = 'booking';
    scheduleEstimate();
  }));
  row.append(actions);
  return row;
}

async function loadOverview() {
  const version = ++versions.overview, epoch = state.epoch;
  loadingRow($('recentRows'), 7);
  loadingRow($('overviewFleetRows'), 6);
  try {
    const [stats, history, fleet] = await Promise.all([api('/api/stats'), api('/api/bookings?page=1'), api('/api/fleet?page=1')]);
    if (version !== versions.overview || epoch !== state.epoch || !state.staff) return;
    $('statTotal').textContent = stats.TotalVehicles;
    $('statAvailable').textContent = stats.AvailableVehicles;
    $('statRented').textContent = stats.RentedVehicles;
    $('statBookings').textContent = stats.TotalBookings;
    $('recentRows').replaceChildren(...history.items.slice(0, 5).map(b => bookingRow(b, true)));
    if (!history.items.length) emptyRow($('recentRows'), 7, 'No bookings yet', 'Create a booking to start recording rentals.');
    $('overviewFleetRows').replaceChildren(...fleet.items.slice(0, 5).map(fleetRow));
    if (!fleet.items.length) emptyRow($('overviewFleetRows'), 6, 'No vehicles found');
  } catch (error) {
    if (epoch !== state.epoch || !state.staff) return;
    notice(error.message, 'error');
    for (const id of ['statTotal', 'statAvailable', 'statRented', 'statBookings']) $(id).textContent = '—';
    emptyRow($('recentRows'), 7, 'Records unavailable', 'Check the connection and refresh.');
    emptyRow($('overviewFleetRows'), 6, 'Inventory unavailable');
  }
}

function pagination(kind, total) {
  const page = state[kind + 'Page'], pages = Math.max(1, Math.ceil(total / 25));
  $(kind + 'Count').textContent = total ? `${total} records · Page ${page} of ${pages}` : '0 records';
  $(kind + 'Previous').disabled = page <= 1;
  $(kind + 'Next').disabled = page >= pages;
}

async function loadFleet() {
  const version = ++versions.fleet, epoch = state.epoch;
  loadingRow($('fleetRows'), 6);
  try {
    const result = await api(`/api/fleet?page=${state.fleetPage}&status=${state.fleetStatus}&search=${encodeURIComponent($('fleetSearch').value.trim())}`);
    if (version !== versions.fleet || epoch !== state.epoch || !state.staff) return;
    if (!result.items.length && state.fleetPage > 1) { state.fleetPage = 1; return loadFleet(); }
    state.fleetTotal = result.total;
    $('fleetRows').replaceChildren(...result.items.map(fleetRow));
    if (!result.items.length) emptyRow($('fleetRows'), 6, 'No matching vehicles', 'Try another search or rental-status filter.');
    pagination('fleet', result.total);
  } catch (error) {
    if (epoch !== state.epoch || !state.staff) return;
    emptyRow($('fleetRows'), 6, 'Inventory unavailable');
    notice(error.message, 'error');
  }
}

async function loadHistory() {
  const version = ++versions.history, epoch = state.epoch;
  loadingRow($('historyRows'), 7);
  try {
    const result = await api(`/api/bookings?page=${state.historyPage}&search=${encodeURIComponent($('historySearch').value.trim())}&sort=${$('historySort').value}`);
    if (version !== versions.history || epoch !== state.epoch || !state.staff) return;
    if (!result.items.length && state.historyPage > 1) { state.historyPage = 1; return loadHistory(); }
    state.historyTotal = result.total;
    $('historyRows').replaceChildren(...result.items.map(b => bookingRow(b)));
    if (!result.items.length) emptyRow($('historyRows'), 7, $('historySearch').value ? 'No matching bookings' : 'No booking records yet', 'Recorded bookings will appear here.');
    pagination('history', result.total);
    $('historyUpdated').textContent = 'Updated ' + new Intl.DateTimeFormat('en-GB', { hour: '2-digit', minute: '2-digit' }).format(new Date());
  } catch (error) {
    if (epoch !== state.epoch || !state.staff) return;
    emptyRow($('historyRows'), 7, 'Records unavailable');
    $('historyUpdated').textContent = '';
    notice(error.message, 'error');
  }
}

function options() { return { category: $('category').value, fuel: $('fuel').value, days: Number($('days').value), mileage: $('mileage').checked, cover: $('cover').checked, ...(state.carId ? { carId: state.carId } : {}) }; }
function customer() { return { firstName: $('firstName').value.trim(), surname: $('surname').value.trim(), address: $('address').value.trim(), age: Number($('age').value), licence: $('licence').checked }; }

function fieldMessage(id) {
  const node = $(id), value = node.value.trim();
  if (['firstName', 'surname', 'address'].includes(id)) return !value ? 'This field is required.' : value.length > (id === 'address' ? 150 : 50) ? 'This value is too long.' : '';
  if (id === 'age') return !value || !Number.isInteger(Number(value)) || Number(value) < 18 || Number(value) > 120 ? 'Enter a whole-number age between 18 and 120.' : '';
  if (id === 'days') return !value || !Number.isInteger(Number(value)) || Number(value) < 1 || Number(value) > 28 ? 'Enter 1 to 28 whole days.' : '';
  if (id === 'licence') return !node.checked ? 'Confirm that the driving licence has been checked.' : '';
  return !value ? 'Choose an option.' : '';
}

const fields = ['firstName', 'surname', 'address', 'age', 'licence', 'category', 'fuel', 'days'];
function validate(show = false) {
  let valid = true, firstInvalid;
  for (const id of fields) {
    const message = fieldMessage(id);
    if (message) { valid = false; firstInvalid ||= id; }
    if (show) {
      $(id + 'Error').textContent = message;
      if (message) $(id).setAttribute('aria-invalid', 'true');
      else $(id).removeAttribute('aria-invalid');
    }
  }
  if (show && firstInvalid) $(firstInvalid).focus();
  return valid;
}

function renderEstimate() {
  const estimate = state.estimate;
  for (const [id, key] of [['baseAmount', 'baseTotal'], ['categoryAmount', 'vehicleFee'], ['fuelAmount', 'fuelFee'], ['mileageAmount', 'mileageTotal'], ['coverAmount', 'coverTotal'], ['estimateTotal', 'total']]) {
    $(id).textContent = estimate?.pricing ? money(estimate.pricing[key]) : '—';
  }
  $('baseLabel').textContent = estimate?.pricing ? `${estimate.pricing.days} ${estimate.pricing.days === 1 ? 'day' : 'days'} × ${money(estimate.pricing.dailyRate)}` : 'Base rental';
  $('allocationText').textContent = estimate?.available ? `${estimate.car.registration} · ${label(estimate.car.category)} · ${label(estimate.car.fuel)}` : estimate ? 'No vehicle available for this selection.' : 'Select a category and fuel type.';
  $('reviewButton').disabled = !estimate?.available || !validate();
}

function scheduleEstimate() {
  clearTimeout(estimateTimer);
  state.quote = null;
  state.estimate = null;
  versions.estimate++;
  renderEstimate();
  const selection = options();
  if (!selection.category || !selection.fuel || fieldMessage('days')) return;
  $('allocationText').textContent = 'Checking availability…';
  const version = versions.estimate, epoch = state.epoch;
  estimateTimer = setTimeout(async () => {
    try {
      const endpoint = state.staff ? '/api/estimate' : '/api/public/estimate';
      const result = await api(endpoint, 'POST', selection);
      if (version !== versions.estimate || epoch !== state.epoch) return;
      state.estimate = result;
      renderEstimate();
    } catch (error) {
      if (version !== versions.estimate || epoch !== state.epoch) return;
      $('allocationText').textContent = error.message;
    }
  }, 100);
}

function detailsFragment(record, confirmation = false) {
  const fragment = document.createDocumentFragment(), grid = element('dl', null, 'detail-grid');
  const values = [
    ['Customer', record.firstName + ' ' + record.surname],
    ['Age / licence', record.anonymized ? 'Removed under retention policy' : record.age + ' · Licence checked'],
    ['Address', record.address, true],
    ['Vehicle', record.car.registration + ' · ' + label(record.car.category)],
    ['Fuel type', label(record.car.fuel)],
    ['Rental duration', record.days + (record.days === 1 ? ' day' : ' days')],
    ['Optional extras', [record.mileage ? 'Unlimited mileage' : '', record.cover ? 'Breakdown cover' : ''].filter(Boolean).join(', ') || 'None']
  ];
  if (confirmation) {
    values.push(
      ['Status', record.status],
      ['Recorded by', record.staff],
      ['Rental starts', date(record.startsAt, true)],
      ['Return due', date(record.dueAt, true)],
      ['Returned', record.returnedAt ? date(record.returnedAt, true) + ' · ' + record.returnedBy : 'Not recorded']
    );
  }
  for (const [name, value, wide] of values) {
    const item = element('div', null, wide ? 'wide' : '');
    item.append(element('dt', name), element('dd', value));
    grid.append(item);
  }
  fragment.append(grid, element('hr', null, 'divider'));
  for (const [name, value] of [
    [`${record.days} days × ${money(record.pricing.dailyRate)}`, record.pricing.baseTotal],
    ['Category surcharge', record.pricing.vehicleFee],
    ['Fuel surcharge', record.pricing.fuelFee],
    ['Unlimited mileage', record.pricing.mileageTotal],
    ['Breakdown cover', record.pricing.coverTotal]
  ]) {
    const line = element('div', null, 'quote-line');
    line.append(element('span', name), element('strong', money(value)));
    fragment.append(line);
  }
  const total = element('div', null, 'review-total');
  total.append(element('span', 'Rental total'), element('strong', money(confirmation ? record.total : record.pricing.total)));
  fragment.append(total);
  fragment.append(element('p', record.anonymized ? 'Customer information has been removed under the retention policy. Financial records are retained.' : record.legacySnapshot ? 'Legacy record: the original stored total is retained. Historic item rates were not fully recorded.' : 'Reservation confirmation. Payment collection occurs upon vehicle collection in branch.', 'dialog-note'));
  return fragment;
}

async function openBooking(id, confirmed) {
  const version = ++versions.details, epoch = state.epoch;
  $('detailsError').hidden = true;
  $('bookingDetails').replaceChildren(element('p', 'Loading booking…', 'loading'));
  $('returnBooking').hidden = true;
  $('printBooking').disabled = true;
  if (!$('detailsDialog').open) showDialog('detailsDialog');
  try {
    const booking = confirmed || await api('/api/bookings/' + id);
    if (version !== versions.details || epoch !== state.epoch || !state.staff) return;
    state.details = booking;
    $('detailsTitle').textContent = booking.reference;
    $('detailsSubtitle').textContent = 'Recorded ' + date(booking.createdAt, true);
    $('bookingDetails').replaceChildren(detailsFragment(booking, true));
    $('returnBooking').hidden = booking.status !== 'Active';
    $('printBooking').disabled = false;
  } catch (error) {
    if (epoch === state.epoch && state.staff) {
      state.details = null;
      $('bookingDetails').replaceChildren();
      errorIn('detailsError', error);
    }
  }
}

function printDetails() {
  const b = state.details;
  if (!b) return;
  const area = $('printArea');
  area.replaceChildren(element('h1', 'WeAreCars — Booking confirmation'), element('p', 'Sunderland branch · ' + b.reference));
  const list = element('dl');
  for (const [key, value] of [
    ['Customer', b.firstName + ' ' + b.surname],
    ['Address', b.address],
    ['Age / licence', b.anonymized ? 'Removed under retention policy' : b.age + ' / Confirmed valid licence'],
    ['Vehicle', b.car.registration + ' · ' + label(b.car.category) + ' · ' + label(b.car.fuel)],
    ['Rental period', date(b.startsAt, true) + ' to ' + date(b.dueAt, true)],
    ['Extras', [b.mileage ? 'Unlimited mileage' : '', b.cover ? 'Breakdown cover' : ''].filter(Boolean).join(', ') || 'None'],
    ['Recorded by', b.staff],
    ['Status', b.status]
  ]) {
    list.append(element('dt', key), element('dd', value));
  }
  area.append(list, element('div', 'Rental total: ' + money(b.total), 'total'), element('p', 'This document confirms a reservation. Payment collection occurs on vehicle pickup.'));
  window.print();
}

/* ----------------------------------------------------
   CUSTOMER SELF-SERVICE LOGIC
---------------------------------------------------- */
function switchCustTab(tab) {
  state.custTab = tab;
  for (const name of ['book', 'fleet', 'lookup']) {
    const section = $('cust-section-' + name);
    if (section) section.hidden = name !== tab;
    const btn = $('custNav' + name.charAt(0).toUpperCase() + name.slice(1));
    if (btn) {
      if (name === tab) btn.classList.add('active');
      else btn.classList.remove('active');
    }
  }
  if (tab === 'fleet') loadCustFleet();
}

async function initCustomerPortal() {
  if (state.custCatalog) return;
  try {
    const catalog = await api('/api/public/catalog');
    state.custCatalog = catalog;
    for (const [id, options] of [['custCategory', catalog.categories], ['custFuel', catalog.fuels]]) {
      const select = $(id);
      const current = select.value;
      while (select.options.length > 1) select.remove(1);
      for (const item of options) {
        const option = element('option', label(item.value) + (item.surcharge ? ' · +' + money(item.surcharge) : ''));
        option.value = item.value;
        select.append(option);
      }
      if (current) select.value = current;
    }
    if ($('custCategory').value && $('custFuel').value) scheduleCustEstimate();
    $('custMileageRate').textContent = '+' + money(catalog.mileageDaily) + ' / day';
    $('custCoverRate').textContent = '+' + money(catalog.coverDaily) + ' / day';
  } catch (error) {
    custNotice('Unable to load rental catalog: ' + error.message, 'error');
  }
}

function custOptions() {
  return {
    category: $('custCategory').value,
    fuel: $('custFuel').value,
    days: Number($('custDays').value),
    mileage: $('custMileage').checked,
    cover: $('custCover').checked,
    ...(state.custCarId ? { carId: state.custCarId } : {})
  };
}

function custCustomer() {
  return {
    firstName: $('custFirstName').value.trim(),
    surname: $('custSurname').value.trim(),
    address: $('custAddress').value.trim(),
    age: Number($('custAge').value),
    licence: $('custLicence').checked
  };
}

const custFields = ['custFirstName', 'custSurname', 'custAddress', 'custAge', 'custLicence', 'custCategory', 'custFuel', 'custDays'];
function custFieldMessage(id) {
  const node = $(id), value = node.value ? node.value.trim() : '';
  if (['custFirstName', 'custSurname', 'custAddress'].includes(id)) {
    return !value ? 'This field is required.' : value.length > (id === 'custAddress' ? 150 : 50) ? 'This value is too long.' : '';
  }
  if (id === 'custAge') return !value || !Number.isInteger(Number(value)) || Number(value) < 18 || Number(value) > 120 ? 'Enter a whole-number age between 18 and 120.' : '';
  if (id === 'custDays') return !value || !Number.isInteger(Number(value)) || Number(value) < 1 || Number(value) > 28 ? 'Enter 1 to 28 whole days.' : '';
  if (id === 'custLicence') return !node.checked ? 'Confirm that you hold a valid driving licence.' : '';
  return !value ? 'Choose an option.' : '';
}

function validateCust(show = false) {
  let valid = true, firstInvalid;
  for (const id of custFields) {
    const message = custFieldMessage(id);
    if (message) { valid = false; firstInvalid ||= id; }
    if (show) {
      const err = $(id + 'Error');
      if (err) {
        err.textContent = message;
        if (message) $(id).setAttribute('aria-invalid', 'true');
        else $(id).removeAttribute('aria-invalid');
      }
    }
  }
  if (show && firstInvalid) $(firstInvalid).focus();
  return valid;
}

function renderCustEstimate() {
  const estimate = state.custEstimate;
  for (const [id, key] of [
    ['custBaseAmount', 'baseTotal'],
    ['custCategoryAmount', 'vehicleFee'],
    ['custFuelAmount', 'fuelFee'],
    ['custMileageAmount', 'mileageTotal'],
    ['custCoverAmount', 'coverTotal'],
    ['custEstimateTotal', 'total']
  ]) {
    $(id).textContent = estimate?.pricing ? money(estimate.pricing[key]) : '—';
  }
  $('custBaseLabel').textContent = estimate?.pricing ? `${estimate.pricing.days} ${estimate.pricing.days === 1 ? 'day' : 'days'} × ${money(estimate.pricing.dailyRate)}` : 'Base rental';
  $('custAllocationText').textContent = estimate?.available ? `${estimate.car.registration} · ${label(estimate.car.category)} · ${label(estimate.car.fuel)}` : estimate ? 'No vehicle available for this selection.' : 'Select a category and fuel type.';
  $('custBookButton').disabled = !estimate?.available || !validateCust();
}

function scheduleCustEstimate() {
  clearTimeout(custEstimateTimer);
  state.custQuote = null;
  state.custEstimate = null;
  versions.custEstimate++;
  renderCustEstimate();
  const selection = custOptions();
  if (!selection.category || !selection.fuel || custFieldMessage('custDays')) return;
  $('custAllocationText').textContent = 'Checking availability…';
  const version = versions.custEstimate;
  custEstimateTimer = setTimeout(async () => {
    try {
      const result = await api('/api/public/estimate', 'POST', selection);
      if (version !== versions.custEstimate) return;
      state.custEstimate = result;
      renderCustEstimate();
    } catch (error) {
      if (version !== versions.custEstimate) return;
      $('custAllocationText').textContent = error.message;
    }
  }, 100);
}

async function loadCustFleet() {
  const version = ++versions.custFleet;
  const grid = $('custFleetGrid');
  grid.replaceChildren(element('p', 'Loading fleet…', 'loading'));
  try {
    const result = await api('/api/public/fleet');
    if (version !== versions.custFleet) return;
    grid.replaceChildren();
    if (!result.items.length) {
      grid.append(element('p', 'No vehicles currently available for reservation. Please check back shortly.', 'hint'));
      return;
    }
    for (const car of result.items) {
      const card = element('div', null, 'fleet-card');
      const head = element('div', null, 'fleet-card-head');
      head.append(element('h3', label(car.category), 'fleet-card-title'), element('span', car.registration, 'registration'));
      const meta = element('div', null, 'fleet-card-meta');
      meta.append(element('span', label(car.fuel), 'fleet-pill'), element('span', 'Sunderland branch', 'fleet-pill'));
      const foot = element('div', null, 'fleet-card-foot');
      const priceBox = element('div');
      priceBox.append(element('div', money(car.dailyRate), 'rate-num'), element('span', 'per day base rate', 'rate-unit'));
      const bookBtn = actionButton('Book this car', () => {
        state.custCarId = car.id;
        $('custCategory').value = car.category;
        $('custFuel').value = car.fuel;
        switchCustTab('book');
        scheduleCustEstimate();
      }, 'primary');
      foot.append(priceBox, bookBtn);
      card.append(head, meta, foot);
      grid.append(card);
    }
  } catch (error) {
    grid.replaceChildren(element('p', 'Unable to load fleet: ' + error.message, 'hint'));
  }
}

function voucherFragment(b) {
  const frag = document.createDocumentFragment();
  const card = element('div', null, 'voucher-card');
  const head = element('div', null, 'voucher-head');
  head.append(element('div', b.reference, 'voucher-ref'), statusBadge(b.status));
  const dl = element('dl', null, 'detail-grid');
  for (const [k, v, wide] of [
    ['Driver', b.firstName + ' ' + b.surname],
    ['Pickup Location', 'Sunderland Branch Desk'],
    ['Vehicle', b.car.registration + ' (' + label(b.car.category) + ' · ' + label(b.car.fuel) + ')'],
    ['Rental Duration', b.days + (b.days === 1 ? ' day' : ' days')],
    ['Collection Time', date(b.startsAt, true)],
    ['Return Due', date(b.dueAt, true)],
    ['Extras Included', [b.mileage ? 'Unlimited mileage' : '', b.cover ? 'Breakdown cover' : ''].filter(Boolean).join(', ') || 'None'],
    ['Rental Total', money(b.total)]
  ]) {
    const item = element('div', null, wide ? 'wide' : '');
    item.append(element('dt', k), element('dd', v));
    dl.append(item);
  }
  card.append(head, dl);
  frag.append(card);
  frag.append(element('p', 'Please present your driving licence and proof of address when collecting your vehicle at the Sunderland desk.', 'dialog-note'));
  return frag;
}

function printCustDetails() {
  const b = state.custDetails;
  if (!b) return;
  const area = $('printArea');
  area.replaceChildren(element('h1', 'WeAreCars — Customer Rental Voucher'), element('p', 'Sunderland branch · Reference: ' + b.reference));
  const list = element('dl');
  for (const [key, value] of [
    ['Driver', b.firstName + ' ' + b.surname],
    ['Address', b.address],
    ['Vehicle', b.car.registration + ' · ' + label(b.car.category) + ' · ' + label(b.car.fuel)],
    ['Rental period', date(b.startsAt, true) + ' to ' + date(b.dueAt, true)],
    ['Extras', [b.mileage ? 'Unlimited mileage' : '', b.cover ? 'Breakdown cover' : ''].filter(Boolean).join(', ') || 'None'],
    ['Status', b.status]
  ]) {
    list.append(element('dt', key), element('dd', value));
  }
  area.append(list, element('div', 'Total to pay on collection: ' + money(b.total), 'total'), element('p', 'Thank you for choosing WeAreCars.'));
  window.print();
}

/* ----------------------------------------------------
   EVENT LISTENERS & BINDINGS
---------------------------------------------------- */
// Customer navigation tabs
for (const tab of ['book', 'fleet', 'lookup']) {
  const btn = $('custNav' + tab.charAt(0).toUpperCase() + tab.slice(1));
  if (btn) btn.addEventListener('click', () => switchCustTab(tab));
}
$('custRefreshFleet').addEventListener('click', loadCustFleet);

// Customer form change & validation
const custOptionsFields = ['custCategory', 'custFuel', 'custDays', 'custMileage', 'custCover'];
for (const id of custOptionsFields) {
  const node = $(id);
  if (!node) continue;
  const handler = () => {
    if (id === 'custCategory' || id === 'custFuel') state.custCarId = null;
    scheduleCustEstimate();
  };
  node.addEventListener('input', handler);
  node.addEventListener('change', handler);
  node.addEventListener('blur', () => {
    const msg = custFieldMessage(id);
    const err = $(id + 'Error');
    if (err) {
      err.textContent = msg;
      if (msg) node.setAttribute('aria-invalid', 'true');
      else node.removeAttribute('aria-invalid');
    }
  });
}

const custDriverFields = ['custFirstName', 'custSurname', 'custAddress', 'custAge', 'custLicence'];
for (const id of custDriverFields) {
  const node = $(id);
  if (!node) continue;
  const handler = () => {
    state.custQuote = null;
    const msg = custFieldMessage(id);
    const err = $(id + 'Error');
    if (!msg && err) {
      err.textContent = '';
      node.removeAttribute('aria-invalid');
    }
    renderCustEstimate();
  };
  node.addEventListener('input', handler);
  node.addEventListener('change', handler);
  node.addEventListener('blur', () => {
    const msg = custFieldMessage(id);
    const err = $(id + 'Error');
    if (err) {
      err.textContent = msg;
      if (msg) node.setAttribute('aria-invalid', 'true');
      else node.removeAttribute('aria-invalid');
    }
  });
}

// Customer booking submission
$('custBookingForm').addEventListener('submit', async event => {
  event.preventDefault();
  if (!validateCust(true) || !state.custEstimate?.available) return;
  const btn = $('custBookButton');
  btn.disabled = true;
  btn.textContent = 'Securing quote…';
  custNotice('');
  try {
    const draft = { ...custOptions(), ...custCustomer() };
    const quote = await api('/api/public/quotes', 'POST', draft);
    btn.textContent = 'Confirming reservation…';
    const booking = await api('/api/public/bookings', 'POST', { token: quote.token });
    state.custDetails = booking;
    $('custVoucherDetails').replaceChildren(voucherFragment(booking));
    showDialog('custVoucherDialog', btn);
    $('custBookingForm').reset();
    state.custCarId = null;
    state.custEstimate = null;
    state.custQuote = null;
    renderCustEstimate();
    custNotice('Booking ' + booking.reference + ' confirmed! A voucher dialog has opened.', 'success');
  } catch (error) {
    custNotice(error.message, 'error');
  } finally {
    btn.textContent = 'Confirm reservation';
    renderCustEstimate();
  }
});

// Customer booking lookup
$('custLookupForm').addEventListener('submit', async event => {
  event.preventDefault();
  const ref = $('custLookupRef').value.trim();
  const surname = $('custLookupSurname').value.trim();
  const btn = $('custLookupSubmit');
  $('custLookupError').hidden = true;
  $('custLookupResult').hidden = true;
  if (!ref || !surname) return;
  btn.disabled = true;
  btn.textContent = 'Searching…';
  try {
    const booking = await api('/api/public/lookup', 'POST', { reference: ref, surname });
    state.custDetails = booking;
    const frag = voucherFragment(booking);
    const printBtn = actionButton('Print confirmation voucher', () => printCustDetails(), 'primary');
    frag.append(element('div', null, 'form-actions'));
    frag.lastChild.append(printBtn);
    $('custLookupResult').replaceChildren(frag);
    $('custLookupResult').hidden = false;
  } catch (error) {
    errorIn('custLookupError', error);
  } finally {
    btn.disabled = false;
    btn.textContent = 'Search reservation';
  }
});

$('custPrintVoucher').addEventListener('click', printCustDetails);

// Staff Login Form
$('loginForm').addEventListener('submit', async event => {
  event.preventDefault();
  $('loginError').hidden = true;
  $('loginButton').disabled = true;
  $('loginButton').textContent = 'Signing in…';
  try {
    const staff = await api('/api/login', 'POST', { username: $('username').value.trim(), password: $('password').value });
    await startSession(staff);
  } catch (error) {
    errorIn('loginError', error);
    $('password').value = '';
    $('password').focus();
  } finally {
    $('loginButton').disabled = false;
    $('loginButton').textContent = 'Sign in';
  }
});

$('togglePassword').addEventListener('click', () => {
  const visible = $('password').type === 'password';
  $('password').type = visible ? 'text' : 'password';
  $('togglePassword').textContent = visible ? 'Hide' : 'Show';
  $('togglePassword').setAttribute('aria-label', visible ? 'Hide password' : 'Show password');
});

$('signOut').addEventListener('click', async () => {
  $('signOut').disabled = true;
  try {
    await api('/api/logout', 'POST', {});
    showLogin();
    location.hash = 'staff';
  } catch (error) {
    if (state.staff) notice('Sign-out could not be confirmed. Retry when the server is available.', 'error');
  } finally {
    $('signOut').disabled = false;
  }
});

window.addEventListener('hashchange', () => {
  route();
  const activeMain = $('customerView').hidden ? $('main') : $('custMain');
  if (activeMain) activeMain.focus();
});

for (const id of fields) {
  $(id).addEventListener('blur', () => {
    const message = fieldMessage(id);
    $(id + 'Error').textContent = message;
    if (message) $(id).setAttribute('aria-invalid', 'true');
    else $(id).removeAttribute('aria-invalid');
  });
}
for (const id of ['firstName', 'surname', 'address', 'age', 'licence']) {
  for (const eventName of ['input', 'change']) {
    $(id).addEventListener(eventName, () => {
      state.quote = null;
      if (!fieldMessage(id)) {
        $(id + 'Error').textContent = '';
        $(id).removeAttribute('aria-invalid');
      }
      renderEstimate();
    });
  }
}
for (const id of ['category', 'fuel', 'days', 'mileage', 'cover']) {
  for (const eventName of ['input', 'change']) {
    $(id).addEventListener(eventName, () => {
      if (id === 'category' || id === 'fuel') state.carId = null;
      scheduleEstimate();
    });
  }
}
$('clearBooking').addEventListener('click', () => {
  if (confirm('Clear the entered customer and rental details?')) resetBooking();
});

$('bookingForm').addEventListener('submit', async event => {
  event.preventDefault();
  if (!validate(true) || !state.estimate?.available) return;
  const epoch = state.epoch, draft = { ...options(), ...customer() };
  $('reviewButton').disabled = true;
  $('reviewButton').textContent = 'Preparing review…';
  try {
    const quote = await api('/api/quotes', 'POST', draft);
    if (epoch !== state.epoch || !state.staff) return;
    state.quote = quote;
    $('reviewError').hidden = true;
    $('reviewDetails').replaceChildren(detailsFragment(quote));
    showDialog('reviewDialog', $('reviewButton'));
  } catch (error) {
    if (state.staff) notice(error.message, 'error');
  } finally {
    $('reviewButton').textContent = 'Review booking';
    renderEstimate();
  }
});

$('confirmBooking').addEventListener('click', async () => {
  if (!state.quote) return;
  const token = state.quote.token, epoch = state.epoch;
  $('confirmBooking').disabled = true;
  $('confirmBooking').textContent = 'Saving booking…';
  $('reviewError').hidden = true;
  try {
    const booking = await api('/api/bookings', 'POST', { token });
    if (epoch !== state.epoch || !state.staff) return;
    $('reviewDialog').close();
    resetBooking();
    notice('Booking ' + booking.reference + ' has been recorded.', 'success');
    await openBooking(booking.id, booking);
  } catch (error) {
    if (epoch === state.epoch && state.staff) errorIn('reviewError', error);
  } finally {
    $('confirmBooking').disabled = false;
    $('confirmBooking').textContent = 'Confirm booking';
  }
});

for (const button of document.querySelectorAll('[data-close]')) {
  button.addEventListener('click', () => {
    if (button.dataset.close === 'reviewDialog' && $('confirmBooking').disabled) return;
    if (button.dataset.close === 'returnDialog' && $('confirmReturn').disabled) return;
    $(button.dataset.close).close();
  });
}

for (const dialog of document.querySelectorAll('dialog')) {
  dialog.addEventListener('keydown', event => {
    if (event.key !== 'Tab') return;
    const controls = [...dialog.querySelectorAll('button, a[href], input, select, textarea, [tabindex]')].filter(node => !node.disabled && node.tabIndex >= 0 && node.getClientRects().length);
    if (!controls.length) { event.preventDefault(); dialog.focus(); return; }
    const first = controls[0], last = controls[controls.length - 1];
    if (event.shiftKey && (document.activeElement === first || !dialog.contains(document.activeElement))) { event.preventDefault(); last.focus(); }
    else if (!event.shiftKey && (document.activeElement === last || !dialog.contains(document.activeElement))) { event.preventDefault(); first.focus(); }
  });
  dialog.addEventListener('close', () => {
    const opener = dialogOpeners.get(dialog);
    if (opener?.isConnected && !opener.disabled && opener.getClientRects().length) opener.focus();
    else if (state.staff) $('main').focus({ preventScroll: true });
    else $('custMain').focus({ preventScroll: true });
  });
}

$('reviewDialog').addEventListener('cancel', event => { if ($('confirmBooking').disabled) event.preventDefault(); });
$('reviewDialog').addEventListener('close', () => { if (!$('confirmBooking').disabled) state.quote = null; });
$('detailsDialog').addEventListener('close', () => {
  versions.details++;
  state.details = null;
  $('bookingDetails').replaceChildren();
  $('printArea').replaceChildren();
});
$('printBooking').addEventListener('click', printDetails);

$('returnBooking').addEventListener('click', () => {
  if (!state.details) return;
  $('returnDescription').textContent = `Confirm ${state.details.car.registration} has been returned for ${state.details.reference}.`;
  $('returnError').hidden = true;
  showDialog('returnDialog');
});

$('confirmReturn').addEventListener('click', async () => {
  if (!state.details) return;
  const id = state.details.id, epoch = state.epoch;
  $('confirmReturn').disabled = true;
  try {
    await api(`/api/bookings/${id}/return`, 'POST', {});
    if (epoch !== state.epoch || !state.staff) return;
    $('returnDialog').close();
    await openBooking(id);
    notice('Vehicle returned. Fleet availability has been updated.', 'success');
  } catch (error) {
    if (epoch === state.epoch && state.staff) errorIn('returnError', error);
  } finally {
    $('confirmReturn').disabled = false;
  }
});

for (const button of document.querySelectorAll('[data-status]')) {
  button.addEventListener('click', () => {
    state.fleetStatus = button.dataset.status;
    state.fleetPage = 1;
    for (const tab of document.querySelectorAll('[data-status]')) tab.setAttribute('aria-pressed', String(tab === button));
    loadFleet();
  });
}

$('fleetSearch').addEventListener('input', () => {
  clearTimeout(fleetTimer);
  state.fleetPage = 1;
  fleetTimer = setTimeout(loadFleet, 250);
});

$('historySearch').addEventListener('input', () => {
  clearTimeout(historyTimer);
  state.historyPage = 1;
  historyTimer = setTimeout(loadHistory, 250);
});

$('historySort').addEventListener('change', () => {
  state.historyPage = 1;
  loadHistory();
});

$('refreshFleet').addEventListener('click', loadFleet);
$('refreshHistory').addEventListener('click', loadHistory);

for (const kind of ['fleet', 'history']) {
  for (const [direction, change] of [['Previous', -1], ['Next', 1]]) {
    $(kind + direction).addEventListener('click', () => {
      state[kind + 'Page'] = Math.max(1, state[kind + 'Page'] + change);
      (kind === 'fleet' ? loadFleet : loadHistory)();
    });
  }
}

$('today').textContent = date(new Date().toISOString());

// Startup routing
if (location.hash === '#staff' || ['#overview', '#booking', '#fleet', '#history'].includes(location.hash)) {
  showLogin();
} else {
  showCustomerView();
}
api('/api/session').then(startSession).catch(() => {
  $('loginError').hidden = true;
  if (location.hash === '#staff' || ['#overview', '#booking', '#fleet', '#history'].includes(location.hash)) {
    showLogin();
  }
});
setInterval(() => { if (state.staff && document.visibilityState === 'visible') health(); }, 60000);
