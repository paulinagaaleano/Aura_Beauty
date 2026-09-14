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

        private void btnAgregarCarrito_Click(
            object sender,
            EventArgs e
        )
        {
            try
            {
                if (
                    productoSeleccionado == null
                )
                {
                    throw new Exception(
                        "Seleccioná primero un producto."
                    );
                }


                if (
                    !int.TryParse(
                        txtCantidad.Text,
                        out int cantidad
                    )
                )
                {
                    throw new Exception(
                        "Ingresá una cantidad válida."
                    );
                }


                if (cantidad <= 0)
                {
                    throw new Exception(
                        "La cantidad debe ser mayor que cero."
                    );
                }


                VentaDetalle detalleExistente =
                    carrito.FirstOrDefault(
                        d =>
                            d.IdProducto ==
                            productoSeleccionado.IdProducto
                    );


                int cantidadYaAgregada =
                    detalleExistente != null
                        ? detalleExistente.Cantidad
                        : 0;


                int cantidadTotal =
                    cantidadYaAgregada
                    +
                    cantidad;


                if (
                    cantidadTotal >
                    productoSeleccionado.Stock
                )
                {
                    throw new Exception(
                        "No hay stock suficiente.\n\n"
                        + "Stock disponible: "
                        + productoSeleccionado.Stock
                    );
                }


                // Si el producto ya existe en el carrito,
                // no generamos una segunda línea.
                //
                // Aumentamos la cantidad de la línea existente.
                if (
                    detalleExistente != null
                )
                {
                    detalleExistente.Cantidad =
                        cantidadTotal;


                    RecalcularDetalle(
                        detalleExistente
                    );
                }
                else
                {
                    VentaDetalle nuevoDetalle =
                        new VentaDetalle
                        {
                            IdProducto =
                                productoSeleccionado.IdProducto,

                            Cantidad =
                                cantidad,

                            PrecioUnitario =
                                productoSeleccionado.Precio,

                            Subtotal =
                                cantidad
                                *
                                productoSeleccionado.Precio,

                            oProducto =
                                productoSeleccionado
                        };


                    carrito.Add(
                        nuevoDetalle
                    );
                }


                ActualizarCarrito();

                LimpiarSeleccionProducto();
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
        /// Incrementa en una unidad la cantidad del producto
        /// seleccionado en el carrito.
        ///
        /// Antes de hacerlo comprueba que exista stock disponible.
        /// </summary>
        private void btnAumentarCantidad_Click(
            object sender,
            EventArgs e
        )
        {
            try
            {
                VentaDetalle detalle =
                    ObtenerDetalleSeleccionadoCarrito();


                Producto producto =
                    listaProductos.FirstOrDefault(
                        p =>
                            p.IdProducto ==
                            detalle.IdProducto
                    );


                if (producto == null)
                {
                    throw new Exception(
                        "No fue posible encontrar el producto."
                    );
                }


                // No permitimos que la cantidad solicitada
                // supere el stock disponible.
                if (
                    detalle.Cantidad + 1 >
                    producto.Stock
                )
                {
                    throw new Exception(
                        "No hay más stock disponible "
                        + "para este producto.\n\n"
                        + "Stock disponible: "
                        + producto.Stock
                    );
                }


                // ++ significa incrementar en una unidad.
                detalle.Cantidad++;


                RecalcularDetalle(
                    detalle
                );


                ActualizarCarrito();
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
        // DISMINUIR UNA UNIDAD
        // =========================================================

        /// <summary>
        /// Disminuye en una unidad la cantidad del producto
        /// seleccionado.
        ///
        /// Nunca baja de una unidad.
        /// Para eliminar completamente el producto existe
        /// el botón "Quitar producto".
        /// </summary>
        private void btnDisminuirCantidad_Click(
            object sender,
            EventArgs e
        )
        {
            try
            {
                VentaDetalle detalle =
                    ObtenerDetalleSeleccionadoCarrito();


                if (
                    detalle.Cantidad <= 1
                )
                {
                    MessageBox.Show(
                        "La cantidad mínima es 1.\n\n"
                        + "Si querés eliminar completamente "
                        + "el producto, utilizá "
                        + "\"Quitar producto\".",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    return;
                }


                // -- significa disminuir en una unidad.
                detalle.Cantidad--;


                RecalcularDetalle(
                    detalle
                );


                ActualizarCarrito();
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

        private void btnQuitarCarrito_Click(
            object sender,
            EventArgs e
        )
        {
            try
            {
                VentaDetalle detalle =
                    ObtenerDetalleSeleccionadoCarrito();


                string nombreProducto =
                    detalle.oProducto != null
                        ? detalle.oProducto.Nombre
                        : "el producto";


                DialogResult respuesta =
                    MessageBox.Show(
                        "¿Querés quitar completamente "
                        + nombreProducto
                        + " del carrito?",
                        "Aura Beauty",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );


                if (
                    respuesta !=
                    DialogResult.Yes
                )
                {
                    return;
                }


                carrito.Remove(
                    detalle
                );


                ActualizarCarrito();
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
        // CONFIRMAR VENTA
        // =========================================================

        private void btnConfirmarVenta_Click(
    object sender,
    EventArgs e
)
        {
            try
            {
                // =====================================================
                // 1. CONSTRUIR LA VENTA
                // =====================================================

                VentaCabecera venta =
                    ConstruirVenta();


                // =====================================================
                // 2. IDENTIFICAR CLIENTE PARA EL COMPROBANTE
                // =====================================================

                // Si Consumidor Final está marcado,
                // clienteComprobante permanecerá en null.
                //
                // El generador de PDF interpreta null
                // como "Consumidor Final".
                Cliente clienteComprobante =
                    null;


                if (
                    !chkConsumidorFinal.Checked
                )
                {
                    clienteComprobante =
                        cboCliente.SelectedItem
                        as Cliente;
                }


                // =====================================================
                // 3. PEDIR CONFIRMACIÓN
                // =====================================================

                DialogResult confirmacion =
                    MessageBox.Show(
                        "¿Confirmar la venta por "
                        + lblTotal.Text
                        + "?",
                        "Aura Beauty",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );


                if (
                    confirmacion !=
                    DialogResult.Yes
                )
                {
                    return;
                }


                // =====================================================
                // 4. REGISTRAR LA VENTA
                // =====================================================

                // CN_Venta valida la información.
                //
                // CD_Venta realiza la transacción:
                // - guarda VentaCabecera;
                // - guarda VentaDetalle;
                // - descuenta stock.
                //
                // Registrar devuelve el ID generado
                // por SQL Server.
                int idVenta =
                    cnVenta.Registrar(
                        venta
                    );


                // =====================================================
                // 5. COMPLETAR DATOS DEL OBJETO PARA EL PDF
                // =====================================================

                // El ID fue generado por la base de datos.
                // Ahora lo asignamos al objeto que utilizaremos
                // para generar el comprobante.
                venta.IdVentaCabecera =
                    idVenta;


                // CD_Venta utiliza GETDATE() en SQL Server.
                //
                // Para mostrar la fecha en el PDF asignamos
                // la fecha actual al objeto en memoria.
                venta.FechaVenta =
                    DateTime.Now;


                // =====================================================
                // 6. GENERAR PDF
                // =====================================================

                string rutaPdf =
                    null;


                try
                {
                    rutaPdf =
                        GeneradorComprobantePdf.Generar(
                            venta,
                            usuarioActual,
                            clienteComprobante
                        );
                }
                catch (Exception exPdf)
                {
                    // MUY IMPORTANTE:
                    //
                    // Si falla el PDF, la venta NO debe considerarse
                    // fallida porque ya fue registrada correctamente
                    // en la base de datos.
                    //
                    // Por eso el error del PDF se controla
                    // independientemente.
                    MessageBox.Show(
                        "La venta fue registrada correctamente, "
                        + "pero no fue posible generar el comprobante PDF."
                        + "\n\n"
                        + exPdf.Message,
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }


                // =====================================================
                // 7. INFORMAR RESULTADO
                // =====================================================

                string mensaje =
                    "Venta registrada correctamente."
                    + "\n\n"
                    + "Venta N.º "
                    + idVenta
                    + "\n"
                    + "Comprobante: "
                    + venta.NroFactura;


                if (
                    !string.IsNullOrWhiteSpace(
                        rutaPdf
                    )
                )
                {
                    mensaje +=
                        "\n\n"
                        + "El comprobante PDF fue generado correctamente.";
                }


                MessageBox.Show(
                    mensaje,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );


                // =====================================================
                // 8. ABRIR EL PDF
                // =====================================================

                if (
                    !string.IsNullOrWhiteSpace(
                        rutaPdf
                    )
                )
                {
                    try
                    {
                        GeneradorComprobantePdf.Abrir(
                            rutaPdf
                        );
                    }
                    catch (Exception exAbrir)
                    {
                        // El PDF ya existe.
                        // Si Windows no puede abrirlo automáticamente,
                        // simplemente informamos dónde quedó guardado.
                        MessageBox.Show(
                            "El comprobante fue generado, "
                            + "pero Windows no pudo abrirlo automáticamente."
                            + "\n\n"
                            + "Podés encontrarlo en:"
                            + "\n"
                            + rutaPdf
                            + "\n\n"
                            + exAbrir.Message,
                            "Aura Beauty",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }


                // =====================================================
                // 9. ACTUALIZAR PRODUCTOS Y PREPARAR NUEVA VENTA
                // =====================================================

                // La venta ya descontó stock en SQL Server.
                // Volvemos a consultar los productos para mostrar
                // las existencias actualizadas.
                CargarProductos();


                PrepararNuevaVenta();
            }
            catch (Exception ex)
            {
                // Este catch corresponde a errores ocurridos
                // ANTES o DURANTE el registro de la venta.
                MessageBox.Show(
                    ex.Message,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
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

        private void btnNuevaVenta_Click(
            object sender,
            EventArgs e
        )
        {
            if (carrito.Count > 0)
            {
                DialogResult respuesta =
                    MessageBox.Show(
                        "La venta actual todavía tiene productos.\n\n"
                        + "¿Deseás descartarla?",
                        "Aura Beauty",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );


                if (
                    respuesta !=
                    DialogResult.Yes
                )
                {
                    return;
                }
            }


            PrepararNuevaVenta();
        }


        /// <summary>
        /// Restablece todos los datos necesarios para comenzar
        /// una nueva operación de venta.
        /// </summary>
        private void PrepararNuevaVenta()
        {
            carrito =
                new List<VentaDetalle>();


            ActualizarCarrito();


            chkConsumidorFinal.Checked =
                true;


            cboCliente.Enabled =
                false;


            if (
                cboCliente.Items.Count > 0
            )
            {
                cboCliente.SelectedIndex =
                    0;
            }


            cboTipoFactura.SelectedIndex =
                0;


            txtNroFactura.Text =
                GenerarNumeroComprobante();


            txtBuscarProducto.Clear();


            LimpiarSeleccionProducto();


            MostrarProductos(
                listaProductos
            );
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
    }
}