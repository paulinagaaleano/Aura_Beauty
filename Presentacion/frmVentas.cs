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
using System;


namespace Aura_Beauty
{
    /// <summary>
    /// Formulario principal del Punto de Venta de Aura Beauty.
    ///
    /// Permite:
    /// - Identificar al usuario que realiza la venta.
    /// - Trabajar con Consumidor Final o con un cliente registrado.
    /// - Buscar productos disponibles.
    /// - Consultar precio y stock.
    /// - Agregar productos al carrito.
    /// - Aumentar o disminuir cantidades.
    /// - Eliminar completamente un producto del carrito.
    /// - Calcular subtotales y total.
    /// - Registrar definitivamente la venta.
    ///
    /// Este formulario pertenece a la capa Presentación.
    /// Las reglas de negocio se controlan en CN_Venta
    /// y el acceso a SQL Server se realiza desde CD_Venta.
    /// </summary>
    public partial class frmVentas : Form
    {
        // =========================================================
        // CAPA DE NEGOCIO
        // =========================================================

        private readonly CN_Producto cnProducto =
            new CN_Producto();

        private readonly CN_Cliente cnCliente =
            new CN_Cliente();

        private readonly CN_Venta cnVenta =
            new CN_Venta();


        // =====================================================
        // RESERVAS Y CONFIRMACIÓN SEGURA
        // =====================================================

        /// <summary>
        /// Permite crear, actualizar y cancelar reservas
        /// mediante la capa Negocio.
        /// </summary>
        private readonly CN_ReservaProducto cnReserva =
            new CN_ReservaProducto();

        /// <summary>
        /// Confirma la venta utilizando las reservas,
        /// sin volver a descontar stock desde C#.
        /// </summary>
        private readonly CN_VentaReservada cnVentaReservada =
            new CN_VentaReservada();

        /// <summary>
        /// Identificador único del carrito actual.
        ///
        /// Se genera al comenzar una venta nueva
        /// y no cambia mientras esa venta esté abierta.
        /// </summary>
        private Guid idOperacionVenta = Guid.Empty;

        /// <summary>
        /// Evita cancelar reservas que ya fueron
        /// confirmadas como venta definitiva.
        /// </summary>
        private bool ventaYaRegistrada = false;


        // =========================================================
        // USUARIO ACTUAL
        // =========================================================

        /// <summary>
        /// Usuario que inició sesión.
        ///
        /// Su IdUsuario se almacena posteriormente
        /// en VentaCabecera para identificar quién realizó la venta.
        /// </summary>
        private readonly Usuario usuarioActual;


        // =========================================================
        // LISTAS EN MEMORIA
        // =========================================================

        private List<Producto> listaProductos =
            new List<Producto>();

        private List<Cliente> listaClientes =
            new List<Cliente>();

        /// <summary>
        /// Contiene temporalmente los productos incluidos
        /// en la venta que todavía no fue confirmada.
        /// </summary>
        private List<VentaDetalle> carrito =
            new List<VentaDetalle>();


        // =========================================================
        // PRODUCTO SELECCIONADO
        // =========================================================

        private Producto productoSeleccionado = null;


        // =========================================================
        // COLORES DE LA APLICACIÓN
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

        private Label lblVendedor;
        private Label lblProductoSeleccionado;
        private Label lblStockDisponible;
        private Label lblPrecioSeleccionado;
        private Label lblTotal;

        private CheckBox chkConsumidorFinal;

        private ComboBox cboCliente;
        private ComboBox cboTipoFactura;

        private TextBox txtBuscarProducto;
        private TextBox txtCantidad;
        private TextBox txtNroFactura;

        private Button btnBuscarProducto;
        private Button btnAgregarCarrito;

        // Estos tres botones administran
        // los productos que ya están en el carrito.
        private Button btnDisminuirCantidad;
        private Button btnAumentarCantidad;
        private Button btnQuitarCarrito;

        private Button btnNuevaVenta;
        private Button btnConfirmarVenta;

        private DataGridView dgvProductos;
        private DataGridView dgvCarrito;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        /// <summary>
        /// Constructor del formulario.
        ///
        /// Recibe obligatoriamente el usuario que inició sesión.
        /// </summary>
        public frmVentas(Usuario usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(
                    nameof(usuario),
                    "Debe existir un usuario autenticado."
                );
            }

            usuarioActual = usuario;

            InitializeComponent();

            ConfigurarFormulario();

            CrearInterfaz();

            CargarClientes();

            CargarProductos();

            PrepararNuevaVenta();


            // Al cerrar el formulario, liberar las reservas
            // que todavía no se hayan convertido en venta.
            this.FormClosing += frmVentas_FormClosing;

        }


        // =========================================================
        // CONFIGURACIÓN DEL FORMULARIO
        // =========================================================

        private void ConfigurarFormulario()
        {
            Text =
                "Aura Beauty - Registro de Ventas";

            StartPosition =
                FormStartPosition.CenterScreen;

            Width = 1280;

            Height = 760;

            BackColor =
                colorFondo;

            Font =
                new Font("Segoe UI", 10F);

            FormBorderStyle =
                FormBorderStyle.FixedSingle;

            MaximizeBox = false;
        }


        // =========================================================
        // CONSTRUCCIÓN VISUAL
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
                115;

            panelEncabezado.BackColor =
                colorRosa;

            Controls.Add(
                panelEncabezado
            );


            Label lblTitulo =
                new Label();

            lblTitulo.Text =
                "REGISTRO DE VENTAS";

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
                new Point(30, 20);

            panelEncabezado.Controls.Add(
                lblTitulo
            );


            lblVendedor =
                new Label();

            lblVendedor.Text =
                "Vendedor: "
                + usuarioActual.Nombre
                + " "
                + usuarioActual.Apellido;

            lblVendedor.Font =
                new Font(
                    "Segoe UI",
                    11F
                );

            lblVendedor.ForeColor =
                Color.White;

            lblVendedor.AutoSize =
                true;

            lblVendedor.Location =
                new Point(33, 72);

            panelEncabezado.Controls.Add(
                lblVendedor
            );


            // =====================================================
            // PANEL DATOS GENERALES DE LA VENTA
            // =====================================================

            Panel panelVenta =
                new Panel();

            panelVenta.Location =
                new Point(20, 135);

            panelVenta.Size =
                new Size(1220, 105);

            panelVenta.BackColor =
                Color.White;

            Controls.Add(
                panelVenta
            );


            panelVenta.Controls.Add(
                CrearEtiqueta(
                    "Cliente",
                    20,
                    15
                )
            );


            chkConsumidorFinal =
                new CheckBox();

            chkConsumidorFinal.Text =
                "Consumidor Final";

            chkConsumidorFinal.Location =
                new Point(20, 45);

            chkConsumidorFinal.AutoSize =
                true;

            chkConsumidorFinal.Checked =
                true;

            chkConsumidorFinal.ForeColor =
                colorTexto;

            chkConsumidorFinal.CheckedChanged +=
                chkConsumidorFinal_CheckedChanged;

            panelVenta.Controls.Add(
                chkConsumidorFinal
            );


            cboCliente =
                new ComboBox();

            cboCliente.Location =
                new Point(165, 42);

            cboCliente.Size =
                new Size(330, 30);

            cboCliente.DropDownStyle =
                ComboBoxStyle.DropDownList;

            panelVenta.Controls.Add(
                cboCliente
            );


            panelVenta.Controls.Add(
                CrearEtiqueta(
                    "Tipo de factura",
                    530,
                    15
                )
            );


            cboTipoFactura =
                new ComboBox();

            cboTipoFactura.Location =
                new Point(530, 42);

            cboTipoFactura.Size =
                new Size(180, 30);

            cboTipoFactura.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cboTipoFactura.Items.Add(
                "Factura C"
            );

            cboTipoFactura.Items.Add(
                "Ticket"
            );

            cboTipoFactura.SelectedIndex =
                0;

            panelVenta.Controls.Add(
                cboTipoFactura
            );


            panelVenta.Controls.Add(
                CrearEtiqueta(
                    "N.º comprobante",
                    745,
                    15
                )
            );


            txtNroFactura =
                CrearCajaTexto(
                    745,
                    42,
                    245
                );

            txtNroFactura.ReadOnly =
                true;

            panelVenta.Controls.Add(
                txtNroFactura
            );


            btnNuevaVenta =
                CrearBotonSecundario(
                    "NUEVA VENTA",
                    1020,
                    39,
                    170
                );

            btnNuevaVenta.Click +=
                btnNuevaVenta_Click;

            panelVenta.Controls.Add(
                btnNuevaVenta
            );


            // =====================================================
            // PANEL DE PRODUCTOS
            // =====================================================

            Panel panelProductos =
                new Panel();

            panelProductos.Location =
                new Point(20, 255);

            panelProductos.Size =
                new Size(580, 420);

            panelProductos.BackColor =
                Color.White;

            Controls.Add(
                panelProductos
            );


            Label lblProductos =
                new Label();

            lblProductos.Text =
                "Productos disponibles";

            lblProductos.Font =
                new Font(
                    "Segoe UI",
                    16F,
                    FontStyle.Bold
                );

            lblProductos.ForeColor =
                colorTexto;

            lblProductos.AutoSize =
                true;

            lblProductos.Location =
                new Point(20, 15);

            panelProductos.Controls.Add(
                lblProductos
            );


            txtBuscarProducto =
                CrearCajaTexto(
                    20,
                    60,
                    385
                );

            panelProductos.Controls.Add(
                txtBuscarProducto
            );


            btnBuscarProducto =
                CrearBoton(
                    "BUSCAR",
                    420,
                    57,
                    135
                );

            btnBuscarProducto.Height =
                35;

            btnBuscarProducto.Click +=
                btnBuscarProducto_Click;

            panelProductos.Controls.Add(
                btnBuscarProducto
            );


            dgvProductos =
                new DataGridView();

            dgvProductos.Location =
                new Point(20, 105);

            dgvProductos.Size =
                new Size(535, 190);

            ConfigurarGrilla(
                dgvProductos
            );

            dgvProductos.CellClick +=
                dgvProductos_CellClick;


            dgvProductos.Columns.Add(
                CrearColumna(
                    "IdProducto",
                    "ID",
                    "IdProducto"
                )
            );

            dgvProductos.Columns.Add(
                CrearColumna(
                    "Nombre",
                    "Producto",
                    "Nombre"
                )
            );

            dgvProductos.Columns.Add(
                CrearColumna(
                    "Precio",
                    "Precio",
                    "Precio"
                )
            );

            dgvProductos.Columns.Add(
                CrearColumna(
                    "Stock",
                    "Stock",
                    "Stock"
                )
            );

            dgvProductos.Columns.Add(
                CrearColumna(
                    "Categoria",
                    "Categoría",
                    "Categoria"
                )
            );

            panelProductos.Controls.Add(
                dgvProductos
            );


            panelProductos.Controls.Add(
                CrearEtiqueta(
                    "Producto seleccionado:",
                    20,
                    310
                )
            );


            lblProductoSeleccionado =
                new Label();

            lblProductoSeleccionado.Text =
                "Ninguno";

            lblProductoSeleccionado.Location =
                new Point(185, 310);

            lblProductoSeleccionado.Size =
                new Size(350, 25);

            lblProductoSeleccionado.ForeColor =
                colorRosaOscuro;

            lblProductoSeleccionado.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            panelProductos.Controls.Add(
                lblProductoSeleccionado
            );


            lblStockDisponible =
                new Label();

            lblStockDisponible.Text =
                "Stock: -";

            lblStockDisponible.Location =
                new Point(20, 345);

            lblStockDisponible.AutoSize =
                true;

            lblStockDisponible.ForeColor =
                colorTexto;

            panelProductos.Controls.Add(
                lblStockDisponible
            );


            lblPrecioSeleccionado =
                new Label();

            lblPrecioSeleccionado.Text =
                "Precio: -";

            lblPrecioSeleccionado.Location =
                new Point(125, 345);

            lblPrecioSeleccionado.AutoSize =
                true;

            lblPrecioSeleccionado.ForeColor =
                colorTexto;

            panelProductos.Controls.Add(
                lblPrecioSeleccionado
            );


            panelProductos.Controls.Add(
                CrearEtiqueta(
                    "Cantidad",
                    285,
                    345
                )
            );


            txtCantidad =
                CrearCajaTexto(
                    355,
                    341,
                    65
                );

            panelProductos.Controls.Add(
                txtCantidad
            );


            btnAgregarCarrito =
                CrearBoton(
                    "AGREGAR",
                    435,
                    338,
                    120
                );

            btnAgregarCarrito.Height =
                35;

            btnAgregarCarrito.Click +=
                btnAgregarCarrito_Click;

            panelProductos.Controls.Add(
                btnAgregarCarrito
            );


            // =====================================================
            // PANEL CARRITO
            // =====================================================

            Panel panelCarrito =
                new Panel();

            panelCarrito.Location =
                new Point(620, 255);

            panelCarrito.Size =
                new Size(620, 420);

            panelCarrito.BackColor =
                Color.White;

            Controls.Add(
                panelCarrito
            );


            Label lblCarrito =
                new Label();

            lblCarrito.Text =
                "Carrito de venta";

            lblCarrito.Font =
                new Font(
                    "Segoe UI",
                    16F,
                    FontStyle.Bold
                );

            lblCarrito.ForeColor =
                colorTexto;

            lblCarrito.AutoSize =
                true;

            lblCarrito.Location =
                new Point(20, 15);

            panelCarrito.Controls.Add(
                lblCarrito
            );


            dgvCarrito =
                new DataGridView();

            dgvCarrito.Location =
                new Point(20, 60);

            dgvCarrito.Size =
                new Size(580, 235);

            ConfigurarGrilla(
                dgvCarrito
            );


            dgvCarrito.Columns.Add(
                CrearColumna(
                    "IdProducto",
                    "ID",
                    "IdProducto"
                )
            );

            dgvCarrito.Columns.Add(
                CrearColumna(
                    "Producto",
                    "Producto",
                    "Producto"
                )
            );

            dgvCarrito.Columns.Add(
                CrearColumna(
                    "Cantidad",
                    "Cant.",
                    "Cantidad"
                )
            );

            dgvCarrito.Columns.Add(
                CrearColumna(
                    "Precio",
                    "Precio unit.",
                    "Precio"
                )
            );

            dgvCarrito.Columns.Add(
                CrearColumna(
                    "Subtotal",
                    "Subtotal",
                    "Subtotal"
                )
            );

            panelCarrito.Controls.Add(
                dgvCarrito
            );


            // =====================================================
            // BOTÓN DISMINUIR CANTIDAD
            // =====================================================

            btnDisminuirCantidad =
                CrearBotonSecundario(
                    "− 1",
                    20,
                    310,
                    75
                );

            btnDisminuirCantidad.Height =
                38;

            btnDisminuirCantidad.Font =
                new Font(
                    "Segoe UI",
                    11F,
                    FontStyle.Bold
                );

            btnDisminuirCantidad.Click +=
                btnDisminuirCantidad_Click;

            panelCarrito.Controls.Add(
                btnDisminuirCantidad
            );


            // =====================================================
            // BOTÓN AUMENTAR CANTIDAD
            // =====================================================

            btnAumentarCantidad =
                CrearBoton(
                    "+ 1",
                    105,
                    310,
                    75
                );

            btnAumentarCantidad.Height =
                38;

            btnAumentarCantidad.Font =
                new Font(
                    "Segoe UI",
                    11F,
                    FontStyle.Bold
                );

            btnAumentarCantidad.Click +=
                btnAumentarCantidad_Click;

            panelCarrito.Controls.Add(
                btnAumentarCantidad
            );


            // =====================================================
            // QUITAR TODA LA LÍNEA
            // =====================================================

            btnQuitarCarrito =
                CrearBotonSecundario(
                    "QUITAR PRODUCTO",
                    190,
                    310,
                    175
                );

            btnQuitarCarrito.Height =
                38;

            btnQuitarCarrito.Click +=
                btnQuitarCarrito_Click;

            panelCarrito.Controls.Add(
                btnQuitarCarrito
            );


            // =====================================================
            // TOTAL
            // =====================================================

            Label lblTotalTitulo =
                new Label();

            lblTotalTitulo.Text =
                "TOTAL:";

            lblTotalTitulo.Location =
                new Point(390, 315);

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

            panelCarrito.Controls.Add(
                lblTotalTitulo
            );


            lblTotal =
                new Label();

            lblTotal.Text =
                "$ 0,00";

            lblTotal.Location =
                new Point(465, 307);

            lblTotal.Size =
                new Size(135, 38);

            lblTotal.TextAlign =
                ContentAlignment.MiddleRight;

            lblTotal.Font =
                new Font(
                    "Segoe UI",
                    16F,
                    FontStyle.Bold
                );

            lblTotal.ForeColor =
                colorRosaOscuro;

            panelCarrito.Controls.Add(
                lblTotal
            );


            // =====================================================
            // CONFIRMAR VENTA
            // =====================================================

            btnConfirmarVenta =
                CrearBoton(
                    "CONFIRMAR VENTA",
                    320,
                    360,
                    280
                );

            btnConfirmarVenta.Height =
                45;

            btnConfirmarVenta.Click +=
                btnConfirmarVenta_Click;

            panelCarrito.Controls.Add(
                btnConfirmarVenta
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
                new Point(x, y);

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


        private TextBox CrearCajaTexto(
            int x,
            int y,
            int ancho
        )
        {
            TextBox caja =
                new TextBox();

            caja.Location =
                new Point(x, y);

            caja.Width =
                ancho;

            caja.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            return caja;
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
                new Point(x, y);

            boton.Size =
                new Size(ancho, 40);

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
                new Point(x, y);

            boton.Size =
                new Size(ancho, 40);

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
        // CLIENTES
        // =========================================================

        private void CargarClientes()
        {
            try
            {
                listaClientes =
                    cnCliente.Listar();


                cboCliente.DataSource =
                    null;

                cboCliente.DataSource =
                    listaClientes;

                cboCliente.ValueMember =
                    "IdCliente";

                cboCliente.DisplayMember =
                    "Apellido";


                cboCliente.Format +=
                    cboCliente_Format;


                cboCliente.Enabled =
                    false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No fue posible cargar los clientes.\n\n"
                    + ex.Message,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        private void cboCliente_Format(
            object sender,
            ListControlConvertEventArgs e
        )
        {
            Cliente cliente =
                e.ListItem as Cliente;


            if (cliente != null)
            {
                e.Value =
                    cliente.Apellido
                    + ", "
                    + cliente.Nombre
                    + " - DNI "
                    + cliente.Dni;
            }
        }


        private void chkConsumidorFinal_CheckedChanged(
            object sender,
            EventArgs e
        )
        {
            cboCliente.Enabled =
                !chkConsumidorFinal.Checked;
        }


        // =========================================================
        // PRODUCTOS
        // =========================================================

        private void CargarProductos()
        {
            try
            {
                listaProductos =
                    cnProducto.Listar();

                MostrarProductos(
                    listaProductos
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No fue posible cargar los productos.\n\n"
                    + ex.Message,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        private void MostrarProductos(
            List<Producto> productos
        )
        {
            var datos =
                productos.Select(
                    p => new
                    {
                        p.IdProducto,

                        p.Nombre,

                        Precio =
                            p.Precio.ToString("N2"),

                        p.Stock,

                        Categoria =
                            p.oCategoria != null
                                ? p.oCategoria.Nombre
                                : ""
                    }
                )
                .ToList();


            dgvProductos.DataSource =
                null;

            dgvProductos.DataSource =
                datos;
        }


        private void btnBuscarProducto_Click(
            object sender,
            EventArgs e
        )
        {
            string texto =
                txtBuscarProducto.Text.Trim();


            if (
                string.IsNullOrWhiteSpace(
                    texto
                )
            )
            {
                MostrarProductos(
                    listaProductos
                );

                return;
            }


            List<Producto> resultado =
                listaProductos
                    .Where(
                        p =>
                            !string.IsNullOrEmpty(
                                p.Nombre
                            )
                            &&
                            p.Nombre.IndexOf(
                                texto,
                                StringComparison.OrdinalIgnoreCase
                            ) >= 0
                    )
                    .ToList();


            MostrarProductos(
                resultado
            );
        }


        private void dgvProductos_CellClick(
            object sender,
            DataGridViewCellEventArgs e
        )
        {
            if (e.RowIndex < 0)
            {
                return;
            }


            object valorId =
                dgvProductos.Rows[e.RowIndex]
                    .Cells["IdProducto"]
                    .Value;


            if (valorId == null)
            {
                return;
            }


            int idProducto =
                Convert.ToInt32(
                    valorId
                );


            productoSeleccionado =
                listaProductos.FirstOrDefault(
                    p =>
                        p.IdProducto ==
                        idProducto
                );


            if (
                productoSeleccionado == null
            )
            {
                return;
            }


            lblProductoSeleccionado.Text =
                productoSeleccionado.Nombre;


            lblStockDisponible.Text =
                "Stock: "
                + productoSeleccionado.Stock;


            lblPrecioSeleccionado.Text =
                "Precio: $ "
                + productoSeleccionado.Precio.ToString(
                    "N2"
                );


            txtCantidad.Text =
                "1";

            txtCantidad.Focus();

            txtCantidad.SelectAll();
        }


        // =========================================================
        // AGREGAR PRODUCTO AL CARRITO
        // =========================================================


        /// <summary>
        /// Agrega unidades de un producto al carrito.
        ///
        /// Primero realiza la reserva en SQL Server.
        /// Solamente actualiza el carrito si la reserva
        /// fue aceptada.
        ///
        /// No descuenta stock físico.
        /// </summary>
        private void btnAgregarCarrito_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (productoSeleccionado == null)
                {
                    throw new Exception(
                        "Seleccioná primero un producto.");
                }

                int cantidad;

                if (!int.TryParse(
                    txtCantidad.Text,
                    out cantidad) || cantidad <= 0)
                {
                    throw new Exception(
                        "Ingresá una cantidad válida.");
                }

                VentaDetalle existente =
                    carrito.FirstOrDefault(d =>
                        d.IdProducto ==
                        productoSeleccionado.IdProducto);

                int cantidadActual =
                    existente == null
                        ? 0
                        : existente.Cantidad;

                int cantidadNueva = checked(
                    cantidadActual + cantidad);

                // IMPORTANTE:
                // La base verifica stock menos las
                // reservas vigentes de otros vendedores.
                // No confiamos en el stock de la grilla.
                ResultadoReservaProducto resultado =
                    cnReserva.Reservar(
                        idOperacionVenta,
                        productoSeleccionado.IdProducto,
                        usuarioActual.IdUsuario,
                        cantidadNueva);

                // Llegamos aquí solamente si SQL Server
                // aceptó la cantidad total solicitada.
                if (existente != null)
                {
                    existente.Cantidad = cantidadNueva;
                    RecalcularDetalle(existente);
                }
                else
                {
                    carrito.Add(new VentaDetalle
                    {
                        IdProducto =
                            productoSeleccionado.IdProducto,

                        Cantidad = cantidadNueva,

                        PrecioUnitario =
                            productoSeleccionado.Precio,

                        Subtotal =
                            cantidadNueva *
                            productoSeleccionado.Precio,

                        oProducto = productoSeleccionado
                    });
                }

                ActualizarCarrito();
                LimpiarSeleccionProducto();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "No se pudo agregar el producto",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }



        // =========================================================
        // OBTENER PRODUCTO SELECCIONADO DEL CARRITO
        // =========================================================

        /// <summary>
        /// Busca dentro de la lista carrito el objeto VentaDetalle
        /// correspondiente a la fila actualmente seleccionada.
        ///
        /// Este método evita repetir la misma lógica en los botones
        /// +1, -1 y Quitar producto.
        /// </summary>
        private VentaDetalle ObtenerDetalleSeleccionadoCarrito()
        {
            if (
                dgvCarrito.CurrentRow == null
            )
            {
                throw new Exception(
                    "Seleccioná primero un producto del carrito."
                );
            }


            object valorId =
                dgvCarrito.CurrentRow
                    .Cells["IdProducto"]
                    .Value;


            if (valorId == null)
            {
                throw new Exception(
                    "No fue posible identificar el producto seleccionado."
                );
            }


            int idProducto =
                Convert.ToInt32(
                    valorId
                );


            VentaDetalle detalle =
                carrito.FirstOrDefault(
                    d =>
                        d.IdProducto ==
                        idProducto
                );


            if (detalle == null)
            {
                throw new Exception(
                    "No fue posible encontrar el producto en el carrito."
                );
            }


            return detalle;
        }


        // =========================================================
        // AUMENTAR UNA UNIDAD
        // =========================================================


        /// <summary>
        /// Aumenta una unidad en el producto elegido.
        ///
        /// SQL Server debe aceptar la nueva cantidad
        /// antes de modificar el carrito local.
        /// </summary>
        private void btnAumentarCantidad_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                VentaDetalle detalle =
                    ObtenerDetalleSeleccionadoCarrito();

                int nuevaCantidad = checked(
                    detalle.Cantidad + 1);

                cnReserva.Reservar(
                    idOperacionVenta,
                    detalle.IdProducto,
                    usuarioActual.IdUsuario,
                    nuevaCantidad);

                // La reserva fue aceptada.
                detalle.Cantidad = nuevaCantidad;

                RecalcularDetalle(detalle);
                ActualizarCarrito();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "No se pudo aumentar la cantidad",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }



        // =========================================================
        // DISMINUIR UNA UNIDAD
        // =========================================================


        /// <summary>
        /// Reduce en una unidad la reserva y el carrito.
        ///
        /// La cantidad mínima de una línea es una unidad.
        /// Para eliminarla se utiliza Quitar producto.
        /// </summary>
        private void btnDisminuirCantidad_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                VentaDetalle detalle =
                    ObtenerDetalleSeleccionadoCarrito();

                if (detalle.Cantidad <= 1)
                {
                    MessageBox.Show(
                        "Para quitar la última unidad, " +
                        "utilizá el botón Quitar producto.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                int nuevaCantidad = detalle.Cantidad - 1;

                // Actualizamos la reserva primero.
                cnReserva.Reservar(
                    idOperacionVenta,
                    detalle.IdProducto,
                    usuarioActual.IdUsuario,
                    nuevaCantidad);

                detalle.Cantidad = nuevaCantidad;

                RecalcularDetalle(detalle);
                ActualizarCarrito();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "No se pudo disminuir la cantidad",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }



        // =========================================================
        // RECALCULAR UN DETALLE
        // =========================================================

        /// <summary>
        /// Actualiza el subtotal de un producto después de
        /// modificar su cantidad.
        ///
        /// Fórmula:
        /// cantidad × precio unitario = subtotal.
        /// </summary>
        private void RecalcularDetalle(
            VentaDetalle detalle
        )
        {
            detalle.Subtotal =
                detalle.Cantidad
                *
                detalle.PrecioUnitario;
        }


        // =========================================================
        // ACTUALIZAR CARRITO EN PANTALLA
        // =========================================================

        private void ActualizarCarrito()
        {
            var datos =
                carrito.Select(
                    d => new
                    {
                        d.IdProducto,

                        Producto =
                            d.oProducto != null
                                ? d.oProducto.Nombre
                                : "",

                        d.Cantidad,

                        Precio =
                            d.PrecioUnitario.ToString(
                                "N2"
                            ),

                        Subtotal =
                            d.Subtotal.ToString(
                                "N2"
                            )
                    }
                )
                .ToList();


            dgvCarrito.DataSource =
                null;

            dgvCarrito.DataSource =
                datos;


            decimal total =
                carrito.Sum(
                    d => d.Subtotal
                );


            lblTotal.Text =
                "$ "
                + total.ToString("N2");
        }


        // =========================================================
        // QUITAR COMPLETAMENTE UN PRODUCTO
        // =========================================================


        /// <summary>
        /// Quita un producto del carrito.
        ///
        /// Cancela su reserva en SQL Server y después
        /// lo elimina de la lista en memoria.
        ///
        /// No suma unidades al stock físico porque
        /// todavía no fueron descontadas.
        /// </summary>
        private void btnQuitarCarrito_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                VentaDetalle detalle =
                    ObtenerDetalleSeleccionadoCarrito();

                string nombre = detalle.oProducto != null
                    ? detalle.oProducto.Nombre
                    : "este producto";

                DialogResult respuesta =
                    MessageBox.Show(
                        "¿Querés quitar " + nombre +
                        " del carrito y liberar sus unidades?",
                        "Aura Beauty",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                if (respuesta != DialogResult.Yes)
                    return;

                // Si SQL no confirma la cancelación,
                // el producto permanece en el carrito.
                cnReserva.CancelarProducto(
                    idOperacionVenta,
                    usuarioActual.IdUsuario,
                    detalle.IdProducto);

                carrito.Remove(detalle);
                ActualizarCarrito();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "No se pudo quitar el producto",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }



        // =========================================================
        // CONFIRMAR VENTA
        // =========================================================


        /// <summary>
        /// Confirma una venta respaldada por reservas.
        ///
        /// La operación definitiva se realiza en SQL:
        /// - Verificación de reservas vigentes.
        /// - Registro de cabecera y detalles.
        /// - Descuento de stock.
        /// - Confirmación de reservas.
        ///
        /// No llama al método anterior Registrar().
        /// </summary>
        private void btnConfirmarVenta_Click(
            object sender,
            EventArgs e)
        {
            bool confirmacionRegistrada = false;

            try
            {
                // =========================================
                // 1. CONSTRUIR LA VENTA
                // =========================================

                VentaCabecera venta = ConstruirVenta();

                Cliente clienteComprobante = null;

                if (!chkConsumidorFinal.Checked)
                {
                    clienteComprobante =
                        cboCliente.SelectedItem as Cliente;
                }

                // =========================================
                // 2. PEDIR CONFIRMACIÓN
                // =========================================

                DialogResult respuesta = MessageBox.Show(
                    "¿Confirmar la venta por " +
                    lblTotal.Text + "?",
                    "Aura Beauty",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (respuesta != DialogResult.Yes)
                    return;

                // Evitar dos clics simultáneos en el botón.
                btnConfirmarVenta.Enabled = false;

                // =========================================
                // 3. CONFIRMAR CON RESERVAS
                // =========================================

                ResultadoVentaReservada resultado =
                    cnVentaReservada.Confirmar(
                        idOperacionVenta,
                        venta);

                /*
                El procedimiento SQL ya realizó toda
                la operación dentro de una transacción.

                No debemos descontar stock otra vez.
                */
                confirmacionRegistrada = true;
                ventaYaRegistrada = true;

                venta.IdVentaCabecera =
                    resultado.IdVentaCabecera;

                // La fecha se utiliza para el PDF.
                venta.FechaVenta = DateTime.Now;

                // =========================================
                // 4. GENERAR COMPROBANTE
                // =========================================

                string rutaPdf = null;

                try
                {
                    rutaPdf = GeneradorComprobantePdf.Generar(
                        venta,
                        usuarioActual,
                        clienteComprobante);
                }
                catch (Exception errorPdf)
                {
                    /*
                    La venta ya está confirmada.

                    Un error en el PDF no debe provocar
                    que se registre otra venta.
                    */
                    MessageBox.Show(
                        "La venta se registró correctamente, " +
                        "pero no se pudo generar el PDF.\n\n" +
                        errorPdf.Message,
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                // =========================================
                // 5. INFORMAR RESULTADO
                // =========================================

                string mensaje = resultado.YaRegistrada
                    ? "Esta operación ya estaba registrada."
                    : "Venta registrada correctamente.";

                mensaje +=
                    "\n\nVenta N.º " +
                    resultado.IdVentaCabecera +
                    "\nComprobante: " +
                    venta.NroFactura;

                MessageBox.Show(
                    mensaje,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // =========================================
                // 6. ABRIR PDF SI EXISTE
                // =========================================

                if (!string.IsNullOrWhiteSpace(rutaPdf))
                {
                    try
                    {
                        GeneradorComprobantePdf.Abrir(
                            rutaPdf);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            "El PDF fue generado, pero no " +
                            "se pudo abrir automáticamente.\n\n" +
                            rutaPdf + "\n\n" + ex.Message,
                            "Aura Beauty",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }

                // =========================================
                // 7. NUEVA OPERACIÓN
                // =========================================

                try
                {
                    CargarProductos();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "La venta ya está registrada, " +
                        "pero no se pudo actualizar el " +
                        "listado de productos.\n\n" +
                        ex.Message,
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                PrepararNuevaVenta();
            }
            catch (Exception ex)
            {
                /*
                Si la confirmación falló antes de ser
                registrada, conservamos el carrito
                y su identificador para poder reintentar.

                La base de datos controla que el
                mismo IdOperacion no genere dos ventas.
                */
                string titulo = confirmacionRegistrada
                    ? "Venta registrada"
                    : "No se pudo confirmar la venta";

                MessageBox.Show(
                    ex.Message,
                    titulo,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            finally
            {
                if (btnConfirmarVenta != null &&
                    !btnConfirmarVenta.IsDisposed)
                {
                    btnConfirmarVenta.Enabled = true;
                }
            }
        }




        // =========================================================
        // CONSTRUIR OBJETO VENTA
        // =========================================================

        /// <summary>
        /// Construye una VentaCabecera con la información
        /// seleccionada en la pantalla.
        ///
        /// Después CN_Venta volverá a validar los datos
        /// y recalcular los importes.
        /// </summary>
        private VentaCabecera ConstruirVenta()
        {
            int? idCliente =
                null;


            if (
                !chkConsumidorFinal.Checked
            )
            {
                Cliente cliente =
                    cboCliente.SelectedItem
                    as Cliente;


                if (cliente == null)
                {
                    throw new Exception(
                        "Seleccioná un cliente."
                    );
                }


                idCliente =
                    cliente.IdCliente;
            }


            if (
                cboTipoFactura.SelectedItem == null
            )
            {
                throw new Exception(
                    "Seleccioná el tipo de factura."
                );
            }


            VentaCabecera venta =
                new VentaCabecera
                {
                    IdCliente =
                        idCliente,

                    IdUsuario =
                        usuarioActual.IdUsuario,

                    TipoFactura =
                        cboTipoFactura.SelectedItem.ToString(),

                    NroFactura =
                        txtNroFactura.Text,

                    Detalles =
                        carrito.Select(
                            d =>
                                new VentaDetalle
                                {
                                    IdProducto =
                                        d.IdProducto,

                                    Cantidad =
                                        d.Cantidad,

                                    PrecioUnitario =
                                        d.PrecioUnitario,

                                    Subtotal =
                                        d.Subtotal,

                                    oProducto =
                                        d.oProducto
                                }
                        )
                        .ToList()
                };


            return venta;
        }


        // =========================================================
        // NUEVA VENTA
        // =========================================================

        
        /// <summary>
        /// Descarta la venta en preparación.
        ///
        /// Antes de limpiar la pantalla cancela
        /// las reservas activas de la operación.
        /// </summary>
        private void btnNuevaVenta_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                if (carrito.Count > 0)
                {
                    DialogResult respuesta =
                        MessageBox.Show(
                            "La venta actual tiene productos.\n\n" +
                            "¿Deseás cancelarla y liberar " +
                            "las unidades reservadas?",
                            "Aura Beauty",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                    if (respuesta != DialogResult.Yes)
                        return;
                }

                if (idOperacionVenta != Guid.Empty &&
                    !ventaYaRegistrada)
                {
                    cnReserva.CancelarOperacion(
                        idOperacionVenta,
                        usuarioActual.IdUsuario);
                }

                PrepararNuevaVenta();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "No se pudo comenzar una nueva venta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }



        /// <summary>
        /// Prepara un carrito completamente nuevo.
        ///
        /// IMPORTANTE:
        /// Este método solo debe utilizarse:
        /// - Al abrir inicialmente el Punto de Venta.
        /// - Después de cancelar correctamente el carrito.
        /// - Después de confirmar definitivamente la venta.
        ///
        /// No debe utilizarse para abandonar reservas
        /// activas sin cancelarlas primero.
        /// </summary>
        private void PrepararNuevaVenta()
        {
            // Un nuevo carrito necesita un nuevo Guid.
            idOperacionVenta =
                cnReserva.CrearNuevaOperacion();

            ventaYaRegistrada = false;

            carrito = new List<VentaDetalle>();

            ActualizarCarrito();

            chkConsumidorFinal.Checked = true;

            cboCliente.Enabled = false;

            if (cboCliente.Items.Count > 0)
            {
                cboCliente.SelectedIndex = 0;
            }

            if (cboTipoFactura.Items.Count > 0)
            {
                cboTipoFactura.SelectedIndex = 0;
            }

            txtNroFactura.Text =
                GenerarNumeroComprobante();

            txtBuscarProducto.Clear();

            LimpiarSeleccionProducto();

            MostrarProductos(listaProductos);
        }



        // =========================================================
        // NÚMERO INTERNO DE COMPROBANTE
        // =========================================================

        /// <summary>
        /// Genera un identificador interno utilizando
        /// fecha y hora.
        ///
        /// No representa numeración fiscal oficial.
        /// </summary>
        private string GenerarNumeroComprobante()
        {
            return
                "V-"
                +
                DateTime.Now.ToString(
                    "yyyyMMddHHmmssfff"
                );
        }


        // =========================================================
        // LIMPIAR SELECCIÓN DE PRODUCTO
        // =========================================================

        private void LimpiarSeleccionProducto()
        {
            productoSeleccionado =
                null;


            lblProductoSeleccionado.Text =
                "Ninguno";


            lblStockDisponible.Text =
                "Stock: -";


            lblPrecioSeleccionado.Text =
                "Precio: -";


            txtCantidad.Clear();


            dgvProductos.ClearSelection();
        }


        /// <summary>
        /// Se ejecuta al cerrar el Punto de Venta.
        ///
        /// Libera las unidades reservadas que todavía
        /// no se hayan convertido en una venta definitiva.
        ///
        /// Si no hay conexión a SQL Server, las reservas
        /// todavía podrán liberarse por vencimiento.
        /// </summary>
        private void frmVentas_FormClosing(
            object sender,
            FormClosingEventArgs e)
        {
            if (ventaYaRegistrada ||
                idOperacionVenta == Guid.Empty ||
                carrito.Count == 0)
            {
                return;
            }

            try
            {
                cnReserva.CancelarOperacion(
                    idOperacionVenta,
                    usuarioActual.IdUsuario);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudieron liberar las reservas " +
                    "al cerrar el formulario.\n\n" +
                    "Las unidades dejarán de estar " +
                    "reservadas al vencer su plazo.\n\n" +
                    ex.Message,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

    }
}