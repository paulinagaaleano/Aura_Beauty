namespace Aura_Beauty
{
    partial class frmVentas
    {
        /// <summary>
        /// Contenedor utilizado por Windows Forms.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        /// <summary>
        /// Libera los recursos utilizados por el formulario.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (
                disposing
                &&
                components != null
            )
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }


        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Inicializa la estructura básica de frmVentas.
        ///
        /// Los controles específicos del Punto de Venta
        /// se construyen desde frmVentas.cs.
        /// </summary>
        private void InitializeComponent()
        {
            this.components =
                new System.ComponentModel.Container();

            this.SuspendLayout();

            // 
            // frmVentas
            // 
            this.AutoScaleDimensions =
                new System.Drawing.SizeF(7F, 15F);

            this.AutoScaleMode =
                System.Windows.Forms.AutoScaleMode.Font;

            this.ClientSize =
                new System.Drawing.Size(1264, 721);

            this.Name =
                "frmVentas";

            this.Text =
                "Aura Beauty - Registro de Ventas";

            this.ResumeLayout(false);
        }

        #endregion
    }
}