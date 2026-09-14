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
    /// Clase perteneciente a la Capa de Negocio.
    ///
    /// Se encarga de aplicar las reglas
    /// relacionadas con los productos.
    ///
    /// Aquí se validan datos antes de enviarlos
    /// a la Capa de Datos.
    /// </summary>
    public class CN_Producto
    {
        private CD_Producto obj_cd_producto =
            new CD_Producto();


        /// <summary>
        /// Obtiene todos los productos activos.
        /// </summary>
        public List<Producto> Listar()
        {
            return obj_cd_producto.Listar();
        }


        /// <summary>
        /// Busca productos por nombre.
        /// </summary>
        public List<Producto> Buscar(
            string texto)
        {
            /*
             * Si la búsqueda está vacía,
             * devolvemos directamente todos los productos.
             */
            if (string.IsNullOrWhiteSpace(texto))
            {
                return obj_cd_producto.Listar();
            }


            texto =
                texto.Trim();


            return obj_cd_producto.Buscar(
                texto
            );
        }


        /// <summary>
        /// Registra un producto nuevo.
        /// </summary>
        public int Registrar(
            Producto producto)
        {
            ValidarProducto(producto);


            producto.Nombre =
                producto.Nombre.Trim();


            if (!string.IsNullOrWhiteSpace(
                producto.Descripcion))
            {
                producto.Descripcion =
                    producto.Descripcion.Trim();
            }


            return obj_cd_producto.Registrar(
                producto
            );
        }


        /// <summary>
        /// Modifica un producto existente.
        /// </summary>
        public bool Editar(
            Producto producto)
        {
            if (producto == null)
            {
                throw new ArgumentException(
                    "Debe indicar un producto."
                );
            }


            if (producto.IdProducto <= 0)
            {
                throw new ArgumentException(
                    "El producto seleccionado no es válido."
                );
            }


            ValidarProducto(producto);


            producto.Nombre =
                producto.Nombre.Trim();


            if (!string.IsNullOrWhiteSpace(
                producto.Descripcion))
            {
                producto.Descripcion =
                    producto.Descripcion.Trim();
            }


            return obj_cd_producto.Editar(
                producto
            );
        }


        /// <summary>
        /// Realiza la baja lógica de un producto.
        /// </summary>
        public bool Eliminar(
            int idProducto)
        {
            if (idProducto <= 0)
            {
                throw new ArgumentException(
                    "Debe seleccionar un producto válido."
                );
            }


            return obj_cd_producto.Eliminar(
                idProducto
            );
        }


        /// <summary>
        /// Aplica todas las validaciones
        /// de negocio necesarias para un producto.
        /// </summary>
        private void ValidarProducto(
            Producto producto)
        {
            if (producto == null)
            {
                throw new ArgumentException(
                    "Los datos del producto son obligatorios."
                );
            }


            if (string.IsNullOrWhiteSpace(
                producto.Nombre))
            {
                throw new ArgumentException(
                    "El nombre del producto es obligatorio."
                );
            }


            if (producto.Nombre.Trim().Length > 100)
            {
                throw new ArgumentException(
                    "El nombre del producto no puede superar los 100 caracteres."
                );
            }


            if (!string.IsNullOrWhiteSpace(
                producto.Descripcion) &&
                producto.Descripcion.Trim().Length > 255)
            {
                throw new ArgumentException(
                    "La descripción no puede superar los 255 caracteres."
                );
            }


            /*
             * El precio no puede ser negativo.
             *
             * Permitimos 0 técnicamente, aunque
             * después podemos endurecer esta regla
             * si la consigna exige precio mayor a cero.
             */
            if (producto.Precio < 0)
            {
                throw new ArgumentException(
                    "El precio no puede ser negativo."
                );
            }


            /*
             * El stock tampoco puede ser negativo.
             */
            if (producto.Stock < 0)
            {
                throw new ArgumentException(
                    "El stock no puede ser negativo."
                );
            }


            /*
             * La categoría es obligatoria.
             *
             * Aunque la columna id_categoria en la base
             * actualmente permita NULL, la regla del sistema
             * exige que todo producto pertenezca a una categoría.
             */
            if (producto.IdCategoria <= 0)
            {
                throw new ArgumentException(
                    "Debe seleccionar una categoría para el producto."
                );
            }
        }
    }
}
