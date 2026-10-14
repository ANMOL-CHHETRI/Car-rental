namespace RentOnCars
{
    partial class BookingReviewDialog
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
            this.subtitleLabel = new System.Windows.Forms.Label();
            this.customerGroup = new System.Windows.Forms.GroupBox();
            this.customerLicenseLabel = new System.Windows.Forms.Label();
            this.customerAgeLabel = new System.Windows.Forms.Label();
            this.customerAddressLabel = new System.Windows.Forms.Label();
            this.customerNameLabel = new System.Windows.Forms.Label();
            this.vehicleGroup = new System.Windows.Forms.GroupBox();
            this.rentalDaysLabel = new System.Windows.Forms.Label();
            this.vehicleRegLabel = new System.Windows.Forms.Label();
            this.pricingGroup = new System.Windows.Forms.GroupBox();
            this.dividerLine = new System.Windows.Forms.Label();
            this.totalCostValueLabel = new System.Windows.Forms.Label();
            this.totalCostItemLabel = new System.Windows.Forms.Label();
            this.extrasValueLabel = new System.Windows.Forms.Label();
            this.extrasItemLabel = new System.Windows.Forms.Label();
            this.fuelSurchargeValueLabel = new System.Windows.Forms.Label();
            this.fuelSurchargeItemLabel = new System.Windows.Forms.Label();
            this.carSurchargeValueLabel = new System.Windows.Forms.Label();
            this.carSurchargeItemLabel = new System.Windows.Forms.Label();
            this.baseRentValueLabel = new System.Windows.Forms.Label();
            this.baseRentItemLabel = new System.Windows.Forms.Label();
            this.confirmButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.customerGroup.SuspendLayout();
            this.vehicleGroup.SuspendLayout();
            this.pricingGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.titleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.titleLabel.Location = new System.Drawing.Point(24, 20);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(262, 30);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "Review Booking Details";
            // 
            // subtitleLabel
            // 
            this.subtitleLabel.AutoSize = true;
            this.subtitleLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.subtitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.subtitleLabel.Location = new System.Drawing.Point(26, 52);
            this.subtitleLabel.Name = "subtitleLabel";
            this.subtitleLabel.Size = new System.Drawing.Size(434, 17);
            this.subtitleLabel.TabIndex = 1;
            this.subtitleLabel.Text = "Verify customer details and pricing before confirming database commitment.";
            // 
            // customerGroup
            // 
            this.customerGroup.Controls.Add(this.customerLicenseLabel);
            this.customerGroup.Controls.Add(this.customerAgeLabel);
            this.customerGroup.Controls.Add(this.customerAddressLabel);
            this.customerGroup.Controls.Add(this.customerNameLabel);
            this.customerGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.customerGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.customerGroup.Location = new System.Drawing.Point(29, 85);
            this.customerGroup.Name = "customerGroup";
            this.customerGroup.Size = new System.Drawing.Size(560, 115);
            this.customerGroup.TabIndex = 2;
            this.customerGroup.TabStop = false;
            this.customerGroup.Text = "Customer Information";
            // 
            // customerLicenseLabel
            // 
            this.customerLicenseLabel.AutoSize = true;
            this.customerLicenseLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.customerLicenseLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.customerLicenseLabel.Location = new System.Drawing.Point(280, 80);
            this.customerLicenseLabel.Name = "customerLicenseLabel";
            this.customerLicenseLabel.Size = new System.Drawing.Size(183, 17);
            this.customerLicenseLabel.TabIndex = 3;
            this.customerLicenseLabel.Text = "Valid Driving Licence (Verified)";
            // 
            // customerAgeLabel
            // 
            this.customerAgeLabel.AutoSize = true;
            this.customerAgeLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.customerAgeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.customerAgeLabel.Location = new System.Drawing.Point(20, 80);
            this.customerAgeLabel.Name = "customerAgeLabel";
            this.customerAgeLabel.Size = new System.Drawing.Size(78, 17);
            this.customerAgeLabel.TabIndex = 2;
            this.customerAgeLabel.Text = "25 years old";
            // 
            // customerAddressLabel
            // 
            this.customerAddressLabel.AutoSize = true;
            this.customerAddressLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.customerAddressLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.customerAddressLabel.Location = new System.Drawing.Point(20, 54);
            this.customerAddressLabel.Name = "customerAddressLabel";
            this.customerAddressLabel.Size = new System.Drawing.Size(201, 17);
            this.customerAddressLabel.TabIndex = 1;
            this.customerAddressLabel.Text = "123 High Street, Sunderland, UK";
            // 
            // customerNameLabel
            // 
            this.customerNameLabel.AutoSize = true;
            this.customerNameLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.customerNameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.customerNameLabel.Location = new System.Drawing.Point(20, 26);
            this.customerNameLabel.Name = "customerNameLabel";
            this.customerNameLabel.Size = new System.Drawing.Size(115, 20);
            this.customerNameLabel.TabIndex = 0;
            this.customerNameLabel.Text = "Customer Name";
            // 
            // vehicleGroup
            // 
            this.vehicleGroup.Controls.Add(this.rentalDaysLabel);
            this.vehicleGroup.Controls.Add(this.vehicleRegLabel);
            this.vehicleGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.vehicleGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.vehicleGroup.Location = new System.Drawing.Point(29, 212);
            this.vehicleGroup.Name = "vehicleGroup";
            this.vehicleGroup.Size = new System.Drawing.Size(560, 85);
            this.vehicleGroup.TabIndex = 3;
            this.vehicleGroup.TabStop = false;
            this.vehicleGroup.Text = "Allocated Vehicle & Duration";
            // 
            // rentalDaysLabel
            // 
            this.rentalDaysLabel.AutoSize = true;
            this.rentalDaysLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.rentalDaysLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.rentalDaysLabel.Location = new System.Drawing.Point(20, 52);
            this.rentalDaysLabel.Name = "rentalDaysLabel";
            this.rentalDaysLabel.Size = new System.Drawing.Size(176, 17);
            this.rentalDaysLabel.TabIndex = 1;
            this.rentalDaysLabel.Text = "3 Days @ £25.00 Base Rate";
            // 
            // vehicleRegLabel
            // 
            this.vehicleRegLabel.AutoSize = true;
            this.vehicleRegLabel.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.vehicleRegLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(148)))), ((int)(((byte)(136)))));
            this.vehicleRegLabel.Location = new System.Drawing.Point(20, 26);
            this.vehicleRegLabel.Name = "vehicleRegLabel";
            this.vehicleRegLabel.Size = new System.Drawing.Size(206, 20);
            this.vehicleRegLabel.TabIndex = 0;
            this.vehicleRegLabel.Text = "WAC-008 (SUV - Hybrid)";
            // 
            // pricingGroup
            // 
            this.pricingGroup.Controls.Add(this.dividerLine);
            this.pricingGroup.Controls.Add(this.totalCostValueLabel);
            this.pricingGroup.Controls.Add(this.totalCostItemLabel);
            this.pricingGroup.Controls.Add(this.extrasValueLabel);
            this.pricingGroup.Controls.Add(this.extrasItemLabel);
            this.pricingGroup.Controls.Add(this.fuelSurchargeValueLabel);
            this.pricingGroup.Controls.Add(this.fuelSurchargeItemLabel);
            this.pricingGroup.Controls.Add(this.carSurchargeValueLabel);
            this.pricingGroup.Controls.Add(this.carSurchargeItemLabel);
            this.pricingGroup.Controls.Add(this.baseRentValueLabel);
            this.pricingGroup.Controls.Add(this.baseRentItemLabel);
            this.pricingGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.pricingGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.pricingGroup.Location = new System.Drawing.Point(29, 310);
            this.pricingGroup.Name = "pricingGroup";
            this.pricingGroup.Size = new System.Drawing.Size(560, 210);
            this.pricingGroup.TabIndex = 4;
            this.pricingGroup.TabStop = false;
            this.pricingGroup.Text = "Itemized Financial Summary";
            // 
            // dividerLine
            // 
            this.dividerLine.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dividerLine.Location = new System.Drawing.Point(20, 150);
            this.dividerLine.Name = "dividerLine";
            this.dividerLine.Size = new System.Drawing.Size(520, 2);
            this.dividerLine.TabIndex = 10;
            // 
            // totalCostValueLabel
            // 
            this.totalCostValueLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.totalCostValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(148)))), ((int)(((byte)(136)))));
            this.totalCostValueLabel.Location = new System.Drawing.Point(360, 162);
            this.totalCostValueLabel.Name = "totalCostValueLabel";
            this.totalCostValueLabel.Size = new System.Drawing.Size(180, 32);
            this.totalCostValueLabel.TabIndex = 9;
            this.totalCostValueLabel.Text = "£206.00";
            this.totalCostValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // totalCostItemLabel
            // 
            this.totalCostItemLabel.AutoSize = true;
            this.totalCostItemLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.totalCostItemLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.totalCostItemLabel.Location = new System.Drawing.Point(20, 166);
            this.totalCostItemLabel.Name = "totalCostItemLabel";
            this.totalCostItemLabel.Size = new System.Drawing.Size(161, 25);
            this.totalCostItemLabel.TabIndex = 8;
            this.totalCostItemLabel.Text = "Total Rental Due:";
            // 
            // extrasValueLabel
            // 
            this.extrasValueLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.extrasValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.extrasValueLabel.Location = new System.Drawing.Point(420, 118);
            this.extrasValueLabel.Name = "extrasValueLabel";
            this.extrasValueLabel.Size = new System.Drawing.Size(120, 17);
            this.extrasValueLabel.TabIndex = 7;
            this.extrasValueLabel.Text = "£36.00";
            this.extrasValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // extrasItemLabel
            // 
            this.extrasItemLabel.AutoSize = true;
            this.extrasItemLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.extrasItemLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.extrasItemLabel.Location = new System.Drawing.Point(20, 118);
            this.extrasItemLabel.Name = "extrasItemLabel";
            this.extrasItemLabel.Size = new System.Drawing.Size(104, 17);
            this.extrasItemLabel.TabIndex = 6;
            this.extrasItemLabel.Text = "Optional Extras:";
            // 
            // fuelSurchargeValueLabel
            // 
            this.fuelSurchargeValueLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.fuelSurchargeValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.fuelSurchargeValueLabel.Location = new System.Drawing.Point(420, 88);
            this.fuelSurchargeValueLabel.Name = "fuelSurchargeValueLabel";
            this.fuelSurchargeValueLabel.Size = new System.Drawing.Size(120, 17);
            this.fuelSurchargeValueLabel.TabIndex = 5;
            this.fuelSurchargeValueLabel.Text = "+£30.00";
            this.fuelSurchargeValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fuelSurchargeItemLabel
            // 
            this.fuelSurchargeItemLabel.AutoSize = true;
            this.fuelSurchargeItemLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.fuelSurchargeItemLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.fuelSurchargeItemLabel.Location = new System.Drawing.Point(20, 88);
            this.fuelSurchargeItemLabel.Name = "fuelSurchargeItemLabel";
            this.fuelSurchargeItemLabel.Size = new System.Drawing.Size(95, 17);
            this.fuelSurchargeItemLabel.TabIndex = 4;
            this.fuelSurchargeItemLabel.Text = "Fuel Surcharge:";
            // 
            // carSurchargeValueLabel
            // 
            this.carSurchargeValueLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.carSurchargeValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.carSurchargeValueLabel.Location = new System.Drawing.Point(420, 58);
            this.carSurchargeValueLabel.Name = "carSurchargeValueLabel";
            this.carSurchargeValueLabel.Size = new System.Drawing.Size(120, 17);
            this.carSurchargeValueLabel.TabIndex = 3;
            this.carSurchargeValueLabel.Text = "+£65.00";
            this.carSurchargeValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // carSurchargeItemLabel
            // 
            this.carSurchargeItemLabel.AutoSize = true;
            this.carSurchargeItemLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.carSurchargeItemLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.carSurchargeItemLabel.Location = new System.Drawing.Point(20, 58);
            this.carSurchargeItemLabel.Name = "carSurchargeItemLabel";
            this.carSurchargeItemLabel.Size = new System.Drawing.Size(113, 17);
            this.carSurchargeItemLabel.TabIndex = 2;
            this.carSurchargeItemLabel.Text = "Vehicle Surcharge:";
            // 
            // baseRentValueLabel
            // 
            this.baseRentValueLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.baseRentValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.baseRentValueLabel.Location = new System.Drawing.Point(420, 28);
            this.baseRentValueLabel.Name = "baseRentValueLabel";
            this.baseRentValueLabel.Size = new System.Drawing.Size(120, 17);
            this.baseRentValueLabel.TabIndex = 1;
            this.baseRentValueLabel.Text = "£75.00";
            this.baseRentValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // baseRentItemLabel
            // 
            this.baseRentItemLabel.AutoSize = true;
            this.baseRentItemLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.baseRentItemLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.baseRentItemLabel.Location = new System.Drawing.Point(20, 28);
            this.baseRentItemLabel.Name = "baseRentItemLabel";
            this.baseRentItemLabel.Size = new System.Drawing.Size(102, 17);
            this.baseRentItemLabel.TabIndex = 0;
            this.baseRentItemLabel.Text = "Base Rental Hire:";
            // 
            // confirmButton
            // 
            this.confirmButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(148)))), ((int)(((byte)(136)))));
            this.confirmButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.confirmButton.FlatAppearance.BorderSize = 0;
            this.confirmButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.confirmButton.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            this.confirmButton.ForeColor = System.Drawing.Color.White;
            this.confirmButton.Location = new System.Drawing.Point(379, 535);
            this.confirmButton.Name = "confirmButton";
            this.confirmButton.Size = new System.Drawing.Size(210, 42);
            this.confirmButton.TabIndex = 5;
            this.confirmButton.Text = "Confirm & Save Booking";
            this.confirmButton.UseVisualStyleBackColor = false;
            this.confirmButton.Click += new System.EventHandler(this.confirmButton_Click);
            // 
            // editButton
            // 
            this.editButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.editButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.editButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.editButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.editButton.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.editButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.editButton.Location = new System.Drawing.Point(219, 535);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(150, 42);
            this.editButton.TabIndex = 6;
            this.editButton.Text = "Return to Editing";
            this.editButton.UseVisualStyleBackColor = false;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // BookingReviewDialog
            // 
            this.AcceptButton = this.confirmButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.editButton;
            this.ClientSize = new System.Drawing.Size(620, 600);
            this.Controls.Add(this.editButton);
            this.Controls.Add(this.confirmButton);
            this.Controls.Add(this.pricingGroup);
            this.Controls.Add(this.vehicleGroup);
            this.Controls.Add(this.customerGroup);
            this.Controls.Add(this.subtitleLabel);
            this.Controls.Add(this.titleLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BookingReviewDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "WeAreCars - Review Booking";
            this.customerGroup.ResumeLayout(false);
            this.customerGroup.PerformLayout();
            this.vehicleGroup.ResumeLayout(false);
            this.vehicleGroup.PerformLayout();
            this.pricingGroup.ResumeLayout(false);
            this.pricingGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label subtitleLabel;
        private System.Windows.Forms.GroupBox customerGroup;
        private System.Windows.Forms.Label customerNameLabel;
        private System.Windows.Forms.Label customerAddressLabel;
        private System.Windows.Forms.Label customerAgeLabel;
        private System.Windows.Forms.Label customerLicenseLabel;
        private System.Windows.Forms.GroupBox vehicleGroup;
        private System.Windows.Forms.Label vehicleRegLabel;
        private System.Windows.Forms.Label rentalDaysLabel;
        private System.Windows.Forms.GroupBox pricingGroup;
        private System.Windows.Forms.Label baseRentItemLabel;
        private System.Windows.Forms.Label baseRentValueLabel;
        private System.Windows.Forms.Label carSurchargeItemLabel;
        private System.Windows.Forms.Label carSurchargeValueLabel;
        private System.Windows.Forms.Label fuelSurchargeItemLabel;
        private System.Windows.Forms.Label fuelSurchargeValueLabel;
        private System.Windows.Forms.Label extrasItemLabel;
        private System.Windows.Forms.Label extrasValueLabel;
        private System.Windows.Forms.Label dividerLine;
        private System.Windows.Forms.Label totalCostItemLabel;
        private System.Windows.Forms.Label totalCostValueLabel;
        private System.Windows.Forms.Button confirmButton;
        private System.Windows.Forms.Button editButton;
    }
}
