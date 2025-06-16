import { test} from './test-parameters.ts';
import { expect } from '@playwright/test';
test('release-notes-codegen-session', async ({ page,testSettings  }) => {
   console.log('startUrl:', testSettings.startUrl );
  // Step 1: Navigate to the intro page
  await page.goto(testSettings.startUrl);

  // Step 2: Verify there is a link with exact text 'Release Notes' present on the sidebar
  await expect(
    page.getByRole('link', { name: 'Release Notes', exact: true })
  ).toBeVisible();

  // Step 3: Click on 'Release Notes' link
  await page.getByRole('link', { name: 'Release Notes', exact: true }).click();

  // Step 4: Click on 'Version 1.0.3' link
  await page.getByRole('link', { name: 'Direct link to Version 1.0.3', exact: true }).click();

  // Step 5: Verify the page contains the text 'start_codegen_session: Start a new session to record Playwright actions'
  await expect(
    page.getByText('start_codegen_session: Start a new session to record Playwright actions', { exact: true })
  ).toBeVisible();

  // Step 6: Verify the page contains the text 'end_codegen_session: End a session and generate test file'
  await expect(
    page.getByText('end_codegen_session: End a session and generate test file', { exact: true })
  ).toBeVisible();

  // Step 7: Verify the page contains the text 'get_codegen_session: Retrieve information about a session'
  await expect(
    page.getByText('get_codegen_session: Retrieve information about a session', { exact: true })
  ).toBeVisible();
});
