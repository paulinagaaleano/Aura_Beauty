using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades
{
    /// <summary>
    /// Representa una categoría de productos
    /// dentro del sistema Aura Beauty.
    ///
    /// Esta clase pertenece a la Capa de Entidades.
    ///
    /// Su responsabilidad es transportar información
    /// entre las distintas capas del sistema.
    ///
    /// No contiene consultas SQL,
    /// reglas de negocio ni elementos visuales.
    /// </summary>
    public class Categoria
    {
        /// <summary>
        /// Identificador único de la categoría.
        ///
        /// Corresponde a la columna Id_categoria
        /// de la tabla Categoria.
        ///
        /// SQL Server lo genera automáticamente
        /// porque es un campo IDENTITY.
        /// </summary>
        public int IdCategoria { get; set; }


        /// <summary>
        /// Nombre de la categoría.
        ///
        /// Ejemplos:
        /// Maquillaje
        /// Labiales
        /// Bases
        /// Sombras
        /// </summary>
        public string Nombre { get; set; }


        /// <summary>
        /// Descripción opcional de la categoría.
        /// </summary>
        public string Descripcion { get; set; }


        /// <summary>
        /// Ruta o referencia de una imagen
        /// asociada a la categoría.
        ///
        /// La columna existente en la base
        /// es de tipo nvarchar.
        /// </summary>
        public string Imagen { get; set; }


        /// <summary>
        /// Fecha de creación del registro.
        ///
        /// El símbolo ? indica que DateTime
        /// puede contener también el valor null.
        /// </summary>
        public DateTime? CreatedAt { get; set; }


        /// <summary>
        /// Fecha de la última modificación.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }


        /// <summary>
        /// Fecha de eliminación lógica.
        ///
        /// Si vale null, la categoría está activa.
        ///
        /// Si contiene una fecha, significa que
        /// la categoría fue dada de baja lógicamente.
        /// </summary>
        public DateTime? DeletedAt { get; set; }
    }
}
