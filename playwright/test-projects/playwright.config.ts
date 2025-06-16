import { defineConfig } from '@playwright/test';
import type { TestOptions } from './test-parameters.ts';

export default defineConfig<TestOptions>({
  projects: [
    {
      name: 'development',
       use: { testSettings:  { startUrl: 'https://executeautomation.github.io/mcp-playwright/docs/intro' }},
    }
  ]
});