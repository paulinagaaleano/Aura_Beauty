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
    /// Formulario que permite al vendedor consultar
    /// exclusivamente su propio historial de ventas.
    ///
    /// Este formulario pertenece a la capa de Presentación.
    ///
    /// El usuario no selecciona qué vendedor consultar:
    /// recibe automáticamente al usuario que inició sesión
    /// y utiliza su IdUsuario como filtro.
    /// </summary>
    public partial class frmMisVentas : Form
    {
        // =========================================================
        // CAPA DE NEGOCIO
        // =========================================================

        /// <summary>
        /// Objeto de la capa de Negocio encargado de
        /// solicitar la información de reportes.
        ///
        /// El formulario no accede directamente a SQL Server.
        /// </summary>
        private readonly CN_Reporte cnReporte =
            new CN_Reporte();


        // =========================================================
        // USUARIO AUTENTICADO
        // =========================================================

        /// <summary>
        /// Usuario que inició sesión en Aura Beauty.
        ///
        /// Su IdUsuario se utiliza para restringir
        /// el historial a sus propias ventas.
        /// </summary>
        private readonly Usuario usuarioActual;


        // =========================================================
        // DATOS EN MEMORIA
        // =========================================================

        /// <summary>
        /// Lista de ventas recuperadas desde Negocio.
        /// </summary>
        private List<ReporteVenta> listaVentas =
            new List<ReporteVenta>();


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        /// <summary>
        /// Inicializa el formulario de historial.
        /// </summary>
        /// <param name="usuario">
        /// Usuario que inició sesión.
        /// </param>
        public frmMisVentas(Usuario usuario)
        {
            /*
             * InitializeComponent() se encuentra en
             * frmMisVentas.Designer.cs.
             *
             * Su función es crear y configurar los
             * controles visuales del formulario.
             */
            InitializeComponent();

            /*
             * ArgumentNullException es una excepción de .NET
             * que indica que un parámetro obligatorio
             * fue recibido con valor null.
             */
            if (usuario == null)
            {
                throw new ArgumentNullException(
                    nameof(usuario)
                );
            }

            usuarioActual = usuario;

            /*
             * Mostramos quién está consultando.
             *
             * El vendedor solamente puede verlo:
             * no puede modificarlo ni seleccionar
             * otro usuario.
             */
            lblVendedor.Text =
                "Vendedor: "
                + usuarioActual.Nombre
                + " "
                + usuarioActual.Apellido;

            PrepararFiltrosIniciales();

            CargarMisVentas();
        }


        // =========================================================
        // FILTROS INICIALES
        // =========================================================

        /// <summary>
        /// Establece como período inicial el primer día
        /// del mes actual hasta la fecha de hoy.
        /// </summary>
        private void PrepararFiltrosIniciales()
        {
            DateTime hoy =
                DateTime.Today;

            dtpDesde.Value =
                new DateTime(
                    hoy.Year,
                    hoy.Month,
                    1
                );

            dtpHasta.Value =
                hoy;
        }


        // =========================================================
        // CARGA DEL HISTORIAL
        // =========================================================

        /// <summary>
        /// Obtiene las ventas realizadas por el usuario
        /// autenticado dentro del período seleccionado.
        ///
        /// La restricción por vendedor se realiza mediante
        /// usuarioActual.IdUsuario.
        /// </summary>
        private void CargarMisVentas()
        {
            try
            {
                /*
                 * Esta llamada sigue el flujo:
                 *
                 * Presentación
                 *      ↓
                 * CN_Reporte
                 *      ↓
                 * CD_Reporte
                 *      ↓
                 * SQL Server
                 *
                 * El tercer parámetro es fundamental:
                 * siempre enviamos el IdUsuario de quien
                 * inició sesión.
                 */
                listaVentas =
                    cnReporte.ObtenerVentas(
                        dtpDesde.Value,
                        dtpHasta.Value,
                        usuarioActual.IdUsuario
                    );


                /*
                 * Primero quitamos el origen anterior.
                 * Después asignamos la lista actualizada.
                 */
                dgvVentas.DataSource =
                    null;

                dgvVentas.DataSource =
                    listaVentas;


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
        // RESUMEN
        // =========================================================

        /// <summary>
        /// Actualiza la cantidad de ventas y el total
        /// correspondiente al período consultado.
        /// </summary>
        private void ActualizarResumen()
        {
            int cantidadVentas =
                listaVentas.Count;


            /*
             * Sum() pertenece a LINQ.
             *
             * La expresión:
             *
             * venta => venta.Total
             *
             * indica qué propiedad de cada elemento
             * queremos sumar.
             */
            decimal totalVendido =
                listaVentas.Sum(
                    venta => venta.Total
                );


            lblCantidad.Text =
                "Cantidad de ventas: "
                + cantidadVentas;


            lblTotal.Text =
                "Total vendido: "
                + totalVendido.ToString("C2");
        }


        // =========================================================
        // EVENTO BUSCAR
        // =========================================================

        /// <summary>
        /// Ejecuta nuevamente la consulta utilizando
        /// las fechas seleccionadas por el vendedor.
        /// </summary>
        private void btnBuscar_Click(
            object sender,
            EventArgs e
        )
        {
            CargarMisVentas();
        }


        // =========================================================
        // EVENTO LIMPIAR
        // =========================================================

        /// <summary>
        /// Restablece las fechas iniciales y vuelve
        /// a cargar el historial.
        /// </summary>
        private void btnLimpiar_Click(
            object sender,
            EventArgs e
        )
        {
            PrepararFiltrosIniciales();

            CargarMisVentas();
        }
    }
}