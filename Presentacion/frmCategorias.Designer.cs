namespace Aura_Beauty
{
    partial class frmCategorias
    {
        /// <summary>
        /// Contenedor de componentes utilizado
        /// por Windows Forms.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        /// <summary>
        /// Libera los recursos utilizados
        /// por el formulario.
        /// </summary>
        protected override void Dispose(
            bool disposing)
        {
            if (
                disposing &&
                (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }


        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Inicializa la estructura básica
        /// del formulario.
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
                    1000,
                    620
                );

            this.Name =
                "frmCategorias";

            this.Text =
                "Aura Beauty - Gestión de Categorías";


            this.ResumeLayout(false);
        }

        #endregion
    }
}