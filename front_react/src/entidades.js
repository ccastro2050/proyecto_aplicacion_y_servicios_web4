// ============================================================
// entidades.js — El REGISTRO del front.
//
// Es el mismo de `front_flask/entidades.py`, traducido. Describe cada entidad
// -endpoint, llave primaria, campos, llaves foraneas, permiso- y una sola
// pantalla generica dibuja las diez.
//
// POR QUE AQUI SI VALE LO GENERICO Y EN LA API NO:
//
//   La API expone un CONTRATO del que otros dependen. Un `/api/{tabla}` deja
//   ese contrato en blanco: Swagger no dice que recursos hay y los permisos
//   no se pueden dar por recurso.
//
//   Este archivo no expone nada: es la configuracion de UNA aplicacion, y el
//   contrato que consume sigue siendo especifico.
//
// LAS REGLAS DE NEGOCIO SIGUEN TODAS EN LA API. Esto dice como se DIBUJA cada
// entidad, no que se puede hacer con ella: el 403 lo sigue dando la API.
//
// Cada campo es { nombre, etiqueta, fk }. Cuando `fk` trae la clave de otra
// entidad, el formulario lo vuelve un <select> cargado desde la API: la llave
// foranea se ELIGE, no se escribe.
// ============================================================

export const ENTIDADES = {
  // ── La v1: las tablas SIN clave foranea ──────────────────────────────
  producto: {
    url: 'productos', titulo: 'Productos', endpoint: '/api/producto', pk: 'codigo',
    desde: 1, permiso: 'interfaz.productos',
    descripcion: 'Código, nombre, stock y valor unitario',
    campos: [
      { nombre: 'codigo', etiqueta: 'Código' },
      { nombre: 'nombre', etiqueta: 'Nombre' },
      { nombre: 'stock', etiqueta: 'Stock' },
      { nombre: 'valorunitario', etiqueta: 'Valor unitario' },
    ],
  },
  empresa: {
    url: 'empresas', titulo: 'Empresas', endpoint: '/api/empresa', pk: 'codigo',
    desde: 1, permiso: 'interfaz.empresas',
    descripcion: 'Las empresas a las que puede pertenecer un cliente',
    campos: [
      { nombre: 'codigo', etiqueta: 'Código' },
      { nombre: 'nombre', etiqueta: 'Nombre' },
    ],
  },
  persona: {
    url: 'personas', titulo: 'Personas', endpoint: '/api/persona', pk: 'codigo',
    desde: 1, permiso: 'interfaz.personas',
    descripcion: 'Quiénes son clientes y vendedores',
    campos: [
      { nombre: 'codigo', etiqueta: 'Código' },
      { nombre: 'nombre', etiqueta: 'Nombre' },
      { nombre: 'email', etiqueta: 'Email' },
      { nombre: 'telefono', etiqueta: 'Teléfono' },
    ],
  },
  rol: {
    url: 'roles', titulo: 'Roles', endpoint: '/api/rol', pk: 'id', pkGenerada: true,
    desde: 1, permiso: 'interfaz.roles',
    descripcion: 'Administrador, Vendedor, Cajero, Contador, Cliente',
    campos: [{ nombre: 'nombre', etiqueta: 'Nombre' }],
  },
  ruta: {
    url: 'rutas', titulo: 'Rutas', endpoint: '/api/ruta', pk: 'id', pkGenerada: true,
    desde: 1, permiso: 'interfaz.rutas',
    descripcion: 'Las interfaces y acciones protegibles',
    campos: [
      { nombre: 'ruta', etiqueta: 'Ruta' },
      { nombre: 'descripcion', etiqueta: 'Descripción' },
    ],
  },
  usuario: {
    url: 'usuarios', titulo: 'Usuarios', endpoint: '/api/usuario', pk: 'email',
    desde: 1, permiso: 'interfaz.usuarios',
    descripcion: 'Quiénes pueden entrar al sistema',
    // La contrasena se escribe al crear, pero NO se lista: la API no la
    // devuelve, y mostrar una columna vacia solo confunde.
    ocultarEnLista: ['contrasena'],
    campos: [
      { nombre: 'email', etiqueta: 'Email' },
      { nombre: 'contrasena', etiqueta: 'Contraseña' },
    ],
  },

  // ── La v2: las tablas CON clave foranea ──────────────────────────────
  cliente: {
    url: 'clientes', titulo: 'Clientes', endpoint: '/api/cliente', pk: 'id', pkGenerada: true,
    desde: 2, permiso: 'interfaz.clientes',
    descripcion: 'Con su persona y su empresa, elegidas de un desplegable',
    campos: [
      { nombre: 'credito', etiqueta: 'Crédito' },
      { nombre: 'fkcodpersona', etiqueta: 'Persona', fk: 'persona' },
      { nombre: 'fkcodempresa', etiqueta: 'Empresa (opcional)', fk: 'empresa' },
    ],
  },
  vendedor: {
    url: 'vendedores', titulo: 'Vendedores', endpoint: '/api/vendedor', pk: 'id', pkGenerada: true,
    desde: 2, permiso: 'interfaz.vendedores',
    descripcion: 'Con su carnet y su persona',
    campos: [
      { nombre: 'carnet', etiqueta: 'Carnet' },
      { nombre: 'direccion', etiqueta: 'Dirección' },
      { nombre: 'fkcodpersona', etiqueta: 'Persona', fk: 'persona' },
    ],
  },
}

// El orden del menu, por version, como en los otros dos fronts.
export const POR_VERSION = [
  { version: 1, titulo: 'De la versión 1 — sin clave foránea',
    claves: ['producto', 'empresa', 'persona', 'rol', 'ruta', 'usuario'] },
  { version: 2, titulo: 'De la versión 2 — con clave foránea',
    claves: ['cliente', 'vendedor'] },
]
