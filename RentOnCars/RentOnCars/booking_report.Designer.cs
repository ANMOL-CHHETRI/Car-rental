namespace RentOnCars
{
    partial class booking_report
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
            this.headerPanel = new System.Windows.Forms.Panel();
            this.screenSubtitleLabel = new System.Windows.Forms.Label();
            this.screenTitleLabel = new System.Windows.Forms.Label();
            this.headerBrandLabel = new System.Windows.Forms.Label();
            this.searchPanel = new System.Windows.Forms.Panel();
            this.refreshButton = new System.Windows.Forms.Button();
            this.recordCountLabel = new System.Windows.Forms.Label();
            this.searchBox = new System.Windows.Forms.TextBox();
            this.searchLabel = new System.Windows.Forms.Label();
            this.gridPanel = new System.Windows.Forms.Panel();
            this.emptyStateLabel = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.detailsGroup = new System.Windows.Forms.GroupBox();
            this.detailStoredTotalLabel = new System.Windows.Forms.Label();
            this.detailExtrasLabel = new System.Windows.Forms.Label();
            this.detailDaysLabel = new System.Windows.Forms.Label();
            this.detailVehicleLabel = new System.Windows.Forms.Label();
            this.detailLicenseLabel = new System.Windows.Forms.Label();
            this.detailAddressLabel = new System.Windows.Forms.Label();
            this.detailCustomerNameLabel = new System.Windows.Forms.Label();
            this.detailBookingRefLabel = new System.Windows.Forms.Label();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.backButton = new System.Windows.Forms.Button();
            this.headerPanel.SuspendLayout();
            this.searchPanel.SuspendLayout();
            this.gridPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.detailsGroup.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.headerPanel.Controls.Add(this.screenSubtitleLabel);
            this.headerPanel.Controls.Add(this.screenTitleLabel);
            this.headerPanel.Controls.Add(this.headerBrandLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Padding = new System.Windows.Forms.Padding(24, 12, 24, 12);
            this.headerPanel.Size = new System.Drawing.Size(984, 65);
            this.headerPanel.TabIndex = 0;
            // 
            // screenSubtitleLabel
            // 
            this.screenSubtitleLabel.AutoSize = true;
            this.screenSubtitleLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.screenSubtitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.screenSubtitleLabel.Location = new System.Drawing.Point(360, 24);
            this.screenSubtitleLabel.Name = "screenSubtitleLabel";
            this.screenSubtitleLabel.Size = new System.Drawing.Size(260, 17);
            this.screenSubtitleLabel.TabIndex = 2;
            this.screenSubtitleLabel.Text = "• Active vehicle rentals and fleet allocations.";
            // 
            // screenTitleLabel
            // 
            this.screenTitleLabel.AutoSize = true;
            this.screenTitleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.screenTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.screenTitleLabel.Location = new System.Drawing.Point(175, 20);
            this.screenTitleLabel.Name = "screenTitleLabel";
            this.screenTitleLabel.Size = new System.Drawing.Size(176, 21);
            this.screenTitleLabel.TabIndex = 1;
            this.screenTitleLabel.Text = "• Rented Cars Report";
            // 
            // headerBrandLabel
            // 
            this.headerBrandLabel.AutoSize = true;
            this.headerBrandLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.headerBrandLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(212)))), ((int)(((byte)(191)))));
            this.headerBrandLabel.Location = new System.Drawing.Point(24, 14);
            this.headerBrandLabel.Name = "headerBrandLabel";
            this.headerBrandLabel.Size = new System.Drawing.Size(129, 30);
            this.headerBrandLabel.TabIndex = 0;
            this.headerBrandLabel.Text = "WeAreCars";
            // 
            // searchPanel
            // 
            this.searchPanel.BackColor = System.Drawing.Color.White;
            this.searchPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.searchPanel.Controls.Add(this.refreshButton);
            this.searchPanel.Controls.Add(this.recordCountLabel);
            this.searchPanel.Controls.Add(this.searchBox);
            this.searchPanel.Controls.Add(this.searchLabel);
            this.searchPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.searchPanel.Location = new System.Drawing.Point(0, 65);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Padding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.searchPanel.Size = new System.Drawing.Size(984, 52);
            this.searchPanel.TabIndex = 1;
            // 
            // refreshButton
            // 
            this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.refreshButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.refreshButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.refreshButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.refreshButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.refreshButton.Location = new System.Drawing.Point(865, 9);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(100, 32);
            this.refreshButton.TabIndex = 3;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = false;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // recordCountLabel
            // 
            this.recordCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.recordCountLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.recordCountLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.recordCountLabel.Location = new System.Drawing.Point(540, 15);
            this.recordCountLabel.Name = "recordCountLabel";
            this.recordCountLabel.Size = new System.Drawing.Size(310, 20);
            this.recordCountLabel.TabIndex = 2;
            this.recordCountLabel.Text = "Showing records...";
            this.recordCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // searchBox
            // 
            this.searchBox.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.searchBox.Location = new System.Drawing.Point(75, 12);
            this.searchBox.Name = "searchBox";
            this.searchBox.Size = new System.Drawing.Size(300, 24);
            this.searchBox.TabIndex = 1;
            this.searchBox.TextChanged += new System.EventHandler(this.searchBox_TextChanged);
            // 
            // searchLabel
            // 
            this.searchLabel.AutoSize = true;
            this.searchLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.searchLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.searchLabel.Location = new System.Drawing.Point(16, 15);
            this.searchLabel.Name = "searchLabel";
            this.searchLabel.Size = new System.Drawing.Size(53, 17);
            this.searchLabel.TabIndex = 0;
            this.searchLabel.Text = "Search:";
            // 
            // gridPanel
            // 
            this.gridPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridPanel.BackColor = System.Drawing.Color.White;
            this.gridPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.gridPanel.Controls.Add(this.emptyStateLabel);
            this.gridPanel.Controls.Add(this.dataGridView1);
            this.gridPanel.Location = new System.Drawing.Point(16, 128);
            this.gridPanel.Name = "gridPanel";
            this.gridPanel.Size = new System.Drawing.Size(952, 280);
            this.gridPanel.TabIndex = 2;
            // 
            // emptyStateLabel
            // 
            this.emptyStateLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.emptyStateLabel.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.emptyStateLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.emptyStateLabel.Location = new System.Drawing.Point(226, 110);
            this.emptyStateLabel.Name = "emptyStateLabel";
            this.emptyStateLabel.Size = new System.Drawing.Size(500, 50);
            this.emptyStateLabel.TabIndex = 1;
            this.emptyStateLabel.Text = "No records to display.";
            this.emptyStateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.emptyStateLabel.Visible = false;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(950, 278);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // detailsGroup
            // 
            this.detailsGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.detailsGroup.BackColor = System.Drawing.Color.White;
            this.detailsGroup.Controls.Add(this.detailStoredTotalLabel);
            this.detailsGroup.Controls.Add(this.detailExtrasLabel);
            this.detailsGroup.Controls.Add(this.detailDaysLabel);
            this.detailsGroup.Controls.Add(this.detailVehicleLabel);
            this.detailsGroup.Controls.Add(this.detailLicenseLabel);
            this.detailsGroup.Controls.Add(this.detailAddressLabel);
            this.detailsGroup.Controls.Add(this.detailCustomerNameLabel);
            this.detailsGroup.Controls.Add(this.detailBookingRefLabel);
            this.detailsGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.detailsGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.detailsGroup.Location = new System.Drawing.Point(16, 418);
            this.detailsGroup.Name = "detailsGroup";
            this.detailsGroup.Padding = new System.Windows.Forms.Padding(16, 12, 16, 12);
            this.detailsGroup.Size = new System.Drawing.Size(952, 145);
            this.detailsGroup.TabIndex = 3;
            this.detailsGroup.TabStop = false;
            this.detailsGroup.Text = "Selected Booking Detail & Saved Extras Audit";
            // 
            // detailStoredTotalLabel
            // 
            this.detailStoredTotalLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.detailStoredTotalLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.detailStoredTotalLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(148)))), ((int)(((byte)(136)))));
            this.detailStoredTotalLabel.Location = new System.Drawing.Point(490, 22);
            this.detailStoredTotalLabel.Name = "detailStoredTotalLabel";
            this.detailStoredTotalLabel.Size = new System.Drawing.Size(445, 28);
            this.detailStoredTotalLabel.TabIndex = 7;
            this.detailStoredTotalLabel.Text = "Stored Total: £0.00 GBP";
            this.detailStoredTotalLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // detailExtrasLabel
            // 
            this.detailExtrasLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.detailExtrasLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.detailExtrasLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(40)))), ((int)(((byte)(217)))));
            this.detailExtrasLabel.Location = new System.Drawing.Point(16, 112);
            this.detailExtrasLabel.Name = "detailExtrasLabel";
            this.detailExtrasLabel.Size = new System.Drawing.Size(915, 20);
            this.detailExtrasLabel.TabIndex = 6;
            this.detailExtrasLabel.Text = "Saved Optional Extras: None";
            // 
            // detailDaysLabel
            // 
            this.detailDaysLabel.AutoSize = true;
            this.detailDaysLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.detailDaysLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.detailDaysLabel.Location = new System.Drawing.Point(490, 84);
            this.detailDaysLabel.Name = "detailDaysLabel";
            this.detailDaysLabel.Size = new System.Drawing.Size(152, 17);
            this.detailDaysLabel.TabIndex = 5;
            this.detailDaysLabel.Text = "Rental Duration: -- Days";
            // 
            // detailVehicleLabel
            // 
            this.detailVehicleLabel.AutoSize = true;
            this.detailVehicleLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.detailVehicleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.detailVehicleLabel.Location = new System.Drawing.Point(490, 56);
            this.detailVehicleLabel.Name = "detailVehicleLabel";
            this.detailVehicleLabel.Size = new System.Drawing.Size(76, 17);
            this.detailVehicleLabel.TabIndex = 4;
            this.detailVehicleLabel.Text = "Vehicle: --";
            // 
            // detailLicenseLabel
            // 
            this.detailLicenseLabel.AutoSize = true;
            this.detailLicenseLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.detailLicenseLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.detailLicenseLabel.Location = new System.Drawing.Point(16, 84);
            this.detailLicenseLabel.Name = "detailLicenseLabel";
            this.detailLicenseLabel.Size = new System.Drawing.Size(199, 17);
            this.detailLicenseLabel.TabIndex = 3;
            this.detailLicenseLabel.Text = "Licence: Valid Driving Licence";
            // 
            // detailAddressLabel
            // 
            this.detailAddressLabel.AutoSize = true;
            this.detailAddressLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.detailAddressLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.detailAddressLabel.Location = new System.Drawing.Point(16, 56);
            this.detailAddressLabel.Name = "detailAddressLabel";
            this.detailAddressLabel.Size = new System.Drawing.Size(77, 17);
            this.detailAddressLabel.TabIndex = 2;
            this.detailAddressLabel.Text = "Address: --";
            // 
            // detailCustomerNameLabel
            // 
            this.detailCustomerNameLabel.AutoSize = true;
            this.detailCustomerNameLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.detailCustomerNameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.detailCustomerNameLabel.Location = new System.Drawing.Point(235, 28);
            this.detailCustomerNameLabel.Name = "detailCustomerNameLabel";
            this.detailCustomerNameLabel.Size = new System.Drawing.Size(89, 17);
            this.detailCustomerNameLabel.TabIndex = 1;
            this.detailCustomerNameLabel.Text = "Customer: --";
            // 
            // detailBookingRefLabel
            // 
            this.detailBookingRefLabel.AutoSize = true;
            this.detailBookingRefLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.detailBookingRefLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(148)))), ((int)(((byte)(136)))));
            this.detailBookingRefLabel.Location = new System.Drawing.Point(16, 28);
            this.detailBookingRefLabel.Name = "detailBookingRefLabel";
            this.detailBookingRefLabel.Size = new System.Drawing.Size(150, 17);
            this.detailBookingRefLabel.TabIndex = 0;
            this.detailBookingRefLabel.Text = "Booking Reference: --";
            // 
            // bottomPanel
            // 
            this.bottomPanel.BackColor = System.Drawing.Color.White;
            this.bottomPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bottomPanel.Controls.Add(this.backButton);
            this.bottomPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomPanel.Location = new System.Drawing.Point(0, 574);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Padding = new System.Windows.Forms.Padding(16, 8, 16, 8);
            this.bottomPanel.Size = new System.Drawing.Size(984, 52);
            this.bottomPanel.TabIndex = 4;
            // 
            // backButton
            // 
            this.backButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.backButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.backButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.backButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.backButton.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.backButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.backButton.Location = new System.Drawing.Point(16, 8);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(180, 34);
            this.backButton.TabIndex = 0;
            this.backButton.Text = "← Back to Dashboard";
            this.backButton.UseVisualStyleBackColor = false;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // booking_report
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(984, 626);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.detailsGroup);
            this.Controls.Add(this.gridPanel);
            this.Controls.Add(this.searchPanel);
            this.Controls.Add(this.headerPanel);
            this.MinimumSize = new System.Drawing.Size(900, 580);
            this.Name = "booking_report";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "WeAreCars - Fleet Report";
            this.Load += new System.EventHandler(this.booking_report_Load);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.searchPanel.ResumeLayout(false);
            this.searchPanel.PerformLayout();
            this.gridPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.detailsGroup.ResumeLayout(false);
            this.detailsGroup.PerformLayout();
            this.bottomPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label headerBrandLabel;
        private System.Windows.Forms.Label screenTitleLabel;
        private System.Windows.Forms.Label screenSubtitleLabel;
        private System.Windows.Forms.Panel searchPanel;
        private System.Windows.Forms.Label searchLabel;
        private System.Windows.Forms.TextBox searchBox;
        private System.Windows.Forms.Label recordCountLabel;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.Panel gridPanel;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label emptyStateLabel;
        private System.Windows.Forms.GroupBox detailsGroup;
        private System.Windows.Forms.Label detailBookingRefLabel;
        private System.Windows.Forms.Label detailCustomerNameLabel;
        private System.Windows.Forms.Label detailAddressLabel;
        private System.Windows.Forms.Label detailLicenseLabel;
        private System.Windows.Forms.Label detailVehicleLabel;
        private System.Windows.Forms.Label detailDaysLabel;
        private System.Windows.Forms.Label detailExtrasLabel;
        private System.Windows.Forms.Label detailStoredTotalLabel;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Button backButton;
    }
}