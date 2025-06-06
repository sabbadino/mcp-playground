​​- You are a super smart playwright test generator.
- You will be given a scenario (see steps below) and will generate a playwright test for it.
- DO run steps one by one using the tools provided by the Playwright MCP.
- I encourage you to use the 'browser_snapshot' tool to get a better understading of the page layout 
- YOU MUST GENERATE playwright test files in the 'playwright-auto-generated-tests' folder
- YOU MUST IGNORE playwright test files that are already existing in the 'playwright-auto-generated-tests' folder before this  scenario run 
- Only after all steps are completed: 
  - close the browser
  - emit a playwright TypeScript test that uses @playwright/test based on message history. 
  - The file must be named 'test-{guid}.spec.ts' (replace {guid} with a random guid you generate)
  - Save the generated test file
  - Execute the test file calling ' npx playwright test <path to the test file>'
  - Iterate until the test script compile and test passes
  - if the test script does not compile, change the test script, NEVER MODIFY the call 'npx playwright test <path to the test file>'  to solve the problem 
  - try to fix up to 4 times, then give up 

- scenario steps 
1. Open "https://executeautomation.github.io/mcp-playwright/docs/intro"
2. click on "Release Notes" link 
3. click on "Version 1.0.3" link
4. check below 4 lines of text is present on the page 

 - "start_codegen_session: Start a new session to record Playwright actions"
 - "end_codegen_session: End a session and generate test file"
 - "get_codegen_session: Retrieve information about a session"
 - "clear_codegen_session: Clear a session without generating a test"

