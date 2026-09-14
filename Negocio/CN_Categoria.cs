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
    /// Se encarga de aplicar las reglas y validaciones
    /// relacionadas con las categorías.
    ///
    /// Esta clase funciona como intermediaria entre
    /// Presentación y Datos.
    /// </summary>
    public class CN_Categoria
    {
        private CD_Categoria obj_cd_categoria =
            new CD_Categoria();


        /// <summary>
        /// Obtiene todas las categorías activas.
        /// </summary>
        public List<Categoria> Listar()
        {
            return obj_cd_categoria.Listar();
        }


        /// <summary>
        /// Registra una nueva categoría.
        /// </summary>
        public int Registrar(
            Categoria categoria)
        {
            ValidarCategoria(categoria);


            categoria.Nombre =
                categoria.Nombre.Trim();


            if (!string.IsNullOrWhiteSpace(
                categoria.Descripcion))
            {
                categoria.Descripcion =
                    categoria.Descripcion.Trim();
            }


            if (!string.IsNullOrWhiteSpace(
                categoria.Imagen))
            {
                categoria.Imagen =
                    categoria.Imagen.Trim();
            }


            return obj_cd_categoria.Registrar(
                categoria
            );
        }


        /// <summary>
        /// Modifica una categoría existente.
        /// </summary>
        public bool Editar(
            Categoria categoria)
        {
            if (categoria == null)
            {
                throw new ArgumentException(
                    "Debe indicar una categoría."
                );
            }


            if (categoria.IdCategoria <= 0)
            {
                throw new ArgumentException(
                    "La categoría seleccionada no es válida."
                );
            }


            ValidarCategoria(categoria);


            categoria.Nombre =
                categoria.Nombre.Trim();


            if (!string.IsNullOrWhiteSpace(
                categoria.Descripcion))
            {
                categoria.Descripcion =
                    categoria.Descripcion.Trim();
            }


            if (!string.IsNullOrWhiteSpace(
                categoria.Imagen))
            {
                categoria.Imagen =
                    categoria.Imagen.Trim();
            }


            return obj_cd_categoria.Editar(
                categoria
            );
        }


        /// <summary>
        /// Realiza la baja lógica de una categoría.
        ///
        /// Antes de solicitar la eliminación a la capa de Datos,
        /// verifica que la categoría no tenga productos activos
        /// asociados.
        ///
        /// Esta validación corresponde a la capa de Negocio
        /// porque representa una regla del sistema:
        ///
        /// una categoría utilizada por productos activos
        /// no puede darse de baja.
        /// </summary>
        /// <param name="idCategoria">
        /// Identificador de la categoría seleccionada.
        /// </param>
        /// <returns>
        /// Devuelve true si la baja lógica se realizó correctamente.
        /// </returns>
        public bool Eliminar(
            int idCategoria)
        {
            // =========================================================
            // 1. VALIDAR IDENTIFICADOR
            // =========================================================

            if (idCategoria <= 0)
            {
                throw new ArgumentException(
                    "Debe seleccionar una categoría válida."
                );
            }


            // =========================================================
            // 2. VERIFICAR PRODUCTOS ACTIVOS
            // =========================================================

            bool tieneProductosActivos =
                obj_cd_categoria.TieneProductosActivos(
                    idCategoria
                );


            if (tieneProductosActivos)
            {
                throw new InvalidOperationException(
                    "No se puede eliminar la categoría porque tiene productos activos asociados."
                );
            }


            // =========================================================
            // 3. REALIZAR BAJA LÓGICA
            // =========================================================

            bool eliminado =
                obj_cd_categoria.Eliminar(
                    idCategoria
                );


            if (!eliminado)
            {
                throw new InvalidOperationException(
                    "No fue posible eliminar la categoría."
                );
            }


            return true;
        }

        /// <summary>
        /// Valida los datos generales
        /// de una categoría.
        /// </summary>
        private void ValidarCategoria(
            Categoria categoria)
        {
            if (categoria == null)
            {
                throw new ArgumentException(
                    "Los datos de la categoría son obligatorios."
                );
            }


            if (string.IsNullOrWhiteSpace(
                categoria.Nombre))
            {
                throw new ArgumentException(
                    "El nombre de la categoría es obligatorio."
                );
            }


            if (categoria.Nombre.Trim().Length > 100)
            {
                throw new ArgumentException(
                    "El nombre de la categoría no puede superar los 100 caracteres."
                );
            }


            if (!string.IsNullOrWhiteSpace(
                categoria.Descripcion) &&
                categoria.Descripcion.Trim().Length > 255)
            {
                throw new ArgumentException(
                    "La descripción no puede superar los 255 caracteres."
                );
            }


            if (!string.IsNullOrWhiteSpace(
                categoria.Imagen) &&
                categoria.Imagen.Trim().Length > 255)
            {
                throw new ArgumentException(
                    "La referencia de la imagen no puede superar los 255 caracteres."
                );
            }
        }
    }
}
