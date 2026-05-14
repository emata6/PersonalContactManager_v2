import { test, expect } from '@playwright/test';

test.describe('Dashboard', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/dashboard');
  });

  test('shows the dashboard heading and stat cards', async ({ page }) => {
    await expect(page.getByRole('heading', { name: 'Dashboard' })).toBeVisible();

    // Four stat cards should be visible
    await expect(page.getByText('Total Contacts')).toBeVisible();
    await expect(page.getByText('Favorites')).toBeVisible();
    await expect(page.getByText('Upcoming Birthdays')).toBeVisible();
    await expect(page.getByText('Pending Reminders')).toBeVisible();
  });

  test('clicking Favorites card navigates to favorites list', async ({ page }) => {
    await page.getByText('Favorites').click();
    await expect(page).toHaveURL(/favoritesOnly=true/);
  });

  test('navigates to dashboard via sidebar link', async ({ page }) => {
    await page.goto('/contacts');
    await page.getByRole('link', { name: 'Dashboard' }).click();
    await expect(page).toHaveURL('/dashboard');
    await expect(page.getByRole('heading', { name: 'Dashboard' })).toBeVisible();
  });
});
