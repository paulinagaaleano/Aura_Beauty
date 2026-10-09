using Entidades;
using Negocio;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Aura_Beauty
{
    public partial class frmReportes : Form
    {
        private readonly CN_Reporte cnReporte = new CN_Reporte();
        private readonly CN_Usuario cnUsuario = new CN_Usuario();

        private List<Usuario> listaUsuarios = new List<Usuario>();
        private List<ReporteVenta> listaReporteVentas = new List<ReporteVenta>();
        private DataTable dtMovimientosStock = new DataTable();

        // COLORES AURA BEAUTY
        private readonly Color colorRosa = Color.FromArgb(201, 143, 149);
        private readonly Color colorRosaOscuro = Color.FromArgb(174, 112, 120);
        private readonly Color colorFondo = Color.FromArgb(255, 249, 248);
        private readonly Color colorTexto = Color.FromArgb(94, 74, 74);

        // CONTROLES
        private ComboBox cboTipoReporte;
        private DateTimePicker dtpDesde;
        private DateTimePicker dtpHasta;
        private ComboBox cboVendedor;
        private TextBox txtBuscarStock;
        private Label lblVendedor;
        private Label lblDesde;
        private Label lblHasta;
        private Label lblBuscarStock;

        private Button btnBuscar;
        private Button btnLimpiar;
        private Button btnExportarPDF;

        private DataGridView dgvReporte;
        private Label lblResumen1;
        private Label lblResumen2;

        private Label lblFacturacion;
        private Label lblCantidadVentas;
        private Label lblTicketPromedio;
        private Label lblUnidadesVendidas;
        private Label lblVentaMaxima;
        private Label lblVentaMinima;

        // Etiquetas que mostraran la comparacion entre meses.
        private Label lblFacturacionActual;
        private Label lblFacturacionAnterior;
        private Label lblDiferenciaFacturacion;
        private Label lblVariacionPorcentual;


        public frmReportes()
        {
            InitializeComponent();
            ConfigurarFormulario();
            CrearInterfaz();
            CargarVendedores();
            CambiarTipoReporte();
        }

        private void frmReportes_Load(object sender, EventArgs e)
        {
            // Evento Load del formulario
        }


        private void ConfigurarFormulario()
        {
            // Configura el título y la posición inicial del formulario.
            Text = "Aura Beauty - Centro de Reportes";
            StartPosition = FormStartPosition.CenterScreen;

            // Permite aprovechar el espacio disponible de la pantalla.
            WindowState = FormWindowState.Maximized;

            // Evita que el formulario se reduzca demasiado.
            MinimumSize = new Size(1000, 700);

            // Conserva los colores y la tipografía del sistema.
            BackColor = colorFondo;
            Font = new Font("Segoe UI", 10F);
        }



        private void CrearInterfaz()
        {
            // =====================================================
            // 1. ENCABEZADO DEL CENTRO DE REPORTES
            // =====================================================

            Panel panelEncabezado = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = colorRosa
            };

            Controls.Add(panelEncabezado);

            // Título principal del formulario.
            Label lblTitulo = new Label
            {
                Text = "CENTRO DE REPORTES",
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(30, 20)
            };

            panelEncabezado.Controls.Add(lblTitulo);

            // Texto descriptivo del contenido de los reportes.
            Label lblSubtitulo = new Label
            {
                Text = "Consulta consolidada de ventas e historial de movimientos de stock",
                Font = new Font("Segoe UI", 11F),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(33, 65)
            };

            panelEncabezado.Controls.Add(lblSubtitulo);


            // =====================================================
            // 2. PANEL DE FILTROS
            // =====================================================

            Panel panelFiltros = new Panel
            {
                Location = new Point(20, 125),
                Size = new Size(1040, 125),
                BackColor = Color.White
            };

            Controls.Add(panelFiltros);

            // Selector del tipo de reporte.
            panelFiltros.Controls.Add(
                CrearEtiqueta("Tipo de Reporte:", 20, 15));

            cboTipoReporte = new ComboBox
            {
                Location = new Point(20, 40),
                Size = new Size(180, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Opciones de reportes disponibles.
            cboTipoReporte.Items.Add("Ventas");
            cboTipoReporte.Items.Add("Movimientos de Stock");

            // Selecciona Ventas como opción inicial.
            cboTipoReporte.SelectedIndex = 0;

            // Actualiza los controles cuando cambia el tipo de reporte.
            cboTipoReporte.SelectedIndexChanged +=
                (s, e) => CambiarTipoReporte();

            panelFiltros.Controls.Add(cboTipoReporte);


            // Filtros por fecha de inicio y finalización.
            lblDesde = CrearEtiqueta("Desde:", 220, 15);

            dtpDesde = new DateTimePicker
            {
                Location = new Point(220, 40),
                Size = new Size(120, 30),
                Format = DateTimePickerFormat.Short
            };

            lblHasta = CrearEtiqueta("Hasta:", 350, 15);

            dtpHasta = new DateTimePicker
            {
                Location = new Point(350, 40),
                Size = new Size(120, 30),
                Format = DateTimePickerFormat.Short
            };

            panelFiltros.Controls.Add(lblDesde);
            panelFiltros.Controls.Add(dtpDesde);
            panelFiltros.Controls.Add(lblHasta);
            panelFiltros.Controls.Add(dtpHasta);


            // Filtro por vendedor para el reporte de ventas.
            lblVendedor = CrearEtiqueta("Vendedor:", 480, 15);

            cboVendedor = new ComboBox
            {
                Location = new Point(480, 40),
                Size = new Size(200, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            panelFiltros.Controls.Add(lblVendedor);
            panelFiltros.Controls.Add(cboVendedor);


            // Campo de búsqueda para los movimientos de stock.
            lblBuscarStock = CrearEtiqueta("Buscar Producto:", 480, 15);

            txtBuscarStock = new TextBox
            {
                Location = new Point(480, 40),
                Size = new Size(220, 30)
            };

            panelFiltros.Controls.Add(lblBuscarStock);
            panelFiltros.Controls.Add(txtBuscarStock);


            // Botón para ejecutar el reporte seleccionado.
            btnBuscar = CrearBoton("BUSCAR", 720, 35, 95);
            btnBuscar.Click += btnBuscar_Click;
            panelFiltros.Controls.Add(btnBuscar);

            // Botón para limpiar filtros y resultados.
            btnLimpiar = CrearBotonSecundario("LIMPIAR", 825, 35, 95);
            btnLimpiar.Click += btnLimpiar_Click;
            panelFiltros.Controls.Add(btnLimpiar);

            // Botón para exportar los resultados a PDF.
            btnExportarPDF = new Button
            {
                Text = "DESCARGAR PDF",
                Location = new Point(930, 35),
                Size = new Size(95, 40),
                BackColor = colorRosaOscuro,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnExportarPDF.FlatAppearance.BorderSize = 0;
            btnExportarPDF.Click += btnExportarPDF_Click;
            panelFiltros.Controls.Add(btnExportarPDF);


            // =====================================================
            // 3. PANEL PRINCIPAL DE RESULTADOS
            // =====================================================

            // El panel ocupa el espacio restante debajo de los filtros.
            Panel panelResultados = new Panel
            {
                Location = new Point(20, 265),
                Size = new Size(
                    ClientSize.Width - 40,
                    ClientSize.Height - 285),

                Anchor = AnchorStyles.Top |
                         AnchorStyles.Bottom |
                         AnchorStyles.Left |
                         AnchorStyles.Right,

                BackColor = Color.White,

                // Permite desplazarse si el contenido supera la altura.
                AutoScroll = true
            };

            Controls.Add(panelResultados);


            // =====================================================
            // 4. TARJETAS DE INDICADORES DE VENTAS
            // =====================================================

            // Cada tarjeta muestra un indicador calculado en Negocio.
            lblFacturacion = CrearIndicador(
                "FACTURACIÓN TOTAL", 20, 20);

            lblCantidadVentas = CrearIndicador(
                "CANTIDAD DE VENTAS", 190, 20);

            lblTicketPromedio = CrearIndicador(
                "TICKET PROMEDIO", 360, 20);

            lblUnidadesVendidas = CrearIndicador(
                "UNIDADES VENDIDAS", 530, 20);

            lblVentaMaxima = CrearIndicador(
                "VENTA MÁXIMA", 700, 20);

            lblVentaMinima = CrearIndicador(
                "VENTA MÍNIMA", 870, 20);

            // Agrega las seis tarjetas al panel.
            panelResultados.Controls.Add(lblFacturacion);
            panelResultados.Controls.Add(lblCantidadVentas);
            panelResultados.Controls.Add(lblTicketPromedio);
            panelResultados.Controls.Add(lblUnidadesVendidas);
            panelResultados.Controls.Add(lblVentaMaxima);
            panelResultados.Controls.Add(lblVentaMinima);


            // =====================================================
            // 5. GRILLA DE RESULTADOS
            // =====================================================

            // Muestra las ventas o los movimientos de stock consultados.
            dgvReporte = new DataGridView
            {
                Location = new Point(20, 115),

                // Se adapta al ancho del panel y deja espacio debajo.
                Size = new Size(
                    panelResultados.ClientSize.Width - 40,
                    150),

                Anchor = AnchorStyles.Top |
                         AnchorStyles.Left |
                         AnchorStyles.Right
            };

            // Aplica las propiedades de visualización ya definidas.
            ConfigurarGrilla(dgvReporte);

            panelResultados.Controls.Add(dgvReporte);


            // =====================================================
            // 6. COMPARACIÓN MENSUAL
            // =====================================================

            // Muestra la facturación del mes seleccionado.
            lblFacturacionActual = CrearEtiqueta(
                "Facturación del mes seleccionado: pendiente",
                23, 280);

            // Muestra la facturación del mes anterior.
            lblFacturacionAnterior = CrearEtiqueta(
                "Facturación del mes anterior: pendiente",
                23, 310);

            // Muestra la diferencia monetaria entre ambos períodos.
            lblDiferenciaFacturacion = CrearEtiqueta(
                "Diferencia de facturación: pendiente",
                23, 340);

            // Muestra el porcentaje de variación mensual.
            lblVariacionPorcentual = CrearEtiqueta(
                "Variación porcentual: pendiente",
                23, 370);

            // Texto que ayuda a interpretar la variación mensual.
            Label lblAyudaVariacion = new Label
            {
                Text = "Variación mensual: porcentaje de aumento o disminución " +
                       "de la facturación respecto del mes anterior.",

                Location = new Point(23, 400),
                Size = new Size(980, 40),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9F),
                ForeColor = colorTexto
            };

            // Agrega cada etiqueta una sola vez.
            panelResultados.Controls.Add(lblFacturacionActual);
            panelResultados.Controls.Add(lblFacturacionAnterior);
            panelResultados.Controls.Add(lblDiferenciaFacturacion);
            panelResultados.Controls.Add(lblVariacionPorcentual);
            panelResultados.Controls.Add(lblAyudaVariacion);


            // =====================================================
            // 7. RESUMEN INFERIOR
            // =====================================================

            // Estas etiquetas se actualizan al ejecutar una consulta.
            lblResumen1 = CrearEtiqueta("", 23, 445);
            lblResumen2 = CrearEtiqueta("", 23, 475);

            panelResultados.Controls.Add(lblResumen1);
            panelResultados.Controls.Add(lblResumen2);
        }


        private void CambiarTipoReporte()
        {
            bool esVentas = cboTipoReporte.SelectedIndex == 0;

            lblVendedor.Visible = esVentas;
            cboVendedor.Visible = esVentas;

            lblBuscarStock.Visible = !esVentas;
            txtBuscarStock.Visible = !esVentas;

            LimpiarResultados();
        }

        private void CargarVendedores()
        {
            try
            {
                listaUsuarios = cnUsuario.Listar();
                var vendedores = listaUsuarios.Where(u => u.IdRol == 2).OrderBy(u => u.Apellido).ToList();
                vendedores.Insert(0, new Usuario { IdUsuario = 0, Nombre = "Todos", Apellido = "los vendedores" });

                cboVendedor.DataSource = vendedores;
                cboVendedor.ValueMember = "IdUsuario";
                cboVendedor.DisplayMember = "Apellido";
                cboVendedor.Format += (s, e) =>
                {
                    if (e.ListItem is Usuario u)
                        e.Value = u.IdUsuario == 0 ? "Todos los vendedores" : $"{u.Apellido}, {u.Nombre}";
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar vendedores: " + ex.Message, "Aura Beauty", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboTipoReporte.SelectedIndex == 0)
                {
                    int? idUsuario =
                        (cboVendedor.SelectedItem as Usuario)?.IdUsuario;

                    if (idUsuario == 0)
                        idUsuario = null;


                    // OBTENER RESUMEN CALCULADO EN NEGOCIO
                    ResumenVentas resumen =
                        cnReporte.ObtenerResumenVentas(
                            dtpDesde.Value,
                            dtpHasta.Value,
                            idUsuario);


                    // MOSTRAR INDICADORES

                    lblFacturacion.Text =
                        "FACTURACIÓN TOTAL\n$ " +
                        resumen.FacturacionTotal.ToString("N2");

                    lblCantidadVentas.Text =
                        "CANTIDAD DE VENTAS\n" +
                        resumen.CantidadVentas.ToString();

                    lblTicketPromedio.Text =
                        "TICKET PROMEDIO\n$ " +
                        resumen.TicketPromedio.ToString("N2");

                    lblUnidadesVendidas.Text =
                        "UNIDADES VENDIDAS\n" +
                        resumen.UnidadesVendidas.ToString();

                    lblVentaMaxima.Text =
                        "VENTA MÁXIMA\n$ " +
                        resumen.VentaMaxima.ToString("N2");

                    lblVentaMinima.Text =
                        "VENTA MÍNIMA\n$ " +
                        resumen.VentaMinima.ToString("N2");

                  
                    // Usa la fecha final seleccionada para determinar el mes actual.
                    DateTime fechaReferencia = dtpHasta.Value;

                    // Calcula el primer día del mes actual.
                    DateTime inicioMesActual = new DateTime(
                        fechaReferencia.Year,
                        fechaReferencia.Month,
                        1);

                    // Calcula el último día del mes actual.
                    DateTime finMesActual = inicioMesActual
                        .AddMonths(1)
                        .AddDays(-1);

                    // Calcula el primer día del mes anterior.
                    DateTime inicioMesAnterior = inicioMesActual.AddMonths(-1);

                    // Calcula el último día del mes anterior.
                    DateTime finMesAnterior = inicioMesActual.AddDays(-1);


                    // Obtiene los indicadores comparativos desde la capa Negocio.
                    ComparacionVentas comparacion = cnReporte.CompararVentas(
                        inicioMesActual,
                        finMesActual,
                        inicioMesAnterior,
                        finMesAnterior,
                        idUsuario);

                    // Muestra la facturación del mes seleccionado.
                    lblFacturacionActual.Text =
                        "FACTURACIÓN DEL MES ACTUAL: $ " +
                        comparacion.FacturacionActual.ToString("N2");

                    // Muestra la facturación del mes anterior.
                    lblFacturacionAnterior.Text =
                        "FACTURACIÓN DEL MES ANTERIOR: $ " +
                        comparacion.FacturacionAnterior.ToString("N2");

                    // Muestra la diferencia monetaria entre ambos meses.
                    lblDiferenciaFacturacion.Text =
                        "DIFERENCIA: $ " +
                        comparacion.DiferenciaFacturacion.ToString("N2");

                    // Muestra la variación porcentual, si se puede calcular.
                    lblVariacionPorcentual.Text =
                        comparacion.VariacionPorcentual.HasValue
                            ? "VARIACIÓN MENSUAL: " +
                              comparacion.VariacionPorcentual.Value.ToString("N2") + "%"
                            : "VARIACIÓN MENSUAL: No calculable (mes anterior sin facturación)";




                    // OBTENER VENTAS PARA LA GRILLA
                    listaReporteVentas =
                        cnReporte.ObtenerVentasAnalisis(
                            dtpDesde.Value,
                            dtpHasta.Value,
                            idUsuario);


                    var datos = listaReporteVentas.Select(v => new
                    {
                        v.IdVenta,
                        Fecha = v.FechaVenta.ToString("dd/MM/yyyy HH:mm"),
                        Total = v.Total.ToString("N2")
                    }).ToList();


                    dgvReporte.AutoGenerateColumns = true;
                    dgvReporte.DataSource = datos;

                    // RESUMEN INFERIOR
                    lblResumen1.Text =
                        "Cantidad de ventas: " +
                        resumen.CantidadVentas;

                    lblResumen2.Text =
                        "TOTAL: $ " +
                        resumen.FacturacionTotal.ToString("N2");

                    lblResumen2.ForeColor = colorRosaOscuro;
                }
                else
                {
                    CargarReporteMovimientosStock();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error al buscar: " + ex.Message,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CargarReporteMovimientosStock()
        {
            try
            {
                string mensaje;
                dtMovimientosStock = cnReporte.ObtenerReporteMovimientosStock(dtpDesde.Value, dtpHasta.Value, out mensaje);

                if (!string.IsNullOrEmpty(mensaje))
                {
                    MessageBox.Show("Aviso de Base de Datos: " + mensaje, "Aura Beauty", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dtMovimientosStock == null || dtMovimientosStock.Rows.Count == 0)
                {
                    dgvReporte.DataSource = null;
                    lblResumen1.Text = "Total de movimientos: 0";
                    lblResumen2.Text = "";
                    MessageBox.Show("No se encontraron movimientos registrados en el rango de fechas seleccionado.", "Aura Beauty", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string filtro = txtBuscarStock.Text.Trim().Replace("'", "''");
                DataView dv = dtMovimientosStock.DefaultView;

                if (!string.IsNullOrEmpty(filtro))
                {
                    List<string> condiciones = new List<string>();
                    foreach (DataColumn col in dtMovimientosStock.Columns)
                    {
                        if (col.DataType == typeof(string))
                        {
                            condiciones.Add($"[{col.ColumnName}] LIKE '%{filtro}%'");
                        }
                    }

                    if (condiciones.Count > 0)
                    {
                        dv.RowFilter = string.Join(" OR ", condiciones);
                    }
                }
                else
                {
                    dv.RowFilter = string.Empty;
                }

                DataTable dtFiltrada = dv.ToTable();
                dgvReporte.AutoGenerateColumns = true;
                dgvReporte.DataSource = dtFiltrada;

                lblResumen1.Text = "Total de movimientos: " + dtFiltrada.Rows.Count;
                lblResumen2.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar movimientos de stock: " + ex.Message, "Aura Beauty", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportarPDF_Click(object sender, EventArgs e)
        {
            if (dgvReporte.Rows.Count < 1)
            {
                MessageBox.Show("No hay datos cargados para exportar.", "Aura Beauty", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                bool esVentas = cboTipoReporte.SelectedIndex == 0;

                PdfDocument document = new PdfDocument();
                document.Info.Title = esVentas ? "Reporte de Ventas" : "Reporte de Movimientos de Stock";

                PdfPage page = document.AddPage();
                page.Size = PdfSharp.PageSize.A4;
                XGraphics gfx = XGraphics.FromPdfPage(page);

                XFont fontTitulo = new XFont("Segoe UI", 18, XFontStyleEx.Bold);
                XFont fontSubtitulo = new XFont("Segoe UI", 10, XFontStyleEx.Regular);
                XFont fontHeaderTabla = new XFont("Segoe UI", 9, XFontStyleEx.Bold);
                XFont fontTexto = new XFont("Segoe UI", 8.5, XFontStyleEx.Regular);
                XFont fontTotal = new XFont("Segoe UI", 11, XFontStyleEx.Bold);

                // Banner Encabezado
                gfx.DrawRectangle(new XSolidBrush(colorRosa), 20, 20, page.Width - 40, 50);
                gfx.DrawString("AURA BEAUTY", fontTitulo, XBrushes.White, new XRect(20, 25, page.Width - 40, 25), XStringFormats.TopCenter);
                gfx.DrawString(esVentas ? "Reporte Oficial de Ventas" : "Reporte de Movimientos de Stock (Ingresos/Egresos)", fontSubtitulo, XBrushes.White, new XRect(20, 48, page.Width - 40, 20), XStringFormats.TopCenter);

                int y = 85;
                gfx.DrawRectangle(new XPen(colorRosa, 0.5), new XSolidBrush(Color.FromArgb(255, 249, 248)), 20, y, page.Width - 40, 35);
                gfx.DrawString($"Rango consultado: {dtpDesde.Value:dd/MM/yyyy} al {dtpHasta.Value:dd/MM/yyyy}", fontTexto, XBrushes.Black, 30, y + 20);
                gfx.DrawString($"Emisión: {DateTime.Now:dd/MM/yyyy HH:mm:ss}", fontTexto, XBrushes.Black, page.Width - 220, y + 20);

                y += 50;
                gfx.DrawRectangle(new XSolidBrush(colorRosaOscuro), 20, y, page.Width - 40, 20);

                if (esVentas)
                {
                    gfx.DrawString("N.º Venta", fontHeaderTabla, XBrushes.White, 25, y + 14);
                    gfx.DrawString("Fecha", fontHeaderTabla, XBrushes.White, 85, y + 14);
                    gfx.DrawString("Comprobante", fontHeaderTabla, XBrushes.White, 185, y + 14);
                    gfx.DrawString("Vendedor", fontHeaderTabla, XBrushes.White, 285, y + 14);
                    gfx.DrawString("Cliente", fontHeaderTabla, XBrushes.White, 400, y + 14);
                    gfx.DrawString("Total", fontHeaderTabla, XBrushes.White, page.Width - 75, y + 14);
                }
                else
                {
                    gfx.DrawString("N.º Solicitud", fontHeaderTabla, XBrushes.White, 25, y + 14);
                    gfx.DrawString("Producto", fontHeaderTabla, XBrushes.White, 90, y + 14);
                    gfx.DrawString("Tipo Mov.", fontHeaderTabla, XBrushes.White, 230, y + 14);
                    gfx.DrawString("Cantidad", fontHeaderTabla, XBrushes.White, 310, y + 14);
                    gfx.DrawString("Motivo", fontHeaderTabla, XBrushes.White, 380, y + 14);
                    gfx.DrawString("Estado", fontHeaderTabla, XBrushes.White, 480, y + 14);
                }

                y += 20;

                foreach (DataGridViewRow row in dgvReporte.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        gfx.DrawLine(new XPen(XColors.LightGray, 0.5), 20, y + 18, page.Width - 20, y + 18);

                        if (esVentas)
                        {
                            gfx.DrawString(ObtenerValorCelda(row, "IdVenta"), fontTexto, XBrushes.Black, 25, y + 13);
                            gfx.DrawString(ObtenerValorCelda(row, "Fecha"), fontTexto, XBrushes.Black, 85, y + 13);
                            gfx.DrawString(ObtenerValorCelda(row, "NroFactura"), fontTexto, XBrushes.Black, 185, y + 13);
                            gfx.DrawString(ObtenerValorCelda(row, "Vendedor"), fontTexto, XBrushes.Black, 285, y + 13);
                            gfx.DrawString(ObtenerValorCelda(row, "Cliente"), fontTexto, XBrushes.Black, 400, y + 13);
                            gfx.DrawString($"$ {ObtenerValorCelda(row, "Total")}", fontTexto, XBrushes.Black, page.Width - 75, y + 13);
                        }
                        else
                        {
                            gfx.DrawString(ObtenerValorCelda(row, "IdSolicitud"), fontTexto, XBrushes.Black, 25, y + 13);
                            gfx.DrawString(ObtenerValorCelda(row, "Producto"), fontTexto, XBrushes.Black, 90, y + 13);

                            string tipoMov = ObtenerValorCelda(row, "TipoMovimiento");
                            XBrush brushTipo = tipoMov.ToUpper().Contains("INGRESO") ? XBrushes.Green : XBrushes.Red;
                            gfx.DrawString(tipoMov, fontTexto, brushTipo, 230, y + 13);

                            gfx.DrawString(ObtenerValorCelda(row, "Cantidad"), fontTexto, XBrushes.Black, 310, y + 13);
                            gfx.DrawString(ObtenerValorCelda(row, "Motivo"), fontTexto, XBrushes.Black, 380, y + 13);
                            gfx.DrawString(ObtenerValorCelda(row, "Estado"), fontTexto, XBrushes.Black, 480, y + 13);
                        }

                        y += 20;

                        if (y > page.Height - 80)
                        {
                            page = document.AddPage();
                            page.Size = PdfSharp.PageSize.A4;
                            gfx = XGraphics.FromPdfPage(page);
                            y = 40;
                        }
                    }
                }

                // Cuadro de Resumen
                y += 15;
                gfx.DrawRectangle(new XPen(colorRosaOscuro, 1), new XSolidBrush(Color.FromArgb(255, 249, 248)), 20, y, page.Width - 40, 30);
                gfx.DrawString(lblResumen1.Text, fontTotal, XBrushes.DarkGray, 30, y + 20);
                gfx.DrawString(lblResumen2.Text, fontTotal, new XSolidBrush(colorRosaOscuro), page.Width - 250, y + 20);

                string tempPdfPath = Path.Combine(Path.GetTempPath(), $"Reporte_{DateTime.Now:ddMMyyyy_HHmmss}.pdf");
                document.Save(tempPdfPath);

                Process.Start(new ProcessStartInfo(tempPdfPath) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar PDF: " + ex.Message, "Aura Beauty", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string ObtenerValorCelda(DataGridViewRow row, string nombreColumna)
        {
            if (row.DataGridView.Columns.Contains(nombreColumna))
            {
                return row.Cells[nombreColumna].Value?.ToString() ?? "";
            }
            return "";
        }

        private void btnLimpiar_Click(object sender, EventArgs e)
        {
            txtBuscarStock.Clear();
            dtpDesde.Value = DateTime.Today;
            dtpHasta.Value = DateTime.Today;
            if (cboVendedor.Items.Count > 0) cboVendedor.SelectedIndex = 0;
            LimpiarResultados();
        }

        private void LimpiarResultados()
        {
            dgvReporte.DataSource = null;
            lblResumen1.Text = "";
            lblResumen2.Text = "";
        }

        private Label CrearEtiqueta(string t, int x, int y) => new Label { Text = t, AutoSize = true, Location = new Point(x, y), ForeColor = colorTexto, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        private Button CrearBoton(string t, int x, int y, int w) => new Button { Text = t, Location = new Point(x, y), Size = new Size(w, 40), BackColor = colorRosa, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand };
        private Button CrearBotonSecundario(string t, int x, int y, int w) => new Button { Text = t, Location = new Point(x, y), Size = new Size(w, 40), BackColor = Color.White, ForeColor = colorRosaOscuro, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 9F, FontStyle.Bold), Cursor = Cursors.Hand };
        private void ConfigurarGrilla(DataGridView g) { g.AllowUserToAddRows = false; g.ReadOnly = true; g.MultiSelect = false; g.SelectionMode = DataGridViewSelectionMode.FullRowSelect; g.BackgroundColor = Color.White; g.RowHeadersVisible = false; g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; }
    

    private Label CrearIndicador(string titulo, int x, int y)
        {
            Label lbl = new Label
            {
                Text = titulo + "\n$ 0,00",
                Location = new Point(x, y),
                Size = new Size(155, 75),
                BackColor = Color.White,
                ForeColor = colorRosaOscuro,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle
            };

            return lbl;
        }
    }
}