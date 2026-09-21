namespace Aura_Beauty
{
    partial class frmCierreCaja
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


        #region Código generado por el Diseñador


        /// <summary>
        /// Crea y configura los controles visuales
        /// del formulario Cierre de Caja.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlEncabezado =
                new System.Windows.Forms.Panel();

            this.lblTitulo =
                new System.Windows.Forms.Label();

            this.lblVendedor =
                new System.Windows.Forms.Label();

            this.pnlConsulta =
                new System.Windows.Forms.Panel();

            this.lblFecha =
                new System.Windows.Forms.Label();

            this.dtpFecha =
                new System.Windows.Forms.DateTimePicker();

            this.btnConsultar =
                new System.Windows.Forms.Button();

            this.lblFechaResumen =
                new System.Windows.Forms.Label();

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


            this.pnlEncabezado.SuspendLayout();

            this.pnlConsulta.SuspendLayout();

            ((System.ComponentModel.ISupportInitialize)
                (this.dgvVentas)).BeginInit();

            this.SuspendLayout();


            // =====================================================
            // pnlEncabezado
            // =====================================================

            this.pnlEncabezado.BackColor =
                System.Drawing.Color.FromArgb(
                    201,
                    143,
                    149
                );

            this.pnlEncabezado.Controls.Add(
                this.lblTitulo
            );

            this.pnlEncabezado.Controls.Add(
                this.lblVendedor
            );

            this.pnlEncabezado.Dock =
                System.Windows.Forms.DockStyle.Top;

            this.pnlEncabezado.Location =
                new System.Drawing.Point(0, 0);

            this.pnlEncabezado.Name =
                "pnlEncabezado";

            this.pnlEncabezado.Size =
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
                "CIERRE DE CAJA";


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
            // pnlConsulta
            // =====================================================

            this.pnlConsulta.BackColor =
                System.Drawing.Color.White;

            this.pnlConsulta.Controls.Add(
                this.lblFecha
            );

            this.pnlConsulta.Controls.Add(
                this.dtpFecha
            );

            this.pnlConsulta.Controls.Add(
                this.btnConsultar
            );

            this.pnlConsulta.Location =
                new System.Drawing.Point(20, 135);

            this.pnlConsulta.Name =
                "pnlConsulta";

            this.pnlConsulta.Size =
                new System.Drawing.Size(1040, 95);


            // =====================================================
            // lblFecha
            // =====================================================

            this.lblFecha.AutoSize =
                true;

            this.lblFecha.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F,
                    System.Drawing.FontStyle.Bold
                );

            this.lblFecha.ForeColor =
                System.Drawing.Color.FromArgb(
                    94,
                    74,
                    74
                );

            this.lblFecha.Location =
                new System.Drawing.Point(20, 15);

            this.lblFecha.Name =
                "lblFecha";

            this.lblFecha.Text =
                "Fecha del cierre";


            // =====================================================
            // dtpFecha
            // =====================================================

            this.dtpFecha.Format =
                System.Windows.Forms.DateTimePickerFormat.Short;

            this.dtpFecha.Location =
                new System.Drawing.Point(20, 45);

            this.dtpFecha.Name =
                "dtpFecha";

            this.dtpFecha.Size =
                new System.Drawing.Size(180, 25);

            this.dtpFecha.ValueChanged +=
                new System.EventHandler(
                    this.dtpFecha_ValueChanged
                );


            // =====================================================
            // btnConsultar
            // =====================================================

            this.btnConsultar.BackColor =
                System.Drawing.Color.FromArgb(
                    201,
                    143,
                    149
                );

            this.btnConsultar.Cursor =
                System.Windows.Forms.Cursors.Hand;

            this.btnConsultar.FlatAppearance.BorderSize =
                0;

            this.btnConsultar.FlatStyle =
                System.Windows.Forms.FlatStyle.Flat;

            this.btnConsultar.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    10F,
                    System.Drawing.FontStyle.Bold
                );

            this.btnConsultar.ForeColor =
                System.Drawing.Color.White;

            this.btnConsultar.Location =
                new System.Drawing.Point(235, 37);

            this.btnConsultar.Name =
                "btnConsultar";

            this.btnConsultar.Size =
                new System.Drawing.Size(160, 38);

            this.btnConsultar.Text =
                "CONSULTAR";

            this.btnConsultar.UseVisualStyleBackColor =
                false;

            this.btnConsultar.Click +=
                new System.EventHandler(
                    this.btnConsultar_Click
                );


            // =====================================================
            // lblFechaResumen
            // =====================================================

            this.lblFechaResumen.AutoSize =
                true;

            this.lblFechaResumen.Font =
                new System.Drawing.Font(
                    "Segoe UI",
                    11F,
                    System.Drawing.FontStyle.Bold
                );

            this.lblFechaResumen.ForeColor =
                System.Drawing.Color.FromArgb(
                    94,
                    74,
                    74
                );

            this.lblFechaResumen.Location =
                new System.Drawing.Point(25, 250);

            this.lblFechaResumen.Name =
                "lblFechaResumen";

            this.lblFechaResumen.Text =
                "Cierre correspondiente al:";


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
                new System.Drawing.Point(20, 290);

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
                new System.Drawing.Size(1040, 285);


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
                "HH:mm";

            this.colFecha.FillWeight =
                80F;

            this.colFecha.HeaderText =
                "Hora";

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
                160F;

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
            // frmCierreCaja
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
                this.lblFechaResumen
            );

            this.Controls.Add(
                this.pnlConsulta
            );

            this.Controls.Add(
                this.pnlEncabezado
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
                "frmCierreCaja";

            this.StartPosition =
                System.Windows.Forms.FormStartPosition.CenterScreen;

            this.Text =
                "Aura Beauty - Cierre de Caja";


            this.pnlEncabezado.ResumeLayout(
                false
            );

            this.pnlEncabezado.PerformLayout();

            this.pnlConsulta.ResumeLayout(
                false
            );

            this.pnlConsulta.PerformLayout();

            ((System.ComponentModel.ISupportInitialize)
                (this.dgvVentas)).EndInit();

            this.ResumeLayout(
                false
            );

            this.PerformLayout();
        }


        #endregion


        // =========================================================
        // CONTROLES
        // =========================================================

        private System.Windows.Forms.Panel pnlEncabezado;

        private System.Windows.Forms.Label lblTitulo;

        private System.Windows.Forms.Label lblVendedor;

        private System.Windows.Forms.Panel pnlConsulta;

        private System.Windows.Forms.Label lblFecha;

        private System.Windows.Forms.DateTimePicker dtpFecha;

        private System.Windows.Forms.Button btnConsultar;

        private System.Windows.Forms.Label lblFechaResumen;

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