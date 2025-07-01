import { test, expect } from '@playwright/test';

test('Release Notes 1.0.3 codegen session lines are present', async ({ page }) => {
  // Step 1: Open the intro page
  await page.goto('https://executeautomation.github.io/mcp-playwright/docs/intro');

  // Step 2: Click on "Release Notes" link in Docs sidebar
  await page.getByRole('link', { name: 'Release Notes', exact: true }).click();

  // Step 3: Click on "Version 1.0.3" link in Docs pages
  await page.getByRole('link', { name: 'Version 1.0.3', exact: true }).click();

  // Step 4: Check for the 4 lines of text individually
  await expect(page.locator('text=start_codegen_session: Start a new session to record Playwright actions')).toBeVisible();
  await expect(page.locator('text=end_codegen_session: End a session and generate test file')).toBeVisible();
  await expect(page.locator('text=get_codegen_session: Retrieve information about a session')).toBeVisible();
  await expect(page.locator('text=clear_codegen_session: Clear a session without generating a test')).toBeVisible();
});
