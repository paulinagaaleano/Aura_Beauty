using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datos;
using Entidades;

namespace Negocio
{
    /// <summary>
    /// Clase de la capa de Negocio encargada de gestionar
    /// la consulta del stock de productos.
    ///
    /// Esta clase funciona como intermediaria entre
    /// la capa de Presentación y la capa de Datos.
    ///
    /// No contiene código de Windows Forms ni consultas SQL.
    /// </summary>
    public class CN_Stock
    {
        /// <summary>
        /// Objeto de la capa de Datos utilizado para realizar
        /// las operaciones relacionadas con el stock.
        /// </summary>
        private readonly CD_Stock cdStock = new CD_Stock();


        /// <summary>
        /// Obtiene la lista de productos activos junto
        /// con sus cantidades disponibles.
        /// </summary>
        /// <returns>
        /// Lista de productos activos.
        /// </returns>
        public List<Producto> Listar()
        {
            return cdStock.Listar();
        }


    }
}
