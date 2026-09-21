using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Aura_Beauty
{
    /// <summary>
    /// Formulario de Cierre de Caja del vendedor.
    ///
    /// Permite consultar las ventas realizadas por el
    /// vendedor autenticado durante un día determinado.
    ///
    /// A partir de esas ventas calcula:
    /// - cantidad de operaciones realizadas;
    /// - total vendido durante la jornada.
    ///
    /// Este formulario pertenece a la capa Presentación.
    /// No realiza consultas SQL directamente.
    /// </summary>
    public partial class frmCierreCaja : Form
    {
        // =========================================================
        // CAPA DE NEGOCIO
        // =========================================================

        /// <summary>
        /// Objeto de la capa de Negocio utilizado para
        /// obtener las ventas registradas en el sistema.
        /// </summary>
        private readonly CN_Reporte cnReporte =
            new CN_Reporte();


        // =========================================================
        // USUARIO AUTENTICADO
        // =========================================================

        /// <summary>
        /// Guarda al usuario que inició sesión.
        ///
        /// El IdUsuario se utiliza para consultar únicamente
        /// las ventas correspondientes a ese vendedor.
        /// </summary>
        private readonly Usuario usuarioActual;


        // =========================================================
        // VENTAS DEL DÍA
        // =========================================================

        /// <summary>
        /// Lista que contiene las ventas encontradas para
        /// el vendedor y la fecha seleccionada.
        /// </summary>
        private List<ReporteVenta> ventasDelDia =
            new List<ReporteVenta>();


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        /// <summary>
        /// Inicializa el formulario de Cierre de Caja.
        /// </summary>
        /// <param name="usuario">
        /// Usuario autenticado que proviene del menú principal.
        /// </param>
        public frmCierreCaja(Usuario usuario)
        {
            /*
             * InitializeComponent() está definido en
             * frmCierreCaja.Designer.cs.
             *
             * Crea todos los controles visuales.
             */
            InitializeComponent();


            /*
             * Validamos que realmente se haya recibido
             * un usuario autenticado.
             */
            if (usuario == null)
            {
                throw new ArgumentNullException(
                    nameof(usuario)
                );
            }


            usuarioActual = usuario;


            /*
             * Mostramos el nombre del vendedor.
             *
             * Este dato no puede ser modificado desde
             * el formulario.
             */
            lblVendedor.Text =
                "Vendedor: "
                + usuarioActual.Nombre
                + " "
                + usuarioActual.Apellido;


            /*
             * El cierre comienza mostrando la fecha actual.
             */
            dtpFecha.Value =
                DateTime.Today;

            dtpFecha.MaxDate =
                DateTime.Today;


            CargarCierre();
        }


        // =========================================================
        // CARGAR CIERRE
        // =========================================================

        /// <summary>
        /// Obtiene las ventas correspondientes al vendedor
        /// autenticado y al día seleccionado.
        /// </summary>
        private void CargarCierre()
        {
            try
            {
                /*
                 * Tomamos únicamente la parte de fecha.
                 *
                 * Date elimina conceptualmente la hora del
                 * DateTime seleccionado y devuelve las 00:00:00
                 * de ese mismo día.
                 */
                DateTime fecha =
                    dtpFecha.Value.Date;


                /*
                 * Solicitamos las ventas indicando:
                 *
                 * - fecha desde;
                 * - fecha hasta;
                 * - Id del vendedor autenticado.
                 *
                 * Como Desde y Hasta contienen el mismo día,
                 * obtenemos el movimiento de esa jornada.
                 */
                ventasDelDia =
                    cnReporte.ObtenerVentas(
                        fecha,
                        fecha,
                        usuarioActual.IdUsuario
                    );


                /*
                 * Actualizamos la grilla.
                 */
                dgvVentas.DataSource =
                    null;

                dgvVentas.DataSource =
                    ventasDelDia;


                ActualizarResumen();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }


        // =========================================================
        // ACTUALIZAR RESUMEN
        // =========================================================

        /// <summary>
        /// Calcula la cantidad de ventas y el total vendido
        /// durante la jornada seleccionada.
        /// </summary>
        private void ActualizarResumen()
        {
            /*
             * Count devuelve la cantidad de elementos
             * existentes en la lista.
             */
            int cantidadVentas =
                ventasDelDia.Count;


            /*
             * Sum pertenece a LINQ.
             *
             * Recorre las ventas y suma la propiedad Total
             * de cada una.
             */
            decimal totalVendido =
                ventasDelDia.Sum(
                    venta => venta.Total
                );


            lblCantidad.Text =
                "Cantidad de ventas: "
                + cantidadVentas;


            lblTotal.Text =
                "Total vendido: "
                + totalVendido.ToString("C2");


            /*
             * Mostramos claramente qué día se está resumiendo.
             */
            lblFechaResumen.Text =
                "Cierre correspondiente al "
                + dtpFecha.Value.ToString("dd/MM/yyyy");
        }


        // =========================================================
        // BOTÓN CONSULTAR
        // =========================================================

        /// <summary>
        /// Actualiza el cierre utilizando la fecha
        /// seleccionada por el vendedor.
        /// </summary>
        private void btnConsultar_Click(
            object sender,
            EventArgs e
        )
        {
            CargarCierre();
        }


        // =========================================================
        // CAMBIO DE FECHA
        // =========================================================

        /// <summary>
        /// Al cambiar la fecha actualizamos el resumen.
        ///
        /// También podría utilizarse solamente el botón Consultar,
        /// pero este evento hace más cómoda la utilización.
        /// </summary>
        private void dtpFecha_ValueChanged(
            object sender,
            EventArgs e
        )
        {
            CargarCierre();
        }
    }
}
