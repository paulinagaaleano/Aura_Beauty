using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Entidades
{
    public class VentaPorDia
    {
        public DateTime Fecha { get; set; }

        public decimal Total { get; set; }

        public int CantidadVentas { get; set; }
    }
}
