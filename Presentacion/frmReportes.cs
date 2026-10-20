
using Entidades;
using Negocio;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Aura_Beauty
{
    /// <summary>
    /// CENTRO DE REPORTES - CAPA PRESENTACIÓN.
    ///
    /// Secciones:
    /// Ventas, Productos, Categorías,
    /// Solicitudes, Vendedores y Clientes.
    ///
    /// Stock fue retirado del Centro de Reportes.
    ///
    /// Esta clase no realiza consultas SQL
    /// ni calcula estadísticas comerciales.
    /// Los resultados provienen de CN_Reporte.
    /// </summary>
    public partial class frmReportes : Form
    {
        // =====================================================
        // CAPA NEGOCIO
        // =====================================================

        private readonly CN_Reporte negocio =
            new CN_Reporte();

        // =====================================================
        // COLORES
        // =====================================================

        private readonly Color rosa =
            Color.FromArgb(195, 139, 147);

        private readonly Color rosaOscuro =
            Color.FromArgb(155, 94, 106);

        private readonly Color beige =
            Color.FromArgb(247, 239, 232);

        private readonly Color fondo =
            Color.FromArgb(250, 246, 243);

        private readonly Color texto =
            Color.FromArgb(64, 55, 57);

        private readonly Color gris =
            Color.FromArgb(130, 118, 120);

        private readonly Color linea =
            Color.FromArgb(231, 220, 217);

        private readonly Color rosaClaro =
            Color.FromArgb(245, 227, 229);

        // =====================================================
        // CONTROLES
        // =====================================================

        private Panel panelLateral;
        private Panel panelPrincipal;

        private TableLayoutPanel contenido;

        private Label lblTituloSeccion;
        private Label lblNombreFiltro;

        private Label lblTituloGrafico1;
        private Label lblTituloGrafico2;

        private Label lblEstado;

        private DateTimePicker dtpDesde;
        private DateTimePicker dtpHasta;

        private ComboBox cboFiltro;

        private Chart grafico1;
        private Chart grafico2;

        private DataGridView dgvResultados;

        private readonly Label[] titulos =
            new Label[6];

        private readonly Label[] valores =
            new Label[6];

        private readonly Dictionary<string, Button> botones =
            new Dictionary<string, Button>();

        // =====================================================
        // ESTADO ACTUAL
        // =====================================================

        private string seccionActual = "Ventas";

        private VistaReporte ultimoReporte;

        // =====================================================
        // CONSTRUCTOR
        // =====================================================

        /// <summary>
        /// Inicializa el formulario y prepara la
        /// sección de Ventas.
        /// </summary>
        public frmReportes()
        {
            InitializeComponent();

            Text = "Aura Beauty - Centro de Reportes";

            StartPosition =
                FormStartPosition.CenterScreen;

            WindowState =
                FormWindowState.Maximized;

            MinimumSize = new Size(1100, 720);

            BackColor = fondo;

            Font = new Font("Segoe UI", 9.5F);

            CrearInterfaz();

            PrepararSeccion("Ventas");

            // Al cambiar fechas se actualizan los filtros
            // para mostrar actividad del nuevo período.
            dtpDesde.ValueChanged += FechasCambiadas;
            dtpHasta.ValueChanged += FechasCambiadas;
        }

        /// <summary>
        /// Evento conservado para compatibilidad
        /// con el archivo Designer.
        /// </summary>
        private void frmReportes_Load(
            object sender,
            EventArgs e)
        {
        }

        // =====================================================
        // INTERFAZ PRINCIPAL
        // =====================================================

        /// <summary>
        /// Divide el formulario en dos columnas.
        ///
        /// Izquierda: menú lateral.
        /// Derecha: contenido del dashboard.
        ///
        /// Esta estructura evita superposiciones.
        /// </summary>
        private void CrearInterfaz()
        {
            SuspendLayout();

            Controls.Clear();

            TableLayoutPanel general =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 1,
                    BackColor = fondo,
                    Margin = Padding.Empty,
                    Padding = Padding.Empty
                };

            general.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Absolute, 210F));

            general.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 100F));

            general.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            Controls.Add(general);

            panelLateral = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = beige,
                Margin = Padding.Empty
            };

            panelPrincipal = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = fondo,
                AutoScroll = true,
                Margin = Padding.Empty
            };

            general.Controls.Add(
                panelLateral, 0, 0);

            general.Controls.Add(
                panelPrincipal, 1, 0);

            CrearMenu();

            contenido = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 1200,
                ColumnCount = 1,
                RowCount = 5,
                Margin = Padding.Empty,
                Padding = new Padding(24, 16, 24, 18),
                BackColor = fondo
            };

            contenido.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 100F));

            contenido.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 103F));

            contenido.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 160F));

            contenido.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 260F));

            contenido.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 310F));

            contenido.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 310F));

            panelPrincipal.Controls.Add(contenido);

            CrearEncabezado();
            CrearFiltros();
            CrearTarjetas();
            CrearGraficos();
            CrearTabla();

            ResumeLayout(true);
        }

        // =====================================================
        // MENÚ LATERAL
        // =====================================================

        /// <summary>
        /// Crea las seis secciones disponibles.
        /// No incluye el panel Stock.
        ///
        /// Volver al menú permanece administrado
        /// por frmMenu.
        /// </summary>
        private void CrearMenu()
        {
            panelLateral.Controls.Add(new Label
            {
                Text = "AURA",
                Location = new Point(24, 28),
                Size = new Size(165, 48),
                ForeColor = rosaOscuro,

                Font = new Font(
                    "Segoe UI",
                    24F,
                    FontStyle.Bold)
            });

            panelLateral.Controls.Add(new Label
            {
                Text = "BEAUTY | REPORTES",
                Location = new Point(25, 82),
                Size = new Size(175, 25),
                ForeColor = gris,

                Font = new Font(
                    "Segoe UI",
                    8.5F,
                    FontStyle.Bold)
            });

            panelLateral.Controls.Add(new Panel
            {
                Location = new Point(20, 125),
                Size = new Size(170, 1),
                BackColor = linea
            });

            string[] secciones =
            {
                "Ventas",
                "Productos",
                "Categorías",
                "Solicitudes",
                "Vendedores",
                "Clientes"
            };

            for (int i = 0; i < secciones.Length; i++)
            {
                string nombre = secciones[i];

                Button boton = new Button
                {
                    Text = "   " + nombre,

                    Location =
                        new Point(15, 160 + 49 * i),

                    Size = new Size(180, 42),

                    TextAlign =
                        ContentAlignment.MiddleLeft,

                    FlatStyle = FlatStyle.Flat,

                    Cursor = Cursors.Hand
                };

                boton.FlatAppearance.BorderSize = 0;

                Redondear(boton, 9);

                boton.Click += (s, e) =>
                {
                    PrepararSeccion(nombre);
                };

                botones[nombre] = boton;

                panelLateral.Controls.Add(boton);
            }
        }

        // =====================================================
        // ENCABEZADO
        // =====================================================

        /// <summary>
        /// Muestra el título del reporte activo.
        /// </summary>
        private void CrearEncabezado()
        {
            Panel encabezado = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = fondo
            };

            lblTituloSeccion = new Label
            {
                Text = "Reportes de ventas",

                Location = new Point(5, 12),

                AutoSize = true,

                ForeColor = texto,

                Font = new Font(
                    "Segoe UI",
                    21F,
                    FontStyle.Bold)
            };

            encabezado.Controls.Add(
                lblTituloSeccion);

            encabezado.Controls.Add(new Label
            {
                Text =
                    "Análisis del rendimiento comercial",

                Location = new Point(7, 59),

                AutoSize = true,

                ForeColor = gris,

                Font = new Font("Segoe UI", 10F)
            });

            contenido.Controls.Add(
                encabezado, 0, 0);
        }

        // =====================================================
        // BARRA DE FILTROS
        // =====================================================

        /// <summary>
        /// Primera fila:
        /// Desde, Hasta y un selector contextual.
        ///
        /// Segunda fila:
        /// Buscar, Limpiar y Exportar PDF.
        ///
        /// Se utiliza un único ComboBox, que cambia
        /// según la sección.
        /// </summary>
        private void CrearFiltros()
        {
            Panel tarjeta = PanelBlanco();

            tarjeta.Dock = DockStyle.Fill;

            tarjeta.Margin =
                new Padding(5, 0, 5, 12);

            tarjeta.Padding =
                new Padding(18, 12, 18, 12);

            TableLayoutPanel filas =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 2
                };

            filas.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 100F));

            filas.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 69F));

            filas.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            TableLayoutPanel campos =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 3,
                    RowCount = 1
                };

            campos.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 23F));

            campos.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 23F));

            campos.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 54F));

            campos.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            dtpDesde = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,

                Value = new DateTime(
                    DateTime.Today.Year,
                    DateTime.Today.Month,
                    1)
            };

            dtpHasta = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };

            cboFiltro = new ComboBox
            {
                DropDownStyle =
                    ComboBoxStyle.DropDownList,

                FormattingEnabled = true
            };

            campos.Controls.Add(
                CampoFiltro("DESDE", dtpDesde),
                0, 0);

            campos.Controls.Add(
                CampoFiltro("HASTA", dtpHasta),
                1, 0);

            TableLayoutPanel campoOpcion =
                CampoFiltro("VENDEDOR", cboFiltro);

            lblNombreFiltro =
                campoOpcion.Controls
                .OfType<Label>()
                .First();

            campos.Controls.Add(
                campoOpcion, 2, 0);

            filas.Controls.Add(campos, 0, 0);

            FlowLayoutPanel acciones =
                new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,

                    FlowDirection =
                        FlowDirection.RightToLeft,

                    WrapContents = false,

                    Padding =
                        new Padding(0, 1, 0, 0)
                };

            Button exportar =
                Boton("Exportar PDF", 155);

            Button limpiar =
                Boton("Limpiar", 125);

            Button buscar =
                Boton("Buscar", 125);

            buscar.Click += (s, e) =>
            {
                Buscar();
            };

            limpiar.Click += (s, e) =>
            {
                Limpiar();
            };

            exportar.Click += (s, e) =>
            {
                ExportarPdf();
            };

            acciones.Controls.Add(exportar);
            acciones.Controls.Add(limpiar);
            acciones.Controls.Add(buscar);

            filas.Controls.Add(acciones, 0, 1);

            tarjeta.Controls.Add(filas);

            contenido.Controls.Add(tarjeta, 0, 1);
        }

        /// <summary>
        /// Crea un filtro con su etiqueta superior.
        /// Todos mantienen las mismas medidas.
        /// </summary>
        private TableLayoutPanel CampoFiltro(
            string titulo,
            Control control)
        {
            TableLayoutPanel campo =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 2,

                    Margin =
                        new Padding(5, 0, 12, 0)
                };

            campo.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 27F));

            campo.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            Label etiqueta = new Label
            {
                Text = titulo,

                Dock = DockStyle.Fill,

                TextAlign =
                    ContentAlignment.MiddleLeft,

                ForeColor = gris,

                Font = new Font(
                    "Segoe UI",
                    8.5F,
                    FontStyle.Bold)
            };

            control.Dock = DockStyle.Top;

            control.Margin =
                new Padding(0, 4, 0, 0);

            campo.Controls.Add(
                etiqueta, 0, 0);

            campo.Controls.Add(
                control, 0, 1);

            return campo;
        }

        // =====================================================
        // TARJETAS ESTADÍSTICAS
        // =====================================================

        /// <summary>
        /// Distribuye seis tarjetas en dos filas
        /// de tres columnas proporcionales.
        /// </summary>
        private void CrearTarjetas()
        {
            TableLayoutPanel tabla =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,

                    ColumnCount = 3,

                    RowCount = 2,

                    Margin = new Padding(0, 4, 0, 9)
                };

            for (int columna = 0; columna < 3; columna++)
            {
                tabla.ColumnStyles.Add(
                    new ColumnStyle(
                        SizeType.Percent,
                        100F / 3F));
            }

            tabla.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 50F));

            tabla.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 50F));

            for (int i = 0; i < 6; i++)
            {
                Panel panel = PanelBlanco();

                panel.Dock = DockStyle.Fill;

                panel.Margin =
                    new Padding(5, 4, 7, 8);

                panel.Padding =
                    new Padding(16, 10, 10, 8);

                TableLayoutPanel interior =
                    new TableLayoutPanel
                    {
                        Dock = DockStyle.Fill,
                        ColumnCount = 1,
                        RowCount = 2
                    };

                interior.RowStyles.Add(
                    new RowStyle(
                        SizeType.Percent, 38F));

                interior.RowStyles.Add(
                    new RowStyle(
                        SizeType.Percent, 62F));

                titulos[i] = new Label
                {
                    Dock = DockStyle.Fill,

                    ForeColor = gris,

                    Font = new Font(
                        "Segoe UI",
                        8.7F,
                        FontStyle.Bold),

                    TextAlign =
                        ContentAlignment.MiddleLeft
                };

                valores[i] = new Label
                {
                    Dock = DockStyle.Fill,

                    ForeColor = texto,

                    Font = new Font(
                        "Segoe UI",
                        16F,
                        FontStyle.Bold),

                    AutoEllipsis = true,

                    TextAlign =
                        ContentAlignment.MiddleLeft
                };

                interior.Controls.Add(
                    titulos[i], 0, 0);

                interior.Controls.Add(
                    valores[i], 0, 1);

                panel.Controls.Add(interior);

                tabla.Controls.Add(
                    panel,
                    i % 3,
                    i / 3);
            }

            contenido.Controls.Add(tabla, 0, 2);
        }

        // =====================================================
        // GRÁFICOS
        // =====================================================

        /// <summary>
        /// Crea dos gráficos distribuidos en columnas.
        /// </summary>
        private void CrearGraficos()
        {
            TableLayoutPanel dos =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 1,

                    Margin =
                        new Padding(0, 0, 0, 12)
                };

            dos.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 50F));

            dos.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent, 50F));

            dos.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            for (int i = 0; i < 2; i++)
            {
                Panel panel = PanelBlanco();

                panel.Dock = DockStyle.Fill;

                panel.Margin = i == 0
                    ? new Padding(5, 0, 7, 0)
                    : new Padding(7, 0, 5, 0);

                panel.Padding = new Padding(12);

                TableLayoutPanel interior =
                    new TableLayoutPanel
                    {
                        Dock = DockStyle.Fill,
                        ColumnCount = 1,
                        RowCount = 2
                    };

                interior.RowStyles.Add(
                    new RowStyle(
                        SizeType.Absolute, 40F));

                interior.RowStyles.Add(
                    new RowStyle(
                        SizeType.Percent, 100F));

                Label titulo = new Label
                {
                    Dock = DockStyle.Fill,

                    TextAlign =
                        ContentAlignment.MiddleLeft,

                    ForeColor = texto,

                    Font = new Font(
                        "Segoe UI",
                        12F,
                        FontStyle.Bold)
                };

                Chart grafico = CrearGrafico();

                grafico.Dock = DockStyle.Fill;

                interior.Controls.Add(
                    titulo, 0, 0);

                interior.Controls.Add(
                    grafico, 0, 1);

                panel.Controls.Add(interior);

                dos.Controls.Add(panel, i, 0);

                if (i == 0)
                {
                    lblTituloGrafico1 = titulo;
                    grafico1 = grafico;
                }
                else
                {
                    lblTituloGrafico2 = titulo;
                    grafico2 = grafico;
                }
            }

            contenido.Controls.Add(dos, 0, 3);
        }

        /// <summary>
        /// Configura un gráfico de columnas.
        /// El ancho máximo de cada barra es de 60 píxeles.
        /// </summary>
        private Chart CrearGrafico()
        {
            Chart grafico = new Chart
            {
                BackColor = Color.White,
                Margin = Padding.Empty
            };

            ChartArea area =
                new ChartArea("Principal")
                {
                    BackColor = Color.White
                };

            area.AxisX.MajorGrid.Enabled = false;

            area.AxisY.MajorGrid.LineColor = linea;

            area.AxisX.LineColor = linea;
            area.AxisY.LineColor = linea;

            area.AxisX.LabelStyle.ForeColor = gris;
            area.AxisY.LabelStyle.ForeColor = gris;

            area.AxisX.LabelStyle.Font =
                new Font("Segoe UI", 8F);

            area.AxisY.LabelStyle.Font =
                new Font("Segoe UI", 8F);

            area.AxisX.Interval = 1;

            area.AxisX.LabelStyle.Angle = -30;

            grafico.ChartAreas.Add(area);

            Series serie = new Series("Valores")
            {
                ChartType = SeriesChartType.Column,

                XValueType = ChartValueType.String,

                YValueType = ChartValueType.Double,

                Color = rosa
            };

            serie.SetCustomProperty(
                "MaxPixelPointWidth", "60");

            grafico.Series.Add(serie);

            return grafico;
        }

        // =====================================================
        // TABLA DE DETALLE
        // =====================================================

        /// <summary>
        /// Crea el DataGridView de solo lectura.
        /// Las columnas cambian según el reporte.
        /// </summary>
        private void CrearTabla()
        {
            Panel tarjeta = PanelBlanco();

            tarjeta.Dock = DockStyle.Fill;

            tarjeta.Margin =
                new Padding(5, 0, 5, 5);

            tarjeta.Padding =
                new Padding(15, 12, 15, 10);

            TableLayoutPanel interior =
                new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 3
                };

            interior.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 36F));

            interior.RowStyles.Add(
                new RowStyle(
                    SizeType.Percent, 100F));

            interior.RowStyles.Add(
                new RowStyle(
                    SizeType.Absolute, 46F));

            interior.Controls.Add(new Label
            {
                Text = "Detalle del reporte",

                Dock = DockStyle.Fill,

                ForeColor = texto,

                Font = new Font(
                    "Segoe UI",
                    12F,
                    FontStyle.Bold)
            }, 0, 0);

            dgvResultados = new DataGridView
            {
                Dock = DockStyle.Fill,

                BackgroundColor = Color.White,

                BorderStyle = BorderStyle.None,

                ReadOnly = true,

                AllowUserToAddRows = false,

                AllowUserToDeleteRows = false,

                RowHeadersVisible = false,

                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,

                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,

                EnableHeadersVisualStyles = false
            };

            dgvResultados.ColumnHeadersHeight = 34;
            dgvResultados.RowTemplate.Height = 29;

            dgvResultados.ColumnHeadersDefaultCellStyle.BackColor =
                rosaClaro;

            dgvResultados.ColumnHeadersDefaultCellStyle.ForeColor =
                texto;

            dgvResultados.ColumnHeadersDefaultCellStyle.Font =
                new Font(
                    "Segoe UI", 9F, FontStyle.Bold);

            dgvResultados.DefaultCellStyle.SelectionBackColor =
                rosaClaro;

            dgvResultados.DefaultCellStyle.SelectionForeColor =
                texto;

            dgvResultados.AlternatingRowsDefaultCellStyle.BackColor =
                fondo;

            lblEstado = new Label
            {
                Dock = DockStyle.Fill,

                ForeColor = gris,

                Font = new Font("Segoe UI", 8.5F),

                TextAlign =
                    ContentAlignment.MiddleLeft,

                AutoEllipsis = true
            };

            interior.Controls.Add(
                dgvResultados, 0, 1);

            interior.Controls.Add(
                lblEstado, 0, 2);

            tarjeta.Controls.Add(interior);

            contenido.Controls.Add(tarjeta, 0, 4);
        }

        // =====================================================
        // NAVEGACIÓN
        // =====================================================

        /// <summary>
        /// Selecciona una sección, cambia el nombre
        /// del filtro y carga sus opciones.
        ///
        /// No cambia las medidas del formulario.
        /// </summary>
        private void PrepararSeccion(string nueva)
        {
            seccionActual = nueva;

            foreach (KeyValuePair<string, Button>
                opcion in botones)
            {
                bool activo =
                    opcion.Key == nueva;

                opcion.Value.BackColor =
                    activo ? rosa : beige;

                opcion.Value.ForeColor =
                    activo ? Color.White : texto;

                opcion.Value.Font = new Font(
                    "Segoe UI",
                    10F,

                    activo
                        ? FontStyle.Bold
                        : FontStyle.Regular);
            }

            lblTituloSeccion.Text =
                "Reportes de " +
                nueva.ToLowerInvariant();

            // Cada sección muestra su filtro específico.
            if (nueva == "Ventas" ||
                nueva == "Vendedores")
            {
                lblNombreFiltro.Text = "VENDEDOR";
            }
            else if (nueva == "Productos")
            {
                lblNombreFiltro.Text = "PRODUCTO";
            }
            else if (nueva == "Categorías")
            {
                lblNombreFiltro.Text = "CATEGORÍA";
            }
            else if (nueva == "Solicitudes")
            {
                lblNombreFiltro.Text = "ESTADO";
            }
            else
            {
                lblNombreFiltro.Text = "CLIENTE";
            }

            ultimoReporte = null;

            LimpiarResultadosVisuales();

            try
            {
                List<OpcionReporte> opciones =
                    negocio.ObtenerOpciones(
                        nueva,
                        dtpDesde.Value.Date,
                        dtpHasta.Value.Date);

                cboFiltro.DataSource = null;

                cboFiltro.DisplayMember = "Nombre";
                cboFiltro.ValueMember = "Id";

                cboFiltro.DataSource = opciones;

                if (cboFiltro.Items.Count > 0)
                {
                    cboFiltro.SelectedIndex = 0;
                }

                lblEstado.Text =
                    "Seleccioná los filtros y presioná Buscar.";
            }
            catch (Exception ex)
            {
                cboFiltro.DataSource = null;

                lblEstado.Text =
                    "No se pudo cargar el filtro: " +
                    ex.Message;
            }
        }

        /// <summary>
        /// Recarga el selector cuando cambian las fechas,
        /// para no mostrar productos o categorías que
        /// no tuvieron actividad en el nuevo período.
        /// </summary>
        private void FechasCambiadas(
            object sender,
            EventArgs e)
        {
            if (dtpDesde.Value.Date >
                dtpHasta.Value.Date)
            {
                return;
            }

            PrepararSeccion(seccionActual);
        }

        // =====================================================
        // LIMPIAR FILTROS
        // =====================================================

        /// <summary>
        /// Restablece fechas y selección.
        /// </summary>
        private void Limpiar()
        {
            dtpDesde.Value = new DateTime(
                DateTime.Today.Year,
                DateTime.Today.Month,
                1);

            dtpHasta.Value = DateTime.Today;

            if (cboFiltro.Items.Count > 0)
            {
                cboFiltro.SelectedIndex = 0;
            }

            ultimoReporte = null;

            LimpiarResultadosVisuales();

            lblEstado.Text =
                "Sin consulta realizada.";
        }

        /// <summary>
        /// Limpia las tarjetas, gráficos y tabla,
        /// sin modificar la base de datos.
        /// </summary>
        private void LimpiarResultadosVisuales()
        {
            for (int i = 0; i < 6; i++)
            {
                titulos[i].Text = "";
                valores[i].Text = "";
            }

            grafico1.Series[0].Points.Clear();
            grafico2.Series[0].Points.Clear();

            lblTituloGrafico1.Text = "";
            lblTituloGrafico2.Text = "";

            dgvResultados.DataSource = null;

            lblEstado.Text = "";
        }

        // =====================================================
        // BUSCAR
        // =====================================================

        /// <summary>
        /// Solicita el reporte a CN_Reporte y presenta
        /// sus resultados.
        ///
        /// Si ocurre un error, el formulario permanece
        /// abierto y muestra el mensaje correspondiente.
        /// </summary>
        private void Buscar()
        {
            if (dtpDesde.Value.Date >
                dtpHasta.Value.Date)
            {
                MessageBox.Show(
                    "La fecha Desde no puede superar a Hasta.",
                    "Aura Beauty");

                return;
            }

            OpcionReporte opcion =
                cboFiltro.SelectedItem as OpcionReporte;

            int id = opcion == null
                ? 0
                : opcion.Id;

            try
            {
                this.UseWaitCursor = true;

                VistaReporte reporte =
                    negocio.ConstruirReporte(
                        seccionActual,
                        dtpDesde.Value.Date,
                        dtpHasta.Value.Date,
                        id);

                PintarReporte(reporte);

                ultimoReporte = reporte;
            }
            catch (Exception ex)
            {
                ultimoReporte = null;

                lblEstado.Text =
                    "Error en " +
                    seccionActual + ": " +
                    ex.Message;

                MessageBox.Show(
                    "No se pudo consultar el reporte de " +
                    seccionActual + ":\n" +
                    ex.Message,

                    "Aura Beauty",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Warning);
            }
            finally
            {
                this.UseWaitCursor = false;
            }
        }

        // =====================================================
        // MOSTRAR RESULTADOS
        // =====================================================

        /// <summary>
        /// Actualiza seis indicadores, dos gráficos
        /// y la tabla de detalle.
        ///
        /// Los datos ya fueron calculados en Negocio.
        /// </summary>
        private void PintarReporte(VistaReporte reporte)
        {
            for (int i = 0; i < 6; i++)
            {
                titulos[i].Text =
                    reporte.TitulosIndicadores[i] ?? "";

                valores[i].Text =
                    reporte.ValoresIndicadores[i] ?? "";

                float tamano =
                    valores[i].Text.Length > 21
                        ? 11F
                        : valores[i].Text.Length > 14
                            ? 13F
                            : 16F;

                valores[i].Font = new Font(
                    "Segoe UI",
                    tamano,
                    FontStyle.Bold);
            }

            lblTituloGrafico1.Text =
                reporte.TituloGrafico1;

            lblTituloGrafico2.Text =
                reporte.TituloGrafico2;

            PintarGrafico(
                grafico1,
                reporte.PuntosGrafico1,
                reporte.Grafico1EsCantidad);

            PintarGrafico(
                grafico2,
                reporte.PuntosGrafico2,
                reporte.Grafico2EsCantidad);

            dgvResultados.DataSource =
                reporte.Detalle;

            lblEstado.Text =
                "Filas: " +
                reporte.Detalle.Rows.Count +
                " | " +
                (reporte.Aviso ?? "");

            lblEstado.Refresh();
        }

        // =====================================================
        // ESCALA DE LOS GRÁFICOS
        // =====================================================

        /// <summary>
        /// Presenta columnas con escala monetaria
        /// o escala entera según corresponda.
        ///
        /// Las cantidades siempre tienen marcas
        /// enteras en el eje vertical.
        /// </summary>
        private void PintarGrafico(
            Chart grafico,
            List<PuntoReporte> puntos,
            bool entero)
        {
            Series serie = grafico.Series[0];

            serie.Points.Clear();

            decimal mayor = 0m;

            foreach (PuntoReporte punto in puntos)
            {
                decimal valor =
                    punto.Valor < 0m
                        ? 0m
                        : punto.Valor;

                string corto =
                    punto.Etiqueta ?? "Sin nombre";

                if (corto.Length > 18)
                {
                    corto =
                        corto.Substring(0, 15) + "...";
                }

                int indice =
                    serie.Points.AddXY(
                        corto,
                        (double)valor);

                serie.Points[indice].ToolTip =
                    punto.Etiqueta + ": " +
                    (entero
                        ? valor.ToString("N0")
                        : valor.ToString("N2"));

                if (valor > mayor)
                    mayor = valor;
            }

            Axis eje =
                grafico.ChartAreas[0].AxisY;

            eje.Minimum = 0;

            eje.LabelStyle.Format = "N0";

            eje.IsStartedFromZero = true;

            if (entero)
            {
                int maximo =
                    (int)Math.Min(
                        int.MaxValue,
                        mayor);

                int paso =
                    maximo <= 8 ? 1 :
                    maximo <= 20 ? 2 :
                    maximo <= 50 ? 5 :
                    maximo <= 100 ? 10 :
                    maximo <= 250 ? 25 :
                    maximo <= 500 ? 50 : 100;

                eje.Interval = paso;

                eje.Maximum = Math.Max(
                    paso * 3,

                    (int)(
                        Math.Ceiling(
                            (maximo + 1.0) / paso) *
                        paso));

                serie.IsValueShownAsLabel = true;

                serie.LabelFormat = "N0";
            }
            else
            {
                eje.Maximum = double.NaN;

                eje.Interval = 0;

                serie.IsValueShownAsLabel = false;
            }
        }

        // =====================================================
        // EXPORTAR PDF
        // =====================================================

        /// <summary>
        /// Exporta los indicadores y las filas
        /// del último reporte mediante PdfSharp.
        ///
        /// No realiza consultas ni modifica SQL Server.
        /// </summary>
        private void ExportarPdf()
        {
            if (ultimoReporte == null)
            {
                MessageBox.Show(
                    "Primero realizá una búsqueda.",
                    "Aura Beauty");

                return;
            }

            try
            {
                using (PdfDocument documento =
                    new PdfDocument())
                {
                    documento.Info.Title =
                        "Aura Beauty - " + seccionActual;

                    PdfPage pagina =
                        documento.AddPage();

                    XGraphics grafica =
                        XGraphics.FromPdfPage(pagina);

                    XFont fuenteTitulo = new XFont(
                        "Segoe UI",
                        16,
                        XFontStyleEx.Bold);

                    XFont fuenteNormal = new XFont(
                        "Segoe UI",
                        9,
                        XFontStyleEx.Regular);

                    int y = 45;

                    grafica.DrawString(
                        "AURA BEAUTY - " +
                        seccionActual.ToUpperInvariant(),

                        fuenteTitulo,

                        XBrushes.Black,

                        new XPoint(38, y));

                    y += 30;

                    grafica.DrawString(
                        "Desde " +
                        dtpDesde.Value.ToString("dd/MM/yyyy") +
                        " hasta " +
                        dtpHasta.Value.ToString("dd/MM/yyyy"),

                        fuenteNormal,

                        XBrushes.Black,

                        new XPoint(38, y));

                    y += 27;

                    for (int i = 0; i < 6; i++)
                    {
                        string linea =
                            titulos[i].Text + ": " +
                            valores[i].Text;

                        grafica.DrawString(
                            Recortar(linea, 95),

                            fuenteNormal,

                            XBrushes.Black,

                            new XPoint(38, y));

                        y += 22;
                    }

                    y += 12;

                    grafica.DrawString(
                        "DETALLE",

                        fuenteTitulo,

                        XBrushes.Black,

                        new XPoint(38, y));

                    y += 24;

                    foreach (System.Data.DataRow fila in
                        ultimoReporte.Detalle.Rows)
                    {
                        if (y >= 780)
                        {
                            pagina =
                                documento.AddPage();

                            grafica.Dispose();

                            grafica =
                                XGraphics.FromPdfPage(
                                    pagina);

                            y = 45;
                        }

                        string linea = string.Join(
                            " | ",

                            fila.ItemArray.Select(
                                celda =>
                                    Convert.ToString(celda)));

                        grafica.DrawString(
                            Recortar(linea, 102),

                            fuenteNormal,

                            XBrushes.Black,

                            new XPoint(38, y));

                        y += 18;
                    }

                    grafica.Dispose();

                    string ruta = Path.Combine(
                        Path.GetTempPath(),

                        "AuraBeauty_" +
                        seccionActual + "_" +
                        DateTime.Now.ToString(
                            "yyyyMMdd_HHmmss") +
                        ".pdf");

                    documento.Save(ruta);

                    Process.Start(
                        new ProcessStartInfo(ruta)
                        {
                            UseShellExecute = true
                        });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al exportar PDF: " +
                    ex.Message,

                    "Aura Beauty",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Recorta textos demasiado largos
        /// para la exportación.
        /// </summary>
        private static string Recortar(
            string texto,
            int maximo)
        {
            return texto.Length <= maximo
                ? texto
                : texto.Substring(
                    0, maximo - 3) + "...";
        }

        // =====================================================
        // COMPONENTES VISUALES
        // =====================================================

        /// <summary>
        /// Crea un panel blanco con esquinas redondeadas.
        /// </summary>
        private Panel PanelBlanco()
        {
            Panel panel = new Panel
            {
                BackColor = Color.White
            };

            Redondear(panel, 14);

            return panel;
        }

        /// <summary>
        /// Crea un botón rosa nude con el ancho indicado.
        /// </summary>
        private Button Boton(
            string titulo,
            int ancho)
        {
            Button boton = new Button
            {
                Text = titulo,

                Width = ancho,

                Height = 37,

                Margin =
                    new Padding(5, 0, 5, 0),

                FlatStyle = FlatStyle.Flat,

                BackColor = rosa,

                ForeColor = Color.White,

                Font = new Font(
                    "Segoe UI",
                    9F,
                    FontStyle.Bold),

                Cursor = Cursors.Hand
            };

            boton.FlatAppearance.BorderSize = 0;

            boton.MouseEnter += (s, e) =>
            {
                boton.BackColor = rosaOscuro;
            };

            boton.MouseLeave += (s, e) =>
            {
                boton.BackColor = rosa;
            };

            Redondear(boton, 9);

            return boton;
        }

        // =====================================================
        // BORDES REDONDEADOS
        // =====================================================

        /// <summary>
        /// Vuelve a calcular la región redondeada
        /// cuando el control cambia de tamaño.
        /// </summary>
        private void Redondear(
            Control control,
            int radio)
        {
            control.Resize += (s, e) =>
            {
                AplicarRedondeo(control, radio);
            };

            AplicarRedondeo(control, radio);
        }

        /// <summary>
        /// Dibuja las esquinas redondeadas usando
        /// GraphicsPath.
        /// </summary>
        private static void AplicarRedondeo(
            Control control,
            int radio)
        {
            if (control.Width <= 0 ||
                control.Height <= 0)
            {
                return;
            }

            int diametro = Math.Min(
                radio * 2,

                Math.Min(
                    control.Width,
                    control.Height));

            using (GraphicsPath ruta =
                new GraphicsPath())
            {
                ruta.AddArc(
                    0,
                    0,
                    diametro,
                    diametro,
                    180,
                    90);

                ruta.AddArc(
                    control.Width - diametro,
                    0,
                    diametro,
                    diametro,
                    270,
                    90);

                ruta.AddArc(
                    control.Width - diametro,
                    control.Height - diametro,
                    diametro,
                    diametro,
                    0,
                    90);

                ruta.AddArc(
                    0,
                    control.Height - diametro,
                    diametro,
                    diametro,
                    90,
                    90);

                ruta.CloseFigure();

                Region anterior = control.Region;

                control.Region = new Region(ruta);

                if (anterior != null)
                {
                    anterior.Dispose();
                }
            }
        }
    }
}
