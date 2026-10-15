const fs = require('fs');
const vm = require('vm');
const path = require('path');
const html = fs.readFileSync(path.resolve(__dirname, '../../web_preview/index.html'), 'utf8');
const script = html.match(/<script>([\s\S]*?)<\/script>/)[1];
const results = [];
function fresh() {
  const nodes = new Map();
  function node(attrs = '') {
    const classes = new Set((attrs.match(/class="([^"]*)"/)?.[1] || '').split(/\s+/).filter(Boolean));
    return { value: attrs.match(/value="([^"]*)"/)?.[1] || '', checked: /\bchecked\b/.test(attrs),
      type: attrs.match(/type="([^"]*)"/)?.[1] || '', style: {}, disabled: false, innerText: '', innerHTML: '', children: [],
      classList: { add: x => classes.add(x), remove: x => classes.delete(x), contains: x => classes.has(x) },
      appendChild(x) { this.children.push(x); }, classes };
  }
  for (const match of html.matchAll(/<[a-z][^>]*\bid="([^"]+)"[^>]*>/gi)) nodes.set(match[1], node(match[0]));
  const document = { getElementById(id) { if (!nodes.has(id)) throw Error('Missing element: ' + id); return nodes.get(id); },
    createElement() { return node(); }, querySelectorAll(selector) {
      const cls = selector.split(' ').at(-1).replace(/^\./, ''); return [...nodes.values()].filter(x => x.classes.has(cls));
    } };
  const context = vm.createContext({ document, window: {}, console, alerts: [], alert(x) { this.alerts?.push(x); } });
  vm.runInContext(script, context, { filename: 'web_preview/index.html' });
  const run = s => vm.runInContext(s, context);
  return { nodes, context, run, fill(values) { for (const [id, value] of Object.entries(values)) nodes.get(id).value = value; } };
}
function probe(name, test) { try { results.push({ name, ...test(fresh()) }); } catch (e) { results.push({ name, error: e.message }); } }
function valid(a) { a.fill({ inpFName: 'Alice', inpSName: 'Jones', inpAddress: '17 High Street', inpAge: '26', selLic: 'yes', inpDaysVal: '3' }); }
probe('Initial authentication and reset on reload', a => {
  const initial = a.run('isStaffAuthenticated'); a.run('logoutStaff()');
  return { initial, loggedOut: a.run('isStaffAuthenticated'), newPageAuthenticated: fresh().run('isStaffAuthenticated') };
});
probe('Benchmark calculation', a => ({ total: a.run("calculateRentalPricing(3, 'SUV', 'Hybrid', true, true).grandTotal") }));
probe('Independent tabs can allocate the same registration', a => {
  valid(a); a.run('commitBookingNow()'); const b = fresh(); valid(b); b.run('commitBookingNow()');
  return { first: a.run('bookingsHistory[0].reg'), second: b.run('bookingsHistory[0].reg'), firstId: a.run('bookingsHistory[0].id'), secondId: b.run('bookingsHistory[0].id') };
});
probe('Reload loses history and rental status', a => {
  valid(a); a.run('commitBookingNow()'); const b = fresh();
  return { before: a.run('bookingsHistory.length'), after: b.run('bookingsHistory.length'), rentedAfterReload: b.run('fleetDb.filter(c => c.isRented).length') };
});
probe('Save bypasses authentication, review and input validation', a => {
  a.fill({ inpFName: '', inpSName: '', inpAddress: '', inpAge: '12', selLic: 'no', inpDaysVal: '99' });
  a.run('logoutStaff(); commitBookingNow()');
  return { authenticated: a.run('isStaffAuthenticated'), saved: a.run('bookingsHistory.length'), daysSaved: a.run('bookingsHistory[0].days'), total: a.run('bookingsHistory[0].totalPaid') };
});
probe('Fractional and exponent rental values are silently truncated', a => {
  valid(a); a.fill({ inpDaysVal: '1.9' }); const fractionalValid = a.run('runValidation()'); a.run('commitBookingNow()');
  const b = fresh(); valid(b); b.fill({ inpDaysVal: '1e2' });
  return { fractionalValid, fractionalSaved: a.run('bookingsHistory[0].days'), exponentValid: b.run('runValidation()'), exponentParsed: b.run("parseInt(document.getElementById('inpDaysVal').value)") };
});
probe('Apostrophes break voucher Inspect handler', a => {
  valid(a); a.fill({ inpSName: "O'Neil", inpAddress: "17 King's Road" }); a.run('commitBookingNow(); drawHistoryTable()');
  const row = a.nodes.get('historyTableContent').children[0].innerHTML;
  const handler = row.match(/onclick="([^"]*)"/)[1]; let handlerError = null;
  try { new vm.Script(handler); } catch (e) { handlerError = e.name; }
  return { handlerError };
});
probe('HTML injection reaches history markup', a => {
  valid(a); const payload = '<img src=x onerror="globalThis.auditXss=1">'; a.fill({ inpFName: payload });
  const validationAccepted = a.run('runValidation()'); a.run('commitBookingNow(); drawHistoryTable()');
  return { payloadLength: payload.length, validationAccepted, rawPayloadInMarkup: a.nodes.get('historyTableContent').children[0].innerHTML.includes(payload), browserExecutionTested: false };
});
probe('Address can inject code into Inspect handler', a => {
  valid(a); a.fill({ inpAddress: "');globalThis.auditInjected=1;//" }); a.run('commitBookingNow(); drawHistoryTable()');
  const handler = a.nodes.get('historyTableContent').children[0].innerHTML.match(/onclick="([^"]*)"/)[1];
  a.run(handler); return { handlerCodeExecutedInVm: a.context.auditInjected === 1 };
});
probe('Review has no immutable snapshot', a => {
  valid(a); a.run('openReviewSheet()'); const reviewedTotal = a.nodes.get('revModalTotal').innerText;
  a.fill({ inpDaysVal: '28' }); a.run('commitBookingNow()');
  return { reviewedTotal, savedTotal: a.run('bookingsHistory[0].totalPaid') };
});
probe('Save drops age, licence, timestamp detail and leaves customer values', a => {
  valid(a); a.run('updatePricingCalculation(); commitBookingNow()');
  return { storedKeys: a.run('Object.keys(bookingsHistory[0])'), storedTime: a.run('bookingsHistory[0].time'), retainedFirstName: a.nodes.get('inpFName').value,
    reviewStillEnabledForRentedCar: !a.nodes.get('btnLaunchReview').disabled, allocationStillContainsRentedRegistration: a.nodes.get('allocSummaryText').innerHTML.includes('WAC-008') };
});
probe('Return changes fleet only, no booking completion record', a => {
  valid(a); a.run("commitBookingNow(); checkInCar('WAC-008')");
  return { rented: a.run("fleetDb.find(c=>c.reg==='WAC-008').isRented"), bookingHasStatus: a.run("'status' in bookingsHistory[0]"), bookingHasReturnDate: a.run("'returnedAt' in bookingsHistory[0]") };
});
probe('Unknown category or fuel silently receives zero surcharge', a => ({ total: a.run("calculateRentalPricing(3, 'Unknown', 'Unknown', false, false).grandTotal") }));
probe('Static accessibility and responsive structure', () => ({
  responsiveMediaQueries: (html.match(/@media/g) || []).length,
  reducedMotionSupport: html.includes('prefers-reduced-motion'), dialogRoles: (html.match(/role="dialog"/g) || []).length,
  ariaModal: html.includes('aria-modal'), keyboardListeners: /addEventListener\(['"]key|onkeydown|onkeyup|onkeypress/.test(html),
  nativeForms: (html.match(/<form\b/g) || []).length, customerErrorAssociations: html.includes('aria-describedby')
}));
const output = { methodology: 'Original inline JavaScript executed in Node VM with a minimal DOM stub. No real browser layout or image QA; code injection executes only in this isolated VM.', results };
fs.writeFileSync(path.join(__dirname, 'web-probe-results.json'), JSON.stringify(output, null, 2));
console.log(JSON.stringify(output, null, 2));
