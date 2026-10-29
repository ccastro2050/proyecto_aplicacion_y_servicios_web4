import { useEffect, useMemo, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { api } from './api.js'

// ============================================================
// Facturas.jsx — el maestro-detalle, que es OTRA cosa que un CRUD.
//
// Las ocho entidades de Entidad.jsx caben en una sola pantalla generica
// porque todas se comportan igual: se listan, se crean, se reemplazan y se
// borran. La factura no cabe ahi, y no por capricho del front:
//
//   NO TIENE PUT NI PATCH. Una factura emitida es un documento: no se
//   corrige, se ANULA y se hace otra. De ahi que el boton rojo sea
//   POST /api/factura/{numero}/anular y no DELETE.
//
//   SE EMITE DE UN SOLO ENVIO, con el maestro y los renglones juntos. Una
//   factura con tres renglones no son cuatro peticiones: si la tercera
//   fallara quedaria media factura en la base de datos, y «media factura»
//   no es un estado que el negocio reconozca. La API lo mete todo en un
//   procedimiento almacenado, dentro de UNA transaccion.
//
//   EL SOBRE ES DISTINTO. GET /api/factura devuelve {tabla, total, datos},
//   como el resto. Pero GET /api/factura/7 devuelve LA FACTURA, no una
//   lista: una factura no es una lista.
//
// LA DIFERENCIA CON EL FRONT DE FLASK: alla cada boton —agregar un renglon,
// quitarlo— es una peticion al servidor, y el borrador vive en la sesion.
// Aqui el borrador vive en la memoria del navegador y no hay ni un viaje
// hasta que se pulsa «Emitir». Lo que se ve y lo que se puede hacer es lo
// mismo; lo unico que cambia es donde se guarda lo que esta a medio armar.
// ============================================================

// Los pesos se muestran sin decimales y con separador de miles, como en los
// otros dos fronts —alla lo hace "{:,.0f}".format de Jinja2—.
const pesos = (valor) =>
  new Intl.NumberFormat('es-CO', { maximumFractionDigits: 0 }).format(Number(valor || 0))

const soloFecha = (fecha) => String(fecha || '').slice(0, 10)

function Estado({ estado }) {
  return (
    <span className={`badge ${estado === 'activa' ? 'bg-success' : 'bg-danger'}`}>
      {estado}
    </span>
  )
}

// ------------------------------------------------------------
// La LISTA — el maestro
// ------------------------------------------------------------
export function ListaFacturas() {
  const [facturas, setFacturas] = useState([])
  const [error, setError] = useState(null)
  const [aviso, setAviso] = useState(null)
  const [cargando, setCargando] = useState(true)

  async function recargar() {
    setCargando(true); setError(null)
    try {
      const r = await api.listarFacturas()
      // Ojo con el 204: si no hay ni una factura, la API no manda cuerpo y
      // `pedir` devuelve null. Sin ese `?.` la pantalla se rompe con la base
      // de datos vacia — justo el dia de la primera demostracion.
      setFacturas(r?.datos || [])
    } catch (e) {
      setError(e.message)
    } finally {
      setCargando(false)
    }
  }

  useEffect(() => { recargar() }, [])

  async function anular(numero) {
    if (!window.confirm(`¿Anular la factura ${numero}? El stock se restaura.`)) return
    setError(null); setAviso(null)
    try {
      await api.anularFactura(numero)
      setAviso(`Factura ${numero} anulada — el disparador devolvió el stock.`)
      await recargar()
    } catch (e) {
      setError(e.message)
    }
  }

  return (
    <>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h1 className="h2 mb-0">Facturas</h1>
        <Link className="btn btn-primary" to="/facturas/nueva">Nueva factura</Link>
      </div>

      {error && <div className="alert alert-danger" role="alert">{error}</div>}
      {aviso && <div className="alert alert-success" role="alert">{aviso}</div>}

      {cargando ? (
        <p className="text-body-secondary">Cargando…</p>
      ) : facturas.length === 0 ? (
        <div className="alert alert-secondary">Todavía no hay facturas.</div>
      ) : (
        <div className="table-responsive">
          <table className="table table-striped align-middle">
            <thead>
              <tr>
                <th>No.</th><th>Fecha</th><th>Cliente</th><th>Vendedor</th>
                <th className="text-end">Total</th><th>Estado</th>
                <th className="text-end">Acciones</th>
              </tr>
            </thead>
            <tbody>
              {facturas.map((f) => (
                <tr key={f.numero}>
                  <td className="font-monospace">{f.numero}</td>
                  <td>{soloFecha(f.fecha)}</td>
                  <td>{f.nombre_cliente}</td>
                  <td>{f.nombre_vendedor}</td>
                  <td className="text-end font-monospace">{pesos(f.total)}</td>
                  <td><Estado estado={f.estado} /></td>
                  <td className="text-end">
                    <Link className="btn btn-sm btn-outline-primary me-1"
                          to={`/facturas/${f.numero}`}>Ver</Link>
                    {f.estado === 'activa' && (
                      <button className="btn btn-sm btn-danger"
                              onClick={() => anular(f.numero)}>Anular</button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </>
  )
}

// ------------------------------------------------------------
// El DETALLE — la factura con sus renglones
// ------------------------------------------------------------
export function DetalleFactura() {
  const { numero } = useParams()
  const [f, setF] = useState(null)
  const [error, setError] = useState(null)

  useEffect(() => {
    setF(null); setError(null)
    api.obtenerFactura(numero).then(setF).catch((e) => setError(e.message))
  }, [numero])

  if (error) return <div className="alert alert-danger" role="alert">{error}</div>
  if (!f) return <p className="text-body-secondary">Cargando…</p>

  return (
    <>
      <div className="d-flex justify-content-between align-items-center mb-3">
        <h1 className="h2 mb-0">Factura {f.numero} <Estado estado={f.estado} /></h1>
        <Link className="btn btn-outline-secondary" to="/facturas">Volver</Link>
      </div>

      <div className="card shadow-sm mb-3">
        <div className="card-body">
          <div className="row">
            <div className="col-6 col-md-3"><strong>Fecha:</strong><br />{soloFecha(f.fecha)}</div>
            <div className="col-6 col-md-3"><strong>Cliente:</strong><br />{f.nombre_cliente}</div>
            <div className="col-6 col-md-3"><strong>Vendedor:</strong><br />{f.nombre_vendedor}</div>
            <div className="col-6 col-md-3"><strong>Total:</strong><br />
              <span className="font-monospace">{pesos(f.total)}</span></div>
          </div>
        </div>
      </div>

      <h2 className="h4">Renglones (los subtotales los calculó la base de datos)</h2>
      <div className="table-responsive">
        <table className="table table-striped">
          <thead>
            <tr>
              <th>Producto</th><th>Nombre</th>
              <th className="text-end">Cantidad</th>
              <th className="text-end">Valor unitario</th>
              <th className="text-end">Subtotal</th>
            </tr>
          </thead>
          <tbody>
            {(f.productos || []).map((r) => (
              <tr key={r.codigo_producto}>
                <td className="font-monospace">{r.codigo_producto}</td>
                <td>{r.nombre_producto}</td>
                <td className="text-end font-monospace">{r.cantidad}</td>
                <td className="text-end font-monospace">{pesos(r.valorunitario)}</td>
                <td className="text-end font-monospace">{pesos(r.subtotal)}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </>
  )
}

// ------------------------------------------------------------
// EMITIR — el formulario
// ------------------------------------------------------------
export function NuevaFactura() {
  const navegar = useNavigate()

  const [clientes, setClientes] = useState([])
  const [vendedores, setVendedores] = useState([])
  const [productos, setProductos] = useState([])

  const [cliente, setCliente] = useState('')
  const [vendedor, setVendedor] = useState('')
  const [renglones, setRenglones] = useState([])     // el DETALLE a medio armar
  const [codigo, setCodigo] = useState('')
  const [cantidad, setCantidad] = useState('1')

  const [error, setError] = useState(null)
  const [emitiendo, setEmitiendo] = useState(false)

  useEffect(() => {
    // El nombre del cliente NO esta en `cliente`: ahi solo vive la clave
    // foranea `fkcodpersona`. El nombre hay que ir a buscarlo a `persona` —
    // la misma razon por la que la consulta 2 del tablero cruza cuatro
    // tablas. Y la llave de `persona` es `codigo` —"P001"—, no `id`.
    Promise.all([
      api.listar('/api/cliente'),
      api.listar('/api/vendedor'),
      api.listar('/api/persona'),
      api.listar('/api/producto'),
    ]).then(([c, v, p, pr]) => {
      const nombre = {}
      for (const persona of p?.datos || []) nombre[persona.codigo] = persona.nombre || ''
      setClientes((c?.datos || []).map((x) => ({
        valor: String(x.id),
        texto: `${nombre[x.fkcodpersona] || '?'} (cliente ${x.id})`,
      })))
      setVendedores((v?.datos || []).map((x) => ({
        valor: String(x.id),
        texto: `${nombre[x.fkcodpersona] || '?'} (carnet ${x.carnet || ''})`,
      })))
      setProductos(pr?.datos || [])
    }).catch((e) => setError(e.message))
  }, [])

  const precios = useMemo(() => {
    const m = {}
    for (const p of productos) m[p.codigo] = p
    return m
  }, [productos])

  // El total que se ve. NO se envia: lo calcula el disparador.
  const totalEstimado = renglones.reduce((s, r) => s + r.cantidad * r.precio, 0)

  function agregar() {
    setError(null)
    const p = precios[codigo]
    if (!p) { setError('Elija un producto.'); return }
    const n = parseInt(cantidad, 10)
    if (!Number.isInteger(n) || n < 1) { setError('La cantidad tiene que ser 1 o más.'); return }

    // El mismo producto dos veces SUMA, no repite el renglon: dos lineas del
    // mismo producto en la misma factura no son dos hechos distintos.
    const ya = renglones.find((r) => r.codigo === codigo)
    if (ya) {
      setRenglones(renglones.map((r) =>
        r.codigo === codigo ? { ...r, cantidad: r.cantidad + n } : r))
    } else {
      setRenglones([...renglones, {
        codigo,
        cantidad: n,
        nombre: p.nombre || codigo,
        precio: Number(p.valorunitario || 0),
      }])
    }
    setCantidad('1')
  }

  function quitar(cod) {
    setRenglones(renglones.filter((r) => r.codigo !== cod))
  }

  function limpiar() {
    setCliente(''); setVendedor(''); setRenglones([])
    setCodigo(''); setCantidad('1'); setError(null)
  }

  async function emitir() {
    setError(null)
    if (!cliente || !vendedor) { setError('Elija el cliente y el vendedor.'); return }
    if (renglones.length === 0) { setError('Agregue al menos un renglón.'); return }

    setEmitiendo(true)
    try {
      // VIAJAN el cliente, el vendedor y los renglones. NO viajan el subtotal
      // ni el total: mandarlos seria tener la misma regla en dos lugares, y
      // el dia que difieran ganaria la que nadie revisa.
      const factura = await api.crearFactura({
        fkidcliente: parseInt(cliente, 10),
        fkidvendedor: parseInt(vendedor, 10),
        productos: renglones.map((r) => ({ codigo: r.codigo, cantidad: r.cantidad })),
      })
      navegar(`/facturas/${factura.numero}`)
    } catch (e) {
      // Aqui cae el stock insuficiente: ese mensaje lo escribio el DISPARADOR
      // de la base de datos, no la API y menos este front.
      setError(e.message)
    } finally {
      setEmitiendo(false)
    }
  }

  const faltanDatos = clientes.length === 0 || vendedores.length === 0 || productos.length === 0

  return (
    <>
      <h1 className="h2 mb-3">Facturas</h1>

      {error && <div className="alert alert-danger" role="alert">{error}</div>}

      <section className="card mb-4">
        <div className="card-body">
          <h2 className="h5 card-title mb-3">Emitir una factura</h2>

          {/* EL MAESTRO: a quien y quien vende */}
          <div className="row g-3">
            <div className="col-12 col-md-6">
              <label className="form-label" htmlFor="fkidcliente">Cliente</label>
              <select className="form-select" id="fkidcliente" value={cliente}
                      onChange={(e) => setCliente(e.target.value)}>
                <option value="">— elija un cliente —</option>
                {clientes.map((c) => <option key={c.valor} value={c.valor}>{c.texto}</option>)}
              </select>
            </div>
            <div className="col-12 col-md-6">
              <label className="form-label" htmlFor="fkidvendedor">Vendedor</label>
              <select className="form-select" id="fkidvendedor" value={vendedor}
                      onChange={(e) => setVendedor(e.target.value)}>
                <option value="">— elija un vendedor —</option>
                {vendedores.map((v) => <option key={v.valor} value={v.valor}>{v.texto}</option>)}
              </select>
            </div>
          </div>

          <hr />

          {/* EL DETALLE: los renglones, que se arman AQUI sin tocar la API */}
          <h2 className="h5">Los renglones</h2>
          <div className="row g-3 align-items-end">
            <div className="col-12 col-md-7">
              <label className="form-label" htmlFor="codigo">Producto</label>
              <select className="form-select" id="codigo" value={codigo}
                      onChange={(e) => setCodigo(e.target.value)}>
                <option value="">— elija un producto —</option>
                {productos.map((p) => (
                  <option key={p.codigo} value={p.codigo}>
                    {p.nombre} — {pesos(p.valorunitario)} (stock {p.stock})
                  </option>
                ))}
              </select>
            </div>
            <div className="col-6 col-md-3">
              <label className="form-label" htmlFor="cantidad">Cantidad</label>
              <input className="form-control" type="number" min="1" id="cantidad"
                     value={cantidad} onChange={(e) => setCantidad(e.target.value)} />
            </div>
            <div className="col-6 col-md-2">
              {/* Este boton NO habla con la API: agrega a la tabla de abajo. */}
              <button className="btn btn-outline-primary w-100" type="button"
                      onClick={agregar}>Agregar al detalle</button>
            </div>
          </div>

          {renglones.length > 0 ? (
            <>
              <div className="table-responsive mt-3">
                <table className="table table-sm align-middle">
                  <thead>
                    <tr>
                      <th>Producto</th>
                      <th className="text-end">Cantidad</th>
                      <th className="text-end">Valor unitario</th>
                      <th className="text-end">Subtotal</th>
                      <th></th>
                    </tr>
                  </thead>
                  <tbody>
                    {renglones.map((r) => (
                      <tr key={r.codigo}>
                        <td>{r.nombre}</td>
                        <td className="text-end font-monospace">{r.cantidad}</td>
                        <td className="text-end font-monospace">{pesos(r.precio)}</td>
                        <td className="text-end font-monospace">{pesos(r.cantidad * r.precio)}</td>
                        <td className="text-end">
                          {/* Quitar un renglon tampoco llama a la API:
                              todavia no se ha enviado nada. */}
                          <button className="btn btn-sm btn-danger" type="button"
                                  onClick={() => quitar(r.codigo)}>Quitar</button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                  <tfoot>
                    <tr>
                      <td colSpan={3} className="text-end"><strong>Total estimado</strong></td>
                      <td className="text-end font-monospace"><strong>{pesos(totalEstimado)}</strong></td>
                      <td></td>
                    </tr>
                  </tfoot>
                </table>
              </div>
              <p className="small text-body-secondary">
                Este total es un cálculo para que usted sepa cuánto va.
                <strong> No se envía:</strong> el total que queda guardado lo calcula
                el disparador de la base de datos, y es el que manda.
              </p>
            </>
          ) : (
            <div className="alert alert-secondary mt-3 text-center">Agregue al menos un renglón.</div>
          )}

          <div className="mt-3 d-flex flex-wrap gap-2 align-items-center">
            {/* UN SOLO ENVIO con el maestro y el detalle juntos. */}
            <button className="btn btn-primary" type="button" onClick={emitir} disabled={emitiendo}>
              {emitiendo ? 'Emitiendo…' : 'Emitir la factura'}
            </button>
            <button className="btn btn-link" type="button" onClick={limpiar}>Empezar de nuevo</button>
            <Link className="btn btn-link" to="/facturas">Volver a las facturas</Link>
          </div>

          {faltanDatos && (
            <p className="small text-body-secondary mt-3 mb-0">
              Para emitir una factura hacen falta un <Link to="/e/cliente">cliente</Link>, un{' '}
              <Link to="/e/vendedor">vendedor</Link> y al menos un{' '}
              <Link to="/e/producto">producto</Link>.
            </p>
          )}
        </div>
      </section>
    </>
  )
}
