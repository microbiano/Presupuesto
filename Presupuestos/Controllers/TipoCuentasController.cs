using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Presupuestos.Models;
using Presupuestos.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Presupuestos.Controllers
{
    public class TipoCuentasController : Controller
    {
        private readonly IRepositorioTipoCuentas repositorioTipoCuentas;
        private readonly IServicioUsuarios servicioUsuarios;

        public TipoCuentasController(IRepositorioTipoCuentas repositorioTipoCuentas, IServicioUsuarios servicioUsuarios)
        {
            this.repositorioTipoCuentas = repositorioTipoCuentas;
            this.servicioUsuarios = servicioUsuarios;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tiposcuentas = await repositorioTipoCuentas.Obtener(usuarioId);
            return View(tiposcuentas);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuentas)
        {
            if (!ModelState.IsValid)
            {
                return View(tipoCuentas);
            }

            tipoCuentas.UsuarioId = servicioUsuarios.ObtenerUsuarioId();

            var existe = await repositorioTipoCuentas.Existe(tipoCuentas.Nombre, tipoCuentas.UsuarioId);

            if (existe)
            {
                ModelState.AddModelError(nameof(tipoCuentas.Nombre), $"El nombre {tipoCuentas.Nombre} ya existe");
                return View(tipoCuentas);
            }

            await repositorioTipoCuentas.Crear(tipoCuentas);


            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<ActionResult>Editar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipocuenta =  await repositorioTipoCuentas.ObtenerPorId(id, usuarioId);
            if (tipocuenta is null)
            {
                return RedirectToAction("NoEncontrado","Home");
            }
            return View(tipocuenta);
        }

        [HttpPost]
        public async Task<ActionResult> Editar(TipoCuenta tipoCuenta)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuentaExiste = await repositorioTipoCuentas.ObtenerPorId(tipoCuenta.Id,usuarioId);

            if (cuentaExiste is null)
            {
                return RedirectToAction("Cuenta no encontrada", "Home");
            }

            await repositorioTipoCuentas.Actualizar(tipoCuenta);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Borrar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuentaExiste = await repositorioTipoCuentas.ObtenerPorId(id, usuarioId);
            if (cuentaExiste is null) 
            {
                return RedirectToAction("NoEncontrato","Home");
            }

            return View(cuentaExiste);
        }

        [HttpPost]
        public async Task<ActionResult> BorrarTipoCuenta(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuentaExiste = await repositorioTipoCuentas.ObtenerPorId(id, usuarioId);
            if (cuentaExiste is null)
            {
                return RedirectToAction("NoEncontrato", "Home");
            }

            await repositorioTipoCuentas.Borrar(id);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> ExisteTipoCuenta(string nombre)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var existecuenta = await repositorioTipoCuentas.Existe(nombre, usuarioId);
            if (existecuenta) {
                return Json($"El nombre {nombre} ya existe en la base de datos");
            }

            return Json(true);
        }

        [HttpPost]
        public async Task<IActionResult> Ordenar([FromBody] int[] ids)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipocuenta = await repositorioTipoCuentas.Obtener(usuarioId);
            var idstipocuenta = tipocuenta.Select(x => x.Id);

            var idsnoperteneceausuario = ids.Except(idstipocuenta).ToList();

            if (idsnoperteneceausuario.Count > 0)
            {
                return Forbid();
            }

            var tipocuentasOrdenados = ids.Select((valor, indice) =>
                                            new TipoCuenta() { Id = valor, Orden = indice + 1 }).AsEnumerable();

            await repositorioTipoCuentas.Ordenar(tipocuentasOrdenados);


            return Ok();
        }
    }    
}

