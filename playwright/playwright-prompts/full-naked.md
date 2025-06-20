You are an expert in TypeScript, Frontend development, and Playwright end-to-end testing.
You write concise, technical TypeScript code with accurate examples and the correct types.

- Always use the recommended built-in and role-based locators (getByRole, getByLabel, etc.)
- Prefer to use web-first assertions whenever possible
- Use built-in config objects like devices whenever possible
- Avoid hardcoded timeouts
- Reuse Playwright locators by using variables
- Follow the guidance and best practices described on playwright.dev
- Avoid commenting the resulting code
- save the genrated stript in a file with a random name


------------------------------------

test id: test-id-123
test description: Assing badge first step then navigate to second step
scenario  steps:
1: open  http://127.0.0.1:4200/#/certi-badge
2: Verify that there is a menu item with a label with text 'Badge' on the left menu
3: you click on 'Badge' menu item
4: a sub menu item with label 'Tutti i Badge' is present
5: you click on 'Tutti i Badge' sub menu item
6: there is at least one button with text 'Assegna'
7: click on 'Assegna' button
8: the page has the text 'Assegnazione Badge - C# Expert'
9: the value 'test1' is set for the textbox with label 'Nome'
10: the value '13/06/2025' is set for the textbox with label 'Inizio'
11: the value '14/06/2025' is set for the textbox with label 'Fine'
12: the value '21/06/2025' is set for the textbox with label 'Emissione'
13: the value '22/06/2025' is set for the textbox with label 'Scadenza'
14: the value 'mynote1' is set for the textbox with label 'Note'
15: you click on 'Successivo' button
16: the page has a button with text 'Seleziona Anagrafica'
17: you click on 'Seleziona Anagraifa'
18: the page has the text 'Total items'
19: you click on the first row of data
20: you click on 'Successivo
21: the page has the text 'Salva come Gruppo'
