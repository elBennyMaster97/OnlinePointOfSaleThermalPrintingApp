using System;

namespace OnlineReceiptPrintingApp
{
    partial class frmConfig
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.txtNombreLogo = new System.Windows.Forms.TextBox();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbImpresoraRecibos = new System.Windows.Forms.ComboBox();
            this.cmbImpresoraEtiquetas = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbCortarPapel = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbAbrirCajon = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbSaltos = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmbRecibosSize = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbEtiquetasSize = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Logo";
            // 
            // txtNombreLogo
            // 
            this.txtNombreLogo.Enabled = false;
            this.txtNombreLogo.Location = new System.Drawing.Point(12, 66);
            this.txtNombreLogo.Name = "txtNombreLogo";
            this.txtNombreLogo.Size = new System.Drawing.Size(270, 33);
            this.txtNombreLogo.TabIndex = 1;
            // 
            // btnBuscar
            // 
            this.btnBuscar.Location = new System.Drawing.Point(172, 25);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(110, 35);
            this.btnBuscar.TabIndex = 2;
            this.btnBuscar.Text = "BUSCAR";
            this.btnBuscar.UseVisualStyleBackColor = true;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // picLogo
            // 
            this.picLogo.Location = new System.Drawing.Point(303, 25);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(133, 74);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picLogo.TabIndex = 3;
            this.picLogo.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(200, 26);
            this.label2.TabIndex = 6;
            this.label2.Text = "Impresora de Tickets";
            // 
            // cmbImpresoraRecibos
            // 
            this.cmbImpresoraRecibos.FormattingEnabled = true;
            this.cmbImpresoraRecibos.Location = new System.Drawing.Point(12, 151);
            this.cmbImpresoraRecibos.Name = "cmbImpresoraRecibos";
            this.cmbImpresoraRecibos.Size = new System.Drawing.Size(265, 34);
            this.cmbImpresoraRecibos.TabIndex = 7;
            // 
            // cmbImpresoraEtiquetas
            // 
            this.cmbImpresoraEtiquetas.FormattingEnabled = true;
            this.cmbImpresoraEtiquetas.Location = new System.Drawing.Point(12, 229);
            this.cmbImpresoraEtiquetas.Name = "cmbImpresoraEtiquetas";
            this.cmbImpresoraEtiquetas.Size = new System.Drawing.Size(265, 34);
            this.cmbImpresoraEtiquetas.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 200);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(220, 26);
            this.label3.TabIndex = 8;
            this.label3.Text = "Impresora de Etiquetas";
            // 
            // cmbCortarPapel
            // 
            this.cmbCortarPapel.FormattingEnabled = true;
            this.cmbCortarPapel.Items.AddRange(new object[] {
            "SI",
            "NO"});
            this.cmbCortarPapel.Location = new System.Drawing.Point(12, 309);
            this.cmbCortarPapel.Name = "cmbCortarPapel";
            this.cmbCortarPapel.Size = new System.Drawing.Size(124, 34);
            this.cmbCortarPapel.TabIndex = 11;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 280);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(124, 26);
            this.label4.TabIndex = 10;
            this.label4.Text = "Cortar Papel";
            // 
            // cmbAbrirCajon
            // 
            this.cmbAbrirCajon.FormattingEnabled = true;
            this.cmbAbrirCajon.Items.AddRange(new object[] {
            "SI",
            "NO"});
            this.cmbAbrirCajon.Location = new System.Drawing.Point(158, 309);
            this.cmbAbrirCajon.Name = "cmbAbrirCajon";
            this.cmbAbrirCajon.Size = new System.Drawing.Size(124, 34);
            this.cmbAbrirCajon.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(153, 280);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(119, 26);
            this.label5.TabIndex = 12;
            this.label5.Text = "Abrir Cajón";
            // 
            // cmbSaltos
            // 
            this.cmbSaltos.FormattingEnabled = true;
            this.cmbSaltos.Items.AddRange(new object[] {
            "SI",
            "NO"});
            this.cmbSaltos.Location = new System.Drawing.Point(301, 309);
            this.cmbSaltos.Name = "cmbSaltos";
            this.cmbSaltos.Size = new System.Drawing.Size(135, 34);
            this.cmbSaltos.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(296, 280);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(148, 26);
            this.label6.TabIndex = 14;
            this.label6.Text = "Saltos de Línea";
            // 
            // cmbRecibosSize
            // 
            this.cmbRecibosSize.FormattingEnabled = true;
            this.cmbRecibosSize.Items.AddRange(new object[] {
            "58MM",
            "80MM"});
            this.cmbRecibosSize.Location = new System.Drawing.Point(301, 151);
            this.cmbRecibosSize.Name = "cmbRecibosSize";
            this.cmbRecibosSize.Size = new System.Drawing.Size(124, 34);
            this.cmbRecibosSize.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(296, 122);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(61, 26);
            this.label7.TabIndex = 16;
            this.label7.Text = "Papel";
            // 
            // cmbEtiquetasSize
            // 
            this.cmbEtiquetasSize.FormattingEnabled = true;
            this.cmbEtiquetasSize.Items.AddRange(new object[] {
            "52X25",
            "4X6",
            "6X4",
            "4X3"});
            this.cmbEtiquetasSize.Location = new System.Drawing.Point(301, 229);
            this.cmbEtiquetasSize.Name = "cmbEtiquetasSize";
            this.cmbEtiquetasSize.Size = new System.Drawing.Size(124, 34);
            this.cmbEtiquetasSize.TabIndex = 19;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(296, 200);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 26);
            this.label8.TabIndex = 18;
            this.label8.Text = "Papel";
            // 
            // btnCerrar
            // 
            this.btnCerrar.Location = new System.Drawing.Point(12, 364);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(105, 35);
            this.btnCerrar.TabIndex = 20;
            this.btnCerrar.Text = "CERRAR";
            this.btnCerrar.UseVisualStyleBackColor = true;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.Location = new System.Drawing.Point(301, 364);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(135, 35);
            this.btnGuardar.TabIndex = 21;
            this.btnGuardar.Text = "GUARDAR";
            this.btnGuardar.UseVisualStyleBackColor = true;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // frmConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 26F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(453, 411);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.cmbEtiquetasSize);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cmbRecibosSize);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cmbSaltos);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbAbrirCajon);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmbCortarPapel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbImpresoraEtiquetas);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbImpresoraRecibos);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.btnBuscar);
            this.Controls.Add(this.txtNombreLogo);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Palatino Linotype", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "frmConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuración | OnRePriApp | BHERSEG - Freelance Website Development";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmConfig_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtNombreLogo;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbImpresoraRecibos;
        private System.Windows.Forms.ComboBox cmbImpresoraEtiquetas;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbCortarPapel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbAbrirCajon;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbSaltos;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmbRecibosSize;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbEtiquetasSize;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}