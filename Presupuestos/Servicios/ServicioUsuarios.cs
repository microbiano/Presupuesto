﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Presupuestos.Servicios
{
    public interface IServicioUsuarios
    {
        int ObtenerUsuarioId();
    }
    public class ServicioUsuarios:IServicioUsuarios
    {
        public int ObtenerUsuarioId()
        {
            return 1;
        }
    }
}
