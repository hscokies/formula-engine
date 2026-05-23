import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import {fileURLToPath} from "node:url";

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
      src: fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    cors: false,
    port: 3000,
    proxy: {
      '/api/': {
        target: 'http://localhost:5000/',
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
