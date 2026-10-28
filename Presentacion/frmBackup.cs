
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Negocio;

namespace Presentacion
{
    public partial class frmBackup : Form
    {
        // Paleta compartida con el Centro de Reportes de Aura Beauty.
        private readonly Color rosa = Color.FromArgb(195, 139, 147);
        private readonly Color rosaOscuro = Color.FromArgb(155, 94, 106);
        private readonly Color beige = Color.FromArgb(247, 239, 232);
        private readonly Color fondo = Color.FromArgb(250, 246, 243);
        private readonly Color texto = Color.FromArgb(64, 55, 57);
        private readonly Color gris = Color.FromArgb(130, 118, 120);
        private readonly Color linea = Color.FromArgb(231, 220, 217);
        private readonly Color rosaClaro = Color.FromArgb(245, 227, 229);
        // Capa de Negocio encargada de las operaciones de backup.
        private readonly CN_Backup objCN_Backup = new CN_Backup();

        // Identificador del administrador que inició sesión.
        private readonly int idUsuarioActual;

        // Controles de la interfaz.
        private Panel panelEncabezado;
        private Panel panelAcciones;
        private Label lblTitulo;
        private Label lblSubtitulo;
        private Label lblHistorial;
        private Label lblEstado;
        private Button btnCrearBackup;
        private Button btnActualizar;
        private Button btnSolicitarRestauracion;
        private Button btnConsultarSolicitud;
        private Button btnProbarRestauracion;
        private DataGridView dgvBackups;


        // Grilla compartida para que los eventos Autorizar y Rechazar
        // puedan acceder a la solicitud seleccionada.
        private DataGridView dgvSolicitudesPendientes;

        // Referencia a la ventana que contiene las solicitudes.
        private Form formularioSolicitudesPendientes;


        /// <summary>
        /// Constructor del formulario.
        /// Recibe el ID del usuario autenticado.
        /// </summary>

        public frmBackup(int idUsuario)
        {
            // Verificar que el usuario autenticado sea válido.
            if (idUsuario <= 0)
            {
                throw new ArgumentException(
                    "El usuario autenticado no es válido.");
            }

            // Guardar el ID para identificar quién realiza las operaciones.
            idUsuarioActual = idUsuario;

            // Crear la interfaz completamente mediante código C#.
            CrearInterfaz();

            // Cargar el historial de respaldos.
            CargarHistorial();
        }


        /// <summary>
        /// Construye todos los controles visuales del formulario.
        /// </summary>

        private void CrearInterfaz()
        {
            // Configuración general: misma identidad visual que Reportes.
            Text = "Aura Beauty - Centro de respaldos";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            Size = new Size(1200, 760);
            MinimumSize = new Size(1000, 650);
            BackColor = fondo;
            Font = new Font("Segoe UI", 9.5F);

            // Barra lateral beige con la marca Aura Beauty.
            Panel panelLateral = new Panel
            {
                Dock = DockStyle.Left,
                Width = 210,
                BackColor = beige,
                Padding = new Padding(20, 25, 15, 15)
            };

            Label lblMarca = new Label
            {
                Text = "AURA",
                Font = new Font("Segoe UI", 25F, FontStyle.Bold),
                ForeColor = rosaOscuro,
                AutoSize = true,
                Location = new Point(20, 24)
            };

            Label lblMarcaSubtitulo = new Label
            {
                Text = "BEAUTY | RESPALDOS",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = gris,
                AutoSize = true,
                Location = new Point(22, 70)
            };

            Panel separador = new Panel
            {
                Location = new Point(20, 112),
                Size = new Size(165, 1),
                BackColor = linea,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            Label lblSeccion = new Label
            {
                Text = "Administración de respaldos",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = texto,
                AutoSize = true,
                Location = new Point(22, 132)
            };

            panelLateral.Controls.Add(lblMarca);
            panelLateral.Controls.Add(lblMarcaSubtitulo);
            panelLateral.Controls.Add(separador);
            panelLateral.Controls.Add(lblSeccion);

            // Área principal de trabajo.
            Panel panelContenido = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = fondo,
                Padding = new Padding(28, 20, 28, 24)
            };

            panelEncabezado = new Panel
            {
                Dock = DockStyle.Top,
                Height = 88,
                BackColor = fondo
            };

            lblTitulo = new Label
            {
                Text = "Centro de respaldos",
                Font = new Font("Segoe UI", 21F, FontStyle.Bold),
                ForeColor = rosaOscuro,
                AutoSize = true,
                Location = new Point(2, 4)
            };

            lblSubtitulo = new Label
            {
                Text = "Administración y recuperación segura de Aura Beauty",
                Font = new Font("Segoe UI", 10F),
                ForeColor = gris,
                AutoSize = true,
                Location = new Point(5, 47)
            };

            panelEncabezado.Controls.Add(lblTitulo);
            panelEncabezado.Controls.Add(lblSubtitulo);

            // Tarjeta blanca con botones organizados en filas.
            panelAcciones = new Panel
            {
                Dock = DockStyle.Top,
                Height = 158,
                BackColor = Color.White,
                Padding = new Padding(12)
            };

            btnCrearBackup = CrearBotonEstilizado("Crear respaldo", true,
                new Point(18, 16), new Size(180, 40));
            btnCrearBackup.Click += btnCrearBackup_Click;

            btnActualizar = CrearBotonEstilizado("Actualizar historial", false,
                new Point(210, 16), new Size(180, 40));
            btnActualizar.Click += btnActualizar_Click;

            btnSolicitarRestauracion = CrearBotonEstilizado("Solicitar restauración", false,
                new Point(402, 16), new Size(190, 40));
            btnSolicitarRestauracion.Click += btnSolicitarRestauracion_Click;

            btnConsultarSolicitud = CrearBotonEstilizado("Consultar solicitud", false,
                new Point(18, 68), new Size(180, 40));
            btnConsultarSolicitud.Click += btnConsultarSolicitud_Click;

            Button btnSolicitudesPendientes = CrearBotonEstilizado(
                "Solicitudes pendientes", true, new Point(210, 68), new Size(190, 40));
            btnSolicitudesPendientes.Name = "btnSolicitudesPendientes";
            btnSolicitudesPendientes.Click += btnSolicitudesPendientes_Click;

            Button btnHistorialRestauraciones = CrearBotonEstilizado(
                "Historial de restauraciones", true, new Point(412, 68), new Size(220, 40));
            btnHistorialRestauraciones.Name = "btnHistorialRestauraciones";
            btnHistorialRestauraciones.Click += btnHistorialRestauraciones_Click;

            btnProbarRestauracion = CrearBotonEstilizado("Probar restauración", false,
                new Point(644, 68), new Size(190, 40));
            btnProbarRestauracion.Name = "btnProbarRestauracion";
            btnProbarRestauracion.Click += btnProbarRestauracion_Click;

            lblEstado = new Label
            {
                Text = "Preparado para consultar los respaldos.",
                AutoSize = true,
                Font = new Font("Segoe UI", 9F),
                ForeColor = gris,
                Location = new Point(20, 119)
            };

            panelAcciones.Controls.AddRange(new Control[]
            {
                btnCrearBackup, btnActualizar, btnSolicitarRestauracion,
                btnConsultarSolicitud, btnSolicitudesPendientes,
                btnHistorialRestauraciones, btnProbarRestauracion, lblEstado
            });

            // Tarjeta de historial: el contenido y los eventos permanecen iguales.
            Panel panelTabla = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(18, 16, 18, 18)
            };

            lblHistorial = new Label
            {
                Text = "Historial de respaldos",
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = texto,
                AutoSize = true,
                Location = new Point(18, 16)
            };

            dgvBackups = new DataGridView
            {
                Location = new Point(18, 50),
                Size = new Size(900, 350),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom |
                         AnchorStyles.Left | AnchorStyles.Right,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                GridColor = linea,
                RowHeadersVisible = false,
                AutoGenerateColumns = true,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 38,
                RowTemplate = { Height = 30 }
            };
            AplicarEstiloGrilla(dgvBackups);

            panelTabla.Controls.Add(dgvBackups);
            panelTabla.Controls.Add(lblHistorial);

            // Agregar primero el área que ocupa el espacio restante y luego las superiores.
            panelContenido.Controls.Add(panelTabla);
            panelContenido.Controls.Add(panelAcciones);
            panelContenido.Controls.Add(panelEncabezado);
            Controls.Add(panelContenido);
            Controls.Add(panelLateral);
        }

        /// <summary>
        /// Crea un botón con la paleta y el estilo usados en el Centro de Reportes.
        /// Este método solo configura la apariencia; los eventos se asignan aparte.
        /// </summary>
        private Button CrearBotonEstilizado(string textoBoton, bool principal,
            Point ubicacion, Size tamaño)
        {
            Button boton = new Button
            {
                Text = textoBoton,
                Location = ubicacion,
                Size = tamaño,
                BackColor = principal ? rosa : rosaClaro,
                ForeColor = principal ? Color.White : texto,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                UseVisualStyleBackColor = false
            };

            boton.FlatAppearance.BorderSize = 0;
            boton.FlatAppearance.MouseOverBackColor = principal
                ? rosaOscuro : Color.FromArgb(238, 216, 219);
            boton.FlatAppearance.MouseDownBackColor = rosaOscuro;

            // Redondear los bordes para que coincidan con los botones de frmReportes.
            AplicarBordesRedondeados(boton, 16);
            return boton;
        }

        /// <summary>
        /// Recorta el botón con esquinas redondeadas y vuelve a aplicar
        /// la forma cuando cambia su tamaño.
        /// </summary>
        private void AplicarBordesRedondeados(Button boton, int radio)
        {
            if (boton == null)
                return;

            void ActualizarForma(object sender, EventArgs e)
            {
                if (boton.Width <= 0 || boton.Height <= 0)
                    return;

                int diametro = Math.Min(radio * 2,
                    Math.Min(boton.Width, boton.Height));
                Rectangle rectangulo = new Rectangle(0, 0,
                    boton.Width, boton.Height);

                using (GraphicsPath ruta = new GraphicsPath())
                {
                    ruta.AddArc(rectangulo.X, rectangulo.Y,
                        diametro, diametro, 180, 90);
                    ruta.AddArc(rectangulo.Right - diametro, rectangulo.Y,
                        diametro, diametro, 270, 90);
                    ruta.AddArc(rectangulo.Right - diametro,
                        rectangulo.Bottom - diametro,
                        diametro, diametro, 0, 90);
                    ruta.AddArc(rectangulo.X,
                        rectangulo.Bottom - diametro,
                        diametro, diametro, 90, 90);
                    ruta.CloseFigure();

                    Region regionAnterior = boton.Region;
                    boton.Region = new Region(ruta);
                    if (regionAnterior != null)
                        regionAnterior.Dispose();
                }
            }

            boton.SizeChanged -= ActualizarForma;
            boton.SizeChanged += ActualizarForma;
            ActualizarForma(boton, EventArgs.Empty);
        }

        /// <summary>
        /// Aplica un estilo consistente a las tablas del módulo.
        /// No modifica columnas, datos ni consultas.
        /// </summary>
        private void AplicarEstiloGrilla(DataGridView grilla)
        {
            grilla.BackgroundColor = Color.White;
            grilla.BorderStyle = BorderStyle.None;
            grilla.GridColor = linea;
            grilla.EnableHeadersVisualStyles = false;
            grilla.ColumnHeadersDefaultCellStyle.BackColor = beige;
            grilla.ColumnHeadersDefaultCellStyle.ForeColor = rosaOscuro;
            grilla.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 9F, FontStyle.Bold);
            grilla.ColumnHeadersDefaultCellStyle.SelectionBackColor = beige;
            grilla.ColumnHeadersDefaultCellStyle.SelectionForeColor = rosaOscuro;
            grilla.DefaultCellStyle.BackColor = Color.White;
            grilla.DefaultCellStyle.ForeColor = texto;
            grilla.DefaultCellStyle.Font = new Font("Segoe UI", 9F);
            grilla.DefaultCellStyle.SelectionBackColor = rosaClaro;
            grilla.DefaultCellStyle.SelectionForeColor = texto;
            grilla.AlternatingRowsDefaultCellStyle.BackColor = fondo;
            grilla.RowHeadersVisible = false;
        }


        /// <summary>
        /// Abre una ventana con las solicitudes de restauración pendientes.
        /// Permite al administrador consultar qué solicitudes esperan
        /// una decisión, sin ejecutar ninguna restauración.
        /// </summary>
        private void btnSolicitudesPendientes_Click(
            object sender, EventArgs e)
        {
            // Crear una ventana independiente para mostrar las solicitudes.
            // Crear la ventana que contendrá las solicitudes pendientes.
            formularioSolicitudesPendientes = new Form();

            // Usar la ventana creada para configurar y mostrar la grilla.
            Form formulario = formularioSolicitudesPendientes;

            {
                // Configurar el título y las dimensiones de la ventana.
                formulario.Text = "Solicitudes de restauración pendientes";
                formulario.StartPosition = FormStartPosition.CenterParent;
                formulario.Size = new Size(1000, 500);
                formulario.MinimumSize = new Size(750, 350);
                formulario.BackColor = fondo;

                // Crear la grilla donde se mostrarán los resultados.
                dgvSolicitudesPendientes = new DataGridView
                {
                    // Dejar un margen alrededor de la grilla.
                    Location = new Point(15, 15),

                    // Permitir que la grilla se adapte al tamaño de la ventana.
                    Size = new Size(950, 400),
                    Anchor = AnchorStyles.Top |
                             AnchorStyles.Bottom |
                             AnchorStyles.Left |
                             AnchorStyles.Right,

                    // Los datos se consultan, no se editan directamente.
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,

                    // Permitir seleccionar una solicitud completa.
                    MultiSelect = false,
                    SelectionMode =
                        DataGridViewSelectionMode.FullRowSelect,

                    // Ajustar las columnas al espacio disponible.
                    AutoSizeColumnsMode =
                        DataGridViewAutoSizeColumnsMode.Fill,

                    // Configurar la apariencia.
                    BackgroundColor = Color.White,
                    RowHeadersVisible = false,
                    AutoGenerateColumns = true
                };


                // Mantener la misma paleta en la grilla de solicitudes.
                AplicarEstiloGrilla(dgvSolicitudesPendientes);

                // ------------------------------------------------------------
                // BOTÓN PARA AUTORIZAR UNA SOLICITUD
                // ------------------------------------------------------------

                // Crear el botón que permite aprobar la solicitud seleccionada.
                Button btnAutorizar = new Button
                {
                    Text = "Autorizar solicitud",
                    Size = new Size(180, 40),
                    Location = new Point(590, 425),
                    BackColor = Color.FromArgb(207, 115, 151),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };

                // Aplicar tipografía y esquinas redondeadas como en frmReportes.
                btnAutorizar.FlatAppearance.BorderSize = 0;
                btnAutorizar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                btnAutorizar.UseVisualStyleBackColor = false;
                btnAutorizar.FlatAppearance.MouseOverBackColor = rosaOscuro;
                btnAutorizar.FlatAppearance.MouseDownBackColor = rosaOscuro;
                AplicarBordesRedondeados(btnAutorizar, 16);

                // Conectar el botón con el evento que valida al segundo administrador.
                btnAutorizar.Click += btnAutorizarSolicitud_Click;


                // ------------------------------------------------------------
                // BOTÓN PARA RECHAZAR UNA SOLICITUD
                // ------------------------------------------------------------

                // Crear el botón que permite rechazar la solicitud seleccionada.
                Button btnRechazar = new Button
                {
                    Text = "Rechazar solicitud",
                    Size = new Size(180, 40),
                    Location = new Point(785, 425),
                    BackColor = Color.FromArgb(240, 230, 235),
                    ForeColor = Color.FromArgb(70, 50, 60),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };

                // Aplicar tipografía y esquinas redondeadas como en frmReportes.
                btnRechazar.FlatAppearance.BorderSize = 0;
                btnRechazar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                btnRechazar.UseVisualStyleBackColor = false;
                btnRechazar.FlatAppearance.MouseOverBackColor = Color.FromArgb(238, 216, 219);
                btnRechazar.FlatAppearance.MouseDownBackColor = rosaOscuro;
                AplicarBordesRedondeados(btnRechazar, 16);

                // Conectar el botón con el evento de rechazo.
                btnRechazar.Click += btnRechazarSolicitud_Click;


                // ------------------------------------------------------------
                // AGREGAR LOS CONTROLES A LA VENTANA
                // ------------------------------------------------------------

                // Dejar espacio debajo de la grilla para los dos botones.
                dgvSolicitudesPendientes.Size = new Size(950, 390);

                // Agregar la grilla y los botones a la ventana.
                formulario.Controls.Add(dgvSolicitudesPendientes);
                formulario.Controls.Add(btnAutorizar);
                formulario.Controls.Add(btnRechazar);

                // Asegurar que los botones queden visibles.
                btnAutorizar.BringToFront();
                btnRechazar.BringToFront();


                try
                {
                    // Consultar las solicitudes pendientes en la base de datos.
                    DataTable solicitudes =
                        objCN_Backup.ListarSolicitudesPendientes(
                            idUsuarioActual);

                    // Mostrar los resultados obtenidos.
                    dgvSolicitudesPendientes.DataSource = solicitudes;

                    // Informar si no existen solicitudes pendientes.
                    if (solicitudes.Rows.Count == 0)
                    {
                        MessageBox.Show(
                            "No hay solicitudes de restauración pendientes.",
                            "Solicitudes pendientes",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }

                    // Abrir la ventana de consulta.
                    formulario.ShowDialog(this);

                    // Liberar los recursos de la ventana al terminar la consulta.
                    formularioSolicitudesPendientes.Dispose();

                    // Limpiar la referencia para evitar reutilizar una ventana cerrada.
                    formularioSolicitudesPendientes = null;

                    // Limpiar la grilla que estaba asociada a esa ventana.
                    dgvSolicitudesPendientes = null;
                }
                catch (Exception ex)
                {
                    // Informar si SQL Server no pudo devolver los datos.
                    MessageBox.Show(
                        "No se pudieron cargar las solicitudes pendientes.\n\n"
                        + ex.Message,
                        "Error de consulta",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }


        // ------------------------------------------------------------
        // AUTORIZAR SOLICITUD
        // ------------------------------------------------------------

        /// <summary>
        /// Solicita las credenciales de un segundo administrador
        /// y registra la autorización de la solicitud seleccionada.
        /// No ejecuta RESTORE DATABASE.
        /// </summary>
        private void btnAutorizarSolicitud_Click(object sender, EventArgs e)
        {
            // Comprobar que exista una solicitud seleccionada.
            if (dgvSolicitudesPendientes == null ||
                dgvSolicitudesPendientes.CurrentRow == null)
            {
                MessageBox.Show(
                    "Seleccioná una solicitud pendiente.",
                    "Autorizar solicitud",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Leer los identificadores de la fila seleccionada.
                int idSolicitud = Convert.ToInt32(
                    dgvSolicitudesPendientes.CurrentRow
                        .Cells["IdSolicitud"].Value);

                int idSolicitante = Convert.ToInt32(
                    dgvSolicitudesPendientes.CurrentRow
                        .Cells["IdSolicitante"].Value);

                // Solicitar correo y contraseña del segundo administrador.
                string correo;
                string contraseña;

                if (!SolicitarCredencialesAdministrador(
                    out correo, out contraseña))
                {
                    // El usuario canceló el ingreso de credenciales.
                    return;
                }

                // Confirmar que se desea registrar la autorización.
                DialogResult confirmacion = MessageBox.Show(
                    "¿Confirmás la autorización de la solicitud " +
                    idSolicitud + "?\n\n" +
                    "Esta acción registra la autorización, " +
                    "pero no restaura todavía la base de datos.",
                    "Confirmar autorización",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes)
                    return;

                // Validar las credenciales, el rol y que el autorizador
                // sea distinto del administrador solicitante.
                objCN_Backup.AutorizarRestauracion(
                    idSolicitud,
                    idSolicitante,
                    correo,
                    contraseña);

                // Informar que la autorización se registró correctamente.
                MessageBox.Show(
                    "La solicitud fue autorizada correctamente.\n" +
                    "La restauración real todavía no se ejecutó.",
                    "Autorización registrada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Volver a consultar la lista para quitar de la grilla
                // la solicitud que ya no está pendiente.
                ActualizarSolicitudesPendientes();
            }
            catch (Exception ex)
            {
                // Mostrar errores de validación o de SQL Server.
                MessageBox.Show(
                    "No se pudo autorizar la solicitud.\n\n" + ex.Message,
                    "Error de autorización",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        // ------------------------------------------------------------
        // RECHAZAR SOLICITUD
        // ------------------------------------------------------------

        /// <summary>
        /// Solicita las credenciales de un segundo administrador
        /// y un motivo para registrar el rechazo.
        /// </summary>
        private void btnRechazarSolicitud_Click(object sender, EventArgs e)
        {
            // Comprobar que exista una fila seleccionada.
            if (dgvSolicitudesPendientes == null ||
                dgvSolicitudesPendientes.CurrentRow == null)
            {
                MessageBox.Show(
                    "Seleccioná una solicitud pendiente.",
                    "Rechazar solicitud",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Obtener los identificadores de la solicitud y del solicitante.
                int idSolicitud = Convert.ToInt32(
                    dgvSolicitudesPendientes.CurrentRow
                        .Cells["IdSolicitud"].Value);

                int idSolicitante = Convert.ToInt32(
                    dgvSolicitudesPendientes.CurrentRow
                        .Cells["IdSolicitante"].Value);

                // Pedir un motivo obligatorio para dejar constancia del rechazo.
                string motivo = SolicitarMotivoRechazo();

                // Si el usuario cancela, no se registra ninguna operación.
                if (motivo == null)
                    return;

                // Solicitar las credenciales del administrador que resolverá.
                string correo;
                string contraseña;

                if (!SolicitarCredencialesAdministrador(
                    out correo, out contraseña))
                {
                    return;
                }

                // Confirmar la decisión antes de modificar el estado.
                DialogResult confirmacion = MessageBox.Show(
                    "¿Confirmás el rechazo de la solicitud " +
                    idSolicitud + "?\n\nMotivo: " + motivo,
                    "Confirmar rechazo",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes)
                    return;

                // Validar las credenciales y registrar el rechazo en SQL Server.
                objCN_Backup.RechazarRestauracion(
                    idSolicitud,
                    idSolicitante,
                    correo,
                    contraseña,
                    motivo);

                // Informar que el rechazo quedó registrado.
                MessageBox.Show(
                    "La solicitud fue rechazada correctamente.",
                    "Rechazo registrado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Actualizar la lista de solicitudes pendientes.
                ActualizarSolicitudesPendientes();
            }
            catch (Exception ex)
            {
                // Informar cualquier problema durante el rechazo.
                MessageBox.Show(
                    "No se pudo rechazar la solicitud.\n\n" + ex.Message,
                    "Error de rechazo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        // ------------------------------------------------------------
        // SOLICITAR CREDENCIALES DEL SEGUNDO ADMINISTRADOR
        // ------------------------------------------------------------

        /// <summary>
        /// Muestra una ventana para que otro administrador ingrese
        /// su correo y contraseña antes de autorizar o rechazar.
        /// Devuelve false si el usuario cancela la operación.
        /// </summary>
        private bool SolicitarCredencialesAdministrador(
            out string correo, out string contraseña)
        {
            // Inicializar los resultados para cubrir el caso de cancelación.
            correo = string.Empty;
            contraseña = string.Empty;

            // Crear una ventana temporal para solicitar las credenciales.
            using (Form ventana = new Form())
            {
                ventana.Text = "Validar segundo administrador";
                ventana.StartPosition = FormStartPosition.CenterParent;
                ventana.FormBorderStyle = FormBorderStyle.FixedDialog;
                ventana.ClientSize = new Size(400, 210);
                ventana.MaximizeBox = false;
                ventana.MinimizeBox = false;

                // Etiqueta para el correo.
                Label lblCorreo = new Label
                {
                    Text = "Correo del administrador:",
                    Location = new Point(20, 20),
                    AutoSize = true
                };

                // Campo donde se ingresa el correo.
                TextBox txtCorreo = new TextBox
                {
                    Location = new Point(20, 45),
                    Width = 350
                };

                // Etiqueta para la contraseña.
                Label lblContraseña = new Label
                {
                    Text = "Contraseña:",
                    Location = new Point(20, 80),
                    AutoSize = true
                };

                // Campo que oculta los caracteres de la contraseña.
                TextBox txtContraseña = new TextBox
                {
                    Location = new Point(20, 105),
                    Width = 350,
                    UseSystemPasswordChar = true
                };

                // Botón para confirmar las credenciales.
                Button btnAceptar = new Button
                {
                    Text = "Continuar",
                    Location = new Point(185, 155),
                    Size = new Size(90, 32),
                    DialogResult = DialogResult.OK
                };

                // Botón para cancelar sin realizar cambios.
                Button btnCancelar = new Button
                {
                    Text = "Cancelar",
                    Location = new Point(280, 155),
                    Size = new Size(90, 32),
                    DialogResult = DialogResult.Cancel
                };

                // Agregar los controles a la ventana.
                ventana.Controls.Add(lblCorreo);
                ventana.Controls.Add(txtCorreo);
                ventana.Controls.Add(lblContraseña);
                ventana.Controls.Add(txtContraseña);
                ventana.Controls.Add(btnAceptar);
                ventana.Controls.Add(btnCancelar);

                // Definir los botones predeterminados del formulario.
                ventana.AcceptButton = btnAceptar;
                ventana.CancelButton = btnCancelar;

                // Mostrar la ventana y comprobar si se confirmó.
                if (ventana.ShowDialog(this) != DialogResult.OK)
                    return false;

                // Leer los datos ingresados.
                correo = txtCorreo.Text.Trim();
                contraseña = txtContraseña.Text;

                // Comprobar que ambos campos estén completos.
                if (string.IsNullOrWhiteSpace(correo) ||
                    string.IsNullOrWhiteSpace(contraseña))
                {
                    MessageBox.Show(
                        "Debés ingresar el correo y la contraseña.",
                        "Datos incompletos",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    // Indicar que no se debe continuar.
                    correo = string.Empty;
                    contraseña = string.Empty;
                    return false;
                }

                // Entregar las credenciales al método que las valida.
                return true;
            }
        }


        // ------------------------------------------------------------
        // SOLICITAR MOTIVO DEL RECHAZO
        // ------------------------------------------------------------

        /// <summary>
        /// Pide un motivo para rechazar la solicitud.
        /// Devuelve null si el usuario cancela.
        /// </summary>
        private string SolicitarMotivoRechazo()
        {
            // Crear una ventana para escribir el motivo.
            using (Form ventana = new Form())
            {
                ventana.Text = "Motivo del rechazo";
                ventana.StartPosition = FormStartPosition.CenterParent;
                ventana.FormBorderStyle = FormBorderStyle.FixedDialog;
                ventana.ClientSize = new Size(420, 220);
                ventana.MaximizeBox = false;
                ventana.MinimizeBox = false;

                // Explicar qué información se espera.
                Label lblMotivo = new Label
                {
                    Text = "Indicá por qué rechazás la solicitud:",
                    Location = new Point(20, 20),
                    AutoSize = true
                };

                // Permitir ingresar un motivo de varias líneas.
                TextBox txtMotivo = new TextBox
                {
                    Location = new Point(20, 50),
                    Size = new Size(380, 95),
                    Multiline = true,
                    MaxLength = 1000,
                    ScrollBars = ScrollBars.Vertical
                };

                // Botón para confirmar el rechazo.
                Button btnAceptar = new Button
                {
                    Text = "Continuar",
                    Location = new Point(205, 165),
                    Size = new Size(90, 32),
                    DialogResult = DialogResult.OK
                };

                // Botón para cancelar la operación.
                Button btnCancelar = new Button
                {
                    Text = "Cancelar",
                    Location = new Point(305, 165),
                    Size = new Size(95, 32),
                    DialogResult = DialogResult.Cancel
                };

                // Agregar los controles a la ventana.
                ventana.Controls.Add(lblMotivo);
                ventana.Controls.Add(txtMotivo);
                ventana.Controls.Add(btnAceptar);
                ventana.Controls.Add(btnCancelar);

                // Configurar los botones predeterminados.
                ventana.AcceptButton = btnAceptar;
                ventana.CancelButton = btnCancelar;

                // Mostrar la ventana y comprobar la respuesta.
                if (ventana.ShowDialog(this) != DialogResult.OK)
                    return null;

                // No permitir un motivo vacío.
                if (string.IsNullOrWhiteSpace(txtMotivo.Text))
                {
                    MessageBox.Show(
                        "Debés indicar el motivo del rechazo.",
                        "Motivo obligatorio",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return null;
                }

                // Devolver el motivo sin espacios innecesarios.
                return txtMotivo.Text.Trim();
            }
        }



        // ------------------------------------------------------------
        // ACTUALIZAR LISTA DE SOLICITUDES
        // ------------------------------------------------------------

        /// <summary>
        /// Vuelve a consultar las solicitudes pendientes
        /// y actualiza la grilla abierta.
        /// </summary>
        private void ActualizarSolicitudesPendientes()
        {
            // No continuar si la ventana o la grilla ya están cerradas.
            if (dgvSolicitudesPendientes == null ||
                dgvSolicitudesPendientes.IsDisposed)
            {
                return;
            }

            // Consultar los pendientes usando el administrador autenticado.
            DataTable solicitudes =
                objCN_Backup.ListarSolicitudesPendientes(idUsuarioActual);

            // Mostrar los nuevos resultados.
            dgvSolicitudesPendientes.DataSource = solicitudes;
        }




        /// <summary>
        /// Abre una ventana para elegir el nombre y la ubicación
        /// del archivo de respaldo.
        /// </summary>
        private void btnCrearBackup_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialogo = new SaveFileDialog())
            {
                dialogo.Title = "Guardar respaldo de Aura Beauty";
                dialogo.Filter =
                    "Respaldo de SQL Server (*.bak)|*.bak";
                dialogo.DefaultExt = "bak";
                dialogo.AddExtension = true;
                dialogo.OverwritePrompt = true;

                // Proponer un nombre con fecha, hora y milisegundos.
                dialogo.FileName =
                    "AuraBeautyDB_" +
                    DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") +
                    ".bak";

                // Si se cancela la ventana, no hacer nada.
                if (dialogo.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                try
                {
                    // Evitar que se inicien dos operaciones a la vez.
                    btnCrearBackup.Enabled = false;
                    lblEstado.Text = "Creando el respaldo...";

                    // Solicitar a Negocio que cree y registre el respaldo.
                    int idBackup = objCN_Backup.CrearBackup(
                        idUsuarioActual,
                        dialogo.FileName,
                        "Respaldo manual desde la interfaz");

                    lblEstado.Text = "Respaldo creado correctamente.";

                    MessageBox.Show(
                        "El respaldo se creó y registró correctamente.\n\n" +
                        "Archivo: " +
                        Path.GetFileName(dialogo.FileName) +
                        "\nUbicación: " + dialogo.FileName +
                        "\nID del registro: " + idBackup,
                        "Respaldo exitoso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    // Actualizar la tabla después de la operación.
                    CargarHistorial();
                }
                catch (Exception ex)
                {
                    lblEstado.Text = "No se pudo crear el respaldo.";

                    MessageBox.Show(
                        ex.Message,
                        "Error al crear el respaldo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    // Volver a habilitar el botón.
                    btnCrearBackup.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Actualiza el historial de respaldos.
        /// </summary>
        private void btnActualizar_Click(object sender, EventArgs e)
        {
            CargarHistorial();
        }

        /// <summary>
        /// Consulta los respaldos mediante la Capa de Negocio.
        /// </summary>
        private void CargarHistorial()
        {
            try
            {
                // Obtener la tabla con los respaldos registrados.
                DataTable tabla =
                    objCN_Backup.ListarBackups(idUsuarioActual);

                // Mostrar los resultados en el DataGridView.
                dgvBackups.DataSource = tabla;

                lblEstado.Text =
                    "Respaldos encontrados: " + tabla.Rows.Count;
            }
            catch (Exception ex)
            {
                lblEstado.Text = "No se pudo cargar el historial.";

                MessageBox.Show(
                    ex.Message,
                    "Error al consultar respaldos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Registra una solicitud pendiente para restaurar
        /// el respaldo seleccionado.
        /// </summary>

        private void btnSolicitarRestauracion_Click(
            object sender, EventArgs e)
        {
            // Verificar que haya un respaldo seleccionado.
            if (dgvBackups.CurrentRow == null)
            {
                MessageBox.Show(
                    "Seleccioná un respaldo del historial.",
                    "Solicitud de restauración",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            try
            {
                // Obtener el ID usando el nombre real de la columna.
                int idBackup = Convert.ToInt32(
                    dgvBackups.CurrentRow.Cells["IdBackup"].Value);

                // Obtener el estado del respaldo seleccionado.
                string estado = Convert.ToString(
                    dgvBackups.CurrentRow.Cells["Estado"].Value);

                // Solo se pueden solicitar respaldos exitosos.
                if (!string.Equals(
                    estado,
                    "Exitoso",
                    StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "Solo podés solicitar la restauración " +
                        "de un respaldo registrado como exitoso.",
                        "Respaldo no válido",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                // Solicitar confirmación antes de registrar la solicitud.
                DialogResult confirmacion = MessageBox.Show(
                    "¿Querés solicitar la restauración del respaldo " +
                    "seleccionado?\n\nLa solicitud quedará pendiente " +
                    "hasta que otro administrador la resuelva.",
                    "Confirmar solicitud",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirmacion != DialogResult.Yes)
                    return;

                // Registrar la solicitud, sin restaurar todavía la base.
                int idSolicitud =
                    objCN_Backup.SolicitarRestauracion(
                        idBackup,
                        idUsuarioActual,
                        "Solicitud desde el Centro de respaldos");

                MessageBox.Show(
                    "La solicitud quedó registrada como pendiente.\n" +
                    "Número de solicitud: " + idSolicitud,
                    "Solicitud registrada",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Mostrar el estado en la interfaz.
                lblEstado.Text =
                    "Solicitud pendiente: " + idSolicitud;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "No se pudo registrar la solicitud.\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Consulta el estado de una solicitud por su identificador.
        /// </summary>
        private void btnConsultarSolicitud_Click(
            object sender, EventArgs e)
        {
            using (Form formulario = new Form())
            using (TextBox txtIdSolicitud = new TextBox())
            using (Button btnConsultar = new Button())
            {
                formulario.Text = "Consultar solicitud";
                formulario.StartPosition =
                    FormStartPosition.CenterParent;
                formulario.Size = new Size(350, 150);
                formulario.FormBorderStyle =
                    FormBorderStyle.FixedDialog;
                formulario.MaximizeBox = false;
                formulario.MinimizeBox = false;

                Label lbl = new Label
                {
                    Text = "Número de solicitud:",
                    AutoSize = true,
                    Location = new Point(15, 18)
                };

                txtIdSolicitud.Location = new Point(165, 15);
                txtIdSolicitud.Width = 145;

                btnConsultar.Text = "Consultar";
                btnConsultar.Location = new Point(190, 55);
                btnConsultar.DialogResult = DialogResult.OK;

                formulario.Controls.Add(lbl);
                formulario.Controls.Add(txtIdSolicitud);
                formulario.Controls.Add(btnConsultar);
                formulario.AcceptButton = btnConsultar;

                if (formulario.ShowDialog(this) != DialogResult.OK)
                {
                    return;
                }

                int idSolicitud;

                if (!int.TryParse(
                    txtIdSolicitud.Text,
                    out idSolicitud) ||
                    idSolicitud <= 0)
                {
                    MessageBox.Show(
                        "Ingresá un número de solicitud válido.",
                        "Dato incorrecto",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    DataTable resultado =
                        objCN_Backup.ConsultarEstadoRestauracion(
                            idSolicitud,
                            idUsuarioActual);

                    if (resultado.Rows.Count == 0)
                    {
                        MessageBox.Show(
                            "No se encontró la solicitud.",
                            "Consulta",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                        return;
                    }

                    // Mostrar los datos devueltos por el procedimiento.
                    using (Form detalle = new Form())
                    using (DataGridView grilla = new DataGridView())
                    {
                        detalle.Text = "Estado de la solicitud";
                        detalle.StartPosition =
                            FormStartPosition.CenterParent;
                        detalle.Size = new Size(800, 300);

                        grilla.Dock = DockStyle.Fill;
                        grilla.ReadOnly = true;
                        grilla.AutoGenerateColumns = true;
                        grilla.AutoSizeColumnsMode =
                            DataGridViewAutoSizeColumnsMode.AllCells;
                        grilla.DataSource = resultado;

                        detalle.Controls.Add(grilla);
                        detalle.ShowDialog(this);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        ex.Message,
                        "Error al consultar la solicitud",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Abre una ventana con todas las solicitudes de restauración.
        /// Solo consulta y muestra información; no ejecuta RESTORE DATABASE.
        /// </summary>
        private void btnHistorialRestauraciones_Click(object sender, EventArgs e)
        {
            try
            {
                // Consultar todas las solicitudes mediante la capa de negocio.
                DataTable historial =
                    objCN_Backup.ListarHistorialRestauraciones(idUsuarioActual);

                // Avisar si todavía no hay solicitudes registradas.
                if (historial.Rows.Count == 0)
                {
                    MessageBox.Show(
                        "No hay solicitudes de restauración registradas.",
                        "Historial de restauraciones",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    return;
                }

                // Crear una ventana independiente para mostrar los resultados.
                using (Form formulario = new Form())
                using (DataGridView grilla = new DataGridView())
                {
                    // Configurar la ventana del historial.
                    formulario.Text = "Historial de restauraciones";
                    formulario.StartPosition = FormStartPosition.CenterParent;
                    formulario.Size = new Size(1100, 500);
                    formulario.MinimumSize = new Size(800, 350);
                    formulario.BackColor = fondo;

                    // Configurar la grilla para consulta, sin edición de datos.
                    grilla.Dock = DockStyle.Fill;
                    grilla.ReadOnly = true;
                    grilla.AllowUserToAddRows = false;
                    grilla.AllowUserToDeleteRows = false;
                    grilla.MultiSelect = false;
                    grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    grilla.AutoGenerateColumns = true;
                    grilla.AutoSizeColumnsMode =
                        DataGridViewAutoSizeColumnsMode.DisplayedCells;
                    grilla.BackgroundColor = Color.White;
                    grilla.RowHeadersVisible = false;

                    // Vincular los datos consultados con la grilla.
                    grilla.DataSource = historial;

                    // Agregar la grilla y abrir la ventana modal.
                    formulario.Controls.Add(grilla);
                    formulario.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                // Mostrar el error si falla la consulta del historial.
                MessageBox.Show(
                    "No se pudo cargar el historial de restauraciones.\n\n" +
                    ex.Message,
                    "Error de consulta",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Ejecuta una prueba manual de restauración en AuraBeautyDB_Prueba2.
        /// Este botón es temporal y no forma parte de la autorización normal.
        /// </summary>
        private void btnProbarRestauracion_Click(object sender, EventArgs e)
        {
            // Ruta del backup que ya se verificó desde SQL Server Management Studio.
            string rutaBackup =
                @"C:\Users\user\OneDrive\Desktop\AuraBeautyDB_20261026_210938_779.bak";

            // Confirmar que se entiende que la prueba restaura una base separada.
            DialogResult confirmacion = MessageBox.Show(
                "Se intentará restaurar el backup en la base de prueba " +
                "AuraBeautyDB_Prueba2.\n\n" +
                "No se restaurará AuraBeautyDB.\n\n" +
                "Si AuraBeautyDB_Prueba2 ya existe, la operación puede fallar. " +
                "¿Querés continuar?",
                "Prueba de restauración",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirmacion != DialogResult.Yes)
                return;

            try
            {
                // Evitar iniciar varias restauraciones desde el botón.
                btnProbarRestauracion.Enabled = false;
                lblEstado.Text = "Restaurando backup en la base de prueba...";

                // Ejecutar la validación de Negocio y la restauración de Datos.
                objCN_Backup.RestaurarBackupEnBasePrueba(rutaBackup);

                lblEstado.Text = "Restauración de prueba completada.";

                MessageBox.Show(
                    "La restauración de prueba terminó correctamente.\n\n" +
                    "Base: AuraBeautyDB_Prueba2",
                    "Prueba exitosa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblEstado.Text = "Falló la restauración de prueba.";

                MessageBox.Show(
                    "No se pudo completar la restauración de prueba.\n\n" +
                    ex.Message,
                    "Error de restauración",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                // Volver a habilitar el botón al finalizar.
                btnProbarRestauracion.Enabled = true;
            }
        }

    }
}
