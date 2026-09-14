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
    /// Formulario de consulta de reportes de ventas.
    ///
    /// Permite:
    /// - Filtrar ventas por fecha desde y hasta.
    /// - Filtrar por vendedor.
    /// - Consultar todos los vendedores.
    /// - Mostrar las ventas encontradas.
    /// - Calcular el total vendido.
    ///
    /// La pantalla pertenece a Presentación.
    /// Las validaciones se realizan en CN_Reporte
    /// y la consulta SQL en CD_Reporte.
    /// </summary>
    public partial class frmReportes : Form
    {
        // =========================================================
        // CAPA DE NEGOCIO
        // =========================================================

        private readonly CN_Reporte cnReporte =
            new CN_Reporte();

        private readonly CN_Usuario cnUsuario =
            new CN_Usuario();


        // =========================================================
        // DATOS EN MEMORIA
        // =========================================================

        private List<Usuario> listaUsuarios =
            new List<Usuario>();

        private List<ReporteVenta> listaReporte =
            new List<ReporteVenta>();


        // =========================================================
        // COLORES AURA BEAUTY
        // =========================================================

        private readonly Color colorRosa =
            Color.FromArgb(201, 143, 149);

        private readonly Color colorRosaOscuro =
            Color.FromArgb(174, 112, 120);

        private readonly Color colorFondo =
            Color.FromArgb(255, 249, 248);

        private readonly Color colorTexto =
            Color.FromArgb(94, 74, 74);


        // =========================================================
        // CONTROLES
        // =========================================================

        private DateTimePicker dtpDesde;
        private DateTimePicker dtpHasta;

        private ComboBox cboVendedor;

        private Button btnBuscar;
        private Button btnLimpiar;

        private DataGridView dgvReporte;

        private Label lblCantidadVentas;
        private Label lblTotalVendido;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        public frmReportes()
        {
            InitializeComponent();

            ConfigurarFormulario();

            CrearInterfaz();

            CargarVendedores();

            PrepararFiltrosIniciales();
        }


        // =========================================================
        // CONFIGURAR FORMULARIO
        // =========================================================

        private void ConfigurarFormulario()
        {
            Text =
                "Aura Beauty - Reportes de Ventas";

            StartPosition =
                FormStartPosition.CenterScreen;

            Width =
                1100;

            Height =
                700;

            BackColor =
                colorFondo;

            Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            FormBorderStyle =
                FormBorderStyle.FixedSingle;

            MaximizeBox =
                false;
        }


        // =========================================================
        // CREAR INTERFAZ
        // =========================================================

        private void CrearInterfaz()
        {
            // =====================================================
            // ENCABEZADO
            // =====================================================

            Panel panelEncabezado =
                new Panel();

            panelEncabezado.Dock =
                DockStyle.Top;

            panelEncabezado.Height =
                110;

            panelEncabezado.BackColor =
                colorRosa;

            Controls.Add(
                panelEncabezado
            );


            Label lblTitulo =
                new Label();

            lblTitulo.Text =
                "REPORTES DE VENTAS";

            lblTitulo.Font =
                new Font(
                    "Segoe UI",
                    24F,
                    FontStyle.Bold
                );

            lblTitulo.ForeColor =
                Color.White;

            lblTitulo.AutoSize =
                true;

            lblTitulo.Location =
                new Point(
                    30,
                    20
                );

            panelEncabezado.Controls.Add(
                lblTitulo
            );


            Label lblSubtitulo =
                new Label();

            lblSubtitulo.Text =
                "Consulta por fechas y vendedor";

            lblSubtitulo.Font =
                new Font(
                    "Segoe UI",
                    11F
                );

            lblSubtitulo.ForeColor =
                Color.White;

            lblSubtitulo.AutoSize =
                true;

            lblSubtitulo.Location =
                new Point(
                    33,
                    70
                );

            panelEncabezado.Controls.Add(
                lblSubtitulo
            );


            // =====================================================
            // PANEL DE FILTROS
            // =====================================================

            Panel panelFiltros =
                new Panel();

            panelFiltros.Location =
                new Point(
                    20,
                    130
                );

            panelFiltros.Size =
                new Size(
                    1040,
                    115
                );

            panelFiltros.BackColor =
                Color.White;

            Controls.Add(
                panelFiltros
            );


            panelFiltros.Controls.Add(
                CrearEtiqueta(
                    "Desde",
                    20,
                    18
                )
            );


            dtpDesde =
                new DateTimePicker();

            dtpDesde.Location =
                new Point(
                    20,
                    48
                );

            dtpDesde.Size =
                new Size(
                    180,
                    30
                );

            dtpDesde.Format =
                DateTimePickerFormat.Short;

            panelFiltros.Controls.Add(
                dtpDesde
            );


            panelFiltros.Controls.Add(
                CrearEtiqueta(
                    "Hasta",
                    225,
                    18
                )
            );


            dtpHasta =
                new DateTimePicker();

            dtpHasta.Location =
                new Point(
                    225,
                    48
                );

            dtpHasta.Size =
                new Size(
                    180,
                    30
                );

            dtpHasta.Format =
                DateTimePickerFormat.Short;

            panelFiltros.Controls.Add(
                dtpHasta
            );


            panelFiltros.Controls.Add(
                CrearEtiqueta(
                    "Vendedor",
                    430,
                    18
                )
            );


            cboVendedor =
                new ComboBox();

            cboVendedor.Location =
                new Point(
                    430,
                    48
                );

            cboVendedor.Size =
                new Size(
                    260,
                    30
                );

            cboVendedor.DropDownStyle =
                ComboBoxStyle.DropDownList;

            panelFiltros.Controls.Add(
                cboVendedor
            );


            btnBuscar =
                CrearBoton(
                    "BUSCAR",
                    725,
                    44,
                    135
                );

            btnBuscar.Click +=
                btnBuscar_Click;

            panelFiltros.Controls.Add(
                btnBuscar
            );


            btnLimpiar =
                CrearBotonSecundario(
                    "LIMPIAR",
                    875,
                    44,
                    135
                );

            btnLimpiar.Click +=
                btnLimpiar_Click;

            panelFiltros.Controls.Add(
                btnLimpiar
            );


            // =====================================================
            // PANEL DE RESULTADOS
            // =====================================================

            Panel panelResultados =
                new Panel();

            panelResultados.Location =
                new Point(
                    20,
                    265
                );

            panelResultados.Size =
                new Size(
                    1040,
                    360
                );

            panelResultados.BackColor =
                Color.White;

            Controls.Add(
                panelResultados
            );


            Label lblResultadoTitulo =
                new Label();

            lblResultadoTitulo.Text =
                "Ventas encontradas";

            lblResultadoTitulo.Font =
                new Font(
                    "Segoe UI",
                    16F,
                    FontStyle.Bold
                );

            lblResultadoTitulo.ForeColor =
                colorTexto;

            lblResultadoTitulo.AutoSize =
                true;

            lblResultadoTitulo.Location =
                new Point(
                    20,
                    15
                );

            panelResultados.Controls.Add(
                lblResultadoTitulo
            );


            dgvReporte =
                new DataGridView();

            dgvReporte.Location =
                new Point(
                    20,
                    55
                );

            dgvReporte.Size =
                new Size(
                    1000,
                    230
                );

            ConfigurarGrilla(
                dgvReporte
            );


            dgvReporte.Columns.Add(
                CrearColumna(
                    "IdVenta",
                    "N.º venta",
                    "IdVenta"
                )
            );

            dgvReporte.Columns.Add(
                CrearColumna(
                    "Fecha",
                    "Fecha",
                    "Fecha"
                )
            );

            dgvReporte.Columns.Add(
                CrearColumna(
                    "NroFactura",
                    "Comprobante",
                    "NroFactura"
                )
            );

            dgvReporte.Columns.Add(
                CrearColumna(
                    "Vendedor",
                    "Vendedor",
                    "Vendedor"
                )
            );

            dgvReporte.Columns.Add(
                CrearColumna(
                    "Cliente",
                    "Cliente",
                    "Cliente"
                )
            );

            dgvReporte.Columns.Add(
                CrearColumna(
                    "Total",
                    "Total",
                    "Total"
                )
            );


            panelResultados.Controls.Add(
                dgvReporte
            );


            // =====================================================
            // RESUMEN DEL REPORTE
            // =====================================================

            lblCantidadVentas =
                new Label();

            lblCantidadVentas.Text =
                "Cantidad de ventas: 0";

            lblCantidadVentas.Location =
                new Point(
                    20,
                    310
                );

            lblCantidadVentas.AutoSize =
                true;

            lblCantidadVentas.Font =
                new Font(
                    "Segoe UI",
                    11F,
                    FontStyle.Bold
                );

            lblCantidadVentas.ForeColor =
                colorTexto;

            panelResultados.Controls.Add(
                lblCantidadVentas
            );


            Label lblTotalTitulo =
                new Label();

            lblTotalTitulo.Text =
                "TOTAL VENDIDO:";

            lblTotalTitulo.Location =
                new Point(
                    720,
                    305
                );

            lblTotalTitulo.AutoSize =
                true;

            lblTotalTitulo.Font =
                new Font(
                    "Segoe UI",
                    13F,
                    FontStyle.Bold
                );

            lblTotalTitulo.ForeColor =
                colorTexto;

            panelResultados.Controls.Add(
                lblTotalTitulo
            );


            lblTotalVendido =
                new Label();

            lblTotalVendido.Text =
                "$ 0,00";

            lblTotalVendido.Location =
                new Point(
                    865,
                    298
                );

            lblTotalVendido.Size =
                new Size(
                    155,
                    40
                );

            lblTotalVendido.TextAlign =
                ContentAlignment.MiddleRight;

            lblTotalVendido.Font =
                new Font(
                    "Segoe UI",
                    16F,
                    FontStyle.Bold
                );

            lblTotalVendido.ForeColor =
                colorRosaOscuro;

            panelResultados.Controls.Add(
                lblTotalVendido
            );
        }


        // =========================================================
        // CONTROLES REUTILIZABLES
        // =========================================================

        private Label CrearEtiqueta(
            string texto,
            int x,
            int y
        )
        {
            Label etiqueta =
                new Label();

            etiqueta.Text =
                texto;

            etiqueta.AutoSize =
                true;

            etiqueta.Location =
                new Point(
                    x,
                    y
                );

            etiqueta.ForeColor =
                colorTexto;

            etiqueta.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            return etiqueta;
        }


        private Button CrearBoton(
            string texto,
            int x,
            int y,
            int ancho
        )
        {
            Button boton =
                new Button();

            boton.Text =
                texto;

            boton.Location =
                new Point(
                    x,
                    y
                );

            boton.Size =
                new Size(
                    ancho,
                    40
                );

            boton.BackColor =
                colorRosa;

            boton.ForeColor =
                Color.White;

            boton.FlatStyle =
                FlatStyle.Flat;

            boton.FlatAppearance.BorderSize =
                0;

            boton.Font =
                new Font(
                    "Segoe UI",
                    9F,
                    FontStyle.Bold
                );

            boton.Cursor =
                Cursors.Hand;

            return boton;
        }


        private Button CrearBotonSecundario(
            string texto,
            int x,
            int y,
            int ancho
        )
        {
            Button boton =
                new Button();

            boton.Text =
                texto;

            boton.Location =
                new Point(
                    x,
                    y
                );

            boton.Size =
                new Size(
                    ancho,
                    40
                );

            boton.BackColor =
                Color.White;

            boton.ForeColor =
                colorRosaOscuro;

            boton.FlatStyle =
                FlatStyle.Flat;

            boton.FlatAppearance.BorderSize =
                1;

            boton.FlatAppearance.BorderColor =
                colorRosa;

            boton.Font =
                new Font(
                    "Segoe UI",
                    9F,
                    FontStyle.Bold
                );

            boton.Cursor =
                Cursors.Hand;

            return boton;
        }


        private DataGridViewTextBoxColumn CrearColumna(
            string nombre,
            string encabezado,
            string propiedad
        )
        {
            DataGridViewTextBoxColumn columna =
                new DataGridViewTextBoxColumn();

            columna.Name =
                nombre;

            columna.HeaderText =
                encabezado;

            columna.DataPropertyName =
                propiedad;

            return columna;
        }


        private void ConfigurarGrilla(
            DataGridView grilla
        )
        {
            grilla.AllowUserToAddRows =
                false;

            grilla.AllowUserToDeleteRows =
                false;

            grilla.ReadOnly =
                true;

            grilla.MultiSelect =
                false;

            grilla.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            grilla.AutoGenerateColumns =
                false;

            grilla.BackgroundColor =
                Color.White;

            grilla.BorderStyle =
                BorderStyle.FixedSingle;

            grilla.RowHeadersVisible =
                false;

            grilla.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;
        }


        // =========================================================
        // CARGAR VENDEDORES
        // =========================================================

        private void CargarVendedores()
        {
            try
            {
                // Obtenemos los usuarios registrados.
                listaUsuarios =
                    cnUsuario.Listar();


                // Filtramos únicamente aquellos
                // cuyo rol corresponde a Vendedor.
                //
                // Según nuestra configuración:
                // 2 = Vendedor.
                List<Usuario> vendedores =
                    listaUsuarios
                        .Where(
                            u =>
                                u.IdRol == 2
                        )
                        .OrderBy(
                            u =>
                                u.Apellido
                        )
                        .ThenBy(
                            u =>
                                u.Nombre
                        )
                        .ToList();


                // Agregamos manualmente una opción
                // especial para consultar todos.
                Usuario todos =
                    new Usuario
                    {
                        IdUsuario = 0,
                        Nombre = "Todos",
                        Apellido = "los vendedores"
                    };


                vendedores.Insert(
                    0,
                    todos
                );


                cboVendedor.DataSource =
                    null;

                cboVendedor.DataSource =
                    vendedores;

                cboVendedor.ValueMember =
                    "IdUsuario";

                cboVendedor.DisplayMember =
                    "Apellido";


                cboVendedor.Format +=
                    cboVendedor_Format;


                cboVendedor.SelectedIndex =
                    0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No fue posible cargar los vendedores."
                    + "\n\n"
                    + ex.Message,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        /// <summary>
        /// Personaliza la forma en que se muestran
        /// los vendedores en el ComboBox.
        /// </summary>
        private void cboVendedor_Format(
            object sender,
            ListControlConvertEventArgs e
        )
        {
            Usuario usuario =
                e.ListItem as Usuario;


            if (usuario == null)
            {
                return;
            }


            if (
                usuario.IdUsuario == 0
            )
            {
                e.Value =
                    "Todos los vendedores";
            }
            else
            {
                e.Value =
                    usuario.Apellido
                    + ", "
                    + usuario.Nombre;
            }
        }


        // =========================================================
        // FILTROS INICIALES
        // =========================================================

        private void PrepararFiltrosIniciales()
        {
            // Mostramos inicialmente el día actual.
            dtpDesde.Value =
                DateTime.Today;

            dtpHasta.Value =
                DateTime.Today;


            if (
                cboVendedor.Items.Count > 0
            )
            {
                cboVendedor.SelectedIndex =
                    0;
            }


            LimpiarResultados();
        }


        // =========================================================
        // BUSCAR
        // =========================================================

        private void btnBuscar_Click(
            object sender,
            EventArgs e
        )
        {
            try
            {
                int? idUsuario =
                    ObtenerIdVendedorSeleccionado();


                listaReporte =
                    cnReporte.ObtenerVentas(
                        dtpDesde.Value,
                        dtpHasta.Value,
                        idUsuario
                    );


                MostrarReporte(
                    listaReporte
                );
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
        // OBTENER VENDEDOR
        // =========================================================

        /// <summary>
        /// Devuelve:
        ///
        /// null -> Todos los vendedores.
        ///
        /// ID -> Un vendedor específico.
        /// </summary>
        private int? ObtenerIdVendedorSeleccionado()
        {
            Usuario usuario =
                cboVendedor.SelectedItem
                as Usuario;


            if (
                usuario == null
                ||
                usuario.IdUsuario == 0
            )
            {
                return null;
            }


            return usuario.IdUsuario;
        }


        // =========================================================
        // MOSTRAR REPORTE
        // =========================================================

        private void MostrarReporte(
            List<ReporteVenta> ventas
        )
        {
            var datos =
                ventas.Select(
                    v => new
                    {
                        v.IdVenta,

                        Fecha =
                            v.FechaVenta.ToString(
                                "dd/MM/yyyy HH:mm"
                            ),

                        v.NroFactura,

                        v.Vendedor,

                        v.Cliente,

                        Total =
                            v.Total.ToString(
                                "N2"
                            )
                    }
                )
                .ToList();


            dgvReporte.DataSource =
                null;

            dgvReporte.DataSource =
                datos;


            lblCantidadVentas.Text =
                "Cantidad de ventas: "
                + ventas.Count;


            decimal totalVendido =
                ventas.Sum(
                    v =>
                        v.Total
                );


            lblTotalVendido.Text =
                "$ "
                + totalVendido.ToString(
                    "N2"
                );
        }


        // =========================================================
        // LIMPIAR
        // =========================================================

        private void btnLimpiar_Click(
            object sender,
            EventArgs e
        )
        {
            PrepararFiltrosIniciales();
        }


        // =========================================================
        // LIMPIAR RESULTADOS
        // =========================================================

        private void LimpiarResultados()
        {
            listaReporte =
                new List<ReporteVenta>();


            dgvReporte.DataSource =
                null;


            lblCantidadVentas.Text =
                "Cantidad de ventas: 0";


            lblTotalVendido.Text =
                "$ 0,00";
        }
    }
}
