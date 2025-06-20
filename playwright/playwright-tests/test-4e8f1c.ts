import { test, expect } from '@playwright/test';

const baseUrl = 'http://127.0.0.1:4200/#/certi-badge';

test.describe('Assing badge first step then navigate to second step', () => {
  test('test-id-123', async ({ page }) => {
    await page.goto(baseUrl);

    const badgeMenu = page.getByRole('menuitem', { name: 'Badge' });
    await expect(badgeMenu).toBeVisible();
    await badgeMenu.click();

    const tuttiBadgeMenu = page.getByRole('menuitem', { name: 'Tutti i Badge' });
    await expect(tuttiBadgeMenu).toBeVisible();
    await tuttiBadgeMenu.click();

    const assegnaButtons = page.getByRole('button', { name: 'Assegna' });
    await expect(assegnaButtons.first()).toBeVisible();
    await assegnaButtons.first().click();

    await expect(page.getByText('Assegnazione Badge - C# Expert')).toBeVisible();

    await expect(page.getByLabel('Nome')).toHaveValue('test1');
    await expect(page.getByLabel('Inizio')).toHaveValue('13/06/2025');
    await expect(page.getByLabel('Fine')).toHaveValue('14/06/2025');
    await expect(page.getByLabel('Emissione')).toHaveValue('21/06/2025');
    await expect(page.getByLabel('Scadenza')).toHaveValue('22/06/2025');
    await expect(page.getByLabel('Note')).toHaveValue('mynote1');

    const successivoBtn = page.getByRole('button', { name: 'Successivo' });
    await successivoBtn.click();

    const selezionaAnagraficaBtn = page.getByRole('button', { name: 'Seleziona Anagrafica' });
    await expect(selezionaAnagraficaBtn).toBeVisible();
    await selezionaAnagraficaBtn.click();

    await expect(page.getByText('Total items')).toBeVisible();

    const firstRow = page.getByRole('row').nth(1);
    await firstRow.click();

    await successivoBtn.click();

    await expect(page.getByText('Salva come Gruppo')).toBeVisible();
  });
});
