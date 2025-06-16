import { test as base } from '@playwright/test';

export type TestSettings = {
  startUrl: string;
};

export type TestOptions = {
   testSettings: TestSettings;
};

export const test = base.extend<TestOptions>({
   testSettings: [ { startUrl: '' }, { option: true }],
});

export { expect } from '@playwright/test';
