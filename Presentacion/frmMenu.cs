using System;
using System.Drawing;
using System.Windows.Forms;
using Entidades; // Importante para reconocer la clase Usuario de tu capa de entidades

namespace Presentacion
{
    public partial class frmMenu: Form
    {
        private Usuario usuarioActual;

        // Constructor que recibe el usuario logueado desde el Login
        public frmMenu  (Usuario usuario)
        {
            this.usuarioActual = usuario;
            AplicarEstiloMenu();
        }

        private void AplicarEstiloMenu()
        {
            // Configuración básica de la ventana
            this.Text = "Aura Beauty - Panel de Administración";
            this.ClientSize = new Size(850, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(250, 246, 245);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Paleta de colores estética (Aura Beauty)
            Color textoOscuro = Color.FromArgb(45, 45, 45);
            Color acentoBoton = Color.FromArgb(190, 140, 150);
            Color fondoBotonSecundario = Color.FromArgb(235, 220, 222);

            // Tipografías
            Font fontTitulo = new Font("Times New Roman", 22F, FontStyle.Bold);
            Font fontBienvenida = new Font("Times New Roman", 12F, FontStyle.Italic);
            Font fontBoton = new Font("Times New Roman", 12F, FontStyle.Bold);

            // Encabezado
            Label lblTitulo = new Label
            {
                Text = "Aura Beauty",
                Font = fontTitulo,
                ForeColor = textoOscuro,
                AutoSize = true,
                Location = new Point(40, 30)
            };

            // Muestra el nombre real del usuario que ingresó con éxito
            string nombreMostrar = usuarioActual != null ? $"{usuarioActual.Nombre} {usuarioActual.Apellido}" : "Administrador";
            string rolMostrar = usuarioActual?.oRol?.Descripcion ?? "Administrador";

            Label lblBienvenida = new Label
            {
                Text = $"Sesión: {nombreMostrar} ({rolMostrar})",
                Font = fontBienvenida,
                ForeColor = Color.FromArgb(100, 95, 95),
                AutoSize = true,
                Location = new Point(42, 70)
            };

            this.Controls.Add(lblTitulo);
            this.Controls.Add(lblBienvenida);

            // Botones de Módulos del Administrador
            string[] modulosAdmin = {
                "Gestión de Usuarios y Roles",
                "Catálogo de Productos",
                "Control de Stock",
                "Registro de Ventas",
                "Gestión de Clientes",
                "Reportes y Estadísticas"
            };

            int startX = 60;
            int startY = 130;
            int buttonWidth = 330;
            int buttonHeight = 60;
            int gapX = 40;
            int gapY = 25;

            for (int i = 0; i < modulosAdmin.Length; i++)
            {
                int col = i % 2;
                int row = i / 2;

                Button btn = new Button
                {
                    Text = modulosAdmin[i],
                    Font = fontBoton,
                    BackColor = acentoBoton,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Size = new Size(buttonWidth, buttonHeight),
                    Location = new Point(startX + col * (buttonWidth + gapX), startY + row * (buttonHeight + gapY)),
                    Tag = i + 1
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.Click += BotonModulo_Click;

                this.Controls.Add(btn);
            }

            // Botón Cerrar Sesión
            Button btnCerrarSesion = new Button
            {
                Text = "Cerrar Sesión",
                Font = new Font("Times New Roman", 10.5F, FontStyle.Regular),
                BackColor = fondoBotonSecundario,
                ForeColor = textoOscuro,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Size = new Size(130, 35),
                Location = new Point(this.ClientSize.Width - 170, 30)
            };
            btnCerrarSesion.FlatAppearance.BorderSize = 0;
            btnCerrarSesion.Click += (s, e) =>
            {
                this.Close();
            };

            this.Controls.Add(btnCerrarSesion);
        }

        private void BotonModulo_Click(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.Tag is int opcion)
            {
                switch (opcion)
                {
                    case 1: // Gestión de Usuarios
                        // Si ya tienes tu formulario de usuarios creado, puedes descomentar esto:
                        // FormUsuarios formUsuarios = new FormUsuarios();
                        // formUsuarios.ShowDialog();
                        MessageBox.Show("Módulo: Gestión de Usuarios y Roles", "Aura Beauty", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    case 2:
                        MessageBox.Show("Módulo: Catálogo de Productos", "Aura Beauty", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    default:
                        MessageBox.Show($"Abriendo {btn.Text}...", "Aura Beauty", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                }
            }
        }

        

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            Application.Exit(); // Cierra todo el sistema al salir del menú
        }

        private void frmMenu_Load(object sender, EventArgs e)
        {
            // Código de carga opcional
        }
    }
}