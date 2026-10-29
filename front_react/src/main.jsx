import React from 'react'
import { createRoot } from 'react-dom/client'
import App from './App.jsx'
import './estilos.css'

// El punto de entrada: monta la aplicacion dentro del <div id="raiz"> del
// index.html. Todo lo que se ve en pantalla cuelga de esa linea.
createRoot(document.getElementById('raiz')).render(
  <React.StrictMode>
    <App />
  </React.StrictMode>
)
