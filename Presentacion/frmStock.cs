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
    /// Formulario encargado de la gestión de existencias
    /// de los productos de Aura Beauty.
    ///
    /// Permite:
    /// - Consultar el stock actual.
    /// - Buscar productos por nombre.
    /// - Filtrar productos por categoría.
    /// - Agregar unidades al stock.
    /// - Quitar unidades del stock.
    /// - Identificar visualmente productos con stock bajo.
    ///
    /// Este formulario NO modifica nombre, precio, descripción
    /// ni categoría del producto. Esas operaciones corresponden
    /// al módulo Catálogo de Productos.
    /// </summary>
    public partial class frmStock : Form
    {
        // =========================================================
        // CAPA DE NEGOCIO
        // =========================================================

        /// <summary>
        /// Objeto de la capa de Negocio encargado
        /// de las operaciones relacionadas con stock.
        /// </summary>
        private readonly CN_Stock cnStock = new CN_Stock();

        /// <summary>
        /// Se utiliza para obtener las categorías activas
        /// que aparecerán en el filtro.
        /// </summary>
        private readonly CN_Categoria cnCategoria = new CN_Categoria();


        // =========================================================
        // DATOS DEL FORMULARIO
        // =========================================================

        /// <summary>
        /// Mantiene en memoria los productos activos.
        /// </summary>
        private List<Producto> listaProductos = new List<Producto>();

        /// <summary>
        /// ID del producto actualmente seleccionado.
        /// Cero significa que no existe selección.
        /// </summary>
        private int idProductoSeleccionado = 0;

        /// <summary>
        /// Stock actual del producto seleccionado.
        /// </summary>
        private int stockActualSeleccionado = 0;

        /// <summary>
        /// Cantidad máxima para considerar un producto
        /// con stock bajo.
        /// </summary>
        private const int STOCK_BAJO = 5;


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

        private TextBox txtBuscar;
        private TextBox txtCantidad;

        private ComboBox cboCategoria;

        private Label lblProductoSeleccionado;
        private Label lblStockActual;

        private Button btnBuscar;
        private Button btnAgregar;
        private Button btnQuitar;
        private Button btnLimpiar;

        private DataGridView dgvStock;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        /// <summary>
        /// Constructor del formulario.
        ///
        /// InitializeComponent inicializa la parte básica
        /// generada por Windows Forms.
        ///
        /// Después se crea nuestra interfaz y se cargan
        /// los datos provenientes de la capa de Negocio.
        /// </summary>
        public frmStock()
        {
            InitializeComponent();

            ConfigurarFormulario();

            CrearInterfaz();

            CargarCategorias();

            CargarProductos();
        }


        // =========================================================
        // CONFIGURACIÓN GENERAL
        // =========================================================

        /// <summary>
        /// Establece las propiedades generales de la ventana.
        /// </summary>
        private void ConfigurarFormulario()
        {
            Text = "Aura Beauty - Gestión de Stock";

            StartPosition = FormStartPosition.CenterScreen;

            Width = 1100;
            Height = 680;

            BackColor = colorFondo;

            Font = new Font("Segoe UI", 10F);

            FormBorderStyle = FormBorderStyle.FixedSingle;

            MaximizeBox = false;
        }


        // =========================================================
        // CREACIÓN DE LA INTERFAZ
        // =========================================================

        /// <summary>
        /// Construye los controles visuales del formulario.
        /// </summary>
        private void CrearInterfaz()
        {
            // -----------------------------------------------------
            // ENCABEZADO
            // -----------------------------------------------------

            Panel panelEncabezado = new Panel();

            panelEncabezado.Dock = DockStyle.Top;

            panelEncabezado.Height = 130;

            panelEncabezado.BackColor = colorRosa;

            Controls.Add(panelEncabezado);


            Label lblTitulo = new Label();

            lblTitulo.Text = "GESTIÓN DE STOCK";

            lblTitulo.Font =
                new Font("Segoe UI", 24F, FontStyle.Bold);

            lblTitulo.ForeColor = Color.White;

            lblTitulo.AutoSize = true;

            lblTitulo.Location = new Point(35, 28);

            panelEncabezado.Controls.Add(lblTitulo);


            Label lblSubtitulo = new Label();

            lblSubtitulo.Text =
                "Control y actualización de existencias";

            lblSubtitulo.Font =
                new Font("Segoe UI", 11F);

            lblSubtitulo.ForeColor = Color.White;

            lblSubtitulo.AutoSize = true;

            lblSubtitulo.Location = new Point(38, 80);

            panelEncabezado.Controls.Add(lblSubtitulo);


            // -----------------------------------------------------
            // PANEL IZQUIERDO
            // -----------------------------------------------------

            Panel panelOperacion = new Panel();

            panelOperacion.Location =
                new Point(25, 160);

            panelOperacion.Size =
                new Size(350, 455);

            panelOperacion.BackColor =
                Color.White;

            Controls.Add(panelOperacion);


            Label lblOperacion = new Label();

            lblOperacion.Text =
                "Actualizar existencias";

            lblOperacion.Font =
                new Font(
                    "Segoe UI",
                    16F,
                    FontStyle.Bold
                );

            lblOperacion.ForeColor =
                colorTexto;

            lblOperacion.AutoSize =
                true;

            lblOperacion.Location =
                new Point(25, 25);

            panelOperacion.Controls.Add(
                lblOperacion
            );


            // Producto seleccionado
            Label lblProductoTitulo =
                CrearEtiqueta(
                    "Producto seleccionado",
                    25,
                    85
                );

            panelOperacion.Controls.Add(
                lblProductoTitulo
            );


            lblProductoSeleccionado =
                new Label();

            lblProductoSeleccionado.Text =
                "Ningún producto seleccionado";

            lblProductoSeleccionado.Location =
                new Point(25, 115);

            lblProductoSeleccionado.Size =
                new Size(300, 40);

            lblProductoSeleccionado.ForeColor =
                colorRosaOscuro;

            lblProductoSeleccionado.Font =
                new Font(
                    "Segoe UI",
                    11F,
                    FontStyle.Bold
                );

            panelOperacion.Controls.Add(
                lblProductoSeleccionado
            );


            // Stock actual
            Label lblStockTitulo =
                CrearEtiqueta(
                    "Stock actual",
                    25,
                    170
                );

            panelOperacion.Controls.Add(
                lblStockTitulo
            );


            lblStockActual =
                new Label();

            lblStockActual.Text = "-";

            lblStockActual.Location =
                new Point(25, 200);

            lblStockActual.AutoSize =
                true;

            lblStockActual.Font =
                new Font(
                    "Segoe UI",
                    20F,
                    FontStyle.Bold
                );

            lblStockActual.ForeColor =
                colorTexto;

            panelOperacion.Controls.Add(
                lblStockActual
            );


            // Cantidad
            Label lblCantidad =
                CrearEtiqueta(
                    "Cantidad",
                    25,
                    260
                );

            panelOperacion.Controls.Add(
                lblCantidad
            );


            txtCantidad =
                CrearCajaTexto(
                    25,
                    290,
                    300
                );

            panelOperacion.Controls.Add(
                txtCantidad
            );


            // Botón agregar
            btnAgregar =
                CrearBoton(
                    "AGREGAR STOCK",
                    25,
                    345,
                    140
                );

            btnAgregar.Click +=
                btnAgregar_Click;

            panelOperacion.Controls.Add(
                btnAgregar
            );


            // Botón quitar
            btnQuitar =
                CrearBoton(
                    "QUITAR STOCK",
                    185,
                    345,
                    140
                );

            btnQuitar.Click +=
                btnQuitar_Click;

            panelOperacion.Controls.Add(
                btnQuitar
            );


            // Limpiar
            btnLimpiar =
                CrearBotonSecundario(
                    "LIMPIAR",
                    25,
                    400,
                    300
                );

            btnLimpiar.Click +=
                btnLimpiar_Click;

            panelOperacion.Controls.Add(
                btnLimpiar
            );


            // -----------------------------------------------------
            // PANEL DERECHO
            // -----------------------------------------------------

            Panel panelListado = new Panel();

            panelListado.Location =
                new Point(400, 160);

            panelListado.Size =
                new Size(660, 455);

            panelListado.BackColor =
                Color.White;

            Controls.Add(panelListado);


            Label lblListado = new Label();

            lblListado.Text =
                "Existencias actuales";

            lblListado.Font =
                new Font(
                    "Segoe UI",
                    16F,
                    FontStyle.Bold
                );

            lblListado.ForeColor =
                colorTexto;

            lblListado.AutoSize =
                true;

            lblListado.Location =
                new Point(25, 20);

            panelListado.Controls.Add(
                lblListado
            );


            // -----------------------------------------------------
            // BUSCADOR
            // -----------------------------------------------------

            Label lblBuscar =
                CrearEtiqueta(
                    "Buscar por producto",
                    25,
                    65
                );

            panelListado.Controls.Add(
                lblBuscar
            );


            txtBuscar =
                CrearCajaTexto(
                    25,
                    92,
                    230
                );

            panelListado.Controls.Add(
                txtBuscar
            );


            // -----------------------------------------------------
            // FILTRO DE CATEGORÍA
            // -----------------------------------------------------

            Label lblCategoria =
                CrearEtiqueta(
                    "Categoría",
                    275,
                    65
                );

            panelListado.Controls.Add(
                lblCategoria
            );


            cboCategoria = new ComboBox();

            cboCategoria.Location =
                new Point(275, 92);

            cboCategoria.Size =
                new Size(190, 30);

            cboCategoria.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cboCategoria.Font =
                new Font("Segoe UI", 10F);

            panelListado.Controls.Add(
                cboCategoria
            );


            // -----------------------------------------------------
            // BOTÓN BUSCAR
            // -----------------------------------------------------

            btnBuscar =
                CrearBoton(
                    "BUSCAR",
                    485,
                    89,
                    145
                );

            btnBuscar.Height = 35;

            btnBuscar.Click +=
                btnBuscar_Click;

            panelListado.Controls.Add(
                btnBuscar
            );


            // -----------------------------------------------------
            // GRILLA
            // -----------------------------------------------------

            dgvStock = new DataGridView();

            dgvStock.Location =
                new Point(25, 140);

            dgvStock.Size =
                new Size(605, 285);

            dgvStock.AllowUserToAddRows =
                false;

            dgvStock.AllowUserToDeleteRows =
                false;

            dgvStock.ReadOnly =
                true;

            dgvStock.MultiSelect =
                false;

            dgvStock.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvStock.AutoGenerateColumns =
                false;

            dgvStock.BackgroundColor =
                Color.White;

            dgvStock.BorderStyle =
                BorderStyle.FixedSingle;

            dgvStock.RowHeadersVisible =
                false;

            dgvStock.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;


            dgvStock.CellClick +=
                dgvStock_CellClick;

            dgvStock.CellFormatting +=
                dgvStock_CellFormatting;


            dgvStock.Columns.Add(
                CrearColumna(
                    "IdProducto",
                    "ID",
                    "IdProducto"
                )
            );


            dgvStock.Columns.Add(
                CrearColumna(
                    "Nombre",
                    "Producto",
                    "Nombre"
                )
            );


            dgvStock.Columns.Add(
                CrearColumna(
                    "Categoria",
                    "Categoría",
                    "Categoria"
                )
            );


            dgvStock.Columns.Add(
                CrearColumna(
                    "Stock",
                    "Stock actual",
                    "Stock"
                )
            );


            panelListado.Controls.Add(
                dgvStock
            );
        }


        // =========================================================
        // CREACIÓN DE CONTROLES REUTILIZABLES
        // =========================================================

        private Label CrearEtiqueta(
            string texto,
            int x,
            int y
        )
        {
            Label etiqueta = new Label();

            etiqueta.Text = texto;

            etiqueta.AutoSize = true;

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
            TextBox caja = new TextBox();

            caja.Location =
                new Point(x, y);

            caja.Width = ancho;

            caja.Font =
                new Font("Segoe UI", 10F);

            return caja;
        }


        private Button CrearBoton(
            string texto,
            int x,
            int y,
            int ancho
        )
        {
            Button boton = new Button();

            boton.Text = texto;

            boton.Location =
                new Point(x, y);

            boton.Size =
                new Size(ancho, 42);

            boton.FlatStyle =
                FlatStyle.Flat;

            boton.FlatAppearance.BorderSize =
                0;

            boton.BackColor =
                colorRosa;

            boton.ForeColor =
                Color.White;

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
            Button boton = new Button();

            boton.Text = texto;

            boton.Location =
                new Point(x, y);

            boton.Size =
                new Size(ancho, 42);

            boton.FlatStyle =
                FlatStyle.Flat;

            boton.FlatAppearance.BorderSize =
                1;

            boton.FlatAppearance.BorderColor =
                colorRosa;

            boton.BackColor =
                Color.White;

            boton.ForeColor =
                colorRosaOscuro;

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


        // =========================================================
        // CARGAR CATEGORÍAS
        // =========================================================

        /// <summary>
        /// Obtiene las categorías activas desde Negocio
        /// y agrega la opción "Todas las categorías".
        /// </summary>
        private void CargarCategorias()
        {
            try
            {
                List<Categoria> categorias =
                    cnCategoria.Listar();


                List<Categoria> categoriasFiltro =
                    new List<Categoria>();


                categoriasFiltro.Add(
                    new Categoria
                    {
                        IdCategoria = 0,
                        Nombre = "Todas las categorías"
                    }
                );


                categoriasFiltro.AddRange(
                    categorias
                );


                cboCategoria.DataSource =
                    categoriasFiltro;

                cboCategoria.DisplayMember =
                    "Nombre";

                cboCategoria.ValueMember =
                    "IdCategoria";

                cboCategoria.SelectedValue =
                    0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No fue posible cargar las categorías.\n\n"
                    + ex.Message,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        // =========================================================
        // CARGAR PRODUCTOS
        // =========================================================

        /// <summary>
        /// Obtiene los productos y sus existencias
        /// mediante la capa de Negocio.
        /// </summary>
        private void CargarProductos()
        {
            try
            {
                listaProductos =
                    cnStock.Listar();

                AplicarFiltros();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No fue posible cargar el stock.\n\n"
                    + ex.Message,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        // =========================================================
        // MOSTRAR PRODUCTOS
        // =========================================================

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

                        Categoria =
                            p.oCategoria != null
                                ? p.oCategoria.Nombre
                                : "",

                        p.Stock
                    }
                )
                .ToList();


            dgvStock.DataSource = null;

            dgvStock.DataSource = datos;
        }


        // =========================================================
        // FILTROS
        // =========================================================

        /// <summary>
        /// Aplica simultáneamente el filtro por nombre
        /// y el filtro por categoría.
        /// </summary>
        private void AplicarFiltros()
        {
            IEnumerable<Producto> resultado =
                listaProductos;


            string texto =
                txtBuscar.Text.Trim();


            // Búsqueda por nombre.
            if (!string.IsNullOrWhiteSpace(texto))
            {
                resultado =
                    resultado.Where(
                        p =>
                            !string.IsNullOrEmpty(p.Nombre)
                            &&
                            p.Nombre.IndexOf(
                                texto,
                                StringComparison.OrdinalIgnoreCase
                            ) >= 0
                    );
            }


            // Filtro por categoría.
            if (cboCategoria.SelectedValue != null)
            {
                int idCategoria;


                if (
                    int.TryParse(
                        cboCategoria.SelectedValue.ToString(),
                        out idCategoria
                    )
                    &&
                    idCategoria > 0
                )
                {
                    resultado =
                        resultado.Where(
                            p =>
                                p.IdCategoria ==
                                idCategoria
                        );
                }
            }


            MostrarProductos(
                resultado.ToList()
            );
        }


        // =========================================================
        // SELECCIONAR PRODUCTO
        // =========================================================

        /// <summary>
        /// Al hacer clic en una fila recupera el producto
        /// seleccionado y muestra su stock actual.
        /// </summary>
        private void dgvStock_CellClick(
            object sender,
            DataGridViewCellEventArgs e
        )
        {
            if (e.RowIndex < 0)
                return;


            object valorId =
                dgvStock.Rows[e.RowIndex]
                .Cells["IdProducto"]
                .Value;


            if (valorId == null)
                return;


            int idProducto =
                Convert.ToInt32(valorId);


            Producto producto =
                listaProductos.FirstOrDefault(
                    p =>
                        p.IdProducto ==
                        idProducto
                );


            if (producto == null)
                return;


            idProductoSeleccionado =
                producto.IdProducto;


            stockActualSeleccionado =
                producto.Stock;


            lblProductoSeleccionado.Text =
                producto.Nombre;


            lblStockActual.Text =
                producto.Stock.ToString();


            txtCantidad.Clear();

            txtCantidad.Focus();
        }


        // =========================================================
        // AGREGAR STOCK
        // =========================================================

        /// <summary>
        /// Agrega la cantidad ingresada al stock existente.
        /// </summary>
        private void btnAgregar_Click(
            object sender,
            EventArgs e
        )
        {
            try
            {
                if (idProductoSeleccionado == 0)
                {
                    MessageBox.Show(
                        "Seleccioná primero un producto.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }


                if (
                    !int.TryParse(
                        txtCantidad.Text,
                        out int cantidad
                    )
                )
                {
                    MessageBox.Show(
                        "Ingresá una cantidad válida.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtCantidad.Focus();

                    return;
                }


                bool resultado =
                    cnStock.AgregarStock(
                        idProductoSeleccionado,
                        stockActualSeleccionado,
                        cantidad
                    );


                if (!resultado)
                {
                    MessageBox.Show(
                        "No fue posible actualizar el stock.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }


                MessageBox.Show(
                    "Stock agregado correctamente.",
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );


                CargarProductos();

                LimpiarSeleccion();
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
        // QUITAR STOCK
        // =========================================================

        /// <summary>
        /// Descuenta la cantidad indicada del stock actual.
        ///
        /// La capa de Negocio verifica que no pueda
        /// generarse un stock negativo.
        /// </summary>
        private void btnQuitar_Click(
            object sender,
            EventArgs e
        )
        {
            try
            {
                if (idProductoSeleccionado == 0)
                {
                    MessageBox.Show(
                        "Seleccioná primero un producto.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }


                if (
                    !int.TryParse(
                        txtCantidad.Text,
                        out int cantidad
                    )
                )
                {
                    MessageBox.Show(
                        "Ingresá una cantidad válida.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtCantidad.Focus();

                    return;
                }


                bool resultado =
                    cnStock.QuitarStock(
                        idProductoSeleccionado,
                        stockActualSeleccionado,
                        cantidad
                    );


                if (!resultado)
                {
                    MessageBox.Show(
                        "No fue posible actualizar el stock.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }


                MessageBox.Show(
                    "Stock actualizado correctamente.",
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );


                CargarProductos();

                LimpiarSeleccion();
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
        // BUSCAR
        // =========================================================

        private void btnBuscar_Click(
            object sender,
            EventArgs e
        )
        {
            AplicarFiltros();
        }


        // =========================================================
        // ALERTA VISUAL DE STOCK BAJO
        // =========================================================

        /// <summary>
        /// Destaca visualmente los productos que poseen
        /// cinco unidades o menos.
        /// </summary>
        private void dgvStock_CellFormatting(
            object sender,
            DataGridViewCellFormattingEventArgs e
        )
        {
            if (e.RowIndex < 0)
                return;


            object valorStock =
                dgvStock.Rows[e.RowIndex]
                .Cells["Stock"]
                .Value;


            if (valorStock == null)
                return;


            if (
                !int.TryParse(
                    valorStock.ToString(),
                    out int stock
                )
            )
            {
                return;
            }


            if (stock <= STOCK_BAJO)
            {
                dgvStock.Rows[e.RowIndex]
                    .DefaultCellStyle.BackColor =
                    Color.MistyRose;


                dgvStock.Rows[e.RowIndex]
                    .DefaultCellStyle.ForeColor =
                    Color.DarkRed;
            }
        }


        // =========================================================
        // LIMPIAR
        // =========================================================

        private void btnLimpiar_Click(
            object sender,
            EventArgs e
        )
        {
            LimpiarSeleccion();
        }


        /// <summary>
        /// Elimina la selección actual y deja preparada
        /// la pantalla para una nueva operación.
        /// </summary>
        private void LimpiarSeleccion()
        {
            idProductoSeleccionado = 0;

            stockActualSeleccionado = 0;

            lblProductoSeleccionado.Text =
                "Ningún producto seleccionado";

            lblStockActual.Text = "-";

            txtCantidad.Clear();

            dgvStock.ClearSelection();
        }
    }
}
