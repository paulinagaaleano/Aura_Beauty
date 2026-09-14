using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    /// <summary>
    /// Representa un producto comercializado
    /// dentro del sistema Aura Beauty.
    ///
    /// Esta clase pertenece a la Capa de Entidades.
    ///
    /// Su función es transportar la información
    /// de los productos entre Presentación,
    /// Negocio y Datos.
    /// </summary>
    public class Producto
    {
        /// <summary>
        /// Identificador único del producto.
        ///
        /// Corresponde a la columna Id_producto.
        ///
        /// SQL Server lo genera automáticamente
        /// porque es un campo IDENTITY.
        /// </summary>
        public int IdProducto { get; set; }


        /// <summary>
        /// Nombre comercial del producto.
        /// </summary>
        public string Nombre { get; set; }


        /// <summary>
        /// Descripción del producto.
        /// </summary>
        public string Descripcion { get; set; }


        /// <summary>
        /// Precio de venta del producto.
        ///
        /// Utilizamos decimal porque es el tipo
        /// apropiado para trabajar con importes monetarios.
        /// </summary>
        public decimal Precio { get; set; }


        /// <summary>
        /// Cantidad disponible actualmente en stock.
        /// </summary>
        public int Stock { get; set; }


        /// <summary>
        /// Identificador de la categoría
        /// a la que pertenece el producto.
        ///
        /// Corresponde a id_categoria
        /// de la tabla Producto.
        /// </summary>
        public int IdCategoria { get; set; }


        /// <summary>
        /// Objeto Categoria relacionado con el producto.
        ///
        /// Nos permite trabajar no solamente
        /// con el número de la categoría,
        /// sino también con sus datos completos.
        ///
        /// Ejemplo:
        /// producto.oCategoria.Nombre
        /// </summary>
        public Categoria oCategoria { get; set; }


        /// <summary>
        /// Fecha de creación del producto.
        /// </summary>
        public DateTime? CreatedAt { get; set; }


        /// <summary>
        /// Fecha de la última modificación.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }


        /// <summary>
        /// Fecha utilizada para la baja lógica.
        ///
        /// null:
        /// el producto se encuentra activo.
        ///
        /// contiene una fecha:
        /// el producto fue dado de baja.
        /// </summary>
        public DateTime? DeletedAt { get; set; }
    }
}