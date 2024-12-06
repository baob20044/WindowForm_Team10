namespace StoreManagerPro.Components
{
    partial class ShopItem
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbName = new System.Windows.Forms.Label();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.lbPrice = new System.Windows.Forms.Label();
            this.pBImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pBImage)).BeginInit();
            this.SuspendLayout();
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbName.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbName.ForeColor = System.Drawing.Color.DimGray;
            this.lbName.Location = new System.Drawing.Point(10, 194);
            this.lbName.MaximumSize = new System.Drawing.Size(300, 0);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(143, 28);
            this.lbName.TabIndex = 9;
            this.lbName.Text = "Product Name";
            this.lbName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbPrice
            // 
            this.lbPrice.AutoSize = true;
            this.lbPrice.Font = new System.Drawing.Font("Segoe UI Semibold", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPrice.ForeColor = System.Drawing.Color.DimGray;
            this.lbPrice.Location = new System.Drawing.Point(117, 244);
            this.lbPrice.Name = "lbPrice";
            this.lbPrice.Size = new System.Drawing.Size(85, 35);
            this.lbPrice.TabIndex = 12;
            this.lbPrice.Text = "$Price";
            this.lbPrice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pBImage
            // 
            this.pBImage.BackColor = System.Drawing.Color.Gainsboro;
            this.pBImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pBImage.Image = global::StoreManagerPro.Properties.Resources.ao_khoac_nam_yody_AKM7015_TIT__4_;
            this.pBImage.Location = new System.Drawing.Point(66, 29);
            this.pBImage.Name = "pBImage";
            this.pBImage.Size = new System.Drawing.Size(197, 162);
            this.pBImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pBImage.TabIndex = 11;
            this.pBImage.TabStop = false;
            // 
            // ShopItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.BorderRadius = 5;
            this.Controls.Add(this.lbPrice);
            this.Controls.Add(this.pBImage);
            this.Controls.Add(this.lbName);
            this.Name = "ShopItem";
            this.Size = new System.Drawing.Size(324, 279);
            ((System.ComponentModel.ISupportInitialize)(this.pBImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.PictureBox pBImage;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label lbPrice;
    }
}
