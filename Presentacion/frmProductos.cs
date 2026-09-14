using Entidades;
using Negocio;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Aura_Beauty
{
    /// <summary>
    /// Formulario encargado de la gestión del catálogo de productos.
    ///
    /// Permite:
    /// - Listar productos activos.
    /// - Buscar productos por nombre.
    /// - Filtrar productos por categoría.
    /// - Registrar productos.
    /// - Editar productos.
    /// - Dar de baja productos.
    /// - Mostrar visualmente productos con stock bajo.
    ///
    /// También puede funcionar en modo de solo consulta
    /// para el rol Vendedor.
    /// </summary>
    public partial class frmProductos : Form
    {
        // ==========================
        // CAPA DE NEGOCIO
        // ==========================

        private readonly CN_Producto cnProducto = new CN_Producto();

        private readonly CN_Categoria cnCategoria = new CN_Categoria();


        // ==========================
        // DATOS DEL FORMULARIO
        // ==========================

        /// <summary>
        /// Contiene todos los productos activos recuperados
        /// desde la base de datos.
        ///
        /// Esta lista será utilizada también para realizar
        /// los filtros por nombre y categoría.
        /// </summary>
        private List<Producto> listaProductos = new List<Producto>();


        /// <summary>
        /// Indica si el formulario está siendo utilizado
        /// solamente para consultar productos.
        /// </summary>
        private readonly bool soloConsulta;


        /// <summary>
        /// ID del producto seleccionado.
        ///
        /// El valor 0 significa que no hay ningún producto
        /// seleccionado actualmente.
        /// </summary>
        private int idProductoSeleccionado = 0;


        /// <summary>
        /// Límite definido para mostrar una advertencia
        /// visual de stock bajo.
        /// </summary>
        private const int STOCK_BAJO = 5;


        // ==========================
        // COLORES AURA BEAUTY
        // ==========================

        private readonly Color colorRosa =
            Color.FromArgb(201, 143, 149);

        private readonly Color colorRosaOscuro =
            Color.FromArgb(174, 112, 120);

        private readonly Color colorFondo =
            Color.FromArgb(255, 249, 248);

        private readonly Color colorTexto =
            Color.FromArgb(94, 74, 74);


        // ==========================
        // CONTROLES
        // ==========================

        private TextBox txtNombre;
        private TextBox txtDescripcion;
        private TextBox txtPrecio;
        private TextBox txtStock;

        private TextBox txtBuscar;

        private ComboBox cboCategoria;

        /// <summary>
        /// ComboBox utilizado exclusivamente para filtrar
        /// los productos que se muestran en la grilla.
        /// </summary>
        private ComboBox cboFiltroCategoria;

        private Button btnGuardar;
        private Button btnEditar;
        private Button btnEliminar;
        private Button btnLimpiar;
        private Button btnCategorias;
        private Button btnBuscar;

        private DataGridView dgvProductos;

        private Panel panelDatos;


        // ==========================
        // CONSTRUCTOR
        // ==========================

        /// <summary>
        /// Constructor del formulario.
        ///
        /// soloConsulta = true:
        /// el usuario solo puede buscar y visualizar productos.
        ///
        /// soloConsulta = false:
        /// puede gestionar el catálogo.
        /// </summary>
        public frmProductos(bool soloConsulta = false)
        {
            InitializeComponent();

            this.soloConsulta = soloConsulta;

            ConfigurarFormulario();

            CrearInterfaz();

            CargarCategorias();

            CargarProductos();

            ConfigurarPermisos();
        }


        // ==========================================================
        // CONFIGURACIÓN GENERAL
        // ==========================================================

        private void ConfigurarFormulario()
        {
            Text = "Aura Beauty - Gestión de Productos";

            StartPosition = FormStartPosition.CenterScreen;

            Width = 1200;
            Height = 720;

            BackColor = colorFondo;

            Font = new Font("Segoe UI", 10F);

            FormBorderStyle = FormBorderStyle.FixedSingle;

            MaximizeBox = false;
        }


        // ==========================================================
        // CREACIÓN DE INTERFAZ
        // ==========================================================

        private void CrearInterfaz()
        {
            // ======================================================
            // ENCABEZADO
            // ======================================================

            Panel panelEncabezado = new Panel();

            panelEncabezado.Dock = DockStyle.Top;

            panelEncabezado.Height = 135;

            panelEncabezado.BackColor = colorRosa;

            Controls.Add(panelEncabezado);


            Label lblTitulo = new Label();

            lblTitulo.Text = soloConsulta
                ? "CONSULTA DE PRODUCTOS"
                : "GESTIÓN DE PRODUCTOS";

            lblTitulo.Font =
                new Font("Segoe UI", 24F, FontStyle.Bold);

            lblTitulo.ForeColor = Color.White;

            lblTitulo.AutoSize = true;

            lblTitulo.Location = new Point(35, 30);

            panelEncabezado.Controls.Add(lblTitulo);


            Label lblSubtitulo = new Label();

            lblSubtitulo.Text = soloConsulta
                ? "Consulta del catálogo disponible"
                : "Administración del catálogo de productos";

            lblSubtitulo.Font =
                new Font("Segoe UI", 11F);

            lblSubtitulo.ForeColor = Color.White;

            lblSubtitulo.AutoSize = true;

            lblSubtitulo.Location = new Point(38, 82);

            panelEncabezado.Controls.Add(lblSubtitulo);


            // ======================================================
            // PANEL DE DATOS
            // ======================================================

            panelDatos = new Panel();

            panelDatos.Location =
                new Point(25, 165);

            panelDatos.Size =
                new Size(420, 500);

            panelDatos.BackColor =
                Color.White;

            Controls.Add(panelDatos);


            // Nombre
            panelDatos.Controls.Add(
                CrearEtiqueta(
                    "Nombre",
                    30,
                    25
                )
            );


            txtNombre =
                CrearCajaTexto(
                    30,
                    55,
                    350
                );

            panelDatos.Controls.Add(txtNombre);


            // Descripción
            panelDatos.Controls.Add(
                CrearEtiqueta(
                    "Descripción",
                    30,
                    105
                )
            );


            txtDescripcion =
                CrearCajaTexto(
                    30,
                    135,
                    350
                );

            txtDescripcion.Multiline = true;

            txtDescripcion.Height = 70;

            panelDatos.Controls.Add(txtDescripcion);


            // Precio
            panelDatos.Controls.Add(
                CrearEtiqueta(
                    "Precio",
                    30,
                    225
                )
            );


            txtPrecio =
                CrearCajaTexto(
                    30,
                    255,
                    160
                );

            panelDatos.Controls.Add(txtPrecio);


            // Stock
            panelDatos.Controls.Add(
                CrearEtiqueta(
                    "Stock",
                    220,
                    225
                )
            );


            txtStock =
                CrearCajaTexto(
                    220,
                    255,
                    160
                );

            panelDatos.Controls.Add(txtStock);


            // Categoría
            panelDatos.Controls.Add(
                CrearEtiqueta(
                    "Categoría",
                    30,
                    305
                )
            );


            cboCategoria =
                new ComboBox();

            cboCategoria.Location =
                new Point(30, 335);

            cboCategoria.Width = 350;

            cboCategoria.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cboCategoria.Font =
                new Font("Segoe UI", 10F);

            panelDatos.Controls.Add(cboCategoria);


            // Botones
            btnGuardar =
                CrearBoton(
                    "GUARDAR",
                    30,
                    390,
                    165
                );

            btnGuardar.Click +=
                btnGuardar_Click;

            panelDatos.Controls.Add(btnGuardar);


            btnEditar =
                CrearBoton(
                    "EDITAR",
                    215,
                    390,
                    165
                );

            btnEditar.Click +=
                btnEditar_Click;

            panelDatos.Controls.Add(btnEditar);


            btnEliminar =
                CrearBotonSecundario(
                    "DAR DE BAJA",
                    30,
                    445,
                    165
                );

            btnEliminar.Click +=
                btnEliminar_Click;

            panelDatos.Controls.Add(btnEliminar);


            btnLimpiar =
                CrearBotonSecundario(
                    "LIMPIAR",
                    215,
                    445,
                    165
                );

            btnLimpiar.Click +=
                btnLimpiar_Click;

            panelDatos.Controls.Add(btnLimpiar);


            // ======================================================
            // PANEL DE LISTADO
            // ======================================================

            Panel panelListado =
                new Panel();

            panelListado.Location =
                new Point(475, 165);

            panelListado.Size =
                new Size(690, 500);

            panelListado.BackColor =
                Color.White;

            Controls.Add(panelListado);


            Label lblListado =
                new Label();

            lblListado.Text =
                "Productos activos";

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

            panelListado.Controls.Add(lblListado);


            // ======================================================
            // BUSCADOR POR NOMBRE
            // ======================================================

            Label lblBuscar =
                CrearEtiqueta(
                    "Buscar por nombre",
                    25,
                    58
                );

            panelListado.Controls.Add(lblBuscar);


            txtBuscar =
                CrearCajaTexto(
                    25,
                    85,
                    245
                );

            panelListado.Controls.Add(txtBuscar);


            // ======================================================
            // FILTRO POR CATEGORÍA
            // ======================================================

            Label lblFiltroCategoria =
                CrearEtiqueta(
                    "Categoría",
                    290,
                    58
                );

            panelListado.Controls.Add(
                lblFiltroCategoria
            );


            cboFiltroCategoria =
                new ComboBox();

            cboFiltroCategoria.Location =
                new Point(290, 85);

            cboFiltroCategoria.Size =
                new Size(180, 30);

            cboFiltroCategoria.DropDownStyle =
                ComboBoxStyle.DropDownList;

            cboFiltroCategoria.Font =
                new Font("Segoe UI", 10F);

            panelListado.Controls.Add(
                cboFiltroCategoria
            );


            btnBuscar =
                CrearBoton(
                    "BUSCAR",
                    485,
                    82,
                    80
                );

            btnBuscar.Height = 35;

            btnBuscar.Click +=
                btnBuscar_Click;

            panelListado.Controls.Add(btnBuscar);


            btnCategorias =
                CrearBotonSecundario(
                    "CATEGORÍAS",
                    575,
                    82,
                    90
                );

            btnCategorias.Height = 35;

            btnCategorias.Font =
                new Font(
                    "Segoe UI",
                    8F,
                    FontStyle.Bold
                );

            btnCategorias.Click +=
                btnCategorias_Click;

            panelListado.Controls.Add(btnCategorias);


            // ======================================================
            // GRILLA
            // ======================================================

            dgvProductos =
                new DataGridView();

            dgvProductos.Location =
                new Point(25, 135);

            dgvProductos.Size =
                new Size(640, 330);

            dgvProductos.AllowUserToAddRows =
                false;

            dgvProductos.AllowUserToDeleteRows =
                false;

            dgvProductos.ReadOnly =
                true;

            dgvProductos.MultiSelect =
                false;

            dgvProductos.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvProductos.AutoGenerateColumns =
                false;

            dgvProductos.BackgroundColor =
                Color.White;

            dgvProductos.BorderStyle =
                BorderStyle.FixedSingle;

            dgvProductos.RowHeadersVisible =
                false;

            dgvProductos.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvProductos.CellClick +=
                dgvProductos_CellClick;

            dgvProductos.CellFormatting +=
                dgvProductos_CellFormatting;


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
                    "Nombre",
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


            panelListado.Controls.Add(
                dgvProductos
            );
        }


        // ==========================================================
        // MÉTODOS PARA CREAR CONTROLES
        // ==========================================================

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
            Button boton =
                new Button();

            boton.Text =
                texto;

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
            Button boton =
                new Button();

            boton.Text =
                texto;

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


        // ==========================================================
        // CATEGORÍAS
        // ==========================================================

        private void CargarCategorias()
        {
            try
            {
                List<Categoria> categorias =
                    cnCategoria.Listar();


                // ----------------------------------------------
                // ComboBox de registro / edición
                // ----------------------------------------------

                cboCategoria.DataSource =
                    categorias.ToList();

                cboCategoria.DisplayMember =
                    "Nombre";

                cboCategoria.ValueMember =
                    "IdCategoria";

                cboCategoria.SelectedIndex =
                    -1;


                // ----------------------------------------------
                // ComboBox utilizado como filtro
                // ----------------------------------------------

                List<Categoria> categoriasFiltro =
                    new List<Categoria>();


                categoriasFiltro.Add(
                    new Categoria
                    {
                        IdCategoria = 0,

                        Nombre =
                            "Todas las categorías"
                    }
                );


                categoriasFiltro.AddRange(
                    categorias
                );


                cboFiltroCategoria.DataSource =
                    categoriasFiltro;

                cboFiltroCategoria.DisplayMember =
                    "Nombre";

                cboFiltroCategoria.ValueMember =
                    "IdCategoria";

                cboFiltroCategoria.SelectedValue =
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


        // ==========================================================
        // PRODUCTOS
        // ==========================================================

        private void CargarProductos()
        {
            try
            {
                listaProductos =
                    cnProducto.Listar();

                AplicarFiltros();
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


        /// <summary>
        /// Muestra los productos recibidos en la grilla.
        /// </summary>
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


        // ==========================================================
        // FILTROS
        // ==========================================================

        /// <summary>
        /// Filtra los productos actualmente cargados.
        ///
        /// Se pueden combinar dos criterios:
        /// - texto contenido en el nombre;
        /// - categoría seleccionada.
        ///
        /// Si no se escribe un nombre y se selecciona
        /// "Todas las categorías", se muestran todos los productos.
        /// </summary>
        private void AplicarFiltros()
        {
            IEnumerable<Producto> resultado =
                listaProductos;


            string texto =
                txtBuscar != null
                    ? txtBuscar.Text.Trim()
                    : "";


            // ----------------------------------------------
            // Filtro por nombre
            // ----------------------------------------------

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


            // ----------------------------------------------
            // Filtro por categoría
            // ----------------------------------------------

            if (
                cboFiltroCategoria != null
                &&
                cboFiltroCategoria.SelectedValue != null
            )
            {
                int idCategoria;


                if (
                    int.TryParse(
                        cboFiltroCategoria.SelectedValue.ToString(),
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


        // ==========================================================
        // PERMISOS
        // ==========================================================

        private void ConfigurarPermisos()
        {
            if (!soloConsulta)
                return;


            txtNombre.Enabled =
                false;

            txtDescripcion.Enabled =
                false;

            txtPrecio.Enabled =
                false;

            txtStock.Enabled =
                false;

            cboCategoria.Enabled =
                false;


            btnGuardar.Visible =
                false;

            btnEditar.Visible =
                false;

            btnEliminar.Visible =
                false;

            btnLimpiar.Visible =
                false;

            btnCategorias.Visible =
                false;
        }


        // ==========================================================
        // GUARDAR
        // ==========================================================

        private void btnGuardar_Click(
            object sender,
            EventArgs e
        )
        {
            try
            {
                if (
                    !decimal.TryParse(
                        txtPrecio.Text,
                        out decimal precio
                    )
                )
                {
                    MessageBox.Show(
                        "Ingresá un precio válido.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtPrecio.Focus();

                    return;
                }


                if (
                    !int.TryParse(
                        txtStock.Text,
                        out int stock
                    )
                )
                {
                    MessageBox.Show(
                        "Ingresá un stock válido.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    txtStock.Focus();

                    return;
                }


                if (
                    cboCategoria.SelectedValue ==
                    null
                )
                {
                    MessageBox.Show(
                        "Seleccioná una categoría.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }


                Producto producto =
                    new Producto
                    {
                        Nombre =
                            txtNombre.Text.Trim(),

                        Descripcion =
                            txtDescripcion.Text.Trim(),

                        Precio =
                            precio,

                        Stock =
                            stock,

                        IdCategoria =
                            Convert.ToInt32(
                                cboCategoria.SelectedValue
                            )
                    };


                int idGenerado =
                    cnProducto.Registrar(
                        producto
                    );


                MessageBox.Show(
                    "Producto registrado correctamente.\n\n"
                    +
                    "ID generado: "
                    +
                    idGenerado,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );


                CargarProductos();

                LimpiarFormulario();
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


        // ==========================================================
        // EDITAR
        // ==========================================================

        private void btnEditar_Click(
            object sender,
            EventArgs e
        )
        {
            try
            {
                if (
                    idProductoSeleccionado ==
                    0
                )
                {
                    MessageBox.Show(
                        "Seleccioná un producto para editar.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }


                if (
                    !decimal.TryParse(
                        txtPrecio.Text,
                        out decimal precio
                    )
                )
                {
                    MessageBox.Show(
                        "Ingresá un precio válido.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }


                if (
                    !int.TryParse(
                        txtStock.Text,
                        out int stock
                    )
                )
                {
                    MessageBox.Show(
                        "Ingresá un stock válido.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }


                if (
                    cboCategoria.SelectedValue ==
                    null
                )
                {
                    MessageBox.Show(
                        "Seleccioná una categoría.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }


                Producto producto =
                    new Producto
                    {
                        IdProducto =
                            idProductoSeleccionado,

                        Nombre =
                            txtNombre.Text.Trim(),

                        Descripcion =
                            txtDescripcion.Text.Trim(),

                        Precio =
                            precio,

                        Stock =
                            stock,

                        IdCategoria =
                            Convert.ToInt32(
                                cboCategoria.SelectedValue
                            )
                    };


                bool resultado =
                    cnProducto.Editar(
                        producto
                    );


                if (!resultado)
                {
                    MessageBox.Show(
                        "No fue posible modificar el producto.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }


                MessageBox.Show(
                    "Producto actualizado correctamente.",
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );


                CargarProductos();

                LimpiarFormulario();
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


        // ==========================================================
        // ELIMINAR / BAJA LÓGICA
        // ==========================================================

        private void btnEliminar_Click(
            object sender,
            EventArgs e
        )
        {
            try
            {
                if (
                    idProductoSeleccionado ==
                    0
                )
                {
                    MessageBox.Show(
                        "Seleccioná un producto para dar de baja.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }


                DialogResult respuesta =
                    MessageBox.Show(
                        "¿Deseás dar de baja el producto seleccionado?",
                        "Confirmar baja",
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


                bool resultado =
                    cnProducto.Eliminar(
                        idProductoSeleccionado
                    );


                if (!resultado)
                {
                    MessageBox.Show(
                        "No fue posible dar de baja el producto.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }


                MessageBox.Show(
                    "Producto dado de baja correctamente.",
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );


                CargarProductos();

                LimpiarFormulario();
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


        // ==========================================================
        // BUSCAR
        // ==========================================================

        private void btnBuscar_Click(
            object sender,
            EventArgs e
        )
        {
            AplicarFiltros();
        }


        // ==========================================================
        // SELECCIÓN DE PRODUCTO
        // ==========================================================

        private void dgvProductos_CellClick(
            object sender,
            DataGridViewCellEventArgs e
        )
        {
            if (e.RowIndex < 0)
                return;


            if (
                dgvProductos.Rows[e.RowIndex]
                .Cells["IdProducto"]
                .Value == null
            )
            {
                return;
            }


            int id =
                Convert.ToInt32(
                    dgvProductos.Rows[e.RowIndex]
                    .Cells["IdProducto"]
                    .Value
                );


            Producto producto =
                listaProductos
                .FirstOrDefault(
                    p =>
                        p.IdProducto ==
                        id
                );


            if (producto == null)
                return;


            idProductoSeleccionado =
                producto.IdProducto;


            if (soloConsulta)
                return;


            txtNombre.Text =
                producto.Nombre;

            txtDescripcion.Text =
                producto.Descripcion;

            txtPrecio.Text =
                producto.Precio.ToString(
                    "0.00"
                );

            txtStock.Text =
                producto.Stock.ToString();

            cboCategoria.SelectedValue =
                producto.IdCategoria;
        }


        // ==========================================================
        // STOCK BAJO
        // ==========================================================

        private void dgvProductos_CellFormatting(
            object sender,
            DataGridViewCellFormattingEventArgs e
        )
        {
            if (
                dgvProductos.Rows.Count ==
                0
            )
            {
                return;
            }


            DataGridViewRow fila =
                dgvProductos.Rows[
                    e.RowIndex
                ];


            object valorStock =
                fila.Cells["Stock"]
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


            if (
                stock <=
                STOCK_BAJO
            )
            {
                fila.DefaultCellStyle.BackColor =
                    Color.MistyRose;

                fila.DefaultCellStyle.ForeColor =
                    Color.DarkRed;
            }
        }


        // ==========================================================
        // CATEGORÍAS
        // ==========================================================

        private void btnCategorias_Click(
            object sender,
            EventArgs e
        )
        {
            using (
                frmCategorias formulario =
                    new frmCategorias()
            )
            {
                formulario.ShowDialog();
            }


            CargarCategorias();

            AplicarFiltros();
        }


        // ==========================================================
        // LIMPIAR
        // ==========================================================

        private void btnLimpiar_Click(
            object sender,
            EventArgs e
        )
        {
            LimpiarFormulario();
        }


        private void LimpiarFormulario()
        {
            idProductoSeleccionado =
                0;


            txtNombre.Clear();

            txtDescripcion.Clear();

            txtPrecio.Clear();

            txtStock.Clear();


            cboCategoria.SelectedIndex =
                -1;


            dgvProductos.ClearSelection();


            if (!soloConsulta)
            {
                txtNombre.Focus();
            }
        }
    }
}