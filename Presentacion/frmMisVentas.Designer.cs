namespace Aura_Beauty
{
    partial class frmMisVentas
    {
        /// <summary>
        /// Contenedor de componentes utilizado por Windows Forms.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        /// <summary>
        /// Libera los recursos utilizados por el formulario.
        ///
        /// Dispose forma parte del ciclo de vida estándar
        /// de los formularios de Windows Forms.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }


        #region Código generado para la interfaz


        /// <summary>
        /// Crea y configura los controles visuales
        /// utilizados por frmMisVentas.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelEncabezado =
                new System.Windows.Forms.Panel();

            this.lblTitulo =
                new System.Windows.Forms.Label();

            this.lblVendedor =
                new System.Windows.Forms.Label();

            this.panelFiltros =
                new System.Windows.Forms.Panel();

            this.lblDesde =
                new System.Windows.Forms.Label();

            this.dtpDesde =
                new System.Windows.Forms.DateTimePicker();

            this.lblHasta =
                new System.Windows.Forms.Label();

            this.dtpHasta =
                new System.Windows.Forms.DateTimePicker();

            this.btnBuscar =
                new System.Windows.Forms.Button();

            this.btnLimpiar =
                new System.Windows.Forms.Button();

            this.dgvVentas =
                new System.Windows.Forms.DataGridView();

            this.colVenta =
                new System.Windows.Forms.DataGridViewTextBoxColumn();

            this.colFecha =
                new System.Windows.Forms.DataGridViewTextBoxColumn();

            this.colComprobante =
                new System.Windows.Forms.DataGridViewTextBoxColumn();

            this.colCliente =
                new System.Windows.Forms.DataGridViewTextBoxColumn();

            this.colTotal =
                new System.Windows.Forms.DataGridViewTextBoxColumn();

            this.lblCantidad =
                new System.Windows.Forms.Label();

            this.lblTotal =
                new System.Windows.Forms.Label();


            this.panelEncabezado.SuspendLayout();

            this.panelFiltros.SuspendLayout();

            ((System.ComponentModel.ISupportInitialize)
                (this.dgvVentas)).BeginInit();

            this.SuspendLayout();


            // =====================================================
            // panelEncabezado
            // =====================================================

            this.panelEncabezado.BackColor =
                System.Drawing.Color.FromArgb(
                    201,
                    143,
                    149
                );

            this.panelEncabezado.Controls.Add(
                this.lblTitulo
            );

            this.panelEncabezado.Controls.Add(
                this.lblVendedor
            );

            this.panelEncabezado.Dock =
                System.Windows.Forms.DockStyle.Top;

            this.panelEncabezado.Location =
                new System.Drawing.Point(0, 0);

            this.panelEncabezado.Name =
                "panelEncabezado";

            this.panelEncabezado.Size =
                new System.Drawing.Size(1084, 115);


            // =====================================================
            // lblTitulo
            // =====================================================

            this.lblTitulo.AutoSize =
                true;

            this.lblTitulo.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    24F,
                    System.Drawing.FontStyle.Bold
                );

            this.lblTitulo.ForeColor =
                System.Drawing.Color.White;

            this.lblTitulo.Location =
                new System.Drawing.Point(30, 18);

            this.lblTitulo.Name =
                "lblTitulo";

            this.lblTitulo.Text =
                "MIS VENTAS";


            // =====================================================
            // lblVendedor
            // =====================================================

            this.lblVendedor.AutoSize =
                true;

            this.lblVendedor.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    11F
                );

            this.lblVendedor.ForeColor =
                System.Drawing.Color.White;

            this.lblVendedor.Location =
                new System.Drawing.Point(33, 72);

            this.lblVendedor.Name =
                "lblVendedor";

            this.lblVendedor.Text =
                "Vendedor:";


            // =====================================================
            // panelFiltros
            // =====================================================

            this.panelFiltros.BackColor =
                System.Drawing.Color.White;

            this.panelFiltros.Controls.Add(
                this.lblDesde
            );

            this.panelFiltros.Controls.Add(
                this.dtpDesde
            );

            this.panelFiltros.Controls.Add(
                this.lblHasta
            );

            this.panelFiltros.Controls.Add(
                this.dtpHasta
            );

            this.panelFiltros.Controls.Add(
                this.btnBuscar
            );

            this.panelFiltros.Controls.Add(
                this.btnLimpiar
            );

            this.panelFiltros.Location =
                new System.Drawing.Point(20, 135);

            this.panelFiltros.Name =
                "panelFiltros";

            this.panelFiltros.Size =
                new System.Drawing.Size(1040, 105);


            // =====================================================
            // lblDesde
            // =====================================================

            this.lblDesde.AutoSize =
                true;

            this.lblDesde.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F,
                    System.Drawing.FontStyle.Bold
                );

            this.lblDesde.ForeColor =
                System.Drawing.Color.FromArgb(
                    94,
                    74,
                    74
                );

            this.lblDesde.Location =
                new System.Drawing.Point(20, 15);

            this.lblDesde.Text =
                "Desde";


            // =====================================================
            // dtpDesde
            // =====================================================

            this.dtpDesde.Format =
                System.Windows.Forms.DateTimePickerFormat.Short;

            this.dtpDesde.Location =
                new System.Drawing.Point(20, 45);

            this.dtpDesde.Name =
                "dtpDesde";

            this.dtpDesde.Size =
                new System.Drawing.Size(180, 25);


            // =====================================================
            // lblHasta
            // =====================================================

            this.lblHasta.AutoSize =
                true;

            this.lblHasta.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F,
                    System.Drawing.FontStyle.Bold
                );

            this.lblHasta.ForeColor =
                System.Drawing.Color.FromArgb(
                    94,
                    74,
                    74
                );

            this.lblHasta.Location =
                new System.Drawing.Point(225, 15);

            this.lblHasta.Text =
                "Hasta";


            // =====================================================
            // dtpHasta
            // =====================================================

            this.dtpHasta.Format =
                System.Windows.Forms.DateTimePickerFormat.Short;

            this.dtpHasta.Location =
                new System.Drawing.Point(225, 45);

            this.dtpHasta.Name =
                "dtpHasta";

            this.dtpHasta.Size =
                new System.Drawing.Size(180, 25);


            // =====================================================
            // btnBuscar
            // =====================================================

            this.btnBuscar.BackColor =
                System.Drawing.Color.FromArgb(
                    201,
                    143,
                    149
                );

            this.btnBuscar.Cursor =
                System.Windows.Forms.Cursors.Hand;

            this.btnBuscar.FlatAppearance.BorderSize =
                0;

            this.btnBuscar.FlatStyle =
                System.Windows.Forms.FlatStyle.Flat;

            this.btnBuscar.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F,
                    System.Drawing.FontStyle.Bold
                );

            this.btnBuscar.ForeColor =
                System.Drawing.Color.White;

            this.btnBuscar.Location =
                new System.Drawing.Point(455, 38);

            this.btnBuscar.Name =
                "btnBuscar";

            this.btnBuscar.Size =
                new System.Drawing.Size(145, 38);

            this.btnBuscar.Text =
                "BUSCAR";

            this.btnBuscar.UseVisualStyleBackColor =
                false;

            this.btnBuscar.Click +=
                new System.EventHandler(
                    this.btnBuscar_Click
                );


            // =====================================================
            // btnLimpiar
            // =====================================================

            this.btnLimpiar.BackColor =
                System.Drawing.Color.FromArgb(
                    201,
                    143,
                    149
                );

            this.btnLimpiar.Cursor =
                System.Windows.Forms.Cursors.Hand;

            this.btnLimpiar.FlatAppearance.BorderSize =
                0;

            this.btnLimpiar.FlatStyle =
                System.Windows.Forms.FlatStyle.Flat;

            this.btnLimpiar.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F,
                    System.Drawing.FontStyle.Bold
                );

            this.btnLimpiar.ForeColor =
                System.Drawing.Color.White;

            this.btnLimpiar.Location =
                new System.Drawing.Point(620, 38);

            this.btnLimpiar.Name =
                "btnLimpiar";

            this.btnLimpiar.Size =
                new System.Drawing.Size(145, 38);

            this.btnLimpiar.Text =
                "LIMPIAR";

            this.btnLimpiar.UseVisualStyleBackColor =
                false;

            this.btnLimpiar.Click +=
                new System.EventHandler(
                    this.btnLimpiar_Click
                );


            // =====================================================
            // dgvVentas
            // =====================================================

            this.dgvVentas.AllowUserToAddRows =
                false;

            this.dgvVentas.AllowUserToDeleteRows =
                false;

            this.dgvVentas.AllowUserToResizeRows =
                false;

            this.dgvVentas.AutoGenerateColumns =
                false;

            this.dgvVentas.AutoSizeColumnsMode =
                System.Windows.Forms
                .DataGridViewAutoSizeColumnsMode.Fill;

            this.dgvVentas.BackgroundColor =
                System.Drawing.Color.White;

            this.dgvVentas.BorderStyle =
                System.Windows.Forms.BorderStyle.None;

            this.dgvVentas.ColumnHeadersHeightSizeMode =
                System.Windows.Forms
                .DataGridViewColumnHeadersHeightSizeMode
                .AutoSize;

            this.dgvVentas.Columns.AddRange(
                new System.Windows.Forms.DataGridViewColumn[]
                {
                    this.colVenta,
                    this.colFecha,
                    this.colComprobante,
                    this.colCliente,
                    this.colTotal
                }
            );

            this.dgvVentas.Location =
                new System.Drawing.Point(20, 260);

            this.dgvVentas.MultiSelect =
                false;

            this.dgvVentas.Name =
                "dgvVentas";

            this.dgvVentas.ReadOnly =
                true;

            this.dgvVentas.RowHeadersVisible =
                false;

            this.dgvVentas.SelectionMode =
                System.Windows.Forms
                .DataGridViewSelectionMode
                .FullRowSelect;

            this.dgvVentas.Size =
                new System.Drawing.Size(1040, 315);


            // =====================================================
            // colVenta
            // =====================================================

            this.colVenta.DataPropertyName =
                "IdVenta";

            this.colVenta.FillWeight =
                55F;

            this.colVenta.HeaderText =
                "Venta";

            this.colVenta.Name =
                "colVenta";

            this.colVenta.ReadOnly =
                true;


            // =====================================================
            // colFecha
            // =====================================================

            this.colFecha.DataPropertyName =
                "FechaVenta";

            this.colFecha.DefaultCellStyle.Format =
                "dd/MM/yyyy HH:mm";

            this.colFecha.FillWeight =
                110F;

            this.colFecha.HeaderText =
                "Fecha y hora";

            this.colFecha.Name =
                "colFecha";

            this.colFecha.ReadOnly =
                true;


            // =====================================================
            // colComprobante
            // =====================================================

            this.colComprobante.DataPropertyName =
                "NroFactura";

            this.colComprobante.FillWeight =
                140F;

            this.colComprobante.HeaderText =
                "Comprobante";

            this.colComprobante.Name =
                "colComprobante";

            this.colComprobante.ReadOnly =
                true;


            // =====================================================
            // colCliente
            // =====================================================

            this.colCliente.DataPropertyName =
                "Cliente";

            this.colCliente.FillWeight =
                150F;

            this.colCliente.HeaderText =
                "Cliente";

            this.colCliente.Name =
                "colCliente";

            this.colCliente.ReadOnly =
                true;


            // =====================================================
            // colTotal
            // =====================================================

            this.colTotal.DataPropertyName =
                "Total";

            this.colTotal.DefaultCellStyle.Format =
                "C2";

            this.colTotal.FillWeight =
                90F;

            this.colTotal.HeaderText =
                "Total";

            this.colTotal.Name =
                "colTotal";

            this.colTotal.ReadOnly =
                true;


            // =====================================================
            // lblCantidad
            // =====================================================

            this.lblCantidad.AutoSize =
                true;

            this.lblCantidad.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    11F,
                    System.Drawing.FontStyle.Bold
                );

            this.lblCantidad.ForeColor =
                System.Drawing.Color.FromArgb(
                    94,
                    74,
                    74
                );

            this.lblCantidad.Location =
                new System.Drawing.Point(25, 600);

            this.lblCantidad.Name =
                "lblCantidad";

            this.lblCantidad.Text =
                "Cantidad de ventas: 0";


            // =====================================================
            // lblTotal
            // =====================================================

            this.lblTotal.AutoSize =
                true;

            this.lblTotal.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    14F,
                    System.Drawing.FontStyle.Bold
                );

            this.lblTotal.ForeColor =
                System.Drawing.Color.FromArgb(
                    174,
                    112,
                    120
                );

            this.lblTotal.Location =
                new System.Drawing.Point(760, 595);

            this.lblTotal.Name =
                "lblTotal";

            this.lblTotal.Text =
                "Total vendido: $ 0,00";


            // =====================================================
            // frmMisVentas
            // =====================================================

            this.AutoScaleDimensions =
                new System.Drawing.SizeF(7F, 15F);

            this.AutoScaleMode =
                System.Windows.Forms.AutoScaleMode.Font;

            this.BackColor =
                System.Drawing.Color.FromArgb(
                    255,
                    249,
                    248
                );

            this.ClientSize =
                new System.Drawing.Size(1084, 651);

            this.Controls.Add(
                this.lblTotal
            );

            this.Controls.Add(
                this.lblCantidad
            );

            this.Controls.Add(
                this.dgvVentas
            );

            this.Controls.Add(
                this.panelFiltros
            );

            this.Controls.Add(
                this.panelEncabezado
            );

            this.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    9F
                );

            this.FormBorderStyle =
                System.Windows.Forms.FormBorderStyle.FixedSingle;

            this.MaximizeBox =
                false;

            this.Name =
                "frmMisVentas";

            this.StartPosition =
                System.Windows.Forms.FormStartPosition.CenterScreen;

            this.Text =
                "Aura Beauty - Mis Ventas";


            this.panelEncabezado.ResumeLayout(
                false
            );

            this.panelEncabezado.PerformLayout();

            this.panelFiltros.ResumeLayout(
                false
            );

            this.panelFiltros.PerformLayout();

            ((System.ComponentModel.ISupportInitialize)
                (this.dgvVentas)).EndInit();

            this.ResumeLayout(
                false
            );

            this.PerformLayout();
        }


        #endregion


        // =========================================================
        // CONTROLES DEL FORMULARIO
        // =========================================================

        private System.Windows.Forms.Panel panelEncabezado;

        private System.Windows.Forms.Label lblTitulo;

        private System.Windows.Forms.Label lblVendedor;

        private System.Windows.Forms.Panel panelFiltros;

        private System.Windows.Forms.Label lblDesde;

        private System.Windows.Forms.DateTimePicker dtpDesde;

        private System.Windows.Forms.Label lblHasta;

        private System.Windows.Forms.DateTimePicker dtpHasta;

        private System.Windows.Forms.Button btnBuscar;

        private System.Windows.Forms.Button btnLimpiar;

        private System.Windows.Forms.DataGridView dgvVentas;

        private System.Windows.Forms.DataGridViewTextBoxColumn colVenta;

        private System.Windows.Forms.DataGridViewTextBoxColumn colFecha;

        private System.Windows.Forms.DataGridViewTextBoxColumn colComprobante;

        private System.Windows.Forms.DataGridViewTextBoxColumn colCliente;

        private System.Windows.Forms.DataGridViewTextBoxColumn colTotal;

        private System.Windows.Forms.Label lblCantidad;

        private System.Windows.Forms.Label lblTotal;
    }
}