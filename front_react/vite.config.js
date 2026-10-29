import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// ============================================================
// La configuracion de Vite, que es corta a proposito.
//
// `base: './'` es la unica linea que no es obvia, y hace falta: hace que el
// HTML compilado pida sus archivos con rutas RELATIVAS -./assets/app.js- en
// vez de absolutas -/assets/app.js-. Asi la carpeta publicada funciona igual
// en la raiz de un sitio que dentro de una subcarpeta.
//
// NO hay proxy ni rescritura de /api: este front le habla a la API por su
// direccion publica, como lo haria cualquier cliente de afuera. Esconder eso
// detras de un proxy en desarrollo seria comodo, y ocultaria justamente lo
// que este front existe para ensenar: que el navegador llama a otro origen,
// y que por eso hace falta CORS.
// ============================================================
export default defineConfig({
  plugins: [react()],
  base: './',
  server: {
    host: '0.0.0.0',
    port: 8119,
    // Dentro de un contenedor, el vigilante de archivos de Linux no se entera
    // de los cambios hechos desde Windows: hay que preguntarle al disco.
    watch: { usePolling: true },
  },
})
