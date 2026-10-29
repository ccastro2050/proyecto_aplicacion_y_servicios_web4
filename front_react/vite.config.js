import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// ============================================================
// La configuracion de Vite, que es corta a proposito.
//
// `base: '/'` -rutas ABSOLUTAS- y no './'. La diferencia importa por el
// web.config de public/: cuando alguien entra a /e/persona, IIS entrega el
// index.html de la raiz. Con rutas relativas, ese HTML pediria sus archivos
// a /e/assets/... -que no existe- y la pagina saldria en blanco.
//
// NO hay proxy ni rescritura de /api: este front le habla a la API por su
// direccion publica, como lo haria cualquier cliente de afuera. Esconder eso
// detras de un proxy en desarrollo seria comodo, y ocultaria justamente lo
// que este front existe para ensenar: que el navegador llama a otro origen,
// y que por eso hace falta CORS.
// ============================================================
export default defineConfig({
  plugins: [react()],
  base: '/',
  server: {
    host: '0.0.0.0',
    port: 8119,
    // Dentro de un contenedor, el vigilante de archivos de Linux no se entera
    // de los cambios hechos desde Windows: hay que preguntarle al disco.
    watch: { usePolling: true },
  },
})
