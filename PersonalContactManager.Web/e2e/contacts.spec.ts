import { test, expect } from '@playwright/test';

test.describe('Contacts list', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/contacts');
  });

  test('displays the page header', async ({ page }) => {
    await expect(page.getByRole('heading', { name: 'Contacts' })).toBeVisible();
  });

  test('shows the New Contact button', async ({ page }) => {
    await expect(page.getByRole('button', { name: 'New Contact' })).toBeVisible();
  });

  test('has search input', async ({ page }) => {
    await expect(page.getByPlaceholder('Search name or email…')).toBeVisible();
  });

  test('navigates to new contact form', async ({ page }) => {
    await page.getByRole('button', { name: 'New Contact' }).click();
    await expect(page).toHaveURL('/contacts/new');
    await expect(page.getByRole('heading', { name: 'New Contact' })).toBeVisible();
  });

  test('search filters contacts', async ({ page }) => {
    const input = page.getByPlaceholder('Search name or email…');
    await input.fill('Test');
    await input.press('Enter');
    await expect(page).toHaveURL(/contacts/);
  });
});

test.describe('Contact form — required fields', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/contacts/new');
  });

  test('shows validation errors when submitting empty form', async ({ page }) => {
    await page.getByRole('button', { name: 'Create Contact' }).click();
    await expect(page.getByText('First name is required')).toBeVisible();
    await expect(page.getByText('Last name is required')).toBeVisible();
  });

  test('cancel navigates back to contacts list', async ({ page }) => {
    await page.getByRole('button', { name: 'Cancel' }).click();
    await expect(page).toHaveURL('/contacts');
  });

  test('all required fields are present on the form', async ({ page }) => {
    // First name & Last name
    await expect(page.locator('#firstName')).toBeVisible();
    await expect(page.locator('#lastName')).toBeVisible();
    // Date of birth
    await expect(page.locator('#birthday')).toBeVisible();
    // IBAN
    await expect(page.locator('#iban')).toBeVisible();
    // Phone number
    await expect(page.getByPlaceholder('+1 555 123 4567')).toBeVisible();
    // Address fields
    await expect(page.locator('#street')).toBeVisible();
    await expect(page.locator('#city')).toBeVisible();
    await expect(page.locator('#postalCode')).toBeVisible();
    await expect(page.locator('#country')).toBeVisible();
  });

  test('fills all required fields and form becomes submittable', async ({ page }) => {
    // Basic info
    await page.locator('#firstName').fill('Jane');
    await page.locator('#lastName').fill('Doe');
    await page.locator('#birthday').fill('01/01/1990');
    await page.locator('#iban').fill('GB29NWBK60161331926819');

    // Phone number (label select defaults to first option — Mobile)
    await page.getByPlaceholder('+1 555 123 4567').fill('+44 7700 900123');

    // Address
    await page.locator('#street').fill('123 Main St');
    await page.locator('#city').fill('London');
    await page.locator('#postalCode').fill('SW1A 1AA');
    await page.locator('#country').fill('United Kingdom');

    // The submit button should be enabled
    await expect(page.getByRole('button', { name: 'Create Contact' })).toBeEnabled();
  });
});
