const { test, expect } = require('@playwright/test');
const path = require('node:path');

async function login(page) {
  await page.goto('/#staff');
  await page.getByLabel('Username', { exact: true }).fill('sta001');
  await page.getByLabel('Password', { exact: true }).fill('givemethekeys123');
  await page.getByRole('button', { name: 'Sign in', exact: true }).click();
  await expect(page.getByRole('heading', { name: 'Rental overview' })).toBeVisible();
  await expect(page.locator('#statTotal')).toHaveText('10');
}

async function book(page, firstName = 'Synthetic', surname = "O'Neil") {
  await page.goto('/#booking');
  await page.getByLabel('First name', { exact: true }).fill(firstName);
  await page.getByLabel('Surname', { exact: true }).fill(surname);
  await page.getByLabel('Address', { exact: true }).fill("17 King's Road");
  await page.getByLabel('Age', { exact: true }).fill('26');
  await page.locator('#licence').check();
  await page.getByLabel('Vehicle category', { exact: true }).selectOption('SUV');
  await page.getByLabel('Fuel type', { exact: true }).selectOption('Hybrid');
  await page.getByLabel('Rental duration', { exact: true }).fill('3');
  await page.locator('#mileage').check();
  await page.locator('#cover').check();
  await expect(page.locator('#estimateTotal')).toHaveText('£206.00');
  await page.getByRole('button', { name: 'Review booking', exact: true }).click();
  await expect(page.getByRole('dialog', { name: 'Review booking' })).toBeVisible();
}

test('Sign-in requires credentials, supports Enter and logout remains enforced after reload', async ({ page }) => {
  await page.goto('/#staff');
  await expect(page.locator('#appShell')).toBeHidden();
  await expect(page.getByLabel('Password', { exact: true })).toBeEmpty();
  await page.getByLabel('Username', { exact: true }).fill('sta001');
  await page.getByLabel('Password', { exact: true }).fill('givemethekeys123');
  await page.getByLabel('Password', { exact: true }).press('Enter');
  await expect(page.getByRole('heading', { name: 'Rental overview' })).toBeVisible();
  await page.getByRole('button', { name: 'Sign out', exact: true }).click();
  await page.reload();
  await expect(page.locator('#appShell')).toBeHidden();
  await expect(page.getByLabel('Password', { exact: true })).toBeEmpty();
});

test('Booking, safe customer rendering, reload persistence and durable return', async ({ page }) => {
  const errors = [];
  page.on('pageerror', error => errors.push(error.message));
  await login(page);
  await book(page, '<img src=x onerror="window.injected=1">');
  await page.getByRole('button', { name: 'Confirm booking', exact: true }).click();
  await expect(page.locator('#detailsDialog')).toBeVisible();
  const reference = await page.locator('#detailsTitle').innerText();
  await expect(page.locator('#bookingDetails')).toContainText('<img src=x');
  await expect(page.locator('#bookingDetails img')).toHaveCount(0);
  expect(await page.evaluate(() => window.injected)).toBeUndefined();
  await page.getByRole('button', { name: 'Done', exact: true }).click();
  await page.goto('/#history');
  await page.getByLabel('Search bookings').fill(reference);
  await expect(page.locator('#historyRows')).toContainText(reference);
  await page.reload();
  await expect(page.locator('#historyRows')).toContainText(reference);
  await page.locator('#historyRows').getByRole('button', { name: 'View', exact: true }).first().click();
  await expect(page.locator('#bookingDetails')).toContainText("17 King's Road");
  await page.getByRole('button', { name: 'Return vehicle', exact: true }).click();
  await page.getByRole('button', { name: 'Confirm return', exact: true }).click();
  await expect(page.locator('#bookingDetails')).toContainText('Returned');
  expect(errors).toEqual([]);
});

test('Native review dialog traps keyboard focus, Escape returns to editing and fractions are rejected', async ({ page }) => {
  await login(page);
  await book(page);
  for (let i = 0; i < 10; i++) {
    await page.keyboard.press('Tab');
    expect(await page.evaluate(() => document.querySelector('#reviewDialog').contains(document.activeElement))).toBe(true);
  }
  await page.keyboard.press('Escape');
  await expect(page.locator('#reviewDialog')).toBeHidden();
  await expect(page.getByLabel('Surname', { exact: true })).toHaveValue("O'Neil");
  await page.getByLabel('Rental duration', { exact: true }).fill('1.9');
  await expect(page.getByRole('button', { name: 'Review booking', exact: true })).toBeDisabled();
});

test('Desktop and mobile layouts avoid page overflow and expose usable navigation', async ({ page }) => {
  await login(page);
  await expect(page.locator('#overviewFleetRows tr')).toHaveCount(5);
  await page.screenshot({ path: path.resolve('test-results/overview-desktop.png'), fullPage: true });
  await page.setViewportSize({ width: 390, height: 844 });
  await expect(page.locator('.mobile-nav')).toBeVisible();
  expect(await page.evaluate(() => document.documentElement.scrollWidth <= window.innerWidth)).toBe(true);
  await page.locator('.mobile-nav').getByRole('link', { name: 'New booking', exact: true }).click();
  await expect(page.getByLabel('First name', { exact: true })).toBeVisible();
  expect(await page.evaluate(() => document.documentElement.scrollWidth <= window.innerWidth)).toBe(true);
  await page.screenshot({ path: path.resolve('test-results/booking-mobile.png'), fullPage: true });
  await page.setViewportSize({ width: 720, height: 500 });
  expect(await page.evaluate(() => document.documentElement.scrollWidth <= window.innerWidth)).toBe(true);
});

test('Public customer self-service: browse fleet, live quotation, book car and lookup reservation', async ({ page }) => {
  const errors = [];
  page.on('pageerror', error => errors.push(error.message));
  await page.goto('/');
  await expect(page.locator('#customerView')).toBeVisible();
  await expect(page.locator('#loginView')).toBeHidden();
  await expect(page.locator('#appShell')).toBeHidden();

  // 1. Browse fleet tab
  await page.locator('#custNavFleet').click();
  await expect(page.locator('#cust-section-fleet')).toBeVisible();
  await expect(page.locator('#custFleetGrid .fleet-card')).not.toHaveCount(0);

  // 2. Select booking tab
  await page.locator('#custNavBook').click();
  await expect(page.locator('#cust-section-book')).toBeVisible();

  // 3. Fill customer booking form with benchmark (SUV, Hybrid, 3 days, mileage + cover)
  await page.locator('#custCategory').selectOption('SUV');
  await page.locator('#custFuel').selectOption('Hybrid');
  await page.locator('#custDays').fill('3');
  await page.locator('#custMileage').check();
  await page.locator('#custCover').check();

  await expect(page.locator('#custEstimateTotal')).toHaveText('£206.00');

  // Fill driver details
  await page.locator('#custFirstName').fill('PublicCustomer');
  await page.locator('#custSurname').fill('Walker');
  await page.locator('#custAddress').fill('42 Marine Walk');
  await page.locator('#custAge').fill('28');
  await page.locator('#custLicence').check();

  await expect(page.locator('#custBookButton')).toBeEnabled();
  await page.locator('#custBookButton').click();

  // Expect voucher dialog
  await expect(page.locator('#custVoucherDialog')).toBeVisible();
  const voucherText = await page.locator('#custVoucherDetails').innerText();
  expect(voucherText).toContain('WAC-BK-');
  expect(voucherText).toContain('£206.00');
  expect(voucherText).toContain('Walker');

  // Extract reference number
  const match = voucherText.match(/WAC-BK-\d+/);
  expect(match).not.toBeNull();
  const reference = match[0];

  // Close voucher
  await page.locator('#custVoucherDialog').getByRole('button', { name: 'Done' }).click();
  await expect(page.locator('#custVoucherDialog')).toBeHidden();

  // 4. Find My Booking tab
  await page.locator('#custNavLookup').click();
  await expect(page.locator('#cust-section-lookup')).toBeVisible();

  // Lookup with wrong surname
  await page.locator('#custLookupRef').fill(reference);
  await page.locator('#custLookupSurname').fill('WrongSurname');
  await page.locator('#custLookupSubmit').click();
  await expect(page.locator('#custLookupError')).toBeVisible();
  await expect(page.locator('#custLookupResult')).toBeHidden();

  // Lookup with correct surname
  await page.locator('#custLookupSurname').fill('Walker');
  await page.locator('#custLookupSubmit').click();
  await expect(page.locator('#custLookupResult')).toBeVisible();
  await expect(page.locator('#custLookupResult')).toContainText(reference);
  await expect(page.locator('#custLookupResult')).toContainText('£206.00');

  // 5. Navigate to staff portal
  await page.locator('#toStaffLogin').click();
  await expect(page.locator('#loginView')).toBeVisible();
  await expect(page.locator('#customerView')).toBeHidden();

  // Return to customer
  await page.locator('#backToCustomer').click();
  await expect(page.locator('#customerView')).toBeVisible();

  expect(errors).toEqual([]);
});
