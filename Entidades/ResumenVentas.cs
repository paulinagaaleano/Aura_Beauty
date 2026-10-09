using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    public class ResumenVentas
    {
        public decimal FacturacionTotal { get; set; }

        public int CantidadVentas { get; set; }

        public decimal TicketPromedio { get; set; }

        public int UnidadesVendidas { get; set; }

        public decimal VentaMaxima { get; set; }

        public decimal VentaMinima { get; set; }
    }
}
