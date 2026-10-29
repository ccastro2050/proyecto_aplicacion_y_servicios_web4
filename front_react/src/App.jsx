import { BrowserRouter, Routes, Route, NavLink, Navigate, useNavigate } from 'react-router-dom'
import { useEffect, useState } from 'react'
import { ProveedorSesion, useSesion } from './Sesion.jsx'
import { ENTIDADES, POR_VERSION } from './entidades.js'
import Entidad from './Entidad.jsx'
import { api, URL_API } from './api.js'

// ============================================================
// App.jsx — el ensamblador: el menu, las rutas y la puerta.
// ============================================================

function Login() {
  const { entrar, cargando } = useSesion()
  const navegar = useNavigate()
  const [email, setEmail] = useState('admin@correo.com')
  const [contrasena, setContrasena] = useState('')
  const [error, setError] = useState(null)

  async function enviar(e) {
    e.preventDefault()
    const falla = await entrar(email, contrasena)
    if (falla) setError(falla)
    else navegar('/')
  }

  return (
    <section className="angosto">
      <h1>Iniciar sesión</h1>
      {error && <p className="aviso error">{error}</p>}
      <form onSubmit={enviar} className="formulario">
        <div className="campos">
          <label>
            <span>Email</span>
            <input value={email} onChange={(e) => setEmail(e.target.value)} autoComplete="username" />
          </label>
          <label>
            <span>Contraseña</span>
            <input type="password" value={contrasena}
                   onChange={(e) => setContrasena(e.target.value)} autoComplete="current-password" />
          </label>
        </div>
        <div className="acciones">
          <button type="submit" disabled={cargando}>{cargando ? 'Entrando…' : 'Entrar'}</button>
        </div>
      </form>
      <p className="apagado">
        Esta interfaz corre <strong>en su navegador</strong> y le habla a la API
        en <code>{URL_API}</code>. Si esa dirección no acepta este origen, el
        navegador bloquea la llamada: eso es CORS.
      </p>
    </section>
  )
}

function Inicio() {
  const [diagnostico, setDiagnostico] = useState(null)
  const [error, setError] = useState(null)

  useEffect(() => {
    api.diagnostico().then(setDiagnostico).catch((e) => setError(e.message))
  }, [])

  return (
    <section>
      <h1>Facturas</h1>
      <p className="apagado">Versión 4 — la interfaz en <strong>React</strong></p>

      <div className="tarjeta">
        <h2>Contra qué está hablando</h2>
        {error && <p className="aviso error">{error}</p>}
        {diagnostico ? (
          <table className="angosta">
            <tbody>
              <tr><th>API</th><td><code>{URL_API}</code></td></tr>
              <tr><th>Versión</th><td>{diagnostico.version}</td></tr>
              <tr><th>Motor</th><td><strong>{diagnostico.motor}</strong></td></tr>
            </tbody>
          </table>
        ) : (!error && <p className="apagado">Preguntándole a la API…</p>)}
        <p className="apagado">
          Ese <strong>motor</strong> es lo único que distingue a un sistema del
          otro desde afuera. La interfaz es la misma.
        </p>
      </div>

      {POR_VERSION.map((grupo) => (
        <div key={grupo.version}>
          <h2>{grupo.titulo}</h2>
          <div className="tarjetas">
            {grupo.claves.map((clave) => (
              <NavLink key={clave} to={`/e/${clave}`} className="tarjeta enlace">
                <strong>{ENTIDADES[clave].titulo}</strong>
                <span className="apagado">{ENTIDADES[clave].descripcion}</span>
              </NavLink>
            ))}
          </div>
        </div>
      ))}
    </section>
  )
}

function Marco({ children }) {
  const { sesion, salir, puede } = useSesion()
  return (
    <>
      <header>
        <div className="contenedor barra">
          <NavLink to="/" className="marca">Los Andes <span>comercial s.a.</span></NavLink>
          <nav>
            {Object.entries(ENTIDADES)
              .filter(([, meta]) => puede(meta.permiso))
              .map(([clave, meta]) => (
                <NavLink key={clave} to={`/e/${clave}`}>{meta.titulo}</NavLink>
              ))}
          </nav>
        </div>
      </header>
      <main className="contenedor">
        {sesion && (
          <div className="sesion">
            <span><strong>{sesion.email}</strong> {sesion.roles?.join(', ')}</span>
            <button className="secundario" onClick={salir}>Salir</button>
          </div>
        )}
        {children}
      </main>
      <footer className="contenedor apagado">
        Aplicación y Servicios Web · <strong>versión 4</strong> · front en React
      </footer>
    </>
  )
}

// Sin sesion, toda ruta devuelve al login. NO es la proteccion —esa es el 401
// de la API— sino cortesia: sin token, la persona veria un error en vez de
// entender que le falta identificarse.
function Protegida({ children }) {
  const { sesion } = useSesion()
  return sesion ? children : <Navigate to="/login" replace />
}

export default function App() {
  return (
    <ProveedorSesion>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Marco><Login /></Marco>} />
          <Route path="/" element={<Protegida><Marco><Inicio /></Marco></Protegida>} />
          <Route path="/e/:clave" element={<Protegida><Marco><Entidad /></Marco></Protegida>} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </ProveedorSesion>
  )
}
