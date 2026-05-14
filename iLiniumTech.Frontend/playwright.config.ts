import { defineConfig, devices } from '@playwright/test'
import process from 'node:process'

const isCi = Boolean(process.env.CI)
const previewPort = 4173
const previewHost = '127.0.0.1'

export default defineConfig({
  testDir: './tests/e2e',
  testMatch: '**/*.e2e.ts',
  fullyParallel: false,
  forbidOnly: isCi,
  retries: isCi ? 1 : 0,
  workers: 1,
  timeout: 30_000,
  outputDir: 'reports/playwright-test-results',
  reporter: isCi
    ? [['line'], ['junit', { outputFile: 'reports/playwright-junit.xml' }]]
    : [['list']],
  use: {
    baseURL: `http://${previewHost}:${previewPort}`,
    screenshot: 'off',
    trace: 'off',
    video: 'off',
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],
  webServer: {
    command: `npm run build && npm run preview -- --host ${previewHost} --port ${previewPort}`,
    env: {
      VITE_USE_BACKEND: 'false',
    },
    url: `http://${previewHost}:${previewPort}/polizas`,
    reuseExistingServer: !isCi,
    timeout: 120_000,
  },
})
