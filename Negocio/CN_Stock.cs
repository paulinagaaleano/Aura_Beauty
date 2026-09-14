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
    /// las reglas relacionadas con el stock de productos.
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


        /// <summary>
        /// Actualiza la cantidad total de stock de un producto.
        ///
        /// Antes de enviar la modificación a la capa de Datos,
        /// verifica que el producto sea válido y que la cantidad
        /// de stock no sea negativa.
        /// </summary>
        /// <param name="idProducto">
        /// Identificador del producto cuyo stock se modificará.
        /// </param>
        /// <param name="nuevoStock">
        /// Nueva cantidad total disponible del producto.
        /// </param>
        /// <returns>
        /// true si la actualización se realizó correctamente.
        /// </returns>
        public bool ActualizarStock(
            int idProducto,
            int nuevoStock
        )
        {
            // Un ID menor o igual a cero no representa
            // un producto válido de nuestra base de datos.
            if (idProducto <= 0)
            {
                throw new Exception(
                    "Seleccioná un producto válido."
                );
            }


            // El stock nunca puede quedar por debajo de cero.
            // Esta es una regla de negocio y por eso se valida
            // en esta capa.
            if (nuevoStock < 0)
            {
                throw new Exception(
                    "El stock no puede ser negativo."
                );
            }


            return cdStock.ActualizarStock(
                idProducto,
                nuevoStock
            );
        }


        /// <summary>
        /// Suma una determinada cantidad al stock actual.
        ///
        /// Por ejemplo:
        /// Stock actual = 10
        /// Cantidad ingresada = 5
        /// Nuevo stock = 15
        /// </summary>
        public bool AgregarStock(
            int idProducto,
            int stockActual,
            int cantidad
        )
        {
            if (idProducto <= 0)
            {
                throw new Exception(
                    "Seleccioná un producto válido."
                );
            }


            if (cantidad <= 0)
            {
                throw new Exception(
                    "La cantidad a agregar debe ser mayor que cero."
                );
            }


            if (stockActual < 0)
            {
                throw new Exception(
                    "El stock actual no es válido."
                );
            }


            int nuevoStock =
                stockActual + cantidad;


            return cdStock.ActualizarStock(
                idProducto,
                nuevoStock
            );
        }


        /// <summary>
        /// Resta una determinada cantidad del stock actual.
        ///
        /// Antes de realizar la operación comprueba que exista
        /// stock suficiente.
        ///
        /// Por ejemplo:
        /// Stock actual = 10
        /// Cantidad retirada = 3
        /// Nuevo stock = 7
        /// </summary>
        public bool QuitarStock(
            int idProducto,
            int stockActual,
            int cantidad
        )
        {
            if (idProducto <= 0)
            {
                throw new Exception(
                    "Seleccioná un producto válido."
                );
            }


            if (cantidad <= 0)
            {
                throw new Exception(
                    "La cantidad a retirar debe ser mayor que cero."
                );
            }


            if (stockActual < 0)
            {
                throw new Exception(
                    "El stock actual no es válido."
                );
            }


            // No permitimos retirar una cantidad superior
            // a las unidades disponibles.
            if (cantidad > stockActual)
            {
                throw new Exception(
                    "No hay stock suficiente para realizar esta operación."
                );
            }


            int nuevoStock =
                stockActual - cantidad;


            return cdStock.ActualizarStock(
                idProducto,
                nuevoStock
            );
        }
    }
}
