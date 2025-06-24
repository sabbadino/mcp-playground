// test-id-123
import { test, expect } from '@playwright/test';

test('Assing badge first step then navigate to second step', async ({ page }) => {
  // 1. Navigate to the badge page
  await page.goto('http://127.0.0.1:4200/#/certi-badge');

  // 2. Verify menu item 'Badge' is present
  const badgeMenu = page.getByRole('link', { name: 'Badge', exact: true });
  await expect(badgeMenu).toBeVisible();

  // 3. Click on 'Badge' menu item
  await badgeMenu.click();

  // 4. Verify sub menu item 'Tutti i Badge' is present
  const tuttiBadgeMenu = page.getByRole('link', { name: 'Tutti i Badge', exact: true });
  await expect(tuttiBadgeMenu).toBeVisible();

  // 5. Click on 'Tutti i Badge' sub menu item
  await tuttiBadgeMenu.click();

  // 6. Verify at least one button with text 'Assegna' is present
  const assegnaBtn = page.getByRole('button', { name: 'Assegna', exact: true });
  await expect(assegnaBtn).toBeVisible();

  // 7. Click on 'Assegna' button
  await assegnaBtn.click();

  // 8. Verify the page has the text 'Assegnazione Badge - C# Expert'
  await expect(page.getByText('Assegnazione Badge - C# Expert', { exact: true })).toBeVisible();

  // 9. Set value 'test1' for textbox with label 'Nome'
  const nomeInput = page.getByRole('textbox', { name: 'Nome *', exact: true });
  await nomeInput.pressSequentially('test1');
  await nomeInput.press('Enter');

  // 10. Set value '13/06/2025' for textbox with label 'Inizio'
  const inizioInput = page.getByRole('combobox', { name: 'Inizio', exact: true });
  await inizioInput.pressSequentially('13/06/2025');
  await inizioInput.press('Enter');

  // 11. Set value '14/06/2025' for textbox with label 'Fine'
  const fineInput = page.getByRole('combobox', { name: 'Fine', exact: true });
  await fineInput.pressSequentially('14/06/2025');
  await fineInput.press('Enter');

  // 12. Set value '21/06/2025' for textbox with label 'Emissione'
  const emissioneInput = page.getByRole('combobox', { name: 'Emissione *', exact: true });
  await emissioneInput.pressSequentially('21/06/2025');
  await emissioneInput.press('Enter');

  // 13. Set value '22/06/2025' for textbox with label 'Scadenza'
  const scadenzaInput = page.getByRole('combobox', { name: 'Scadenza', exact: true });
  await scadenzaInput.pressSequentially('22/06/2025');
  await scadenzaInput.press('Enter');

  // 14. Set value 'mynote1' for textbox with label 'Note'
  const noteInput = page.getByRole('textbox', { name: 'Note', exact: true });
  await noteInput.pressSequentially('mynote1');
  await noteInput.press('Enter');

  // 15. Click on 'Successivo' button
  const successivoBtn = page.getByRole('button', { name: 'Successivo', exact: true });
  await successivoBtn.click();

  // 16. Verify the page has a button with text 'Seleziona da Anagrafica'
  const selezionaAnagraficaBtn = page.getByRole('button', { name: 'Seleziona da Anagrafica', exact: true });
  await expect(selezionaAnagraficaBtn).toBeVisible();

  // 17. Click on 'Seleziona da Anagrafica'
  await selezionaAnagraficaBtn.click();

  // 18. Verify the page has the text 'Total items'
  await expect(page.getByText('Total items', { exact: false })).toBeVisible();

  // 19. Click on the first row of data
  const firstRow = page.getByRole('row', { name: /giulia verdi/i });
  await firstRow.click();

  // 20. Click on 'Aggiungi' button
  const aggiungiBtn = page.getByRole('button', { name: 'Aggiungi', exact: true });
  await aggiungiBtn.click();

  // 21. Click on 'Successivo' button
  const successivoBtn2 = page.getByRole('button', { name: 'Successivo', exact: true });
  await successivoBtn2.click();

  // 22. Verify the page has the text 'Salva come Gruppo'
  await expect(page.getByText('Salva come Gruppo', { exact: true })).toBeVisible();
});
