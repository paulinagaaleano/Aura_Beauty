namespace Aura_Beauty
{
    partial class frmStock
    {
        /// <summary>
        /// Contenedor de componentes utilizado por Windows Forms.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        /// <summary>
        /// Libera los recursos utilizados por el formulario.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }


        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Inicializa la estructura básica del formulario.
        ///
        /// Los controles específicos de Aura Beauty se crean
        /// posteriormente desde frmStock.cs.
        /// </summary>
        private void InitializeComponent()
        {
            this.components =
                new System.ComponentModel.Container();

            this.SuspendLayout();

            // 
            // frmStock
            // 
            this.AutoScaleDimensions =
                new System.Drawing.SizeF(7F, 15F);

            this.AutoScaleMode =
                System.Windows.Forms.AutoScaleMode.Font;

            this.ClientSize =
                new System.Drawing.Size(1084, 641);

            this.Name =
                "frmStock";

            this.Text =
                "Aura Beauty - Gestión de Stock";

            this.ResumeLayout(false);
        }

        #endregion
    }
}