using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Entidades;
using Negocio;

namespace Presentacion
{
    /// <summary>
    /// Formulario utilizado por el Administrador para
    /// consultar y resolver solicitudes de modificación
    /// de stock realizadas por los repositorios.
    ///
    /// Desde esta pantalla el Administrador puede:
    ///
    /// - consultar las solicitudes pendientes;
    /// - conocer el stock actual del producto;
    /// - conocer el tipo de movimiento solicitado;
    /// - consultar la cantidad;
    /// - leer el motivo o justificación;
    /// - identificar al usuario solicitante;
    /// - aprobar la solicitud;
    /// - rechazar la solicitud.
    ///
    /// El formulario NO modifica directamente el stock.
    ///
    /// La operación sigue el flujo:
    ///
    /// Presentación
    ///      ↓
    /// Negocio
    ///      ↓
    /// Datos
    ///      ↓
    /// Stored Procedure
    ///      ↓
    /// SQL Server
    /// </summary>
    public partial class frmSolicitudesStock : Form
    {
        // =====================================================
        // CAPA DE NEGOCIO
        // =====================================================

        /// <summary>
        /// Objeto que permite comunicarnos con la lógica
        /// de negocio correspondiente a SolicitudStock.
        /// </summary>
        private readonly CN_SolicitudStock cnSolicitudStock =
            new CN_SolicitudStock();


        // =====================================================
        // USUARIO ACTUAL
        // =====================================================

        /// <summary>
        /// Administrador que inició sesión.
        ///
        /// Necesitamos conservarlo porque, cuando apruebe
        /// o rechace una solicitud, debemos registrar
        /// quién realizó esa acción.
        /// </summary>
        private readonly Usuario usuarioActual;


        // =====================================================
        // SOLICITUD SELECCIONADA
        // =====================================================

        /// <summary>
        /// Guarda el identificador de la solicitud
        /// seleccionada actualmente en la grilla.
        ///
        /// El valor 0 significa que todavía no se
        /// seleccionó ninguna solicitud.
        /// </summary>
        private int idSolicitudSeleccionada = 0;


        // =====================================================
        // CONTROLES
        // =====================================================

        private DataGridView dgvSolicitudes;

        private Label lblTitulo;
        private Label lblSubtitulo;
        private Label lblCantidadPendientes;

        private Button btnAprobar;
        private Button btnRechazar;
        private Button btnActualizar;


        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        /// <summary>
        /// Constructor del formulario.
        ///
        /// Recibe el usuario autenticado desde frmMenu.
        /// </summary>
        /// <param name="usuario">
        /// Usuario que actualmente se encuentra
        /// utilizando el sistema.
        /// </param>
        public frmSolicitudesStock(Usuario usuario)
        {
            InitializeComponent();

            /*
             * Guardamos el usuario recibido.
             *
             * De esta manera conocemos el ID del
             * Administrador que aprueba o rechaza.
             */
            usuarioActual = usuario;

            ConfigurarFormulario();

            CrearControles();

            CargarSolicitudes();
        }


        // =====================================================
        // CONFIGURACIÓN GENERAL
        // =====================================================

        /// <summary>
        /// Configura las características visuales
        /// generales de la ventana.
        /// </summary>
        private void ConfigurarFormulario()
        {
            Text = "Solicitudes de Stock - Aura Beauty";

            StartPosition =
                FormStartPosition.CenterScreen;

            Size = new Size(1100, 680);

            MinimumSize =
                new Size(1000, 600);

            BackColor =
                Color.FromArgb(250, 246, 245);

            Font =
                new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Regular
                );
        }


        // =====================================================
        // CREACIÓN DE CONTROLES
        // =====================================================

        /// <summary>
        /// Crea los controles principales del formulario.
        ///
        /// La interfaz se genera mediante código para
        /// mantener el mismo criterio utilizado en
        /// otros formularios del proyecto.
        /// </summary>
        private void CrearControles()
        {
            // -------------------------------------------------
            // TÍTULO
            // -------------------------------------------------

            lblTitulo = new Label();

            lblTitulo.Text =
                "SOLICITUDES DE STOCK";

            lblTitulo.Font =
                new Font(
                    "Segoe UI",
                    20,
                    FontStyle.Bold
                );

            lblTitulo.ForeColor =
                Color.FromArgb(45, 45, 45);

            lblTitulo.AutoSize = true;

            lblTitulo.Location =
                new Point(40, 30);

            Controls.Add(lblTitulo);


            // -------------------------------------------------
            // SUBTÍTULO
            // -------------------------------------------------

            lblSubtitulo = new Label();

            lblSubtitulo.Text =
                "Solicitudes pendientes de aprobación";

            lblSubtitulo.Font =
                new Font(
                    "Segoe UI",
                    11,
                    FontStyle.Regular
                );

            lblSubtitulo.ForeColor =
                Color.DimGray;

            lblSubtitulo.AutoSize = true;

            lblSubtitulo.Location =
                new Point(43, 75);

            Controls.Add(lblSubtitulo);


            // -------------------------------------------------
            // CANTIDAD DE SOLICITUDES
            // -------------------------------------------------

            lblCantidadPendientes =
                new Label();

            lblCantidadPendientes.Text =
                "Solicitudes pendientes: 0";

            lblCantidadPendientes.Font =
                new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Bold
                );

            lblCantidadPendientes.AutoSize =
                true;

            lblCantidadPendientes.Location =
                new Point(43, 115);

            Controls.Add(
                lblCantidadPendientes
            );


            // -------------------------------------------------
            // BOTÓN ACTUALIZAR
            // -------------------------------------------------

            btnActualizar =
                new Button();

            btnActualizar.Text =
                "ACTUALIZAR";

            btnActualizar.Size =
                new Size(150, 40);

            btnActualizar.Location =
                new Point(895, 95);

            btnActualizar.BackColor =
                Color.White;

            btnActualizar.FlatStyle =
                FlatStyle.Flat;

            btnActualizar.Cursor =
                Cursors.Hand;

            btnActualizar.Click +=
                btnActualizar_Click;

            Controls.Add(btnActualizar);


            // -------------------------------------------------
            // GRILLA
            // -------------------------------------------------

            dgvSolicitudes =
                new DataGridView();

            dgvSolicitudes.Location =
                new Point(40, 155);

            dgvSolicitudes.Size =
                new Size(1005, 360);

            dgvSolicitudes.Anchor =
                AnchorStyles.Top |
                AnchorStyles.Bottom |
                AnchorStyles.Left |
                AnchorStyles.Right;

            dgvSolicitudes.ReadOnly =
                true;

            dgvSolicitudes.AllowUserToAddRows =
                false;

            dgvSolicitudes.AllowUserToDeleteRows =
                false;

            dgvSolicitudes.AllowUserToResizeRows =
                false;

            dgvSolicitudes.MultiSelect =
                false;

            dgvSolicitudes.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvSolicitudes.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvSolicitudes.RowHeadersVisible =
                false;

            dgvSolicitudes.BackgroundColor =
                Color.White;

            dgvSolicitudes.BorderStyle =
                BorderStyle.FixedSingle;

            dgvSolicitudes.CellClick +=
                dgvSolicitudes_CellClick;

            Controls.Add(dgvSolicitudes);


            // -------------------------------------------------
            // BOTÓN APROBAR
            // -------------------------------------------------

            btnAprobar =
                new Button();

            btnAprobar.Text =
                "APROBAR SOLICITUD";

            btnAprobar.Size =
                new Size(220, 48);

            btnAprobar.Location =
                new Point(40, 545);

            btnAprobar.Anchor =
                AnchorStyles.Bottom |
                AnchorStyles.Left;

            btnAprobar.BackColor =
                Color.FromArgb(
                    190,
                    140,
                    150
                );

            btnAprobar.ForeColor =
                Color.White;

            btnAprobar.FlatStyle =
                FlatStyle.Flat;

            btnAprobar.FlatAppearance.BorderSize =
                0;

            btnAprobar.Cursor =
                Cursors.Hand;

            btnAprobar.Enabled =
                false;

            btnAprobar.Click +=
                btnAprobar_Click;

            Controls.Add(btnAprobar);


            // -------------------------------------------------
            // BOTÓN RECHAZAR
            // -------------------------------------------------

            btnRechazar =
                new Button();

            btnRechazar.Text =
                "RECHAZAR SOLICITUD";

            btnRechazar.Size =
                new Size(220, 48);

            btnRechazar.Location =
                new Point(280, 545);

            btnRechazar.Anchor =
                AnchorStyles.Bottom |
                AnchorStyles.Left;

            btnRechazar.BackColor =
                Color.White;

            btnRechazar.ForeColor =
                Color.FromArgb(
                    80,
                    80,
                    80
                );

            btnRechazar.FlatStyle =
                FlatStyle.Flat;

            btnRechazar.Cursor =
                Cursors.Hand;

            btnRechazar.Enabled =
                false;

            btnRechazar.Click +=
                btnRechazar_Click;

            Controls.Add(btnRechazar);
        }


        // =====================================================
        // CARGAR SOLICITUDES
        // =====================================================

        /// <summary>
        /// Obtiene desde la capa de Negocio todas las
        /// solicitudes que todavía están PENDIENTES
        /// y las muestra en la grilla.
        /// </summary>
        private void CargarSolicitudes()
        {
            try
            {
                /*
                 * Pedimos a Negocio la información.
                 */
                List<SolicitudStock> solicitudes =
                    cnSolicitudStock
                        .ListarPendientes();


                /*
                 * Utilizamos Select para transformar
                 * los objetos SolicitudStock en una
                 * estructura cómoda para mostrar
                 * dentro del DataGridView.
                 *
                 * Select pertenece a LINQ.
                 */
                var datos =
                    solicitudes
                        .Select(s => new
                        {
                            Id =
                                s.IdSolicitud,

                            Fecha =
                                s.FechaSolicitud,

                            Producto =
                                s.oProducto.Nombre,

                            StockActual =
                                s.oProducto.Stock,

                            Tipo =
                                s.TipoMovimiento,

                            Cantidad =
                                s.Cantidad,

                            Motivo =
                                s.Motivo,

                            Solicitante =
                                s.oUsuarioSolicitante.Nombre
                                + " "
                                + s.oUsuarioSolicitante.Apellido
                        })
                        .ToList();


                dgvSolicitudes.DataSource =
                    null;

                dgvSolicitudes.DataSource =
                    datos;


                /*
                 * Mostramos la cantidad de solicitudes
                 * que esperan resolución.
                 */
                lblCantidadPendientes.Text =
                    "Solicitudes pendientes: "
                    + solicitudes.Count;


                ConfigurarColumnas();


                /*
                 * Cada vez que recargamos la grilla,
                 * quitamos la selección anterior.
                 */
                LimpiarSeleccion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Error al cargar solicitudes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        // =====================================================
        // CONFIGURAR COLUMNAS
        // =====================================================

        /// <summary>
        /// Ajusta los textos visibles de las columnas.
        /// </summary>
        private void ConfigurarColumnas()
        {
            if (dgvSolicitudes.Columns.Count == 0)
                return;


            dgvSolicitudes.Columns["Id"]
                .HeaderText =
                "ID";

            dgvSolicitudes.Columns["Fecha"]
                .HeaderText =
                "Fecha";

            dgvSolicitudes.Columns["Producto"]
                .HeaderText =
                "Producto";

            dgvSolicitudes.Columns["StockActual"]
                .HeaderText =
                "Stock actual";

            dgvSolicitudes.Columns["Tipo"]
                .HeaderText =
                "Movimiento";

            dgvSolicitudes.Columns["Cantidad"]
                .HeaderText =
                "Cantidad";

            dgvSolicitudes.Columns["Motivo"]
                .HeaderText =
                "Motivo / Justificación";

            dgvSolicitudes.Columns["Solicitante"]
                .HeaderText =
                "Solicitante";


            /*
             * El motivo necesita un poco más de
             * espacio que las demás columnas.
             */
            dgvSolicitudes.Columns["Motivo"]
                .FillWeight = 180;


            dgvSolicitudes.Columns["Producto"]
                .FillWeight = 130;


            /*
             * Mostramos la fecha de una manera
             * más fácil de leer.
             */
            dgvSolicitudes.Columns["Fecha"]
                .DefaultCellStyle
                .Format =
                "dd/MM/yyyy HH:mm";
        }


        // =====================================================
        // SELECCIONAR SOLICITUD
        // =====================================================

        /// <summary>
        /// Se ejecuta cuando el Administrador hace clic
        /// sobre una fila de la grilla.
        /// </summary>
        private void dgvSolicitudes_CellClick(
            object sender,
            DataGridViewCellEventArgs e
        )
        {
            /*
             * e.RowIndex contiene el número de fila
             * sobre la cual se hizo clic.
             *
             * -1 significa que se hizo clic sobre
             * el encabezado y no sobre una solicitud.
             */
            if (e.RowIndex < 0)
                return;


            DataGridViewRow fila =
                dgvSolicitudes.Rows[
                    e.RowIndex
                ];


            idSolicitudSeleccionada =
                Convert.ToInt32(
                    fila.Cells["Id"].Value
                );


            /*
             * Ahora que existe una solicitud
             * seleccionada habilitamos las acciones.
             */
            btnAprobar.Enabled =
                true;

            btnRechazar.Enabled =
                true;
        }


        // =====================================================
        // APROBAR
        // =====================================================

        /// <summary>
        /// Solicita confirmación y envía la aprobación
        /// a la capa de Negocio.
        /// </summary>
        private void btnAprobar_Click(
            object sender,
            EventArgs e
        )
        {
            if (idSolicitudSeleccionada <= 0)
            {
                MessageBox.Show(
                    "Seleccione una solicitud.",
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                return;
            }


            DialogResult respuesta =
                MessageBox.Show(
                    "¿Desea aprobar la solicitud seleccionada?\n\n" +
                    "Al aprobarla se modificará el stock del producto.",
                    "Confirmar aprobación",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );


            if (respuesta != DialogResult.Yes)
                return;


            try
            {
                bool resultado =
                    cnSolicitudStock.Aprobar(
                        idSolicitudSeleccionada,
                        usuarioActual.IdUsuario
                    );


                if (resultado)
                {
                    MessageBox.Show(
                        "La solicitud fue aprobada correctamente.\n\n" +
                        "El stock del producto fue actualizado.",
                        "Solicitud aprobada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );


                    /*
                     * Volvemos a consultar SQL Server.
                     *
                     * Como la solicitud ahora está APROBADA,
                     * dejará de aparecer entre las pendientes.
                     */
                    CargarSolicitudes();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "No fue posible aprobar la solicitud",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        // =====================================================
        // RECHAZAR
        // =====================================================

        /// <summary>
        /// Rechaza la solicitud seleccionada.
        ///
        /// Esta acción NO modifica Producto.stock.
        /// </summary>
        private void btnRechazar_Click(
            object sender,
            EventArgs e
        )
        {
            if (idSolicitudSeleccionada <= 0)
            {
                MessageBox.Show(
                    "Seleccione una solicitud.",
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                return;
            }


            DialogResult respuesta =
                MessageBox.Show(
                    "¿Desea rechazar la solicitud seleccionada?\n\n" +
                    "El stock del producto no será modificado.",
                    "Confirmar rechazo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );


            if (respuesta != DialogResult.Yes)
                return;


            try
            {
                bool resultado =
                    cnSolicitudStock.Rechazar(
                        idSolicitudSeleccionada,
                        usuarioActual.IdUsuario
                    );


                if (resultado)
                {
                    MessageBox.Show(
                        "La solicitud fue rechazada correctamente.\n\n" +
                        "El stock del producto no fue modificado.",
                        "Solicitud rechazada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );


                    CargarSolicitudes();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "No fue posible rechazar la solicitud",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        // =====================================================
        // ACTUALIZAR
        // =====================================================

        /// <summary>
        /// Vuelve a consultar las solicitudes pendientes.
        /// </summary>
        private void btnActualizar_Click(
            object sender,
            EventArgs e
        )
        {
            CargarSolicitudes();
        }


        // =====================================================
        // LIMPIAR SELECCIÓN
        // =====================================================

        /// <summary>
        /// Restablece la solicitud seleccionada y
        /// deshabilita los botones de resolución.
        /// </summary>
        private void LimpiarSeleccion()
        {
            idSolicitudSeleccionada = 0;

            dgvSolicitudes.ClearSelection();

            btnAprobar.Enabled =
                false;

            btnRechazar.Enabled =
                false;
        }
    }
}
