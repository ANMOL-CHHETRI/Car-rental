const { defineConfig } = require('@playwright/test');
if (!process.env.WEARECARS_TEST_CONNECTION_STRING || !process.env.WEARECARS_TEST_CONNECTION_STRING.includes('Initial Catalog=WeAreCarsTest_')) throw new Error('Run UI tests through scripts/test.ps1 against a disposable database.');
module.exports = defineConfig({
  testDir: './tests/ui', workers: 1, fullyParallel: false, timeout: 60000,
  reporter: [['list']], outputDir: 'test-results/ui',
  use: { baseURL: 'http://127.0.0.1:8061', browserName: 'chromium', headless: true, viewport: { width: 1440, height: 1000 }, screenshot: 'only-on-failure' },
  webServer: { command: 'node server/server.cjs --demo', url: 'http://127.0.0.1:8061/api/health', reuseExistingServer: false, timeout: 60000, env: { PORT: '8061', WEARECARS_CONNECTION_STRING: process.env.WEARECARS_TEST_CONNECTION_STRING } }
});
