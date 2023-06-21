using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Presupuestos.Models
{
	public class Cuenta
	{
		public int Id { get; set; }

		[Required(ErrorMessage ="{0} es Requerido")]
		[StringLength(maximumLength:50)]
		public string Nombre { get; set; }
		[Display(Name ="Tipo Cuenta")]
		public int TipoCuentaId { get; set; }
		public decimal Balance { get; set; }
		[StringLength(maximumLength:1000)]
		public string Descripcion { get; set; }
        public string TipoCuenta { get; set; }
    }
}
