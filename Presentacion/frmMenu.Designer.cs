namespace Aura_Beauty
{
    partial class frmMenu
    {
        /// <summary>
        /// Variable requerida por el diseñador.
        ///
        /// components contiene los componentes creados
        /// por el diseñador de Windows Forms, si existieran.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        /// <summary>
        /// Libera los recursos utilizados por el formulario.
        ///
        /// Dispose es un método heredado de Form.
        /// Windows Forms lo utiliza cuando el formulario
        /// deja de necesitarse.
        /// </summary>
        /// <param name="disposing">
        /// true: libera recursos administrados.
        /// false: solamente libera recursos no administrados.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            /*
             * Si disposing es true y existen componentes,
             * los liberamos de memoria.
             */
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            /*
             * También ejecutamos el Dispose
             * de la clase Form de la cual heredamos.
             */
            base.Dispose(disposing);
        }


        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método requerido por el diseñador.
        ///
        /// En nuestro proyecto la interfaz del menú
        /// se crea manualmente desde frmMenu.cs,
        /// por eso aquí no necesitamos agregar controles.
        /// </summary>
        private void InitializeComponent()
        {
            this.components =
                new System.ComponentModel.Container();

            this.SuspendLayout();

            // 
            // frmMenu
            // 
            this.AutoScaleMode =
                System.Windows.Forms.AutoScaleMode.Font;

            this.Name = "frmMenu";

            this.ResumeLayout(false);
        }

        #endregion
    }
}