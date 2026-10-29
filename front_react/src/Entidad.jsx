import { useEffect, useMemo, useState } from 'react'
import { useParams } from 'react-router-dom'
import { api } from './api.js'
import { ENTIDADES } from './entidades.js'
import { useSesion } from './Sesion.jsx'

// ============================================================
// Entidad.jsx — UNA pantalla para las ocho entidades.
//
// No hay ocho componentes copiados: hay uno que lee el registro y dibuja lo
// que diga. Lo mismo que hacen el front de Flask y el de Blazor.
//
// Los cinco verbos del contrato estan aqui: GET lista, POST crea, PUT
// reemplaza, PATCH modifica y DELETE borra. Si un verbo falta, no es que la
// pantalla no lo ofrezca: es que la API no lo tiene.
// ============================================================

export default function Entidad() {
  const { clave } = useParams()
  const meta = ENTIDADES[clave]
  const { puede } = useSesion()

  const [filas, setFilas] = useState([])
  const [opciones, setOpciones] = useState({})   // las listas de las llaves foraneas
  const [borrador, setBorrador] = useState({})
  const [editando, setEditando] = useState(null) // la llave primaria en edicion
  const [error, setError] = useState(null)
  const [aviso, setAviso] = useState(null)
  const [cargando, setCargando] = useState(true)

  const columnas = useMemo(
    () => (meta?.campos || []).filter((c) => !(meta.ocultarEnLista || []).includes(c.nombre)),
    [meta]
  )

  async function recargar() {
    setCargando(true); setError(null)
    try {
      const r = await api.listar(meta.endpoint)
      setFilas(r?.datos || [])

      // Las llaves foraneas se cargan de la API, no se escriben: por eso el
      // formulario necesita la lista de la OTRA entidad.
      const fks = (meta.campos || []).filter((c) => c.fk)
      const cargadas = {}
      for (const campo of fks) {
        const otra = ENTIDADES[campo.fk]
        const lista = await api.listar(otra.endpoint)
        cargadas[campo.nombre] = { meta: otra, datos: lista?.datos || [] }
      }
      setOpciones(cargadas)
    } catch (e) {
      setError(e.message)
    } finally {
      setCargando(false)
    }
  }

  useEffect(() => {
    if (!meta) return
    setBorrador({}); setEditando(null); setAviso(null)
    recargar()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [clave])

  if (!meta) return <p className="aviso error">No existe esa entidad.</p>

  // El rotulo de una fila de otra entidad: su campo «nombre» si lo tiene.
  function rotulo(entidadMeta, fila) {
    const pk = fila[entidadMeta.pk]
    const texto = fila.nombre || fila.descripcion || fila.ruta || fila.email
    return texto ? `${pk} — ${texto}` : String(pk)
  }

  async function guardar(evento) {
    evento.preventDefault()
    setError(null); setAviso(null)
    try {
      if (editando === null) {
        await api.crear(meta.endpoint, borrador)
        setAviso('Creado.')
      } else {
        // PUT y no PATCH: el formulario manda la fila COMPLETA, y eso es
        // exactamente lo que significa reemplazar.
        await api.reemplazar(meta.endpoint, editando, borrador)
        setAviso('Actualizado.')
      }
      setBorrador({}); setEditando(null)
      await recargar()
    } catch (e) {
      // El 422 de la API trae la lista de campos que no cumplen: se muestra
      // tal cual, porque la validacion de verdad esta alla.
      setError(e.errores ? `${e.message} ${JSON.stringify(e.errores)}` : e.message)
    }
  }

  async function borrar(id) {
    setError(null); setAviso(null)
    try {
      await api.borrar(meta.endpoint, id)
      setAviso('Borrado.')
      await recargar()
    } catch (e) {
      setError(e.message)
    }
  }

  function editar(fila) {
    const copia = {}
    for (const campo of meta.campos) copia[campo.nombre] = fila[campo.nombre] ?? ''
    setBorrador(copia)
    setEditando(fila[meta.pk])
    setAviso(null)
  }

  const camposDelFormulario = meta.campos.filter(
    (c) => !(meta.pkGenerada && c.nombre === meta.pk)
  )

  return (
    <section>
      <h1>{meta.titulo}</h1>
      <p className="apagado">{meta.descripcion}</p>

      {error && <p className="aviso error">{error}</p>}
      {aviso && <p className="aviso ok">{aviso}</p>}

      <form onSubmit={guardar} className="formulario">
        <h2>{editando === null ? 'Nuevo' : `Editando ${editando}`}</h2>
        <div className="campos">
          {camposDelFormulario.map((campo) => (
            <label key={campo.nombre}>
              <span>{campo.etiqueta}</span>
              {campo.fk ? (
                <select
                  value={borrador[campo.nombre] ?? ''}
                  onChange={(e) => setBorrador({ ...borrador, [campo.nombre]: e.target.value })}
                >
                  <option value="">— elija —</option>
                  {(opciones[campo.nombre]?.datos || []).map((fila) => {
                    const otra = opciones[campo.nombre].meta
                    return (
                      <option key={fila[otra.pk]} value={fila[otra.pk]}>
                        {rotulo(otra, fila)}
                      </option>
                    )
                  })}
                </select>
              ) : (
                <input
                  value={borrador[campo.nombre] ?? ''}
                  onChange={(e) => setBorrador({ ...borrador, [campo.nombre]: e.target.value })}
                  disabled={editando !== null && campo.nombre === meta.pk}
                />
              )}
            </label>
          ))}
        </div>
        <div className="acciones">
          <button type="submit">{editando === null ? 'Crear' : 'Guardar'}</button>
          {editando !== null && (
            <button type="button" className="secundario"
                    onClick={() => { setBorrador({}); setEditando(null) }}>
              Cancelar
            </button>
          )}
        </div>
      </form>

      {cargando ? (
        <p className="apagado">Cargando…</p>
      ) : (
        <table>
          <thead>
            <tr>
              {columnas.map((c) => <th key={c.nombre}>{c.etiqueta}</th>)}
              <th className="derecha">Acciones</th>
            </tr>
          </thead>
          <tbody>
            {filas.map((fila) => (
              <tr key={fila[meta.pk]}>
                {columnas.map((c) => <td key={c.nombre}>{String(fila[c.nombre] ?? '')}</td>)}
                <td className="derecha">
                  <button className="secundario" onClick={() => editar(fila)}>Editar</button>
                  <button className="peligro" onClick={() => borrar(fila[meta.pk])}>Borrar</button>
                </td>
              </tr>
            ))}
            {filas.length === 0 && (
              <tr><td colSpan={columnas.length + 1} className="apagado">Sin filas.</td></tr>
            )}
          </tbody>
        </table>
      )}
    </section>
  )
}
