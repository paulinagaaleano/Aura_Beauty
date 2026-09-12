using Entidades;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Aura_Beauty
{
    /// <summary>
    /// Formulario principal del sistema Aura Beauty.
    ///
    /// Esta clase pertenece a la Capa de Presentación.
    ///
    /// Su función es:
    /// - Mostrar los módulos disponibles.
    /// - Identificar al usuario que inició sesión.
    /// - Mostrar su nombre y rol.
    /// - Habilitar únicamente las opciones correspondientes
    ///   según el rol del usuario.
    /// - Permitir cerrar la sesión.
    ///
    /// Este formulario NO realiza consultas SQL.
    /// </summary>
    public partial class frmMenu : Form
    {
        // ---------------------------------------------------------
        // CONSTANTES DE ROLES
        // ---------------------------------------------------------

        /*
         * Una constante es un valor que no cambia
         * durante la ejecución del programa.
         *
         * Estos valores coinciden con la tabla Rol
         * de nuestra base de datos.
         */

        private const int ROL_ADMINISTRADOR = 1;
        private const int ROL_VENDEDOR = 2;
        private const int ROL_REPOSITOR = 3;


        // ---------------------------------------------------------
        // USUARIO ACTUAL
        // ---------------------------------------------------------

        /*
         * Guarda al usuario que logró iniciar sesión.
         *
         * Gracias a este objeto podemos conocer:
         * - nombre,
         * - apellido,
         * - correo,
         * - rol.
         */
        private Usuario usuarioActual;


        // ---------------------------------------------------------
        // CONTROL DEL CIERRE
        // ---------------------------------------------------------

        /*
         * Esta variable nos permite distinguir
         * entre:
         *
         * - cerrar sesión,
         * - cerrar completamente la aplicación.
         */
        private bool cerrandoSesion = false;


        // ---------------------------------------------------------
        // CONTROLES VISUALES
        // ---------------------------------------------------------

        private Panel pnlSuperior;
        private Panel pnlContenido;

        private Label lblMarca;
        private Label lblBienvenida;
        private Label lblRol;
        private Label lblTituloModulos;

        private Button btnCerrarSesion;


        // ---------------------------------------------------------
        // COLORES DEL SISTEMA
        // ---------------------------------------------------------

        private Color rosaNude =
            Color.FromArgb(201, 143, 149);

        private Color rosaOscuro =
            Color.FromArgb(174, 112, 120);

        private Color fondoClaro =
            Color.FromArgb(255, 249, 248);

        private Color textoPrincipal =
            Color.FromArgb(94, 74, 74);

        private Color textoSecundario =
            Color.FromArgb(138, 116, 116);


        // ---------------------------------------------------------
        // CONSTRUCTOR
        // ---------------------------------------------------------

        /// <summary>
        /// Constructor del menú.
        ///
        /// Recibe como parámetro al usuario que inició sesión.
        /// </summary>
        /// <param name="usuario">
        /// Usuario autenticado proveniente del login.
        /// </param>
        public frmMenu(Usuario usuario)
        {
            /*
             * Guardamos el usuario recibido.
             */
            usuarioActual = usuario;


            /*
             * Configuramos la ventana.
             */
            ConfigurarFormulario();


            /*
             * Creamos la interfaz.
             */
            CrearInterfaz();


            /*
             * Creamos los botones según
             * el rol del usuario.
             */
            CargarModulosSegunRol();
        }


        // ---------------------------------------------------------
        // CONFIGURACIÓN DEL FORMULARIO
        // ---------------------------------------------------------

        /// <summary>
        /// Configura las características generales
        /// de la ventana principal.
        /// </summary>
        private void ConfigurarFormulario()
        {
            this.Text =
                "Aura Beauty - Sistema de Gestión";

            this.ClientSize =
                new Size(950, 620);

            this.StartPosition =
                FormStartPosition.CenterScreen;

            this.BackColor =
                fondoClaro;

            this.FormBorderStyle =
                FormBorderStyle.FixedSingle;

            this.MaximizeBox = false;

            this.Name = "frmMenu";
        }


        // ---------------------------------------------------------
        // CREACIÓN DE LA INTERFAZ
        // ---------------------------------------------------------

        /// <summary>
        /// Construye visualmente el menú principal.
        /// </summary>
        private void CrearInterfaz()
        {
            // =====================================================
            // PANEL SUPERIOR
            // =====================================================

            pnlSuperior = new Panel();

            pnlSuperior.Name = "pnlSuperior";

            pnlSuperior.Size =
                new Size(950, 120);

            pnlSuperior.Location =
                new Point(0, 0);

            pnlSuperior.BackColor =
                rosaNude;

            this.Controls.Add(pnlSuperior);


            // -----------------------------------------------------
            // MARCA
            // -----------------------------------------------------

            lblMarca = new Label();

            lblMarca.Text =
                "AURA BEAUTY";

            lblMarca.Font =
                new Font(
                    "Segoe UI",
                    23F,
                    FontStyle.Bold
                );

            lblMarca.ForeColor =
                Color.White;

            lblMarca.AutoSize = true;

            lblMarca.Location =
                new Point(35, 22);

            pnlSuperior.Controls.Add(
                lblMarca
            );


            // -----------------------------------------------------
            // NOMBRE DEL USUARIO
            // -----------------------------------------------------

            string nombreMostrar =
                usuarioActual.Nombre
                + " "
                + usuarioActual.Apellido;


            lblBienvenida = new Label();

            lblBienvenida.Text =
                "Bienvenido/a, "
                + nombreMostrar;

            lblBienvenida.Font =
                new Font(
                    "Segoe UI",
                    10.5F,
                    FontStyle.Regular
                );

            lblBienvenida.ForeColor =
                Color.FromArgb(
                    255,
                    242,
                    242
                );

            lblBienvenida.AutoSize = true;

            lblBienvenida.Location =
                new Point(38, 67);

            pnlSuperior.Controls.Add(
                lblBienvenida
            );


            // -----------------------------------------------------
            // ROL
            // -----------------------------------------------------

            /*
             * El operador ?. verifica que oRol
             * exista antes de intentar leer
             * su propiedad Descripcion.
             *
             * ?? permite indicar un valor alternativo
             * si el resultado fuera null.
             */

            string rolMostrar =
                usuarioActual.oRol?.Descripcion
                ?? "Rol no identificado";


            lblRol = new Label();

            lblRol.Text =
                "Rol: " + rolMostrar;

            lblRol.Font =
                new Font(
                    "Segoe UI",
                    9.5F,
                    FontStyle.Italic
                );

            lblRol.ForeColor =
                Color.White;

            lblRol.AutoSize = true;

            lblRol.Location =
                new Point(38, 91);

            pnlSuperior.Controls.Add(
                lblRol
            );


            // -----------------------------------------------------
            // BOTÓN CERRAR SESIÓN
            // -----------------------------------------------------

            btnCerrarSesion =
                new Button();

            btnCerrarSesion.Name =
                "btnCerrarSesion";

            btnCerrarSesion.Text =
                "Cerrar sesión";

            btnCerrarSesion.Font =
                new Font(
                    "Segoe UI",
                    10F,
                    FontStyle.Bold
                );

            btnCerrarSesion.ForeColor =
                textoPrincipal;

            btnCerrarSesion.BackColor =
                Color.White;

            btnCerrarSesion.FlatStyle =
                FlatStyle.Flat;

            btnCerrarSesion.FlatAppearance.BorderSize =
                0;

            btnCerrarSesion.Cursor =
                Cursors.Hand;

            btnCerrarSesion.Size =
                new Size(145, 40);

            btnCerrarSesion.Location =
                new Point(760, 38);

            btnCerrarSesion.Click +=
                btnCerrarSesion_Click;

            pnlSuperior.Controls.Add(
                btnCerrarSesion
            );

            RedondearControl(
                btnCerrarSesion,
                16
            );


            // =====================================================
            // PANEL DE CONTENIDO
            // =====================================================

            pnlContenido =
                new Panel();

            pnlContenido.Name =
                "pnlContenido";

            pnlContenido.Size =
                new Size(950, 500);

            pnlContenido.Location =
                new Point(0, 120);

            pnlContenido.BackColor =
                fondoClaro;

            this.Controls.Add(
                pnlContenido
            );


            // -----------------------------------------------------
            // TÍTULO DE LOS MÓDULOS
            // -----------------------------------------------------

            lblTituloModulos =
                new Label();

            lblTituloModulos.Text =
                "Panel principal";

            lblTituloModulos.Font =
                new Font(
                    "Segoe UI",
                    21F,
                    FontStyle.Bold
                );

            lblTituloModulos.ForeColor =
                textoPrincipal;

            lblTituloModulos.AutoSize =
                true;

            lblTituloModulos.Location =
                new Point(60, 35);

            pnlContenido.Controls.Add(
                lblTituloModulos
            );
        }


        // ---------------------------------------------------------
        // CARGAR MÓDULOS SEGÚN ROL
        // ---------------------------------------------------------

        /// <summary>
        /// Determina qué botones debe visualizar
        /// el usuario según su rol.
        ///
        /// Administrador:
        /// tiene acceso a todos los módulos.
        ///
        /// Vendedor:
        /// puede consultar productos,
        /// registrar ventas y gestionar clientes.
        ///
        /// Repositor:
        /// puede trabajar con productos y stock.
        /// </summary>
        private void CargarModulosSegunRol()
        {
            /*
             * Verificamos que el usuario
             * realmente exista.
             */
            if (usuarioActual == null)
            {
                MessageBox.Show(
                    "No se pudo identificar al usuario.",
                    "Aura Beauty",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
            }


            /*
             * switch permite ejecutar un bloque
             * diferente según un valor.
             *
             * En este caso analizamos IdRol.
             */
            switch (usuarioActual.IdRol)
            {
                // =================================================
                // ADMINISTRADOR
                // =================================================

                case ROL_ADMINISTRADOR:

                    CrearBotonModulo(
                        "Gestión de Usuarios y Roles",
                        1,
                        60,
                        110
                    );

                    CrearBotonModulo(
                        "Catálogo de Productos",
                        2,
                        480,
                        110
                    );

                    CrearBotonModulo(
                        "Control de Stock",
                        3,
                        60,
                        210
                    );

                    CrearBotonModulo(
                        "Registro de Ventas",
                        4,
                        480,
                        210
                    );

                    CrearBotonModulo(
                        "Gestión de Clientes",
                        5,
                        60,
                        310
                    );

                    CrearBotonModulo(
                        "Reportes y Estadísticas",
                        6,
                        480,
                        310
                    );

                    break;


                // =================================================
                // VENDEDOR
                // =================================================

                case ROL_VENDEDOR:

                    CrearBotonModulo(
                        "Consulta de Productos",
                        2,
                        60,
                        110
                    );

                    CrearBotonModulo(
                        "Registro de Ventas",
                        4,
                        480,
                        110
                    );

                    CrearBotonModulo(
                        "Gestión de Clientes",
                        5,
                        60,
                        210
                    );

                    break;


                // =================================================
                // REPOSITOR
                // =================================================

                case ROL_REPOSITOR:

                    CrearBotonModulo(
                        "Catálogo de Productos",
                        2,
                        60,
                        110
                    );

                    CrearBotonModulo(
                        "Control de Stock",
                        3,
                        480,
                        110
                    );

                    break;


                // =================================================
                // ROL NO RECONOCIDO
                // =================================================

                default:

                    MessageBox.Show(
                        "El usuario posee un rol no reconocido.",
                        "Aura Beauty",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    break;
            }
        }


        // ---------------------------------------------------------
        // CREACIÓN DE BOTONES
        // ---------------------------------------------------------

        /// <summary>
        /// Crea un botón para acceder a un módulo.
        ///
        /// Este método evita repetir muchas veces
        /// el mismo código visual.
        /// </summary>
        /// <param name="texto">
        /// Texto que aparecerá en el botón.
        /// </param>
        /// <param name="opcion">
        /// Número interno utilizado para identificar
        /// qué módulo debe abrirse.
        /// </param>
        /// <param name="x">
        /// Posición horizontal.
        /// </param>
        /// <param name="y">
        /// Posición vertical.
        /// </param>
        private void CrearBotonModulo(
            string texto,
            int opcion,
            int x,
            int y
        )
        {
            Button btnModulo =
                new Button();

            btnModulo.Text =
                texto;

            btnModulo.Font =
                new Font(
                    "Segoe UI",
                    11F,
                    FontStyle.Bold
                );

            btnModulo.BackColor =
                rosaNude;

            btnModulo.ForeColor =
                Color.White;

            btnModulo.FlatStyle =
                FlatStyle.Flat;

            btnModulo.FlatAppearance.BorderSize =
                0;

            btnModulo.Cursor =
                Cursors.Hand;

            btnModulo.Size =
                new Size(360, 65);

            btnModulo.Location =
                new Point(x, y);


            /*
             * Tag permite guardar información
             * adicional dentro de un control.
             *
             * Nosotros guardamos un número
             * que identifica cada módulo.
             */
            btnModulo.Tag =
                opcion;


            /*
             * Todos los botones utilizan
             * el mismo evento Click.
             */
            btnModulo.Click +=
                BotonModulo_Click;


            /*
             * Efecto visual cuando el mouse
             * pasa sobre el botón.
             */
            btnModulo.MouseEnter +=
                (sender, e) =>
                {
                    btnModulo.BackColor =
                        rosaOscuro;
                };


            btnModulo.MouseLeave +=
                (sender, e) =>
                {
                    btnModulo.BackColor =
                        rosaNude;
                };


            pnlContenido.Controls.Add(
                btnModulo
            );


            RedondearControl(
                btnModulo,
                18
            );
        }


        // ---------------------------------------------------------
        // EVENTO DE LOS BOTONES DE MÓDULO
        // ---------------------------------------------------------

        /// <summary>
        /// Identifica qué botón fue presionado
        /// y ejecuta la acción correspondiente.
        /// </summary>
        private void BotonModulo_Click(
            object sender,
            EventArgs e
        )
        {
            /*
             * sender representa al control
             * que produjo el evento.
             *
             * Primero verificamos que sender
             * sea realmente un Button.
             */
            if (sender is Button btn)
            {
                /*
                 * Después verificamos que Tag
                 * contenga un número entero.
                 */
                if (btn.Tag is int opcion)
                {
                    switch (opcion)
                    {
                        case 1:

                            MessageBox.Show(
                                "Módulo: Gestión de Usuarios y Roles",
                                "Aura Beauty",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );

                            break;


                        case 2:

                            MessageBox.Show(
                                "Módulo: Productos",
                                "Aura Beauty",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );

                            break;


                        case 3:

                            MessageBox.Show(
                                "Módulo: Control de Stock",
                                "Aura Beauty",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );

                            break;


                        case 4:

                            MessageBox.Show(
                                "Módulo: Registro de Ventas",
                                "Aura Beauty",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );

                            break;


                        case 5:

                            MessageBox.Show(
                                "Módulo: Gestión de Clientes",
                                "Aura Beauty",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );

                            break;


                        case 6:

                            MessageBox.Show(
                                "Módulo: Reportes y Estadísticas",
                                "Aura Beauty",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );

                            break;
                    }
                }
            }
        }


        // ---------------------------------------------------------
        // CERRAR SESIÓN
        // ---------------------------------------------------------

        /// <summary>
        /// Cierra la sesión actual y vuelve
        /// al formulario de inicio de sesión.
        /// </summary>
        private void btnCerrarSesion_Click(
            object sender,
            EventArgs e
        )
        {
            DialogResult respuesta =
                MessageBox.Show(
                    "¿Deseás cerrar la sesión actual?",
                    "Cerrar sesión",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );


            /*
             * Solamente cerramos la sesión
             * si el usuario elige Sí.
             */
            if (respuesta == DialogResult.Yes)
            {
                cerrandoSesion = true;


                /*
                 * Application.OpenForms contiene
                 * los formularios que actualmente
                 * están abiertos en la aplicación.
                 *
                 * Buscamos el login que habíamos
                 * ocultado al iniciar sesión.
                 */
                Form formularioLogin =
                    Application.OpenForms["frmLogin"];


                if (formularioLogin != null)
                {
                    /*
                     * Volvemos a mostrar el login.
                     */
                    formularioLogin.Show();


                    /*
                     * Lo llevamos al frente.
                     */
                    formularioLogin.BringToFront();
                }


                /*
                 * Cerramos solamente el menú.
                 */
                this.Close();
            }
        }


        // ---------------------------------------------------------
        // CIERRE DE LA APLICACIÓN
        // ---------------------------------------------------------

        /// <summary>
        /// Se ejecuta cuando el menú se cierra.
        ///
        /// Si el usuario presionó Cerrar sesión,
        /// volvemos al login.
        ///
        /// Si cerró la ventana con la X,
        /// cerramos completamente la aplicación.
        /// </summary>
        protected override void OnFormClosed(
            FormClosedEventArgs e
        )
        {
            base.OnFormClosed(e);


            /*
             * ! significa "NO".
             *
             * Por lo tanto:
             *
             * !cerrandoSesion
             *
             * significa:
             * "si NO estamos cerrando sesión".
             */
            if (!cerrandoSesion)
            {
                Application.Exit();
            }
        }


        // ---------------------------------------------------------
        // REDONDEAR CONTROLES
        // ---------------------------------------------------------

        /// <summary>
        /// Permite darle bordes redondeados
        /// a botones y otros controles.
        /// </summary>
        private void RedondearControl(
            Control control,
            int radio
        )
        {
            GraphicsPath ruta =
                new GraphicsPath();


            Rectangle rectangulo =
                new Rectangle(
                    0,
                    0,
                    control.Width,
                    control.Height
                );


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


            ruta.CloseFigure();


            control.Region =
                new Region(ruta);
        }
    }
}