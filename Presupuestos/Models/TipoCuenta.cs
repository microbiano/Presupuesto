using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Presupuestos.Models
{
    public class TipoCuenta
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="{0} Requerido")]
        [Remote(action: "ExisteTipoCuenta",controller: "TipoCuentas")]
        public string Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }

    }
}
