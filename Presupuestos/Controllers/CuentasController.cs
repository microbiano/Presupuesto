using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Presupuestos.Models.ViewModels;
using Presupuestos.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Presupuestos.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IRepositorioTipoCuentas repositorioTipoCuentas;
        private readonly IServicioUsuarios servicioUsuarios;
        private readonly IRepositorioCuentas repositorioCuentas;
        private readonly IMapper mapper;

        public CuentasController(IRepositorioTipoCuentas repositorioTipoCuentas, IServicioUsuarios servicioUsuarios, IRepositorioCuentas repositorioCuentas,IMapper mapper)
        {
            this.repositorioTipoCuentas = repositorioTipoCuentas;
            this.servicioUsuarios = servicioUsuarios;
            this.repositorioCuentas = repositorioCuentas;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuentasConTipoCuentas = await repositorioCuentas.Buscar(usuarioId);

            var modelo = cuentasConTipoCuentas
                         .GroupBy(x => x.TipoCuenta)
                         .Select(grupo => new IndiceCuentasViewModel
                         {
                             TipoCuenta = grupo.Key,
                             Cuentas = grupo.AsEnumerable()
                         }).ToList();

            return View(modelo);

        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();

            var modelo = new CuentaCreacionViewModel();

            modelo.TipoCuenta = await ObtenerTipoCuentas(usuarioId);

            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var tipoCuenta = await repositorioTipoCuentas.ObtenerPorId(cuenta.TipoCuentaId, usuarioId);
            if (tipoCuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid)
            {
                cuenta.TipoCuenta = await ObtenerTipoCuentas(usuarioId);
            }

            await repositorioCuentas.Crear(cuenta);
            return RedirectToAction("index");

        }


        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id,usuarioId);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            //var modelo = new CuentaCreacionViewModel()
            //{
            //    Id = cuenta.Id,
            //    Nombre = cuenta.Nombre,
            //    TipoCuentaId = cuenta.TipoCuentaId,
            //    Descripcion = cuenta.Descripcion,
            //    Balance = cuenta.Balance
            //};

            var modelo = mapper.Map<CuentaCreacionViewModel>(cuenta);

            modelo.TipoCuenta = await ObtenerTipoCuentas(usuarioId);

            return View(modelo);
        }
        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaEditar)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(cuentaEditar.Id, usuarioId);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var tipocuenta = await repositorioTipoCuentas.ObtenerPorId(cuentaEditar.TipoCuentaId,usuarioId);
            if (tipocuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCuentas.Actualizar(cuentaEditar);

            return RedirectToAction("Index");

        }


        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();            
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(cuenta);
        }

        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id)
        {
            var usuarioId = servicioUsuarios.ObtenerUsuarioId();
            var cuenta = await repositorioCuentas.ObtenerPorId(id, usuarioId);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCuentas.Borrar(id);

            return RedirectToAction("Index");
        }


        private async Task<IEnumerable<SelectListItem>> ObtenerTipoCuentas(int usuarioId)
        {
            var tipoCuentas = await repositorioTipoCuentas.Obtener(usuarioId);
           return tipoCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }
    }
}
