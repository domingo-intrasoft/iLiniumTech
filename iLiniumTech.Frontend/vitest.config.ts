import { mergeConfig, defineConfig } from 'vitest/config'
import viteConfig from './vite.config'

export default mergeConfig(
  viteConfig,
  defineConfig({
    test: {
      environment: 'happy-dom',
      globals: false,
      include: ['src/**/*.test.ts'],
      reporters: ['default'],
      outputFile: {
        junit: 'reports/vitest-junit.xml',
      },
      coverage: {
        provider: 'v8',
        reporter: ['text', 'html', 'lcov'],
        reportsDirectory: 'reports/coverage',
        include: ['src/**/*.{ts,vue}'],
        exclude: ['src/**/*.test.ts', 'src/main.ts', 'src/router/**', 'src/types/**', 'e2e/**'],
      },
      exclude: ['e2e/**'],
    },
  }),
)
