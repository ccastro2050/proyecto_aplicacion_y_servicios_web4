import React from 'react'
import { createRoot } from 'react-dom/client'
import App from './App.jsx'

// No hay hoja de estilos propia: la marca entra por el index.html, con los
// MISMOS archivos que los otros dos fronts.
createRoot(document.getElementById('raiz')).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
)
