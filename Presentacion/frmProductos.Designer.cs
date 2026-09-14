namespace Aura_Beauty
{
    partial class frmProductos
    {
        /// <summary>
        /// Variable requerida por el diseñador.
        /// Guarda los componentes visuales administrados por Windows Forms.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Libera los recursos utilizados por el formulario.
        ///
        /// El método Dispose forma parte de Windows Forms.
        /// Si disposing es true y existen componentes,
        /// los libera de memoria.
        /// </summary>
        /// <param name="disposing">
        /// Indica si deben liberarse los recursos administrados.
        /// </param>
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
        /// Inicializa los componentes básicos del formulario.
        ///
        /// En nuestro proyecto la mayoría de los controles
        /// se crean manualmente desde frmProductos.cs,
        /// por eso este método es intencionalmente sencillo.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.SuspendLayout();

            // 
            // frmProductos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);

            this.AutoScaleMode =
                System.Windows.Forms.AutoScaleMode.Font;

            this.ClientSize =
                new System.Drawing.Size(1184, 681);

            this.Name = "frmProductos";

            this.Text = "Aura Beauty - Gestión de Productos";

            this.ResumeLayout(false);
        }

        #endregion
    }
}