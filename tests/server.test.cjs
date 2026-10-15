'use strict';
const { test, before, after } = require('node:test');
const assert = require('node:assert/strict');
const { spawn } = require('node:child_process');
const path = require('node:path');
const root = path.resolve(__dirname, '..');
const connection = process.env.WEARECARS_TEST_CONNECTION_STRING;
if (!connection || !/Initial Catalog=WeAreCarsTest_[a-z0-9]+/i.test(connection) || !/Data Source=\(localdb\)\\WeAreCarsTest_/i.test(connection)) throw new Error('API tests require a dedicated disposable test database. Run scripts/test.ps1.');
const port = 8060, base = `http://127.0.0.1:${port}`;
let server, output = '';
function client() {
  let cookie = '', csrf = '';
  return { async call(url, method = 'GET', payload, headers = {}) {
    const response = await fetch(base + url, { method, headers: { ...(cookie ? { Cookie: cookie } : {}), ...(csrf ? { 'X-CSRF-Token': csrf } : {}), ...(payload ? { 'Content-Type': 'application/json' } : {}), ...headers }, body: payload ? JSON.stringify(payload) : undefined });
    if (response.headers.get('set-cookie')) cookie = response.headers.get('set-cookie').split(';')[0];
    const data = await response.json(); if (data.csrf) csrf = data.csrf;
    return { status: response.status, data, headers: response.headers };
  }, async login() { const result = await this.call('/api/login', 'POST', { username: 'sta001', password: 'givemethekeys123' }); assert.equal(result.status, 200, JSON.stringify(result.data)); } };
}
const draft = { firstName: 'Synthetic', surname: "O'Neil", address: "17 King's Road", age: 26, licence: true, days: 3, category: 'SUV', fuel: 'Hybrid', mileage: true, cover: true };
before(async () => {
  server = spawn(process.execPath, ['server/server.cjs', '--demo'], { cwd: root, windowsHide: true, env: { ...process.env, PORT: String(port), WEARECARS_CONNECTION_STRING: connection } });
  server.stdout.on('data', x => { output += x; }); server.stderr.on('data', x => { output += x; });
  const start = Date.now(); while (Date.now() - start < 60000) {
    if (server.exitCode !== null) throw new Error(output);
    try { const health = await fetch(base + '/api/health'); if (health.ok) return; } catch {}
    await new Promise(resolve => setTimeout(resolve, 250));
  }
  throw new Error('Test server did not become healthy: ' + output);
});
after(async () => {
  if (!server) return;
  // Closing stdin ends the bridge safely before terminating the parent on Windows.
  const cleanup = spawn('powershell.exe', ['-NoProfile', '-Command', `$children = Get-CimInstance Win32_Process -Filter 'ParentProcessId = ${server.pid}'; foreach ($child in $children) { if ($child.Name -eq 'RentOnCars.WebBridge.exe') { Stop-Process -Id $child.ProcessId -ErrorAction SilentlyContinue } }`], { windowsHide: true });
  await new Promise(resolve => cleanup.on('exit', resolve)); server.kill();
});
test('Unauthenticated data access, hostile origins and missing CSRF are rejected', async () => {
  const c = client(); assert.equal((await c.call('/api/fleet')).status, 401);
  assert.equal((await c.call('/api/login', 'POST', { username: 'sta001', password: 'givemethekeys123' }, { Origin: 'http://hostile.example' })).status, 403);
  await c.login(); assert.equal((await c.call('/api/quotes', 'POST', draft, { 'X-CSRF-Token': '' })).status, 403);
});
test('Server rejects fractional, out-of-range, invalid licence and unknown enum values', async () => {
  const c = client(); await c.login();
  for (const change of [{ days: 1.9 }, { days: 100 }, { age: 17 }, { age: 121 }, { licence: false }, { category: '999' }, { firstName: '' }]) {
    const result = await c.call('/api/quotes', 'POST', { ...draft, ...change }); assert.equal(result.status, 400, JSON.stringify(result.data));
  }
});
test('Reviewed snapshot, durable booking, exact benchmark and duplicate-request protection', async () => {
  const c = client(); await c.login(); const quote = await c.call('/api/quotes', 'POST', draft);
  assert.equal(quote.status, 200); assert.equal(quote.data.pricing.total, 206);
  const [first, retry] = await Promise.all([c.call('/api/bookings', 'POST', { token: quote.data.token, days: 99, total: 1 }), c.call('/api/bookings', 'POST', { token: quote.data.token })]);
  assert.equal(first.status, 201, JSON.stringify(first.data)); assert.equal(retry.data.id, first.data.id);
  assert.equal(first.data.days, 3); assert.equal(first.data.total, 206); assert.equal(first.data.age, 26); assert.equal(first.data.licence, true);
  assert.match(first.data.createdAt, /T.*Z$/); assert.equal(first.data.staff, 'sta001');
  const reloaded = await c.call('/api/bookings/' + first.data.id); assert.equal(reloaded.data.surname, "O'Neil"); assert.equal(reloaded.data.address, "17 King's Road");
  const returned = await c.call(`/api/bookings/${first.data.id}/return`, 'POST', {}); assert.equal(returned.status, 200);
  assert.equal((await c.call('/api/bookings/' + first.data.id)).data.status, 'Returned');
});
test('Separate staff sessions cannot save the same allocated car', async () => {
  const a = client(), b = client(); await a.login(); await b.login();
  const [qa, qb] = await Promise.all([a.call('/api/quotes', 'POST', draft), b.call('/api/quotes', 'POST', draft)]);
  assert.equal(qa.data.car.id, qb.data.car.id);
  assert.equal((await b.call('/api/bookings', 'POST', { token: qa.data.token })).status, 410);
  const saves = await Promise.all([a.call('/api/bookings', 'POST', { token: qa.data.token }), b.call('/api/bookings', 'POST', { token: qb.data.token })]);
  assert.deepEqual(saves.map(x => x.status).sort(), [201, 409]); const saved = saves.find(x => x.status === 201);
  assert.equal((await a.call(`/api/bookings/${saved.data.id}/return`, 'POST', {})).status, 200);
});
test('Records remain text data, inventory filters work, and logout survives a new request', async () => {
  const c = client(); await c.login(); const quote = await c.call('/api/quotes', 'POST', { ...draft, firstName: '<img src=x onerror="globalThis.injected=1">', category: 'CityCar', fuel: 'Diesel' });
  assert.equal(quote.status, 200); const saved = await c.call('/api/bookings', 'POST', { token: quote.data.token });
  assert.equal(saved.status, 201); assert.match(saved.data.firstName, /^<img/);
  const search = await c.call('/api/bookings?search=' + encodeURIComponent("O'Neil")); assert.ok(search.data.items.some(b => b.id === saved.data.id));
  assert.ok((await c.call('/api/fleet?status=rented')).data.items.every(car => car.rented));
  assert.equal((await c.call(`/api/bookings/${saved.data.id}/return`, 'POST', {})).status, 200);
  const logout = await c.call('/api/logout', 'POST', {}); assert.equal(logout.status, 200); assert.match(logout.headers.get('set-cookie'), /Max-Age=0/);
  assert.equal((await c.call('/api/session')).status, 401);
});
test('Public customer self-service endpoints operate without staff authentication', async () => {
  const c = client();
  // 1. Catalog
  const catalog = await c.call('/api/public/catalog');
  assert.equal(catalog.status, 200);
  assert.equal(typeof catalog.data.baseDaily, 'number');
  assert.ok(catalog.data.categories.length > 0);

  // 2. Fleet
  const fleet = await c.call('/api/public/fleet');
  assert.equal(fleet.status, 200);
  assert.ok(Array.isArray(fleet.data.items));

  // 3. Estimate
  const estimate = await c.call('/api/public/estimate', 'POST', { category: 'SUV', fuel: 'Hybrid', days: 3, mileage: true, cover: true });
  assert.equal(estimate.status, 200);
  assert.equal(estimate.data.available, true);
  assert.equal(estimate.data.pricing.total, 206);

  // 4. Quote (10m locked quote)
  const pubQuote = await c.call('/api/public/quotes', 'POST', { firstName: 'PublicCustomer', surname: 'Walker', address: '42 Marine Walk', age: 30, licence: true, category: 'SUV', fuel: 'Hybrid', days: 3, mileage: true, cover: true });
  assert.equal(pubQuote.status, 200);
  assert.ok(pubQuote.data.token);
  assert.equal(pubQuote.data.pricing.total, 206);

  // 5. Commit booking
  const pubBooking = await c.call('/api/public/bookings', 'POST', { token: pubQuote.data.token });
  assert.equal(pubBooking.status, 201);
  assert.equal(pubBooking.data.staff, 'self-service');
  assert.equal(pubBooking.data.total, 206);
  assert.ok(pubBooking.data.reference.startsWith('WAC-BK-'));

  // 6. Public Lookup with reference + surname
  const lookup = await c.call('/api/public/lookup', 'POST', { reference: pubBooking.data.reference, surname: 'Walker' });
  assert.equal(lookup.status, 200);
  assert.equal(lookup.data.id, pubBooking.data.id);
  assert.equal(lookup.data.total, 206);

  // 7. Lookup with incorrect surname rejected
  const failedLookup = await c.call('/api/public/lookup', 'POST', { reference: pubBooking.data.reference, surname: 'WrongSurname' });
  assert.equal(failedLookup.status, 400);

  // 8. Return the vehicle so subsequent tests have full fleet availability
  const admin = client();
  await admin.login();
  assert.equal((await admin.call(`/api/bookings/${pubBooking.data.id}/return`, 'POST', {})).status, 200);
});
