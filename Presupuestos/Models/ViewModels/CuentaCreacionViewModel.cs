using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Presupuestos.Models.ViewModels
{
    public class CuentaCreacionViewModel:Cuenta
    {
        public IEnumerable<SelectListItem> TipoCuenta { get; set; }
    }
}
