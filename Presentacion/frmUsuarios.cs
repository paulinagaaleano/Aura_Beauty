using Entidades;
using Negocio;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Aura_Beauty
{
    /// <summary>
    /// Formulario de Gestión de Usuarios de Aura Beauty.
    ///
    /// Este formulario pertenece a la Capa de Presentación.
    ///
    /// Su responsabilidad es:
    /// - mostrar los usuarios registrados;
    /// - permitir registrar nuevos usuarios;
    /// - permitir modificar usuarios;
    /// - permitir eliminar usuarios;
    /// - permitir seleccionar el rol correspondiente.
    ///
    /// IMPORTANTE:
    /// Este formulario NO realiza consultas SQL.
    ///
    /// Para trabajar con usuarios utiliza CN_Usuario.
    /// Para trabajar con roles utiliza CN_Rol.
    ///
    /// De esta manera se respeta la arquitectura por capas:
    ///
    /// Presentación
    ///      ↓
    /// Negocio
    ///      ↓
    /// Datos
    ///      ↓
    /// Base de datos
    /// </summary>
    public partial class frmUsuarios : Form
    {
        // ============================================================
        // OBJETOS DE LA CAPA DE NEGOCIO
        // ============================================================

        /// <summary>
        /// Objeto utilizado para acceder a las operaciones
        /// de negocio relacionadas con Usuario.
        /// </summary>
        private CN_Usuario obj_cn_usuario =
            new CN_Usuario();


        /// <summary>
        /// Objeto utilizado para obtener los roles
        /// disponibles en el sistema.
        /// </summary>
        private CN_Rol obj_cn_rol =
            new CN_Rol();


        // ============================================================
        // VARIABLES DEL FORMULARIO
        // ============================================================

        /// <summary>
        /// Guarda el identificador del usuario seleccionado
        /// en la grilla.
        ///
        /// Cuando vale 0 significa que no hay ningún
        /// usuario seleccionado para editar o eliminar.
        /// </summary>
        private int idUsuarioSeleccionado = 0;


        /// <summary>
        /// Usuario que actualmente tiene iniciada la sesión.
        ///
        /// Se conserva para poder aplicar reglas de seguridad,
        /// por ejemplo impedir que un administrador
        /// se dé de baja a sí mismo.
        /// </summary>
        private readonly Usuario usuarioActual;



        /// <summary>
        /// Mantiene en memoria la lista de usuarios
        /// obtenida desde la Capa de Negocio.
        ///
        /// Nos permite recuperar el objeto completo
        /// cuando seleccionamos una fila de la grilla.
        /// </summary>
        private List<Usuario> usuariosCargados =
            new List<Usuario>();


        // ============================================================
        // CONTROLES VISUALES
        // ============================================================

        private Panel pnlEncabezado;
        private Panel pnlFormulario;
        private Panel pnlListado;

        private Label lblTitulo;
        private Label lblSubtitulo;

        private Label lblNombre;
        private Label lblApellido;
        private Label lblCorreo;
        private Label lblContraseña;
        private Label lblRol;
        private Label lblListado;

        private TextBox txtNombre;
        private TextBox txtApellido;
        private TextBox txtCorreo;
        private TextBox txtContraseña;

        private ComboBox cboRol;

        private CheckBox chkMostrarContraseña;

        private Button btnGuardar;
        private Button btnEditar;
        private Button btnEliminar;
        private Button btnLimpiar;

        private DataGridView dgvUsuarios;


        // ============================================================
        // COLORES DEL SISTEMA
        // ============================================================

        /*
         * Color.FromArgb(...)
         *
         * Es un método de .NET que permite crear un color
         * indicando sus valores RGB:
         *
         * R = Red   (rojo)
         * G = Green (verde)
         * B = Blue  (azul)
         */

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
        /// 
        /// /// Recibe el usuario que tiene actualmente iniciada la sesión.
        /// </summary>
        /// <param name="usuario">
        /// Usuario autenticado que abrió este formulario.
        /// </param>
        ///
        /// Se ejecuta cuando se crea una instancia
        /// de frmUsuarios.
        /// </summary>
        public frmUsuarios(Usuario usuario)
        {
            /*
             * InitializeComponent()
             *
             * Método generado por Windows Forms.
             *
             * Inicializa la parte básica del formulario
             * definida en frmUsuarios.Designer.cs.
             */
            InitializeComponent();

            /*
             * Guardamos el usuario que inició sesión.
             *
             * De esta manera el formulario puede saber
             * quién está utilizando actualmente el sistema.
             */
            usuarioActual = usuario;


            // Configuramos la ventana.
            ConfigurarFormulario();


            // Creamos los controles visuales.
            CrearInterfaz();


            // Cargamos los roles desde la base de datos
            // a través de CN_Rol.
            CargarRoles();


            // Cargamos los usuarios existentes.
            CargarUsuarios();
        }


        // ============================================================
        // CONFIGURACIÓN GENERAL
        // ============================================================

        /// <summary>
        /// Configura las propiedades principales
        /// de la ventana.
        /// </summary>
        private void ConfigurarFormulario()
        {
            this.Text = "Aura Beauty - Gestión de Usuarios";

            this.Name = "frmUsuarios";

            this.StartPosition =
                FormStartPosition.CenterScreen;

            this.Size =
                new Size(1050, 690);

            this.MinimumSize =
                new Size(1050, 690);

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
        // CREACIÓN DE LA INTERFAZ
        // ============================================================

        /// <summary>
        /// Construye visualmente el formulario.
        ///
        /// Los controles se crean desde código para conservar
        /// un estilo uniforme con el Login y el Menú.
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
                "GESTIÓN DE USUARIOS";

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
                "Administración de empleados y roles";

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
            // PANEL DEL FORMULARIO
            // --------------------------------------------------------

            pnlFormulario =
                new Panel();

            pnlFormulario.Location =
                new Point(25, 115);

            pnlFormulario.Size =
                new Size(340, 500);

            pnlFormulario.BackColor =
                Color.White;


            // --------------------------------------------------------
            // NOMBRE
            // --------------------------------------------------------

            lblNombre =
                CrearLabel(
                    "Nombre",
                    25,
                    25
                );


            txtNombre =
                CrearTextBox(
                    25,
                    50
                );


            // --------------------------------------------------------
            // APELLIDO
            // --------------------------------------------------------

            lblApellido =
                CrearLabel(
                    "Apellido",
                    25,
                    95
                );


            txtApellido =
                CrearTextBox(
                    25,
                    120
                );


            // --------------------------------------------------------
            // CORREO
            // --------------------------------------------------------

            lblCorreo =
                CrearLabel(
                    "Correo electrónico",
                    25,
                    165
                );


            txtCorreo =
                CrearTextBox(
                    25,
                    190
                );


            // --------------------------------------------------------
            // CONTRASEÑA
            // --------------------------------------------------------

            lblContraseña =
                CrearLabel(
                    "Contraseña",
                    25,
                    235
                );


            txtContraseña =
                CrearTextBox(
                    25,
                    260
                );


            /*
             * UseSystemPasswordChar
             *
             * Cuando es true,
             * Windows oculta los caracteres escritos.
             */
            txtContraseña.UseSystemPasswordChar =
                true;


            chkMostrarContraseña =
                new CheckBox();

            chkMostrarContraseña.Text =
                "Mostrar contraseña";

            chkMostrarContraseña.AutoSize =
                true;

            chkMostrarContraseña.Location =
                new Point(25, 295);

            chkMostrarContraseña.ForeColor =
                colorTextoSecundario;

            chkMostrarContraseña.CheckedChanged +=
                chkMostrarContraseña_CheckedChanged;


            // --------------------------------------------------------
            // ROL
            // --------------------------------------------------------

            lblRol =
                CrearLabel(
                    "Rol",
                    25,
                    325
                );


            cboRol =
                new ComboBox();

            cboRol.Location =
                new Point(25, 350);

            cboRol.Size =
                new Size(285, 30);

            /*
             * DropDownList evita que el usuario escriba
             * cualquier texto manualmente.
             *
             * Solo podrá seleccionar roles existentes.
             */
            cboRol.DropDownStyle =
                ComboBoxStyle.DropDownList;


            // --------------------------------------------------------
            // BOTONES
            // --------------------------------------------------------

            btnGuardar =
                CrearBoton(
                    "GUARDAR",
                    25,
                    405,
                    135
                );

            btnGuardar.Click +=
                btnGuardar_Click;


            btnEditar =
                CrearBoton(
                    "EDITAR",
                    175,
                    405,
                    135
                );

            btnEditar.Click +=
                btnEditar_Click;


            btnEliminar =
                CrearBotonSecundario(
                    "ELIMINAR",
                    25,
                    445,
                    135
                );

            btnEliminar.Click +=
                btnEliminar_Click;


            btnLimpiar =
                CrearBotonSecundario(
                    "LIMPIAR",
                    175,
                    445,
                    135
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
                lblApellido
            );

            pnlFormulario.Controls.Add(
                txtApellido
            );

            pnlFormulario.Controls.Add(
                lblCorreo
            );

            pnlFormulario.Controls.Add(
                txtCorreo
            );

            pnlFormulario.Controls.Add(
                lblContraseña
            );

            pnlFormulario.Controls.Add(
                txtContraseña
            );

            pnlFormulario.Controls.Add(
                chkMostrarContraseña
            );

            pnlFormulario.Controls.Add(
                lblRol
            );

            pnlFormulario.Controls.Add(
                cboRol
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
                new Size(625, 500);

            pnlListado.BackColor =
                Color.White;


            lblListado =
                new Label();

            lblListado.Text =
                "Usuarios registrados";

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
            // DATAGRIDVIEW
            // --------------------------------------------------------

            dgvUsuarios =
                new DataGridView();

            dgvUsuarios.Location =
                new Point(20, 60);

            dgvUsuarios.Size =
                new Size(585, 390);

            dgvUsuarios.BackgroundColor =
                Color.White;

            dgvUsuarios.BorderStyle =
                BorderStyle.None;


            /*
             * ReadOnly = true
             *
             * Significa que el usuario no puede modificar
             * directamente las celdas de la tabla.
             *
             * Para editar se utilizan los campos
             * del formulario de la izquierda.
             */
            dgvUsuarios.ReadOnly =
                true;


            /*
             * AllowUserToAddRows = false
             *
             * Evita que DataGridView muestre
             * una fila vacía al final.
             */
            dgvUsuarios.AllowUserToAddRows =
                false;


            dgvUsuarios.AllowUserToDeleteRows =
                false;


            /*
             * SelectionMode.FullRowSelect
             *
             * Cuando se hace clic en una celda,
             * se selecciona la fila completa.
             */
            dgvUsuarios.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;


            dgvUsuarios.MultiSelect =
                false;


            dgvUsuarios.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;


            dgvUsuarios.RowHeadersVisible =
                false;


            dgvUsuarios.EnableHeadersVisualStyles =
                false;


            dgvUsuarios.ColumnHeadersDefaultCellStyle.BackColor =
                colorRosa;

            dgvUsuarios.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgvUsuarios.ColumnHeadersDefaultCellStyle.Font =
                new Font(
                    "Segoe UI",
                    9,
                    FontStyle.Bold
                );


            /*
             * AutoGenerateColumns = false
             *
             * Nosotros vamos a indicar explícitamente
             * qué columnas queremos mostrar.
             *
             * De esta manera NO mostramos la contraseña.
             */
            dgvUsuarios.AutoGenerateColumns =
                false;


            dgvUsuarios.Columns.Add(
                CrearColumna(
                    "IdUsuario",
                    "ID"
                )
            );

            dgvUsuarios.Columns.Add(
                CrearColumna(
                    "Nombre",
                    "Nombre"
                )
            );

            dgvUsuarios.Columns.Add(
                CrearColumna(
                    "Apellido",
                    "Apellido"
                )
            );

            dgvUsuarios.Columns.Add(
                CrearColumna(
                    "Correo",
                    "Correo"
                )
            );

            dgvUsuarios.Columns.Add(
                CrearColumna(
                    "Rol",
                    "Rol"
                )
            );


            /*
             * CellClick es un evento.
             *
             * Se produce cuando el usuario hace clic
             * sobre una celda de la grilla.
             */
            dgvUsuarios.CellClick +=
                dgvUsuarios_CellClick;


            pnlListado.Controls.Add(
                lblListado
            );

            pnlListado.Controls.Add(
                dgvUsuarios
            );


            // --------------------------------------------------------
            // AGREGAMOS LOS PANELES AL FORMULARIO
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


            // Redondeamos los paneles.
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
        // MÉTODOS PARA CREAR CONTROLES
        // ============================================================

        /// <summary>
        /// Crea un Label con el estilo visual del sistema.
        /// </summary>
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


        /// <summary>
        /// Crea un TextBox con las medidas
        /// utilizadas en el formulario.
        /// </summary>
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


        /// <summary>
        /// Crea un botón principal de color rosa.
        /// </summary>
        private Button CrearBoton(
            string texto,
            int x,
            int y,
            int ancho)
        {
            Button boton =
                new Button();

            boton.Text =
                texto;

            boton.Location =
                new Point(x, y);

            boton.Size =
                new Size(ancho, 35);

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


        /// <summary>
        /// Crea un botón secundario.
        /// </summary>
        private Button CrearBotonSecundario(
            string texto,
            int x,
            int y,
            int ancho)
        {
            Button boton =
                new Button();

            boton.Text =
                texto;

            boton.Location =
                new Point(x, y);

            boton.Size =
                new Size(ancho, 35);

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


        /// <summary>
        /// Crea una columna para el DataGridView.
        /// </summary>
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
        // CARGA DE ROLES
        // ============================================================

        /// <summary>
        /// Obtiene los roles disponibles
        /// desde CN_Rol.
        /// </summary>
        private void CargarRoles()
        {
            try
            {
                List<Rol> listaRoles =
                    obj_cn_rol.Listar();


                /*
                 * DataSource
                 *
                 * Indica de dónde obtiene sus datos
                 * el ComboBox.
                 */
                cboRol.DataSource =
                    listaRoles;


                /*
                 * DisplayMember
                 *
                 * Es la propiedad que el usuario ve.
                 *
                 * En este caso:
                 * Administrador
                 * Vendedor
                 * Repositor
                 */
                cboRol.DisplayMember =
                    "Descripcion";


                /*
                 * ValueMember
                 *
                 * Es el valor interno asociado
                 * a cada opción.
                 *
                 * En nuestro caso será IdRol.
                 */
                cboRol.ValueMember =
                    "IdRol";


                cboRol.SelectedIndex =
                    -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No fue posible cargar los roles.\n\n" +
                    ex.Message,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        // ============================================================
        // CARGA DE USUARIOS
        // ============================================================

        /// <summary>
        /// Obtiene los usuarios mediante CN_Usuario
        /// y los muestra en la grilla.
        /// </summary>
        private void CargarUsuarios()
        {
            try
            {
                usuariosCargados =
                    obj_cn_usuario.Listar();


                dgvUsuarios.Rows.Clear();


                foreach (Usuario usuario in usuariosCargados)
                {
                    /*
                     * Rows.Add(...)
                     *
                     * Agrega una nueva fila
                     * al DataGridView.
                     */
                    dgvUsuarios.Rows.Add(
                        usuario.IdUsuario,
                        usuario.Nombre,
                        usuario.Apellido,
                        usuario.Correo,
                        usuario.oRol != null
                            ? usuario.oRol.Descripcion
                            : ""
                    );
                }


                dgvUsuarios.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No fue posible cargar los usuarios.\n\n" +
                    ex.Message,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        // ============================================================
        // BOTÓN GUARDAR
        // ============================================================

        /// <summary>
        /// Registra un nuevo usuario.
        /// </summary>
        private void btnGuardar_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                /*
                 * SelectedValue contiene el IdRol
                 * correspondiente al rol seleccionado.
                 */
                if (cboRol.SelectedIndex == -1)
                {
                    MessageBox.Show(
                        "Debe seleccionar un rol.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }


                Usuario nuevoUsuario =
                    new Usuario()
                    {
                        Nombre =
                            txtNombre.Text,

                        Apellido =
                            txtApellido.Text,

                        Correo =
                            txtCorreo.Text,

                        Contraseña =
                            txtContraseña.Text,

                        IdRol =
                            Convert.ToInt32(
                                cboRol.SelectedValue
                            )
                    };


                int nuevoId =
                    obj_cn_usuario.Registrar(
                        nuevoUsuario
                    );


                MessageBox.Show(
                    "Usuario registrado correctamente.\n\n" +
                    "ID asignado: " + nuevoId,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );


                CargarUsuarios();

                LimpiarFormulario();
            }
            catch (ArgumentException ex)
            {
                /*
                 * ArgumentException normalmente representa
                 * una validación de la Capa de Negocio.
                 */
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
                    "Ocurrió un error al registrar el usuario.\n\n" +
                    ex.Message,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        // ============================================================
        // BOTÓN EDITAR
        // ============================================================

        /// <summary>
        /// Modifica el usuario seleccionado.
        /// </summary>
        private void btnEditar_Click(
            object sender,
            EventArgs e)
        {
            if (idUsuarioSeleccionado == 0)
            {
                MessageBox.Show(
                    "Primero seleccione un usuario de la lista.",
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }


            try
            {
                if (cboRol.SelectedIndex == -1)
                {
                    MessageBox.Show(
                        "Debe seleccionar un rol.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }


                Usuario usuarioEditado =
                    new Usuario()
                    {
                        IdUsuario =
                            idUsuarioSeleccionado,

                        Nombre =
                            txtNombre.Text,

                        Apellido =
                            txtApellido.Text,

                        Correo =
                            txtCorreo.Text,

                        Contraseña =
                            txtContraseña.Text,

                        IdRol =
                            Convert.ToInt32(
                                cboRol.SelectedValue
                            )
                    };


                bool editado =
                    obj_cn_usuario.Editar(
                        usuarioEditado
                    );


                if (editado)
                {
                    MessageBox.Show(
                        "Usuario modificado correctamente.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );


                    CargarUsuarios();

                    LimpiarFormulario();
                }
                else
                {
                    MessageBox.Show(
                        "No se encontró el usuario que se intentó modificar.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
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
                    "Ocurrió un error al modificar el usuario.\n\n" +
                    ex.Message,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }


        /// <summary>
        /// Realiza la baja lógica del usuario seleccionado.
        ///
        /// Antes de realizar la operación comprueba
        /// que el administrador no intente darse
        /// de baja a sí mismo.
        /// </summary>
        private void btnEliminar_Click(
            object sender,
            EventArgs e)
        {
            if (idUsuarioSeleccionado == 0)
            {
                MessageBox.Show(
                    "Primero seleccione un usuario de la lista.",
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }


            /*
             * Comprobamos si el usuario seleccionado
             * es el mismo usuario que tiene iniciada
             * la sesión actual.
             *
             * Comparamos los identificadores porque
             * cada usuario tiene un IdUsuario único.
             */
            if (usuarioActual != null &&
                idUsuarioSeleccionado == usuarioActual.IdUsuario)
            {
                MessageBox.Show(
                    "No puede dar de baja su propio usuario mientras tiene una sesión iniciada.",
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }


            DialogResult respuesta =
                MessageBox.Show(
                    "¿Está seguro de dar de baja este usuario?\n\n" +
                    "El usuario perderá el acceso al sistema, " +
                    "pero se conservará su historial de operaciones.",
                    "Confirmar baja de usuario",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );


            if (respuesta != DialogResult.Yes)
            {
                return;
            }


            try
            {
                bool eliminado =
                    obj_cn_usuario.Eliminar(
                        idUsuarioSeleccionado
                    );


                if (eliminado)
                {
                    MessageBox.Show(
                        "Usuario dado de baja correctamente.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    CargarUsuarios();

                    LimpiarFormulario();
                }
                else
                {
                    MessageBox.Show(
                        "No se encontró el usuario seleccionado o ya se encontraba dado de baja.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No fue posible dar de baja el usuario.\n\n" +
                    ex.Message,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }



        // ============================================================
        // SELECCIÓN DE USUARIO
        // ============================================================

        /// <summary>
        /// Se ejecuta cuando se selecciona una fila
        /// de la grilla.
        ///
        /// Recupera los datos del usuario y los coloca
        /// en los campos del formulario.
        /// </summary>
        private void dgvUsuarios_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            /*
             * e.RowIndex contiene el número
             * de la fila seleccionada.
             *
             * Los encabezados tienen índice -1,
             * por eso comprobamos que sea >= 0.
             */
            if (e.RowIndex < 0)
            {
                return;
            }


            DataGridViewRow fila =
                dgvUsuarios.Rows[e.RowIndex];


            idUsuarioSeleccionado =
                Convert.ToInt32(
                    fila.Cells["IdUsuario"].Value
                );


            Usuario usuarioSeleccionado =
                BuscarUsuarioEnMemoria(
                    idUsuarioSeleccionado
                );


            if (usuarioSeleccionado == null)
            {
                return;
            }


            txtNombre.Text =
                usuarioSeleccionado.Nombre;

            txtApellido.Text =
                usuarioSeleccionado.Apellido;

            txtCorreo.Text =
                usuarioSeleccionado.Correo;

            txtContraseña.Text =
                usuarioSeleccionado.Contraseña;


            cboRol.SelectedValue =
                usuarioSeleccionado.IdRol;
        }


        /// <summary>
        /// Busca dentro de usuariosCargados
        /// un usuario según su identificador.
        /// </summary>
        private Usuario BuscarUsuarioEnMemoria(
            int idUsuario)
        {
            /*
             * foreach
             *
             * Permite recorrer uno por uno
             * los objetos de una colección.
             */
            foreach (Usuario usuario in usuariosCargados)
            {
                if (usuario.IdUsuario == idUsuario)
                {
                    return usuario;
                }
            }


            return null;
        }


        // ============================================================
        // MOSTRAR / OCULTAR CONTRASEÑA
        // ============================================================

        private void chkMostrarContraseña_CheckedChanged(
            object sender,
            EventArgs e)
        {
            /*
             * Checked contiene:
             *
             * true  → está marcado
             * false → no está marcado
             *
             * Si está marcado queremos mostrar
             * la contraseña, por eso usamos !.
             */
            txtContraseña.UseSystemPasswordChar =
                !chkMostrarContraseña.Checked;
        }


        // ============================================================
        // BOTÓN LIMPIAR
        // ============================================================

        private void btnLimpiar_Click(
            object sender,
            EventArgs e)
        {
            LimpiarFormulario();
        }


        /// <summary>
        /// Limpia los controles y vuelve
        /// el formulario al estado inicial.
        /// </summary>
        private void LimpiarFormulario()
        {
            idUsuarioSeleccionado =
                0;


            txtNombre.Clear();

            txtApellido.Clear();

            txtCorreo.Clear();

            txtContraseña.Clear();


            cboRol.SelectedIndex =
                -1;


            chkMostrarContraseña.Checked =
                false;


            dgvUsuarios.ClearSelection();


            txtNombre.Focus();
        }


        // ============================================================
        // REDONDEO VISUAL
        // ============================================================

        /// <summary>
        /// Redondea las esquinas de un control.
        ///
        /// Este método se utiliza solamente
        /// para mejorar la apariencia visual.
        /// </summary>
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