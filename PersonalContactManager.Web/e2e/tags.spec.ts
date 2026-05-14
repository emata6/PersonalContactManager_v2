import { test, expect } from '@playwright/test';

test.describe('Tags page', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/tags');
  });

  test('displays page header', async ({ page }) => {
    await expect(page.getByRole('heading', { name: 'Tags' })).toBeVisible();
  });

  test('shows New Tag button', async ({ page }) => {
    await expect(page.getByRole('button', { name: 'New Tag' })).toBeVisible();
  });

  test('opens tag dialog on button click', async ({ page }) => {
    await page.getByRole('button', { name: 'New Tag' }).click();
    await expect(page.getByRole('dialog')).toBeVisible();
    await expect(page.getByPlaceholder('Tag name')).toBeVisible();
  });

  test('closes dialog on cancel', async ({ page }) => {
    await page.getByRole('button', { name: 'New Tag' }).click();
    await page.getByRole('button', { name: 'Cancel' }).click();
    await expect(page.getByRole('dialog')).not.toBeVisible();
  });
});

test.describe('Groups page', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/groups');
  });

  test('displays page header', async ({ page }) => {
    await expect(page.getByRole('heading', { name: 'Groups' })).toBeVisible();
  });

  test('shows New Group button', async ({ page }) => {
    await expect(page.getByRole('button', { name: 'New Group' })).toBeVisible();
  });

  test('opens group dialog on button click', async ({ page }) => {
    await page.getByRole('button', { name: 'New Group' }).click();
    await expect(page.getByRole('dialog')).toBeVisible();
    await expect(page.getByPlaceholder('Group name')).toBeVisible();
  });
});
