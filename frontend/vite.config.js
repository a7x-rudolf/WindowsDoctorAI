import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig(({ command }) => ({
  plugins: [react()],
  // base './' hanya untuk production build (Electron)
  // dev server pakai '/' agar asset paths benar
  base: command === 'build' ? './' : '/',
  server: {
    port: 5173,
  },
}))