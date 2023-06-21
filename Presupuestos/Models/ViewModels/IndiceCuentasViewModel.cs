using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Presupuestos.Models.ViewModels
{
    public class IndiceCuentasViewModel
    {
        public string TipoCuenta  { get; set; }
        public IEnumerable<Cuenta> Cuentas { get; set; }

        public decimal Balance => Cuentas.Sum(x=>x.Balance);
    }
}
