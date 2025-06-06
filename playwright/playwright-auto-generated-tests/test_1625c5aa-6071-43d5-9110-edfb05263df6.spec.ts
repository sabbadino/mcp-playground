
import { test } from '@playwright/test';
import { expect } from '@playwright/test';

test('Test_2025-06-06', async ({ page, context }) => {
  // Navigate to URL
  await page.goto('https://executeautomation.github.io/mcp-playwright/docs/intro', { waitUntil: 'load' });

  // Click on "Release Notes" link
  await page.click('a:has-text("Release Notes")');

  // Click on "Version 1.0.3" link
  await page.click('a:has-text("Version 1.0.3")');

  // Assert the presence of the required 4 lines of text
  await expect(page.locator('text=start_codegen_session: Start a new session to record Playwright actions')).toBeVisible();
  await expect(page.locator('text=end_codegen_session: End a session and generate test file')).toBeVisible();
  await expect(page.locator('text=get_codegen_session: Retrieve information about a session')).toBeVisible();
  await expect(page.locator('text=clear_codegen_session: Clear a session without generating a test')).toBeVisible();
});