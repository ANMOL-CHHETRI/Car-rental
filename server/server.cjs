'use strict';
const http = require('node:http');
const fs = require('node:fs');
const path = require('node:path');
const crypto = require('node:crypto');
const readline = require('node:readline');
const { spawn } = require('node:child_process');

const root = path.resolve(__dirname, '..');
const port = Number(process.env.PORT || 8000);
if (!Number.isInteger(port) || port < 1024 || port > 65535) throw new Error('PORT must be 1024–65535.');
if (process.argv.includes('--demo')) process.env.WEARECARS_DEMO = '1';
const workerPath = process.env.WEARECARS_BRIDGE || path.join(root, 'RentOnCars/RentOnCars.WebBridge/bin/Debug/RentOnCars.WebBridge.exe');
if (!fs.existsSync(workerPath)) throw new Error('Build RentOnCars.sln before starting the web server.');
const publicOrigin = process.env.WEARECARS_PUBLIC_ORIGIN ? new URL(process.env.WEARECARS_PUBLIC_ORIGIN) : null;
if (publicOrigin && (publicOrigin.protocol !== 'https:' || publicOrigin.pathname !== '/' || publicOrigin.search || publicOrigin.hash)) throw new Error('Production origin must be an HTTPS origin without a path.');
if (publicOrigin && process.env.WEARECARS_DEMO === '1') throw new Error('Demo accounts cannot be used with a public production origin.');
const secureCookie = publicOrigin ? '; Secure' : '';
const sessions = new Map();
const quotes = new Map();
const attempts = new Map();
const pending = new Map();
let sequence = 0, ready = false, databaseMessage = 'Preparing database', stopping = false;
const worker = spawn(workerPath, ['--worker-id', process.env.WEARECARS_WORKER_TAG || 'web'], { windowsHide: true, cwd: root, env: process.env, stdio: ['pipe', 'pipe', 'pipe'] });
worker.stderr.on('data', chunk => process.stderr.write(chunk));
worker.on('error', error => { databaseMessage = error.message; });
worker.on('exit', () => {
  ready = false; databaseMessage = 'Database worker stopped. Restart the server to reconnect.';
  for (const job of pending.values()) { clearTimeout(job.timer); job.reject(Object.assign(new Error(databaseMessage), { status: 503 })); }
  pending.clear();
  if (!stopping) console.error(databaseMessage);
});
readline.createInterface({ input: worker.stdout }).on('line', line => {
  try {
    const reply = JSON.parse(line);
    if ('ready' in reply) { ready = reply.ready; databaseMessage = reply.message; console.log(reply.message); return; }
    const job = pending.get(reply.id); if (!job) return;
    clearTimeout(job.timer); pending.delete(reply.id);
    if (reply.ok) job.resolve(reply.result);
    else job.reject(Object.assign(new Error(reply.message), { status: reply.status }));
  } catch { console.error('Invalid response from the database worker.'); }
});
function rpc(action, payload = {}, actor = '') {
  if (!ready) return Promise.reject(Object.assign(new Error(databaseMessage), { status: 503 }));
  if (pending.size >= 40) return Promise.reject(Object.assign(new Error('The server is busy. Try again shortly.'), { status: 503 }));
  return new Promise((resolve, reject) => {
    const id = ++sequence;
    const timer = setTimeout(() => { pending.delete(id); reject(Object.assign(new Error('The database did not respond in time. Retry the same booking request.'), { status: 503 })); }, 30000);
    pending.set(id, { resolve, reject, timer });
    worker.stdin.write(JSON.stringify({ id, action, payload, actor }) + '\n');
  });
}
function json(res, status, value) {
  res.writeHead(status, { 'Content-Type': 'application/json; charset=utf-8', 'Cache-Control': 'no-store' });
  res.end(JSON.stringify(value));
}
async function body(req) {
  if (!(req.headers['content-type'] || '').startsWith('application/json')) throw Object.assign(new Error('Use JSON request data.'), { status: 415 });
  let size = 0; const chunks = [];
  for await (const chunk of req) {
    size += chunk.length;
    if (size > 16384) throw Object.assign(new Error('Request is too large.'), { status: 413 });
    chunks.push(chunk);
  }
  let value; try { value = JSON.parse(Buffer.concat(chunks).toString('utf8')); }
  catch { throw Object.assign(new Error('Invalid JSON data.'), { status: 400 }); }
  if (!value || typeof value !== 'object' || Array.isArray(value)) throw Object.assign(new Error('Invalid request data.'), { status: 400 });
  return value;
}
function token() { return crypto.randomBytes(32).toString('hex'); }
function session(req) {
  const id = (req.headers.cookie || '').split(';').map(x => x.trim()).find(x => x.startsWith('wearecars_session='))?.slice(18);
  const value = id && sessions.get(id);
  if (!value || value.idleUntil < Date.now() || value.absoluteUntil < Date.now()) {
    if (id) sessions.delete(id);
    throw Object.assign(new Error('Your session has ended. Sign in again.'), { status: 401 });
  }
  value.idleUntil = Date.now() + 20 * 60 * 1000;
  return { id, ...value };
}
function verifyMutation(req, staff) {
  if (req.headers['x-csrf-token'] !== staff.csrf) throw Object.assign(new Error('Request could not be verified. Refresh and try again.'), { status: 403 });
}
function pageQuery(url) {
  const page = Number(url.searchParams.get('page') || 1);
  if (!Number.isSafeInteger(page) || page < 1 || page > 100000) throw Object.assign(new Error('Invalid page.'), { status: 400 });
  const search = (url.searchParams.get('search') || '').trim();
  if (search.length > 200) throw Object.assign(new Error('Search is too long.'), { status: 400 });
  const sort=url.searchParams.get('sort') || 'newest';
  if(!['newest','customer','total'].includes(sort)) throw Object.assign(new Error('Invalid sort order.'), {status:400});
  return { page, pageSize: 25, search, sort };
}
const server = http.createServer(async (req, res) => {
  res.setHeader('X-Content-Type-Options', 'nosniff');
  res.setHeader('Referrer-Policy', 'same-origin');
  res.setHeader('Permissions-Policy', 'camera=(), microphone=(), geolocation=()');
  res.setHeader('Content-Security-Policy', "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data:; base-uri 'none'; frame-ancestors 'none'; form-action 'self'");
  try {
    const allowedHosts = new Set([`127.0.0.1:${port}`, `localhost:${port}`, ...(publicOrigin ? [publicOrigin.host] : [])]);
    if (!allowedHosts.has(req.headers.host)) throw Object.assign(new Error('Invalid host.'), { status: 403 });
    const origin = publicOrigin?.origin || `http://${req.headers.host}`;
    if (req.headers.origin && req.headers.origin !== origin) throw Object.assign(new Error('Cross-origin requests are not allowed.'), { status: 403 });
    const url = new URL(req.url, origin);
    if (url.pathname === '/api/health' && req.method === 'GET') {
      if (!ready) return json(res, 503, { database: false, message: databaseMessage });
      const health = await rpc('health');
      return json(res, health.database ? 200 : 503, { ...health, service: 'WeAreCars', mode: process.env.WEARECARS_DEMO === '1' ? 'demo' : 'staff' });
    }
    if (url.pathname === '/api/login' && req.method === 'POST') {
      const data = await body(req);
      const key = req.socket.remoteAddress;
      const window = attempts.get(key) || { count: 0, until: Date.now() + 15 * 60 * 1000 };
      if (window.until < Date.now()) { window.count = 0; window.until = Date.now() + 15 * 60 * 1000; }
      if (window.count >= 20) throw Object.assign(new Error('Too many sign-in attempts. Try again later.'), { status: 429 });
      window.count++; attempts.set(key, window);
      if (typeof data.username !== 'string' || typeof data.password !== 'string' || data.password.length > 256) throw Object.assign(new Error('Enter your username and password.'), { status: 400 });
      const identity = await rpc('login', { username: data.username.trim(), password: data.password });
      if (!identity) throw Object.assign(new Error('Unable to sign in. Check your details or try again later.'), { status: 401 });
      window.count = 0;
      if (sessions.size >= 1000) throw Object.assign(new Error('Session limit reached.'), { status: 503 });
      const id = token(), csrf = token();
      sessions.set(id, { ...identity, csrf, idleUntil: Date.now() + 20 * 60 * 1000, absoluteUntil: Date.now() + 8 * 60 * 60 * 1000 });
      res.setHeader('Set-Cookie', `wearecars_session=${id}; HttpOnly; SameSite=Strict; Path=/; Max-Age=28800${secureCookie}`);
      return json(res, 200, { ...identity, csrf });
    }
    if (url.pathname.startsWith('/api/public/')) {
      if (url.pathname === '/api/public/catalog' && req.method === 'GET') return json(res, 200, await rpc('catalog'));
      if (url.pathname === '/api/public/fleet' && req.method === 'GET') {
        const search = (url.searchParams.get('search') || '').trim();
        return json(res, 200, await rpc('public-fleet', { search, page: 1, pageSize: 50 }));
      }
      if (url.pathname === '/api/public/estimate' && req.method === 'POST') return json(res, 200, await rpc('estimate', await body(req)));
      if (url.pathname === '/api/public/quotes' && req.method === 'POST') {
        if (quotes.size >= 2000) throw Object.assign(new Error('Quotation limit reached. Try again shortly.'), { status: 503 });
        const snapshot = await rpc('quote', await body(req), 'self-service');
        const id = token(); quotes.set(id, { sessionId: 'public', snapshot, expires: Date.now() + 10 * 60 * 1000 });
        return json(res, 200, { token: id, expiresAt: new Date(Date.now() + 10 * 60 * 1000).toISOString(), ...snapshot });
      }
      if (url.pathname === '/api/public/bookings' && req.method === 'POST') {
        const data = await body(req), quote = quotes.get(data.token);
        if (!quote || quote.sessionId !== 'public' || quote.expires < Date.now()) throw Object.assign(new Error('Your quotation has expired. Please review your selection again.'), { status: 410 });
        if (!quote.saved) {
          if (!quote.inFlight) quote.inFlight = rpc('save', quote.snapshot, 'self-service').then(result => (quote.saved = result)).finally(() => { quote.inFlight = null; });
          await quote.inFlight;
        }
        return json(res, 201, quote.saved);
      }
      if (url.pathname === '/api/public/lookup' && req.method === 'POST') return json(res, 200, await rpc('public-lookup', await body(req)));
      return json(res, 404, { message: 'Public endpoint not found.' });
    }
    if (url.pathname.startsWith('/api/')) {
      const staff = session(req);
      if (req.method !== 'GET') verifyMutation(req, staff);
      if (url.pathname === '/api/session' && req.method === 'GET') return json(res, 200, { username: staff.username, role: staff.role, csrf: staff.csrf });
      if (url.pathname === '/api/logout' && req.method === 'POST') {
        sessions.delete(staff.id); for (const [key, quote] of quotes) if (quote.sessionId === staff.id) quotes.delete(key);
        res.setHeader('Set-Cookie', `wearecars_session=; HttpOnly; SameSite=Strict; Path=/; Max-Age=0${secureCookie}`);
        return json(res, 200, { signedOut: true });
      }
      if (url.pathname === '/api/catalog' && req.method === 'GET') return json(res, 200, await rpc('catalog', {}, staff.username));
      if (url.pathname === '/api/stats' && req.method === 'GET') return json(res, 200, await rpc('stats', {}, staff.username));
      if (url.pathname === '/api/fleet' && req.method === 'GET') {
        const query = pageQuery(url), status = url.searchParams.get('status') || 'all';
        if (!['all', 'rented', 'available'].includes(status)) throw Object.assign(new Error('Invalid fleet filter.'), { status: 400 });
        return json(res, 200, await rpc('fleet', { ...query, status }, staff.username));
      }
      if (url.pathname === '/api/bookings' && req.method === 'GET') return json(res, 200, await rpc('history', pageQuery(url), staff.username));
      const bookingMatch = url.pathname.match(/^\/api\/bookings\/(\d+)$/);
      if (bookingMatch && req.method === 'GET') return json(res, 200, await rpc('booking', { id: Number(bookingMatch[1]) }, staff.username));
      if (url.pathname === '/api/estimate' && req.method === 'POST') return json(res, 200, await rpc('estimate', await body(req), staff.username));
      if (url.pathname === '/api/quotes' && req.method === 'POST') {
        if (quotes.size >= 2000) throw Object.assign(new Error('Quotation limit reached. Try again shortly.'), { status: 503 });
        const snapshot = await rpc('quote', await body(req), staff.username);
        const id = token(); quotes.set(id, { sessionId: staff.id, snapshot, expires: Date.now() + 5 * 60 * 1000 });
        return json(res, 200, { token: id, expiresAt: new Date(Date.now() + 5 * 60 * 1000).toISOString(), ...snapshot });
      }
      if (url.pathname === '/api/bookings' && req.method === 'POST') {
        const data = await body(req), quote = quotes.get(data.token);
        if (!quote || quote.sessionId !== staff.id || quote.expires < Date.now()) throw Object.assign(new Error('Your quotation has expired. Review the booking again.'), { status: 410 });
        if (!quote.saved) {
          // Concurrent/retried submissions share the same durable request ID.
          if (!quote.inFlight) quote.inFlight = rpc('save', quote.snapshot, staff.username).then(result => (quote.saved = result)).finally(() => { quote.inFlight = null; });
          await quote.inFlight;
        }
        return json(res, 201, quote.saved);
      }
      const returnMatch = url.pathname.match(/^\/api\/bookings\/(\d+)\/return$/);
      if (returnMatch && req.method === 'POST') return json(res, 200, await rpc('return', { id: Number(returnMatch[1]) }, staff.username));
      return json(res, 404, { message: 'Endpoint not found.' });
    }
    if (req.method !== 'GET' && req.method !== 'HEAD') return json(res, 405, { message: 'Method not allowed.' });
    const assets = { '/': ['index.html', 'text/html'], '/index.html': ['index.html', 'text/html'], '/styles.css': ['styles.css', 'text/css'], '/app.js': ['app.js', 'text/javascript'] };
    const asset = assets[url.pathname]; if (!asset) return json(res, 404, { message: 'Page not found.' });
    const file = path.join(root, 'web_preview', asset[0]);
    res.writeHead(200, { 'Content-Type': asset[1] + '; charset=utf-8', 'Cache-Control': 'no-store' });
    if (req.method === 'HEAD') res.end(); else fs.createReadStream(file).pipe(res);
  } catch (error) { if (!res.headersSent) json(res, error.status || 500, { message: error.status ? error.message : 'The request could not be completed.' }); else res.destroy(); }
});
server.requestTimeout = 35000;
server.headersTimeout = 10000;
const cleanup = setInterval(() => {
  const now = Date.now();
  for (const [id, value] of sessions) if (value.idleUntil < now || value.absoluteUntil < now) sessions.delete(id);
  for (const [id, value] of quotes) if (value.expires < now || !sessions.has(value.sessionId)) quotes.delete(id);
  for (const [id, value] of attempts) if (value.until < now) attempts.delete(id);
}, 30000).unref();
server.listen(port, '127.0.0.1', () => console.log(`WeAreCars: http://127.0.0.1:${port}/`));
server.on('error', error => { console.error(error.message); stopping = true; worker.kill(); process.exitCode = 1; });
function stop() { stopping = true; clearInterval(cleanup); server.close(); worker.kill(); }
process.on('SIGINT', stop); process.on('SIGTERM', stop);

module.exports = { rpc, server };
