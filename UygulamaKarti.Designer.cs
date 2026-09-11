namespace PcProgramKurucu
{
    partial class UygulamaKarti
    {
        /// <summary> 
        ///Gerekli tasarımcı değişkeni.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        ///Kullanılan tüm kaynakları temizleyin.
        /// </summary>
        ///<param name="disposing">yönetilen kaynaklar dispose edilmeliyse doğru; aksi halde yanlış.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Bileşen Tasarımcısı üretimi kod

        /// <summary> 
        /// Tasarımcı desteği için gerekli metot - bu metodun 
        ///içeriğini kod düzenleyici ile değiştirmeyin.
        /// </summary>
        private void InitializeComponent()
        {
            this.guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            this.btnIndir = new Guna.UI2.WinForms.Guna2Button();
            this.lblAd = new System.Windows.Forms.Label();
            this.picLogo = new Guna.UI2.WinForms.Guna2PictureBox();
            this.guna2Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // guna2Panel1
            // 
            this.guna2Panel1.BackColor = System.Drawing.Color.Transparent;
            this.guna2Panel1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.guna2Panel1.BorderRadius = 20;
            this.guna2Panel1.BorderThickness = 2;
            this.guna2Panel1.Controls.Add(this.btnIndir);
            this.guna2Panel1.Controls.Add(this.lblAd);
            this.guna2Panel1.Controls.Add(this.picLogo);
            this.guna2Panel1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.guna2Panel1.Location = new System.Drawing.Point(19, 20);
            this.guna2Panel1.Name = "guna2Panel1";
            this.guna2Panel1.ShadowDecoration.BorderRadius = 20;
            this.guna2Panel1.ShadowDecoration.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.guna2Panel1.ShadowDecoration.Enabled = true;
            this.guna2Panel1.Size = new System.Drawing.Size(124, 172);
            this.guna2Panel1.TabIndex = 0;
            // 
            // btnIndir
            // 
            this.btnIndir.BackColor = System.Drawing.Color.Transparent;
            this.btnIndir.BorderRadius = 5;
            this.btnIndir.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnIndir.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnIndir.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnIndir.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnIndir.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnIndir.Font = new System.Drawing.Font("Impact", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.btnIndir.ForeColor = System.Drawing.Color.White;
            this.btnIndir.Location = new System.Drawing.Point(24, 131);
            this.btnIndir.Name = "btnIndir";
            this.btnIndir.ShadowDecoration.BorderRadius = 10;
            this.btnIndir.ShadowDecoration.Color = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnIndir.ShadowDecoration.Enabled = true;
            this.btnIndir.Size = new System.Drawing.Size(76, 24);
            this.btnIndir.TabIndex = 2;
            this.btnIndir.Text = "İNDİR";
            // 
            // lblAd
            // 
            this.lblAd.Font = new System.Drawing.Font("Impact", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblAd.ForeColor = System.Drawing.Color.White;
            this.lblAd.Location = new System.Drawing.Point(3, 103);
            this.lblAd.Name = "lblAd";
            this.lblAd.Size = new System.Drawing.Size(118, 16);
            this.lblAd.TabIndex = 1;
            this.lblAd.Text = "label1";
            this.lblAd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // picLogo
            // 
            this.picLogo.ImageRotate = 0F;
            this.picLogo.Location = new System.Drawing.Point(24, 15);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new System.Drawing.Size(76, 78);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 0;
            this.picLogo.TabStop = false;
            // 
            // UygulamaKarti
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.guna2Panel1);
            this.Name = "UygulamaKarti";
            this.Size = new System.Drawing.Size(163, 214);
            this.guna2Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2Button btnIndir;
        private System.Windows.Forms.Label lblAd;
        private Guna.UI2.WinForms.Guna2PictureBox picLogo;
    }
}
