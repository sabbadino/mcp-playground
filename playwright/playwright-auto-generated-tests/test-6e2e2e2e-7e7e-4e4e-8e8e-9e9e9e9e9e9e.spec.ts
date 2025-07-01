import { test, expect } from '@playwright/test';

test('Scenario: Open docs intro, click Release Notes, click Version 1.0.3, check for 4 lines of text on the page.', async ({ page }) => {
  // Step 1: Navigate to the docs intro page
  await page.goto('https://executeautomation.github.io/mcp-playwright/docs/intro');

  // Step 2: Click the 'Release Notes' link in the sidebar
  await page.getByRole('link', { name: 'Release Notes', exact: true }).click();

  // Step 3: Click the 'Version 1.0.3' link
  await page.getByRole('link', { name: 'Version 1.0.3', exact: true }).click();

  // Step 4: Assert the required lines of text are present
  await expect(page.getByText('start_codegen_session: Start a new session to record Playwright actions')).toBeVisible();
  await expect(page.getByText('end_codegen_session: End a session and generate test file')).toBeVisible();
  await expect(page.getByText('get_codegen_session: Retrieve information about a session')).toBeVisible();
  await expect(page.getByText('clear_codegen_session: Clear a session without generating a test')).toBeVisible();
});
