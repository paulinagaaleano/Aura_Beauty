using Entidades;
using Negocio;
using Presentacion;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Aura_Beauty
{
    public partial class frmLogin : Form
    {
        // Declaramos los controles manualmente
        private Label lblTitulo;
        private Label lblCorreo;
        private TextBox txtCorreo;
        private Label lblContraseña;
        private TextBox txtContraseña;
        private Button btnIngresar;

        // Constructor único
        public frmLogin()
        {
            // Configuramos las propiedades básicas de la ventana directamente
            this.ClientSize = new Size(700, 550);
            this.Text = "Aura Beauty - Iniciar Sesión";
            this.Name = "frmLogin";

            CrearControlesManualmente();
        }

        private void CrearControlesManualmente()
        {
            // Estilo general de la ventana
            this.BackColor = Color.FromArgb(250, 246, 245);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            Color textoOscuro = Color.FromArgb(45, 45, 45);
            Color acentoBoton = Color.FromArgb(190, 140, 150);

            // 1. Título
            lblTitulo = new Label();
            lblTitulo.Text = "Aura Beauty";
            lblTitulo.Font = new Font("Times New Roman", 26F, FontStyle.Bold);
            lblTitulo.ForeColor = textoOscuro;
            lblTitulo.BackColor = Color.Transparent;
            lblTitulo.AutoSize = true;
            lblTitulo.Location = new Point((this.ClientSize.Width - 200) / 2, 60);
            this.Controls.Add(lblTitulo);

            // 2. Correo
            lblCorreo = new Label();
            lblCorreo.Text = "Correo:";
            lblCorreo.Font = new Font("Times New Roman", 13F, FontStyle.Regular);
            lblCorreo.ForeColor = textoOscuro;
            lblCorreo.Location = new Point(110, 184);
            lblCorreo.AutoSize = true;
            this.Controls.Add(lblCorreo);

            txtCorreo = new TextBox();
            txtCorreo.Name = "txtCorreo";
            txtCorreo.Font = new Font("Times New Roman", 13F, FontStyle.Regular);
            txtCorreo.Location = new Point(350, 180);
            txtCorreo.Size = new Size(280, 28);
            this.Controls.Add(txtCorreo);

            // 3. Contraseña
            lblContraseña = new Label();
            lblContraseña.Text = "Contraseña:";
            lblContraseña.Font = new Font("Times New Roman", 13F, FontStyle.Regular);
            lblContraseña.ForeColor = textoOscuro;
            lblContraseña.Location = new Point(110, 254);
            lblContraseña.AutoSize = true;
            this.Controls.Add(lblContraseña);

            txtContraseña = new TextBox();
            txtContraseña.Name = "txtContraseña";
            txtContraseña.Font = new Font("Times New Roman", 13F, FontStyle.Regular);
            txtContraseña.UseSystemPasswordChar = true;
            txtContraseña.Location = new Point(350, 250);
            txtContraseña.Size = new Size(280, 28);
            this.Controls.Add(txtContraseña);

            // 4. Botón Ingresar
            btnIngresar = new Button();
            btnIngresar.Name = "btnIngresar";
            btnIngresar.Text = "Ingresar";
            btnIngresar.Font = new Font("Times New Roman", 13F, FontStyle.Bold);
            btnIngresar.BackColor = acentoBoton;
            btnIngresar.ForeColor = Color.White;
            btnIngresar.FlatStyle = FlatStyle.Flat;
            btnIngresar.FlatAppearance.BorderSize = 0;
            btnIngresar.Cursor = Cursors.Hand;
            btnIngresar.Size = new Size(220, 48);
            btnIngresar.Location = new Point((this.ClientSize.Width - 220) / 2, 360);

            // Conexión limpia del evento del botón
            btnIngresar.Click += (sender, e) => {
                string correo = txtCorreo.Text.Trim();
                string clave = txtContraseña.Text.Trim();

                if (string.IsNullOrEmpty(correo) || string.IsNullOrEmpty(clave))
                {
                    MessageBox.Show("Por favor complete su correo y contraseña.", "Campos obligatorios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<Usuario> listaUsuarios = new CN_Usuario().Listar();
                Usuario empleadoActual = listaUsuarios.Find(u => u.Correo.Trim().Equals(correo, StringComparison.OrdinalIgnoreCase) && u.Contraseña.Trim() == clave);

                if (empleadoActual != null)
                {
                    MessageBox.Show($"¡Bienvenido/a {empleadoActual.Nombre} {empleadoActual.Apellido}!", "Aura Beauty", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    frmMenu formMenu = new frmMenu(empleadoActual);
                    formMenu.Show();

                    // 2. Ocultamos el login actual
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Acceso denegado. Verificá tu correo o contraseña.", "Error de inicio de sesión", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtContraseña.Clear();
                    txtContraseña.Focus();
                }
            };

            this.Controls.Add(btnIngresar);

            // Configurar tecla Enter
            this.AcceptButton = btnIngresar;
           
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // frmLogin
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "frmLogin";
            this.Load += new System.EventHandler(this.frmLogin_Load);
            this.ResumeLayout(false);

        }

        private void frmLogin_Load(object sender, EventArgs e)
        {

        }
    }
}