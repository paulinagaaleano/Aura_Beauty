namespace Aura_Beauty
{
    partial class frmUsuarios
    {
        /// <summary>
        /// Variable requerida por el diseñador.
        ///
        /// IContainer permite mantener una colección
        /// de componentes que deben liberarse cuando
        /// se cierra el formulario.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        /// <summary>
        /// Libera los recursos utilizados por el formulario.
        ///
        /// "override" indica que estamos reemplazando
        /// el comportamiento de un método heredado
        /// desde la clase Form.
        /// </summary>
        /// <param name="disposing">
        /// true cuando deben liberarse los recursos administrados.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (disposing &&
                (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }


        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método utilizado por Windows Forms
        /// para inicializar el formulario.
        ///
        /// En nuestro caso la interfaz visual
        /// se construye principalmente desde frmUsuarios.cs,
        /// para mantener el mismo criterio visual
        /// utilizado en Aura Beauty.
        /// </summary>
        private void InitializeComponent()
        {
            this.components =
                new System.ComponentModel.Container();

            this.SuspendLayout();


            this.AutoScaleMode =
                System.Windows.Forms.AutoScaleMode.Font;

            this.ClientSize =
                new System.Drawing.Size(
                    1050,
                    650
                );

            this.Name =
                "frmUsuarios";

            this.Text =
                "Aura Beauty - Gestión de Usuarios";


            this.ResumeLayout(false);
        }

        #endregion
    }
}