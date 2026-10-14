namespace RentOnCars
{
    partial class Welcome_page
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.titleLabel = new System.Windows.Forms.Label();
            this.brandLabel = new System.Windows.Forms.Label();
            this.subtitleLabel = new System.Windows.Forms.Label();
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.Startbutton = new System.Windows.Forms.Button();
            this.cardPanel = new System.Windows.Forms.Panel();
            this.cardPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // brandLabel
            // 
            this.brandLabel.AutoSize = true;
            this.brandLabel.Font = new System.Drawing.Font("Segoe UI", 28F, System.Drawing.FontStyle.Bold);
            this.brandLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(212)))), ((int)(((byte)(191)))));
            this.brandLabel.Location = new System.Drawing.Point(40, 36);
            this.brandLabel.Name = "brandLabel";
            this.brandLabel.Size = new System.Drawing.Size(217, 51);
            this.brandLabel.TabIndex = 0;
            this.brandLabel.Text = "WeAreCars";
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.titleLabel.Location = new System.Drawing.Point(42, 98);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(395, 32);
            this.titleLabel.TabIndex = 1;
            this.titleLabel.Text = "Car Rental Management Solution";
            // 
            // subtitleLabel
            // 
            this.subtitleLabel.AutoSize = true;
            this.subtitleLabel.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.subtitleLabel.Location = new System.Drawing.Point(44, 138);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.Size = new System.Drawing.Size(378, 20);
            this.subtitleLabel.TabIndex = 2;
            this.subtitleLabel.Text = "Sunderland Branch • Fleet Operations and Bookings Desk";
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.descriptionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.descriptionLabel.Location = new System.Drawing.Point(44, 175);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(510, 50);
            this.descriptionLabel.TabIndex = 3;
            this.descriptionLabel.Text = "Welcome to the internal WeAreCars desktop application. Authorized staff can register new vehicle rentals, review itemized bills, inspect the rented fleet, and query historical booking details.";
            // 
            // Startbutton
            // 
            this.Startbutton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(148)))), ((int)(((byte)(136)))));
            this.Startbutton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Startbutton.FlatAppearance.BorderSize = 0;
            this.Startbutton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Startbutton.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.Startbutton.ForeColor = System.Drawing.Color.White;
            this.Startbutton.Location = new System.Drawing.Point(48, 245);
            this.Startbutton.Name = "Startbutton";
            this.Startbutton.Size = new System.Drawing.Size(220, 44);
            this.Startbutton.TabIndex = 4;
            this.Startbutton.Text = "Staff Login →";
            this.Startbutton.UseVisualStyleBackColor = false;
            this.Startbutton.Click += new System.EventHandler(this.Startbutton_Click);
            // 
            // cardPanel
            // 
            this.cardPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cardPanel.BackColor = System.Drawing.Color.White;
            this.cardPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardPanel.Controls.Add(this.Startbutton);
            this.cardPanel.Controls.Add(this.descriptionLabel);
            this.cardPanel.Controls.Add(this.subtitleLabel);
            this.cardPanel.Controls.Add(this.titleLabel);
            this.cardPanel.Controls.Add(this.brandLabel);
            this.cardPanel.Location = new System.Drawing.Point(85, 55);
            this.cardPanel.Name = "cardPanel";
            this.cardPanel.Size = new System.Drawing.Size(630, 340);
            this.cardPanel.TabIndex = 0;
            // 
            // Welcome_page
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.ClientSize = new System.Drawing.Size(800, 460);
            this.Controls.Add(this.cardPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Welcome_page";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WeAreCars - Welcome";
            this.cardPanel.ResumeLayout(false);
            this.cardPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel cardPanel;
        private System.Windows.Forms.Label brandLabel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label subtitleLabel;
        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.Button Startbutton;
    }
}