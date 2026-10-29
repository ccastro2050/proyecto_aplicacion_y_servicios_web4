import { BrowserRouter, Routes, Route, NavLink, Navigate, useNavigate } from 'react-router-dom'
import { useEffect, useState } from 'react'
import { ProveedorSesion, useSesion } from './Sesion.jsx'
import { ENTIDADES, POR_VERSION } from './entidades.js'
import Entidad from './Entidad.jsx'
import { api, URL_API } from './api.js'

// ============================================================
// App.jsx — el ensamblador: la barra, las rutas y la puerta.
//
// El HTML es el MISMO de base.html del front de Flask, con las mismas clases
// de Bootstrap. Lo unico que cambia es quien lo escribe: alla, Jinja2 en el
// servidor; aqui, React en el navegador.
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
    <div className="row justify-content-center">
      <div className="col-md-6 col-lg-5">
        <h1 className="h3 mb-3">Iniciar sesión</h1>

        {error && <div className="alert alert-danger" role="alert">{error}</div>}

        <div className="card">
          <div className="card-body">
            <form onSubmit={enviar}>
              <div className="mb-3">
                <label className="form-label">Email</label>
                <input className="form-control" value={email} autoComplete="username"
                       onChange={(e) => setEmail(e.target.value)} />
              </div>
              <div className="mb-3">
                <label className="form-label">Contraseña</label>
                <input className="form-control" type="password" value={contrasena}
                       autoComplete="current-password"
                       onChange={(e) => setContrasena(e.target.value)} />
              </div>
              <button className="btn btn-primary" type="submit" disabled={cargando}>
                {cargando ? 'Entrando…' : 'Entrar'}
              </button>
            </form>
          </div>
        </div>

        <p className="text-body-secondary small mt-3">
          Esta interfaz corre <strong>en su navegador</strong> y le habla a la API
          en <code>{URL_API}</code>. Si esa dirección no acepta este origen, el
          navegador bloquea la llamada: eso es CORS.
        </p>
      </div>
    </div>
  )
}

function Inicio() {
  const [diagnostico, setDiagnostico] = useState(null)
  const [error, setError] = useState(null)
  const { puede } = useSesion()

  useEffect(() => {
    api.diagnostico().then(setDiagnostico).catch((e) => setError(e.message))
  }, [])

  return (
    <>
      <h1>Facturas</h1>
      <p className="lead text-body-secondary">
        Versión 4 — la interfaz en <strong>React</strong>
      </p>

      <div className="alert alert-primary" role="alert">
        <strong>Esta interfaz corre en el navegador.</strong> Los otros dos fronts
        del proyecto arman el HTML en el servidor; éste recibe un documento vacío
        y lo llena aquí. Por eso es el único que necesita que la API acepte su
        origen — eso es <strong>CORS</strong>.
      </div>

      <div className="card mb-4">
        <div className="card-body">
          <h2 className="h6 card-title">Contra qué está hablando</h2>
          {error && <div className="alert alert-danger mb-0" role="alert">{error}</div>}
          {diagnostico && (
            <table className="table table-sm mb-0 w-auto">
              <tbody>
                <tr><th scope="row">API</th><td><code>{URL_API}</code></td></tr>
                <tr><th scope="row">Versión</th><td>{diagnostico.version}</td></tr>
                <tr><th scope="row">Motor</th><td><strong>{diagnostico.motor}</strong></td></tr>
              </tbody>
            </table>
          )}
          {!diagnostico && !error && <p className="text-body-secondary mb-0">Preguntándole a la API…</p>}
        </div>
      </div>

      {POR_VERSION.map((grupo) => (
        <div key={grupo.version} className="mb-4">
          <h2 className="h6 text-uppercase text-body-secondary">{grupo.titulo}</h2>
          <div className="row g-3">
            {grupo.claves
              .filter((clave) => puede(ENTIDADES[clave].permiso))
              .map((clave) => (
                <div className="col-md-4" key={clave}>
                  <NavLink to={`/e/${clave}`} className="card h-100 text-decoration-none">
                    <div className="card-body">
                      <h3 className="h6 card-title">{ENTIDADES[clave].titulo}</h3>
                      <p className="card-text small text-body-secondary mb-0">
                        {ENTIDADES[clave].descripcion}
                      </p>
                    </div>
                  </NavLink>
                </div>
              ))}
          </div>
        </div>
      ))}
    </>
  )
}

function Marco({ children }) {
  const { sesion, salir, puede } = useSesion()

  return (
    <>
      <nav className="navbar navbar-expand-lg navbar-dark navbar-marca">
        <div className="container">
          <NavLink to="/" className="navbar-brand logosimbolo mb-0 text-decoration-none">
            <span className="logotipo">Los Andes</span> <span className="isotipo">▲</span>{' '}
            <small className="descriptor">comercial s.a.</small>
          </NavLink>

          <button className="navbar-toggler" type="button" data-bs-toggle="collapse"
                  data-bs-target="#menuPrincipal" aria-controls="menuPrincipal"
                  aria-expanded="false" aria-label="Abrir el menú">
            <span className="navbar-toggler-icon"></span>
          </button>

          {sesion ? (
            <div className="collapse navbar-collapse" id="menuPrincipal">
              <div className="navbar-nav flex-wrap me-auto">
                <NavLink className="nav-link" to="/">Inicio</NavLink>
                {/* El menu se arma con los PERMISOS de quien entro. Y ojo:
                    esconder una entrada NO protege nada — quien escriba la
                    direccion a mano llega igual, y lo que lo para es el 403
                    de la API. */}
                {Object.entries(ENTIDADES)
                  .filter(([, meta]) => puede(meta.permiso))
                  .map(([clave, meta]) => (
                    <NavLink key={clave} className="nav-link" to={`/e/${clave}`}>
                      {meta.titulo}
                    </NavLink>
                  ))}
              </div>
              <div className="d-flex align-items-center gap-2">
                <span className="navbar-text small">
                  {sesion.email}
                  {(sesion.roles || []).map((r) => (
                    <span key={r} className="badge text-bg-light ms-1">{r}</span>
                  ))}
                </span>
                <button className="btn btn-outline-light btn-sm" onClick={salir}>Salir</button>
              </div>
            </div>
          ) : (
            <div className="ms-auto">
              <NavLink className="btn btn-outline-light btn-sm" to="/login">Iniciar sesión</NavLink>
            </div>
          )}
        </div>
      </nav>

      <main className="container py-4">{children}</main>

      <footer className="border-top mt-5 py-4">
        <div className="container text-body-secondary small">
          Aplicación y Servicios Web · <strong>versión 4</strong> · front en React
        </div>
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
