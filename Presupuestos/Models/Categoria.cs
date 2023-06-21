using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Presupuestos.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="{0} es requerido)")]
        [StringLength(maximumLength:50,ErrorMessage ="{0} no puede ser mayor de 50")]
        public string Nombre { get; set; }
        [Display(Name ="Tipo Operación")]
        public TipoOperacion TipoOperacioId { get; set; }
            
        public int UsuarioId { get; set; }
    }
}
