import { configDefaults, defineConfig } from 'vitest/config';

export default defineConfig({
  test: {
    environment: 'jsdom',
    setupFiles: ['./tests/setup.ts'],
    exclude: [...configDefaults.exclude, 'dist/**'],
    globals: true,
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html', 'lcov'],
      reportsDirectory: './coverage',
      thresholds: {
        lines: 70,
        functions: 70,
        branches: 70,
        statements: 70
      },
      include: ['src/**/*.ts'],
      exclude: ['node_modules', 'dist', 'tests', '**/*.test.ts', '**/*.d.ts']
    },
    testTimeout: 10000,
    reporters: ['verbose'],
    mockReset: true,
    restoreMocks: true,
    clearMocks: true
  }
});
