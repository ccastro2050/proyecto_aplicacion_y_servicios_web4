// ============================================================
// formularios.js — leer un campo DIRECTAMENTE del navegador.
//
// POR QUE EXISTE ESTE ARCHIVO, que es la unica razon para meter
// JavaScript en un proyecto de Blazor:
//
// Blazor se entera de lo que usted escribe porque el navegador
// dispara un evento `oninput` en cada tecla. Pero cuando es EL
// NAVEGADOR el que rellena el campo —el autocompletado de Edge o
// de Chrome, el gestor de contrasenas— ESCRIBE EN LA PANTALLA SIN
// DISPARAR NINGUN EVENTO.
//
// Resultado: el campo se ve lleno y la variable de C# sigue vacia.
// La pantalla responde «escriba el correo» con el correo escrito
// delante, y no hay forma de adivinar por que.
//
// Esta funcion lee el valor del DOM tal como esta en ese momento,
// sin depender de ningun evento.
// ============================================================
window.leerCampo = (id) => {
    const e = document.getElementById(id);
    return e ? e.value : "";
};
