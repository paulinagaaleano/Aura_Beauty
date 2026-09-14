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
using System.Drawing.Drawing2D;

namespace Aura_Beauty
{
    /// <summary>
    /// Formulario de Gestión de Categorías de Aura Beauty.
    ///
    /// Pertenece a la Capa de Presentación.
    ///
    /// Permite:
    /// - visualizar categorías activas;
    /// - registrar nuevas categorías;
    /// - editar categorías existentes;
    /// - realizar bajas lógicas;
    /// - limpiar el formulario.
    ///
    /// Este formulario NO accede directamente
    /// a SQL Server.
    ///
    /// Se comunica con CN_Categoria,
    /// perteneciente a la Capa de Negocio.
    /// </summary>
    public partial class frmCategorias : Form
    {
        // ============================================================
        // CAPA DE NEGOCIO
        // ============================================================

        /// <summary>
        /// Objeto utilizado para acceder a las operaciones
        /// de negocio relacionadas con categorías.
        /// </summary>
        private CN_Categoria obj_cn_categoria =
            new CN_Categoria();


        // ============================================================
        // VARIABLES
        // ============================================================

        /// <summary>
        /// Guarda el ID de la categoría seleccionada.
        ///
        /// El valor 0 significa que actualmente
        /// no hay ninguna categoría seleccionada.
        /// </summary>
        private int idCategoriaSeleccionada = 0;


        /// <summary>
        /// Lista de categorías cargadas desde
        /// la Capa de Negocio.
        /// </summary>
        private List<Categoria> categoriasCargadas =
            new List<Categoria>();


        // ============================================================
        // CONTROLES
        // ============================================================

        private Panel pnlEncabezado;
        private Panel pnlFormulario;
        private Panel pnlListado;

        private Label lblTitulo;
        private Label lblSubtitulo;
        private Label lblNombre;
        private Label lblDescripcion;
        private Label lblImagen;
        private Label lblListado;

        private TextBox txtNombre;
        private TextBox txtDescripcion;
        private TextBox txtImagen;

        private Button btnGuardar;
        private Button btnEditar;
        private Button btnEliminar;
        private Button btnLimpiar;

        private DataGridView dgvCategorias;


        // ============================================================
        // PALETA AURA BEAUTY
        // ============================================================

        private readonly Color colorRosa =
            Color.FromArgb(201, 143, 149);

        private readonly Color colorRosaOscuro =
            Color.FromArgb(174, 112, 120);

        private readonly Color colorFondo =
            Color.FromArgb(255, 249, 248);

        private readonly Color colorTexto =
            Color.FromArgb(94, 74, 74);

        private readonly Color colorTextoSecundario =
            Color.FromArgb(138, 116, 116);


        // ============================================================
        // CONSTRUCTOR
        // ============================================================

        /// <summary>
        /// Constructor del formulario.
        /// </summary>
        public frmCategorias()
        {
            InitializeComponent();

            ConfigurarFormulario();

            CrearInterfaz();

            CargarCategorias();
        }


        // ============================================================
        // CONFIGURACIÓN
        // ============================================================

        /// <summary>
        /// Configura las características generales
        /// de la ventana.
        /// </summary>
        private void ConfigurarFormulario()
        {
            this.Text =
                "Aura Beauty - Gestión de Categorías";

            this.Name =
                "frmCategorias";

            this.StartPosition =
                FormStartPosition.CenterScreen;

            this.Size =
                new Size(1000, 620);

            this.MinimumSize =
                new Size(1000, 620);

            this.BackColor =
                colorFondo;

            this.Font =
                new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Regular
                );
        }


        // ============================================================
        // INTERFAZ
        // ============================================================

        /// <summary>
        /// Construye la interfaz visual del formulario.
        /// </summary>
        private void CrearInterfaz()
        {
            // --------------------------------------------------------
            // ENCABEZADO
            // --------------------------------------------------------

            pnlEncabezado =
                new Panel();

            pnlEncabezado.Dock =
                DockStyle.Top;

            pnlEncabezado.Height =
                90;

            pnlEncabezado.BackColor =
                colorRosa;


            lblTitulo =
                new Label();

            lblTitulo.Text =
                "GESTIÓN DE CATEGORÍAS";

            lblTitulo.Font =
                new Font(
                    "Segoe UI",
                    20,
                    FontStyle.Bold
                );

            lblTitulo.ForeColor =
                Color.White;

            lblTitulo.AutoSize =
                true;

            lblTitulo.Location =
                new Point(30, 18);


            lblSubtitulo =
                new Label();

            lblSubtitulo.Text =
                "Organización del catálogo de productos";

            lblSubtitulo.Font =
                new Font(
                    "Segoe UI",
                    10,
                    FontStyle.Regular
                );

            lblSubtitulo.ForeColor =
                Color.White;

            lblSubtitulo.AutoSize =
                true;

            lblSubtitulo.Location =
                new Point(32, 57);


            pnlEncabezado.Controls.Add(
                lblTitulo
            );

            pnlEncabezado.Controls.Add(
                lblSubtitulo
            );


            // --------------------------------------------------------
            // PANEL FORMULARIO
            // --------------------------------------------------------

            pnlFormulario =
                new Panel();

            pnlFormulario.Location =
                new Point(25, 115);

            pnlFormulario.Size =
                new Size(340, 440);

            pnlFormulario.BackColor =
                Color.White;


            // NOMBRE

            lblNombre =
                CrearLabel(
                    "Nombre",
                    25,
                    30
                );

            txtNombre =
                CrearTextBox(
                    25,
                    58
                );


            // DESCRIPCIÓN

            lblDescripcion =
                CrearLabel(
                    "Descripción",
                    25,
                    110
                );

            txtDescripcion =
                new TextBox();

            txtDescripcion.Location =
                new Point(25, 138);

            txtDescripcion.Size =
                new Size(285, 90);

            txtDescripcion.Multiline =
                true;

            txtDescripcion.ScrollBars =
                ScrollBars.Vertical;

            txtDescripcion.Font =
                new Font(
                    "Segoe UI",
                    10
                );


            // IMAGEN

            lblImagen =
                CrearLabel(
                    "Referencia de imagen (opcional)",
                    25,
                    250
                );

            txtImagen =
                CrearTextBox(
                    25,
                    278
                );


            // BOTONES

            btnGuardar =
                CrearBotonPrincipal(
                    "GUARDAR",
                    25,
                    335
                );

            btnGuardar.Click +=
                btnGuardar_Click;


            btnEditar =
                CrearBotonPrincipal(
                    "EDITAR",
                    175,
                    335
                );

            btnEditar.Click +=
                btnEditar_Click;


            btnEliminar =
                CrearBotonSecundario(
                    "DAR DE BAJA",
                    25,
                    385
                );

            btnEliminar.Click +=
                btnEliminar_Click;


            btnLimpiar =
                CrearBotonSecundario(
                    "LIMPIAR",
                    175,
                    385
                );

            btnLimpiar.Click +=
                btnLimpiar_Click;


            pnlFormulario.Controls.Add(
                lblNombre
            );

            pnlFormulario.Controls.Add(
                txtNombre
            );

            pnlFormulario.Controls.Add(
                lblDescripcion
            );

            pnlFormulario.Controls.Add(
                txtDescripcion
            );

            pnlFormulario.Controls.Add(
                lblImagen
            );

            pnlFormulario.Controls.Add(
                txtImagen
            );

            pnlFormulario.Controls.Add(
                btnGuardar
            );

            pnlFormulario.Controls.Add(
                btnEditar
            );

            pnlFormulario.Controls.Add(
                btnEliminar
            );

            pnlFormulario.Controls.Add(
                btnLimpiar
            );


            // --------------------------------------------------------
            // PANEL LISTADO
            // --------------------------------------------------------

            pnlListado =
                new Panel();

            pnlListado.Location =
                new Point(390, 115);

            pnlListado.Size =
                new Size(570, 440);

            pnlListado.BackColor =
                Color.White;


            lblListado =
                new Label();

            lblListado.Text =
                "Categorías activas";

            lblListado.Font =
                new Font(
                    "Segoe UI",
                    14,
                    FontStyle.Bold
                );

            lblListado.ForeColor =
                colorTexto;

            lblListado.AutoSize =
                true;

            lblListado.Location =
                new Point(20, 20);


            // --------------------------------------------------------
            // GRILLA
            // --------------------------------------------------------

            dgvCategorias =
                new DataGridView();

            dgvCategorias.Location =
                new Point(20, 60);

            dgvCategorias.Size =
                new Size(530, 355);

            dgvCategorias.BackgroundColor =
                Color.White;

            dgvCategorias.BorderStyle =
                BorderStyle.None;

            dgvCategorias.ReadOnly =
                true;

            dgvCategorias.AllowUserToAddRows =
                false;

            dgvCategorias.AllowUserToDeleteRows =
                false;

            dgvCategorias.MultiSelect =
                false;

            dgvCategorias.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvCategorias.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvCategorias.RowHeadersVisible =
                false;

            dgvCategorias.EnableHeadersVisualStyles =
                false;

            dgvCategorias.ColumnHeadersDefaultCellStyle.BackColor =
                colorRosa;

            dgvCategorias.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgvCategorias.ColumnHeadersDefaultCellStyle.Font =
                new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold
                );

            dgvCategorias.AutoGenerateColumns =
                false;


            dgvCategorias.Columns.Add(
                CrearColumna(
                    "IdCategoria",
                    "ID"
                )
            );

            dgvCategorias.Columns.Add(
                CrearColumna(
                    "Nombre",
                    "Nombre"
                )
            );

            dgvCategorias.Columns.Add(
                CrearColumna(
                    "Descripcion",
                    "Descripción"
                )
            );

            dgvCategorias.Columns.Add(
                CrearColumna(
                    "Imagen",
                    "Imagen"
                )
            );


            dgvCategorias.CellClick +=
                dgvCategorias_CellClick;


            pnlListado.Controls.Add(
                lblListado
            );

            pnlListado.Controls.Add(
                dgvCategorias
            );


            // --------------------------------------------------------
            // AGREGAMOS AL FORMULARIO
            // --------------------------------------------------------

            this.Controls.Add(
                pnlListado
            );

            this.Controls.Add(
                pnlFormulario
            );

            this.Controls.Add(
                pnlEncabezado
            );


            RedondearControl(
                pnlFormulario,
                18
            );

            RedondearControl(
                pnlListado,
                18
            );
        }


        // ============================================================
        // CONTROLES AUXILIARES
        // ============================================================

        private Label CrearLabel(
            string texto,
            int x,
            int y)
        {
            Label label =
                new Label();

            label.Text =
                texto;

            label.AutoSize =
                true;

            label.Location =
                new Point(x, y);

            label.ForeColor =
                colorTexto;

            label.Font =
                new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold
                );

            return label;
        }


        private TextBox CrearTextBox(
            int x,
            int y)
        {
            TextBox textbox =
                new TextBox();

            textbox.Location =
                new Point(x, y);

            textbox.Size =
                new Size(285, 28);

            textbox.Font =
                new Font(
                    "Segoe UI",
                    10
                );

            return textbox;
        }


        private Button CrearBotonPrincipal(
            string texto,
            int x,
            int y)
        {
            Button boton =
                new Button();

            boton.Text =
                texto;

            boton.Location =
                new Point(x, y);

            boton.Size =
                new Size(135, 38);

            boton.BackColor =
                colorRosa;

            boton.ForeColor =
                Color.White;

            boton.FlatStyle =
                FlatStyle.Flat;

            boton.FlatAppearance.BorderSize =
                0;

            boton.Cursor =
                Cursors.Hand;

            boton.Font =
                new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold
                );

            RedondearControl(
                boton,
                12
            );

            return boton;
        }


        private Button CrearBotonSecundario(
            string texto,
            int x,
            int y)
        {
            Button boton =
                new Button();

            boton.Text =
                texto;

            boton.Location =
                new Point(x, y);

            boton.Size =
                new Size(135, 38);

            boton.BackColor =
                Color.White;

            boton.ForeColor =
                colorRosaOscuro;

            boton.FlatStyle =
                FlatStyle.Flat;

            boton.FlatAppearance.BorderColor =
                colorRosa;

            boton.FlatAppearance.BorderSize =
                1;

            boton.Cursor =
                Cursors.Hand;

            boton.Font =
                new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold
                );

            RedondearControl(
                boton,
                12
            );

            return boton;
        }


        private DataGridViewTextBoxColumn CrearColumna(
            string nombre,
            string titulo)
        {
            DataGridViewTextBoxColumn columna =
                new DataGridViewTextBoxColumn();

            columna.Name =
                nombre;

            columna.HeaderText =
                titulo;

            return columna;
        }


        // ============================================================
        // CARGAR CATEGORÍAS
        // ============================================================

        /// <summary>
        /// Obtiene las categorías activas
        /// desde la Capa de Negocio.
        /// </summary>
        private void CargarCategorias()
        {
            try
            {
                categoriasCargadas =
                    obj_cn_categoria.Listar();

                dgvCategorias.Rows.Clear();


                foreach (
                    Categoria categoria
                    in categoriasCargadas)
                {
                    dgvCategorias.Rows.Add(
                        categoria.IdCategoria,
                        categoria.Nombre,
                        categoria.Descripcion,
                        categoria.Imagen
                    );
                }


                dgvCategorias.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No fue posible cargar las categorías.\n\n" +
                    ex.Message,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        // ============================================================
        // GUARDAR
        // ============================================================

        private void btnGuardar_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                Categoria nuevaCategoria =
                    new Categoria()
                    {
                        Nombre =
                            txtNombre.Text,

                        Descripcion =
                            txtDescripcion.Text,

                        Imagen =
                            txtImagen.Text
                    };


                int nuevoId =
                    obj_cn_categoria.Registrar(
                        nuevaCategoria
                    );


                MessageBox.Show(
                    "Categoría registrada correctamente.\n\n" +
                    "ID asignado: " + nuevoId,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );


                CargarCategorias();

                LimpiarFormulario();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Datos incorrectos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ocurrió un error al registrar la categoría.\n\n" +
                    ex.Message,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        // ============================================================
        // EDITAR
        // ============================================================

        private void btnEditar_Click(
            object sender,
            EventArgs e)
        {
            if (idCategoriaSeleccionada == 0)
            {
                MessageBox.Show(
                    "Primero seleccione una categoría.",
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }


            try
            {
                Categoria categoriaEditada =
                    new Categoria()
                    {
                        IdCategoria =
                            idCategoriaSeleccionada,

                        Nombre =
                            txtNombre.Text,

                        Descripcion =
                            txtDescripcion.Text,

                        Imagen =
                            txtImagen.Text
                    };


                bool editada =
                    obj_cn_categoria.Editar(
                        categoriaEditada
                    );


                if (editada)
                {
                    MessageBox.Show(
                        "Categoría modificada correctamente.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    CargarCategorias();

                    LimpiarFormulario();
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Datos incorrectos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No fue posible modificar la categoría.\n\n" +
                    ex.Message,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        // ============================================================
        // BAJA LÓGICA
        // ============================================================

        private void btnEliminar_Click(
            object sender,
            EventArgs e)
        {
            if (idCategoriaSeleccionada == 0)
            {
                MessageBox.Show(
                    "Primero seleccione una categoría.",
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }


            DialogResult respuesta =
                MessageBox.Show(
                    "¿Desea dar de baja esta categoría?\n\n" +
                    "La categoría dejará de aparecer como activa.",
                    "Confirmar baja",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );


            if (respuesta != DialogResult.Yes)
            {
                return;
            }


            try
            {
                bool eliminada =
                    obj_cn_categoria.Eliminar(
                        idCategoriaSeleccionada
                    );


                if (eliminada)
                {
                    MessageBox.Show(
                        "Categoría dada de baja correctamente.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    CargarCategorias();

                    LimpiarFormulario();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No fue posible dar de baja la categoría.\n\n" +
                    ex.Message,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        // ============================================================
        // SELECCIÓN EN GRILLA
        // ============================================================

        private void dgvCategorias_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }


            DataGridViewRow fila =
                dgvCategorias.Rows[e.RowIndex];


            idCategoriaSeleccionada =
                Convert.ToInt32(
                    fila.Cells["IdCategoria"].Value
                );


            Categoria categoriaSeleccionada =
                BuscarCategoriaEnMemoria(
                    idCategoriaSeleccionada
                );


            if (categoriaSeleccionada == null)
            {
                return;
            }


            txtNombre.Text =
                categoriaSeleccionada.Nombre;

            txtDescripcion.Text =
                categoriaSeleccionada.Descripcion;

            txtImagen.Text =
                categoriaSeleccionada.Imagen;
        }


        /// <summary>
        /// Busca una categoría dentro de la lista
        /// que ya tenemos cargada en memoria.
        /// </summary>
        private Categoria BuscarCategoriaEnMemoria(
            int idCategoria)
        {
            foreach (
                Categoria categoria
                in categoriasCargadas)
            {
                if (
                    categoria.IdCategoria ==
                    idCategoria)
                {
                    return categoria;
                }
            }

            return null;
        }


        // ============================================================
        // LIMPIAR
        // ============================================================

        private void btnLimpiar_Click(
            object sender,
            EventArgs e)
        {
            LimpiarFormulario();
        }


        private void LimpiarFormulario()
        {
            idCategoriaSeleccionada =
                0;

            txtNombre.Clear();

            txtDescripcion.Clear();

            txtImagen.Clear();

            dgvCategorias.ClearSelection();

            txtNombre.Focus();
        }


        // ============================================================
        // DISEÑO
        // ============================================================

        private void RedondearControl(
            Control control,
            int radio)
        {
            GraphicsPath path =
                new GraphicsPath();

            int diametro =
                radio * 2;


            path.AddArc(
                0,
                0,
                diametro,
                diametro,
                180,
                90
            );

            path.AddArc(
                control.Width - diametro,
                0,
                diametro,
                diametro,
                270,
                90
            );

            path.AddArc(
                control.Width - diametro,
                control.Height - diametro,
                diametro,
                diametro,
                0,
                90
            );

            path.AddArc(
                0,
                control.Height - diametro,
                diametro,
                diametro,
                90,
                90
            );

            path.CloseFigure();

            control.Region =
                new Region(path);
        }
    }
}
