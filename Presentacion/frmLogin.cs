using Entidades;
using Negocio;
using Presentacion;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Aura_Beauty
{
    /// <summary>
    /// Formulario de inicio de sesión del sistema Aura Beauty.
    ///
    /// Esta clase pertenece a la Capa de Presentación.
    ///
    /// Sus responsabilidades son:
    /// - Mostrar los controles visuales del login.
    /// - Capturar el correo y la contraseña ingresados.
    /// - Realizar validaciones simples de interfaz.
    /// - Enviar los datos a la Capa de Negocio.
    /// - Mostrar mensajes al usuario.
    /// - Abrir el menú principal cuando el acceso es correcto.
    ///
    /// IMPORTANTE:
    /// Este formulario NO realiza consultas SQL directamente.
    /// Para validar las credenciales se comunica con CN_Usuario,
    /// que pertenece a la Capa de Negocio.
    /// </summary>
    public partial class frmLogin : Form
    {
        // ---------------------------------------------------------
        // CONTROLES VISUALES DEL LOGIN
        // ---------------------------------------------------------

        private Panel pnlMarca;
        private Panel pnlLogin;

        private Label lblMarca;
        private Label lblEslogan;

        private Label lblTitulo;
        private Label lblSubtitulo;
        private Label lblCorreo;
        private Label lblContraseña;

        private TextBox txtCorreo;
        private TextBox txtContraseña;

        private CheckBox chkMostrarContraseña;

        private Button btnIngresar;

        private Label lblPie;


        // ---------------------------------------------------------
        // OBJETO DE LA CAPA DE NEGOCIO
        // ---------------------------------------------------------

        /*
         * Creamos un objeto CN_Usuario para poder utilizar
         * las funciones relacionadas con usuarios.
         *
         * Este formulario NO utiliza CD_Usuario porque
         * Presentación no debe comunicarse directamente
         * con la Capa de Datos.
         */
        private CN_Usuario obj_cn_usuario = new CN_Usuario();


        // ---------------------------------------------------------
        // CONSTRUCTOR
        // ---------------------------------------------------------

        /// <summary>
        /// Constructor del formulario.
        ///
        /// Se ejecuta automáticamente cuando se crea
        /// una instancia de frmLogin.
        /// </summary>
        public frmLogin()
        {
            // Tamaño de la ventana.
            this.ClientSize = new Size(900, 560);

            // Texto de la barra superior.
            this.Text = "Aura Beauty - Iniciar Sesión";

            // Nombre interno del formulario.
            this.Name = "frmLogin";

            /*
             * FixedSingle evita que el usuario cambie
             * manualmente el tamaño de la ventana.
             */
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // No permitimos maximizar.
            this.MaximizeBox = false;

            // La ventana aparece centrada.
            this.StartPosition = FormStartPosition.CenterScreen;

            // Color general de respaldo.
            this.BackColor = Color.FromArgb(255, 249, 248);

            // Creamos la interfaz.
            CrearControlesManualmente();
        }


        // ---------------------------------------------------------
        // CREACIÓN DE LA INTERFAZ
        // ---------------------------------------------------------

        /// <summary>
        /// Crea y configura todos los controles visuales del login.
        ///
        /// Este método se ocupa solamente de la interfaz:
        /// colores, tamaños, textos, posiciones y eventos.
        ///
        /// No contiene consultas a la base de datos.
        /// </summary>
        private void CrearControlesManualmente()
        {
            // ---------------------------------------------------------
            // PALETA DE COLORES DE AURA BEAUTY
            // ---------------------------------------------------------

            /*
             * Color.FromArgb(R, G, B)
             *
             * Es un método de .NET que permite construir un color
             * utilizando valores de rojo, verde y azul.
             */

            Color rosaNude = Color.FromArgb(201, 143, 149);

            Color rosaOscuro = Color.FromArgb(174, 112, 120);

            Color fondoClaro = Color.FromArgb(255, 249, 248);

            Color textoPrincipal = Color.FromArgb(94, 74, 74);

            Color textoSecundario = Color.FromArgb(138, 116, 116);


            // =========================================================
            // PANEL IZQUIERDO
            // =========================================================

            pnlMarca = new Panel();

            pnlMarca.Name = "pnlMarca";

            pnlMarca.Size = new Size(360, 560);

            pnlMarca.Location = new Point(0, 0);

            pnlMarca.BackColor = rosaNude;

            this.Controls.Add(pnlMarca);


            // ---------------------------------------------------------
            // NOMBRE DE LA MARCA
            // ---------------------------------------------------------

            lblMarca = new Label();

            lblMarca.Text = "AURA\nBEAUTY";

            lblMarca.Font =
                new Font(
                    "Segoe UI",
                    31F,
                    FontStyle.Bold
                );

            lblMarca.ForeColor = Color.White;

            lblMarca.BackColor = Color.Transparent;

            lblMarca.AutoSize = true;

            lblMarca.Location = new Point(70, 155);

            pnlMarca.Controls.Add(lblMarca);


            // ---------------------------------------------------------
            // ESLOGAN
            // ---------------------------------------------------------

            lblEslogan = new Label();

            lblEslogan.Text =
                "Belleza y gestión\n"
                + "en un solo lugar.";

            lblEslogan.Font =
                new Font(
                    "Segoe UI",
                    12F,
                    FontStyle.Regular
                );

            lblEslogan.ForeColor =
                Color.FromArgb(255, 244, 244);

            lblEslogan.BackColor = Color.Transparent;

            lblEslogan.AutoSize = true;

            lblEslogan.Location =
                new Point(73, 270);

            pnlMarca.Controls.Add(lblEslogan);


            // =========================================================
            // PANEL DERECHO
            // =========================================================

            pnlLogin = new Panel();

            pnlLogin.Name = "pnlLogin";

            pnlLogin.Size = new Size(540, 560);

            pnlLogin.Location = new Point(360, 0);

            pnlLogin.BackColor = fondoClaro;

            this.Controls.Add(pnlLogin);


            // ---------------------------------------------------------
            // TÍTULO
            // ---------------------------------------------------------

            lblTitulo = new Label();

            lblTitulo.Text = "Iniciar sesión";

            lblTitulo.Font =
                new Font(
                    "Segoe UI",
                    24F,
                    FontStyle.Bold
                );

            lblTitulo.ForeColor = textoPrincipal;

            lblTitulo.AutoSize = true;

            lblTitulo.Location =
                new Point(100, 65);

            pnlLogin.Controls.Add(lblTitulo);


            // ---------------------------------------------------------
            // SUBTÍTULO
            // ---------------------------------------------------------

            lblSubtitulo = new Label();

            lblSubtitulo.Text =
                "Ingresá tus datos para acceder al sistema.";

            lblSubtitulo.Font =
                new Font(
                    "Segoe UI",
                    10.5F,
                    FontStyle.Regular
                );

            lblSubtitulo.ForeColor = textoSecundario;

            lblSubtitulo.AutoSize = true;

            lblSubtitulo.Location =
                new Point(104, 115);

            pnlLogin.Controls.Add(lblSubtitulo);


            // =========================================================
            // CORREO
            // =========================================================

            lblCorreo = new Label();

            lblCorreo.Text = "Correo electrónico";

            lblCorreo.Font =
                new Font(
                    "Segoe UI",
                    10.5F,
                    FontStyle.Bold
                );

            lblCorreo.ForeColor = textoPrincipal;

            lblCorreo.AutoSize = true;

            lblCorreo.Location =
                new Point(105, 180);

            pnlLogin.Controls.Add(lblCorreo);


            txtCorreo = new TextBox();

            txtCorreo.Name = "txtCorreo";

            txtCorreo.Font =
                new Font(
                    "Segoe UI",
                    12F,
                    FontStyle.Regular
                );

            txtCorreo.BorderStyle =
                BorderStyle.FixedSingle;

            txtCorreo.Size =
                new Size(330, 32);

            txtCorreo.Location =
                new Point(108, 210);

            pnlLogin.Controls.Add(txtCorreo);


            // =========================================================
            // CONTRASEÑA
            // =========================================================

            lblContraseña = new Label();

            lblContraseña.Text = "Contraseña";

            lblContraseña.Font =
                new Font(
                    "Segoe UI",
                    10.5F,
                    FontStyle.Bold
                );

            lblContraseña.ForeColor = textoPrincipal;

            lblContraseña.AutoSize = true;

            lblContraseña.Location =
                new Point(105, 275);

            pnlLogin.Controls.Add(lblContraseña);


            txtContraseña = new TextBox();

            txtContraseña.Name = "txtContraseña";

            txtContraseña.Font =
                new Font(
                    "Segoe UI",
                    12F,
                    FontStyle.Regular
                );

            txtContraseña.BorderStyle =
                BorderStyle.FixedSingle;

            txtContraseña.Size =
                new Size(330, 32);

            txtContraseña.Location =
                new Point(108, 305);

            /*
             * Oculta visualmente los caracteres
             * ingresados en la contraseña.
             */
            txtContraseña.UseSystemPasswordChar = true;

            pnlLogin.Controls.Add(txtContraseña);


            // =========================================================
            // MOSTRAR CONTRASEÑA
            // =========================================================

            chkMostrarContraseña =
                new CheckBox();

            chkMostrarContraseña.Name =
                "chkMostrarContraseña";

            chkMostrarContraseña.Text =
                "Mostrar contraseña";

            chkMostrarContraseña.Font =
                new Font(
                    "Segoe UI",
                    9.5F,
                    FontStyle.Regular
                );

            chkMostrarContraseña.ForeColor =
                textoSecundario;

            chkMostrarContraseña.AutoSize = true;

            chkMostrarContraseña.Location =
                new Point(108, 348);

            /*
             * Asociamos el evento CheckedChanged
             * con nuestro método correspondiente.
             */
            chkMostrarContraseña.CheckedChanged +=
                chkMostrarContraseña_CheckedChanged;

            pnlLogin.Controls.Add(
                chkMostrarContraseña
            );


            // =========================================================
            // BOTÓN INGRESAR
            // =========================================================

            btnIngresar = new Button();

            btnIngresar.Name = "btnIngresar";

            btnIngresar.Text = "INGRESAR";

            btnIngresar.Font =
                new Font(
                    "Segoe UI",
                    11F,
                    FontStyle.Bold
                );

            btnIngresar.ForeColor = Color.White;

            btnIngresar.BackColor = rosaNude;

            btnIngresar.FlatStyle =
                FlatStyle.Flat;

            btnIngresar.FlatAppearance.BorderSize = 0;

            btnIngresar.Size =
                new Size(330, 48);

            btnIngresar.Location =
                new Point(108, 405);

            btnIngresar.Cursor =
                Cursors.Hand;

            /*
             * Al hacer clic se ejecuta la lógica
             * de inicio de sesión.
             */
            btnIngresar.Click +=
                btnIngresar_Click;


            // ---------------------------------------------------------
            // EFECTO AL PASAR EL MOUSE
            // ---------------------------------------------------------

            btnIngresar.MouseEnter +=
                (sender, e) =>
                {
                    btnIngresar.BackColor =
                        rosaOscuro;
                };


            btnIngresar.MouseLeave +=
                (sender, e) =>
                {
                    btnIngresar.BackColor =
                        rosaNude;
                };


            pnlLogin.Controls.Add(btnIngresar);


            // ---------------------------------------------------------
            // BORDES REDONDEADOS DEL BOTÓN
            // ---------------------------------------------------------

            RedondearControl(
                btnIngresar,
                18
            );


            // =========================================================
            // TEXTO INFERIOR
            // =========================================================

            lblPie = new Label();

            lblPie.Text =
                "Aura Beauty · Sistema de gestión comercial";

            lblPie.Font =
                new Font(
                    "Segoe UI",
                    8.5F,
                    FontStyle.Regular
                );

            lblPie.ForeColor =
                Color.FromArgb(170, 150, 150);

            lblPie.AutoSize = true;

            lblPie.Location =
                new Point(145, 510);

            pnlLogin.Controls.Add(lblPie);


            // =========================================================
            // BOTÓN PREDETERMINADO
            // =========================================================

            /*
             * Permite ejecutar btnIngresar
             * al presionar ENTER.
             */
            this.AcceptButton = btnIngresar;
        }


        // ---------------------------------------------------------
        // MOSTRAR / OCULTAR CONTRASEÑA
        // ---------------------------------------------------------

        /// <summary>
        /// Muestra u oculta visualmente la contraseña
        /// cuando se marca o desmarca el CheckBox.
        /// </summary>
        private void chkMostrarContraseña_CheckedChanged(
            object sender,
            EventArgs e
        )
        {
            if (chkMostrarContraseña.Checked)
            {
                txtContraseña.UseSystemPasswordChar = false;
            }
            else
            {
                txtContraseña.UseSystemPasswordChar = true;
            }
        }


        // ---------------------------------------------------------
        // MÉTODO PARA REDONDEAR CONTROLES
        // ---------------------------------------------------------

        /// <summary>
        /// Modifica la región visible de un control
        /// para darle bordes redondeados.
        /// </summary>
        /// <param name="control">
        /// Control que queremos redondear.
        /// </param>
        /// <param name="radio">
        /// Nivel de redondeo de las esquinas.
        /// </param>
        private void RedondearControl(
            Control control,
            int radio
        )
        {
            /*
             * GraphicsPath representa una figura geométrica
             * formada por líneas y curvas.
             */
            GraphicsPath ruta =
                new GraphicsPath();


            /*
             * Rectangle representa el área total
             * del control recibido.
             */
            Rectangle rectangulo =
                new Rectangle(
                    0,
                    0,
                    control.Width,
                    control.Height
                );


            /*
             * AddArc agrega los arcos de las cuatro esquinas.
             */

            ruta.AddArc(
                rectangulo.X,
                rectangulo.Y,
                radio,
                radio,
                180,
                90
            );

            ruta.AddArc(
                rectangulo.Right - radio,
                rectangulo.Y,
                radio,
                radio,
                270,
                90
            );

            ruta.AddArc(
                rectangulo.Right - radio,
                rectangulo.Bottom - radio,
                radio,
                radio,
                0,
                90
            );

            ruta.AddArc(
                rectangulo.X,
                rectangulo.Bottom - radio,
                radio,
                radio,
                90,
                90
            );


            // Cierra la figura.
            ruta.CloseFigure();


            /*
             * Region determina el área visible del control.
             *
             * Al asignarle la figura construida,
             * el botón toma una forma redondeada.
             */
            control.Region =
                new Region(ruta);
        }


        // ---------------------------------------------------------
        // EVENTO DEL BOTÓN INGRESAR
        // ---------------------------------------------------------

        /// <summary>
        /// Se ejecuta cuando el usuario presiona INGRESAR
        /// o la tecla ENTER.
        ///
        /// 1. Obtiene los datos escritos.
        /// 2. Realiza validaciones básicas de presentación.
        /// 3. Envía las credenciales a CN_Usuario.
        /// 4. Analiza el resultado.
        /// 5. Abre frmMenu si corresponde.
        /// </summary>
        private void btnIngresar_Click(
            object sender,
            EventArgs e
        )
        {
            /*
             * Trim() elimina espacios al principio
             * y al final del correo.
             */
            string correo =
                txtCorreo.Text.Trim();


            /*
             * En la contraseña NO usamos Trim()
             * porque los espacios podrían formar parte
             * de una contraseña válida.
             */
            string contraseña =
                txtContraseña.Text;


            // -----------------------------------------------------
            // VALIDACIONES DE PRESENTACIÓN
            // -----------------------------------------------------

            if (string.IsNullOrWhiteSpace(correo))
            {
                MessageBox.Show(
                    "Por favor, ingresá tu correo electrónico.",
                    "Campo obligatorio",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtCorreo.Focus();

                return;
            }


            if (string.IsNullOrWhiteSpace(contraseña))
            {
                MessageBox.Show(
                    "Por favor, ingresá tu contraseña.",
                    "Campo obligatorio",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtContraseña.Focus();

                return;
            }


            // -----------------------------------------------------
            // VALIDACIÓN MEDIANTE LA CAPA DE NEGOCIO
            // -----------------------------------------------------

            try
            {
                /*
                 * Enviamos correo y contraseña
                 * a la Capa de Negocio.
                 */
                Usuario empleadoActual =
                    obj_cn_usuario.ValidarLogin(
                        correo,
                        contraseña
                    );


                if (empleadoActual != null)
                {
                    MessageBox.Show(
                        "¡Bienvenido/a "
                        + empleadoActual.Nombre
                        + " "
                        + empleadoActual.Apellido
                        + "!",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );


                    /*
                     * Creamos el menú principal
                     * y le enviamos el usuario autenticado.
                     */
                    frmMenu formMenu =
                        new frmMenu(empleadoActual);


                    // Mostramos el menú.
                    formMenu.Show();


                    // Ocultamos el login.
                    this.Hide();
                }
                else
                {
                    MessageBox.Show(
                        "Acceso denegado. Verificá tu correo o contraseña.",
                        "Error de inicio de sesión",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );


                    // Borramos solamente la contraseña.
                    txtContraseña.Clear();


                    // Dejamos el cursor allí.
                    txtContraseña.Focus();
                }
            }


            // -----------------------------------------------------
            // ERRORES DE VALIDACIÓN DE NEGOCIO
            // -----------------------------------------------------

            catch (ArgumentException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Datos no válidos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }


            // -----------------------------------------------------
            // OTROS ERRORES
            // -----------------------------------------------------

            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ocurrió un problema al intentar iniciar sesión."
                    + Environment.NewLine
                    + Environment.NewLine
                    + "Detalle: "
                    + ex.Message,
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
    }
}