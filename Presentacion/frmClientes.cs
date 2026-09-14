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
    /// Formulario destinado a la gestión de clientes.
    ///
    /// Permite:
    /// - Registrar clientes.
    /// - Editar clientes existentes.
    /// - Buscar por DNI o apellido.
    /// - Seleccionar clientes desde una grilla.
    /// - Limpiar los campos del formulario.
    ///
    /// Las validaciones principales pertenecen a CN_Cliente,
    /// es decir, a la capa de Negocio.
    /// </summary>
    public partial class frmClientes : Form
    {
        // =========================================================
        // CAPA DE NEGOCIO
        // =========================================================

        /// <summary>
        /// Objeto de Negocio utilizado para trabajar
        /// con los clientes.
        /// </summary>
        private readonly CN_Cliente cnCliente =
            new CN_Cliente();


        // =========================================================
        // DATOS DEL FORMULARIO
        // =========================================================

        /// <summary>
        /// Lista de clientes cargados desde la base de datos.
        /// </summary>
        private List<Cliente> listaClientes =
            new List<Cliente>();


        /// <summary>
        /// Identificador del cliente seleccionado.
        ///
        /// El valor cero significa que todavía
        /// no se seleccionó ningún cliente.
        /// </summary>
        private int idClienteSeleccionado = 0;


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

        private TextBox txtDni;
        private TextBox txtNombre;
        private TextBox txtApellido;
        private TextBox txtDomicilio;
        private TextBox txtCorreo;
        private TextBox txtBuscar;

        private DateTimePicker dtpFechaNacimiento;

        private Button btnGuardar;
        private Button btnEditar;
        private Button btnLimpiar;
        private Button btnBuscar;

        private DataGridView dgvClientes;


        // =========================================================
        // CONSTRUCTOR
        // =========================================================

        /// <summary>
        /// Constructor del formulario.
        ///
        /// Primero inicializa la estructura básica
        /// creada por Windows Forms.
        ///
        /// Luego configura la ventana, crea los controles
        /// y carga los clientes registrados.
        /// </summary>
        public frmClientes()
        {
            InitializeComponent();

            ConfigurarFormulario();

            CrearInterfaz();

            CargarClientes();
        }


        // =========================================================
        // CONFIGURACIÓN DEL FORMULARIO
        // =========================================================

        /// <summary>
        /// Define las propiedades generales de la ventana.
        /// </summary>
        private void ConfigurarFormulario()
        {
            Text =
                "Aura Beauty - Gestión de Clientes";

            StartPosition =
                FormStartPosition.CenterScreen;

            Width = 1180;

            Height = 720;

            BackColor =
                colorFondo;

            Font =
                new Font("Segoe UI", 10F);

            FormBorderStyle =
                FormBorderStyle.FixedSingle;

            MaximizeBox = false;
        }


        // =========================================================
        // CREACIÓN DE LA INTERFAZ
        // =========================================================

        /// <summary>
        /// Construye visualmente el formulario.
        /// </summary>
        private void CrearInterfaz()
        {
            // -----------------------------------------------------
            // ENCABEZADO
            // -----------------------------------------------------

            Panel panelEncabezado =
                new Panel();

            panelEncabezado.Dock =
                DockStyle.Top;

            panelEncabezado.Height =
                125;

            panelEncabezado.BackColor =
                colorRosa;

            Controls.Add(
                panelEncabezado
            );


            Label lblTitulo =
                new Label();

            lblTitulo.Text =
                "GESTIÓN DE CLIENTES";

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
                new Point(35, 25);

            panelEncabezado.Controls.Add(
                lblTitulo
            );


            Label lblSubtitulo =
                new Label();

            lblSubtitulo.Text =
                "Registro, modificación y consulta de clientes";

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
                new Point(38, 78);

            panelEncabezado.Controls.Add(
                lblSubtitulo
            );


            // -----------------------------------------------------
            // PANEL IZQUIERDO - DATOS
            // -----------------------------------------------------

            Panel panelDatos =
                new Panel();

            panelDatos.Location =
                new Point(25, 150);

            panelDatos.Size =
                new Size(390, 500);

            panelDatos.BackColor =
                Color.White;

            Controls.Add(
                panelDatos
            );


            Label lblDatos =
                new Label();

            lblDatos.Text =
                "Datos del cliente";

            lblDatos.Font =
                new Font(
                    "Segoe UI",
                    16F,
                    FontStyle.Bold
                );

            lblDatos.ForeColor =
                colorTexto;

            lblDatos.AutoSize =
                true;

            lblDatos.Location =
                new Point(25, 20);

            panelDatos.Controls.Add(
                lblDatos
            );


            // DNI
            panelDatos.Controls.Add(
                CrearEtiqueta(
                    "DNI",
                    25,
                    70
                )
            );


            txtDni =
                CrearCajaTexto(
                    25,
                    98,
                    340
                );

            panelDatos.Controls.Add(
                txtDni
            );


            // Nombre
            panelDatos.Controls.Add(
                CrearEtiqueta(
                    "Nombre",
                    25,
                    140
                )
            );


            txtNombre =
                CrearCajaTexto(
                    25,
                    168,
                    160
                );

            panelDatos.Controls.Add(
                txtNombre
            );


            // Apellido
            panelDatos.Controls.Add(
                CrearEtiqueta(
                    "Apellido",
                    205,
                    140
                )
            );


            txtApellido =
                CrearCajaTexto(
                    205,
                    168,
                    160
                );

            panelDatos.Controls.Add(
                txtApellido
            );


            // Fecha nacimiento
            panelDatos.Controls.Add(
                CrearEtiqueta(
                    "Fecha de nacimiento",
                    25,
                    210
                )
            );


            dtpFechaNacimiento =
                new DateTimePicker();

            dtpFechaNacimiento.Location =
                new Point(25, 238);

            dtpFechaNacimiento.Size =
                new Size(340, 30);

            dtpFechaNacimiento.Font =
                new Font(
                    "Segoe UI",
                    10F
                );

            dtpFechaNacimiento.Format =
                DateTimePickerFormat.Short;

            dtpFechaNacimiento.MaxDate =
                DateTime.Today;

            panelDatos.Controls.Add(
                dtpFechaNacimiento
            );


            // Domicilio
            panelDatos.Controls.Add(
                CrearEtiqueta(
                    "Domicilio",
                    25,
                    280
                )
            );


            txtDomicilio =
                CrearCajaTexto(
                    25,
                    308,
                    340
                );

            panelDatos.Controls.Add(
                txtDomicilio
            );


            // Correo
            panelDatos.Controls.Add(
                CrearEtiqueta(
                    "Correo electrónico",
                    25,
                    350
                )
            );


            txtCorreo =
                CrearCajaTexto(
                    25,
                    378,
                    340
                );

            panelDatos.Controls.Add(
                txtCorreo
            );


            // Botones
            btnGuardar =
                CrearBoton(
                    "GUARDAR",
                    25,
                    430,
                    105
                );

            btnGuardar.Click +=
                btnGuardar_Click;

            panelDatos.Controls.Add(
                btnGuardar
            );


            btnEditar =
                CrearBoton(
                    "EDITAR",
                    142,
                    430,
                    105
                );

            btnEditar.Click +=
                btnEditar_Click;

            panelDatos.Controls.Add(
                btnEditar
            );


            btnLimpiar =
                CrearBotonSecundario(
                    "LIMPIAR",
                    260,
                    430,
                    105
                );

            btnLimpiar.Click +=
                btnLimpiar_Click;

            panelDatos.Controls.Add(
                btnLimpiar
            );


            // -----------------------------------------------------
            // PANEL DERECHO - LISTADO
            // -----------------------------------------------------

            Panel panelListado =
                new Panel();

            panelListado.Location =
                new Point(440, 150);

            panelListado.Size =
                new Size(690, 500);

            panelListado.BackColor =
                Color.White;

            Controls.Add(
                panelListado
            );


            Label lblListado =
                new Label();

            lblListado.Text =
                "Clientes registrados";

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

            panelListado.Controls.Add(
                CrearEtiqueta(
                    "Buscar por DNI o apellido",
                    25,
                    70
                )
            );


            txtBuscar =
                CrearCajaTexto(
                    25,
                    98,
                    460
                );

            panelListado.Controls.Add(
                txtBuscar
            );


            btnBuscar =
                CrearBoton(
                    "BUSCAR",
                    505,
                    95,
                    155
                );

            btnBuscar.Height =
                35;

            btnBuscar.Click +=
                btnBuscar_Click;

            panelListado.Controls.Add(
                btnBuscar
            );


            // -----------------------------------------------------
            // GRILLA
            // -----------------------------------------------------

            dgvClientes =
                new DataGridView();

            dgvClientes.Location =
                new Point(25, 145);

            dgvClientes.Size =
                new Size(635, 325);

            dgvClientes.AllowUserToAddRows =
                false;

            dgvClientes.AllowUserToDeleteRows =
                false;

            dgvClientes.ReadOnly =
                true;

            dgvClientes.MultiSelect =
                false;

            dgvClientes.SelectionMode =
                DataGridViewSelectionMode.FullRowSelect;

            dgvClientes.AutoGenerateColumns =
                false;

            dgvClientes.BackgroundColor =
                Color.White;

            dgvClientes.BorderStyle =
                BorderStyle.FixedSingle;

            dgvClientes.RowHeadersVisible =
                false;

            dgvClientes.AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill;

            dgvClientes.CellClick +=
                dgvClientes_CellClick;


            dgvClientes.Columns.Add(
                CrearColumna(
                    "IdCliente",
                    "ID",
                    "IdCliente"
                )
            );


            dgvClientes.Columns.Add(
                CrearColumna(
                    "Dni",
                    "DNI",
                    "Dni"
                )
            );


            dgvClientes.Columns.Add(
                CrearColumna(
                    "Nombre",
                    "Nombre",
                    "Nombre"
                )
            );


            dgvClientes.Columns.Add(
                CrearColumna(
                    "Apellido",
                    "Apellido",
                    "Apellido"
                )
            );


            dgvClientes.Columns.Add(
                CrearColumna(
                    "Correo",
                    "Correo",
                    "Correo"
                )
            );


            panelListado.Controls.Add(
                dgvClientes
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
                new Size(ancho, 40);

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
        // CARGAR CLIENTES
        // =========================================================

        /// <summary>
        /// Recupera todos los clientes desde la capa
        /// de Negocio y los muestra en la grilla.
        /// </summary>
        private void CargarClientes()
        {
            try
            {
                listaClientes =
                    cnCliente.Listar();

                MostrarClientes(
                    listaClientes
                );
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


        // =========================================================
        // MOSTRAR CLIENTES
        // =========================================================

        private void MostrarClientes(
            List<Cliente> clientes
        )
        {
            dgvClientes.DataSource =
                null;

            dgvClientes.DataSource =
                clientes;
        }


        // =========================================================
        // CREAR OBJETO CLIENTE DESDE EL FORMULARIO
        // =========================================================

        /// <summary>
        /// Toma los valores escritos en los controles
        /// y construye una entidad Cliente.
        /// </summary>
        private Cliente ObtenerClienteFormulario()
        {
            if (
                !int.TryParse(
                    txtDni.Text.Trim(),
                    out int dni
                )
            )
            {
                throw new Exception(
                    "Ingresá un DNI válido."
                );
            }


            Cliente cliente =
                new Cliente();


            cliente.IdCliente =
                idClienteSeleccionado;


            cliente.Dni =
                dni;


            cliente.Nombre =
                txtNombre.Text;


            cliente.Apellido =
                txtApellido.Text;


            cliente.FechaNacimiento =
                dtpFechaNacimiento.Value.Date;


            cliente.Domicilio =
                txtDomicilio.Text;


            cliente.Correo =
                txtCorreo.Text;


            return cliente;
        }


        // =========================================================
        // GUARDAR
        // =========================================================

        private void btnGuardar_Click(
            object sender,
            EventArgs e
        )
        {
            try
            {
                Cliente cliente =
                    ObtenerClienteFormulario();


                int idGenerado =
                    cnCliente.Registrar(
                        cliente
                    );


                MessageBox.Show(
                    "Cliente registrado correctamente.\n\n"
                    + "ID generado: "
                    + idGenerado,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );


                CargarClientes();

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


        // =========================================================
        // EDITAR
        // =========================================================

        private void btnEditar_Click(
            object sender,
            EventArgs e
        )
        {
            try
            {
                if (
                    idClienteSeleccionado <= 0
                )
                {
                    MessageBox.Show(
                        "Seleccioná un cliente para editar.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }


                Cliente cliente =
                    ObtenerClienteFormulario();


                bool resultado =
                    cnCliente.Editar(
                        cliente
                    );


                if (!resultado)
                {
                    MessageBox.Show(
                        "No fue posible modificar el cliente.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    return;
                }


                MessageBox.Show(
                    "Cliente modificado correctamente.",
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );


                CargarClientes();

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
                List<Cliente> resultado =
                    cnCliente.Buscar(
                        txtBuscar.Text
                    );


                MostrarClientes(
                    resultado
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
        // SELECCIONAR CLIENTE
        // =========================================================

        /// <summary>
        /// Cuando el usuario hace clic sobre una fila,
        /// recuperamos el cliente completo y cargamos
        /// sus datos en los controles.
        /// </summary>
        private void dgvClientes_CellClick(
            object sender,
            DataGridViewCellEventArgs e
        )
        {
            if (e.RowIndex < 0)
                return;


            object valorId =
                dgvClientes.Rows[e.RowIndex]
                    .Cells["IdCliente"]
                    .Value;


            if (valorId == null)
                return;


            int idCliente =
                Convert.ToInt32(
                    valorId
                );


            Cliente cliente =
                listaClientes.FirstOrDefault(
                    c =>
                        c.IdCliente ==
                        idCliente
                );


            if (cliente == null)
                return;


            idClienteSeleccionado =
                cliente.IdCliente;


            txtDni.Text =
                cliente.Dni.ToString();


            txtNombre.Text =
                cliente.Nombre;


            txtApellido.Text =
                cliente.Apellido;


            txtDomicilio.Text =
                cliente.Domicilio;


            txtCorreo.Text =
                cliente.Correo;


            if (
                cliente.FechaNacimiento.HasValue
            )
            {
                dtpFechaNacimiento.Value =
                    cliente.FechaNacimiento.Value;
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
            LimpiarFormulario();
        }


        /// <summary>
        /// Restablece todos los controles
        /// para cargar un nuevo cliente.
        /// </summary>
        private void LimpiarFormulario()
        {
            idClienteSeleccionado =
                0;


            txtDni.Clear();

            txtNombre.Clear();

            txtApellido.Clear();

            txtDomicilio.Clear();

            txtCorreo.Clear();

            txtBuscar.Clear();


            dtpFechaNacimiento.Value =
                DateTime.Today;


            dgvClientes.ClearSelection();


            txtDni.Focus();
        }
    }
}
