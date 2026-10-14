namespace RentOnCars
{
    partial class Details
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
            this.headerSubtitleLabel = new System.Windows.Forms.Label();
            this.headerBrandLabel = new System.Windows.Forms.Label();
            this.mainContainer = new System.Windows.Forms.Panel();
            this.bottomBarPanel = new System.Windows.Forms.Panel();
            this.backButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.proceedButton = new System.Windows.Forms.Button();
            this.estimateCard = new System.Windows.Forms.Panel();
            this.estimateStatusLabel = new System.Windows.Forms.Label();
            this.estimateDividerLine = new System.Windows.Forms.Label();
            this.estimateTotalValueLabel = new System.Windows.Forms.Label();
            this.estimateTotalItemLabel = new System.Windows.Forms.Label();
            this.estimateExtrasValueLabel = new System.Windows.Forms.Label();
            this.estimateExtrasItemLabel = new System.Windows.Forms.Label();
            this.estimateFuelTypeValueLabel = new System.Windows.Forms.Label();
            this.estimateFuelTypeItemLabel = new System.Windows.Forms.Label();
            this.estimateCarTypeValueLabel = new System.Windows.Forms.Label();
            this.estimateCarTypeItemLabel = new System.Windows.Forms.Label();
            this.estimateBaseValueLabel = new System.Windows.Forms.Label();
            this.estimateBaseItemLabel = new System.Windows.Forms.Label();
            this.estimateDaysLabel = new System.Windows.Forms.Label();
            this.estimateHeaderLabel = new System.Windows.Forms.Label();
            this.leftColumnPanel = new System.Windows.Forms.Panel();
            this.availabilityBadgePanel = new System.Windows.Forms.Panel();
            this.availabilityStatusLabel = new System.Windows.Forms.Label();
            this.availabilityHeaderLabel = new System.Windows.Forms.Label();
            this.rentalOptionsGroup = new System.Windows.Forms.GroupBox();
            this.bccheckbox = new System.Windows.Forms.CheckBox();
            this.umcheckbox = new System.Windows.Forms.CheckBox();
            this.extrasLabel = new System.Windows.Forms.Label();
            this.fuelcombobox = new System.Windows.Forms.ComboBox();
            this.fuelTypeLabel = new System.Windows.Forms.Label();
            this.carcombobox = new System.Windows.Forms.ComboBox();
            this.carTypeLabel = new System.Windows.Forms.Label();
            this.daysHintLabel = new System.Windows.Forms.Label();
            this.daysupdown = new System.Windows.Forms.NumericUpDown();
            this.daysLabel = new System.Windows.Forms.Label();
            this.customerGroup = new System.Windows.Forms.GroupBox();
            this.licensePanel = new System.Windows.Forms.Panel();
            this.noradiobutton = new System.Windows.Forms.RadioButton();
            this.yesradiobutton = new System.Windows.Forms.RadioButton();
            this.licenseLabel = new System.Windows.Forms.Label();
            this.ageHintLabel = new System.Windows.Forms.Label();
            this.ageupdown = new System.Windows.Forms.NumericUpDown();
            this.ageLabel = new System.Windows.Forms.Label();
            this.addressbox = new System.Windows.Forms.TextBox();
            this.addressLabel = new System.Windows.Forms.Label();
            this.lastnamebox = new System.Windows.Forms.TextBox();
            this.surnameLabel = new System.Windows.Forms.Label();
            this.firstnamebox = new System.Windows.Forms.TextBox();
            this.firstNameLabel = new System.Windows.Forms.Label();
            this.headerPanel.SuspendLayout();
            this.mainContainer.SuspendLayout();
            this.bottomBarPanel.SuspendLayout();
            this.estimateCard.SuspendLayout();
            this.leftColumnPanel.SuspendLayout();
            this.availabilityBadgePanel.SuspendLayout();
            this.rentalOptionsGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.daysupdown)).BeginInit();
            this.customerGroup.SuspendLayout();
            this.licensePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ageupdown)).BeginInit();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.headerPanel.Controls.Add(this.headerSubtitleLabel);
            this.headerPanel.Controls.Add(this.headerBrandLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Padding = new System.Windows.Forms.Padding(24, 12, 24, 12);
            this.headerPanel.Size = new System.Drawing.Size(1024, 65);
            this.headerPanel.TabIndex = 0;
            // 
            // headerSubtitleLabel
            // 
            this.headerSubtitleLabel.AutoSize = true;
            this.headerSubtitleLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.headerSubtitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.headerSubtitleLabel.Location = new System.Drawing.Point(180, 24);
            this.headerSubtitleLabel.Name = "headerSubtitleLabel";
            this.headerSubtitleLabel.Size = new System.Drawing.Size(277, 17);
            this.headerSubtitleLabel.TabIndex = 1;
            this.headerSubtitleLabel.Text = "• New Vehicle Booking & Live Rental Quotation";
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
            // mainContainer
            // 
            this.mainContainer.AutoScroll = true;
            this.mainContainer.Controls.Add(this.bottomBarPanel);
            this.mainContainer.Controls.Add(this.estimateCard);
            this.mainContainer.Controls.Add(this.leftColumnPanel);
            this.mainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContainer.Location = new System.Drawing.Point(0, 65);
            this.mainContainer.Name = "mainContainer";
            this.mainContainer.Padding = new System.Windows.Forms.Padding(20);
            this.mainContainer.Size = new System.Drawing.Size(1024, 616);
            this.mainContainer.TabIndex = 1;
            // 
            // bottomBarPanel
            // 
            this.bottomBarPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bottomBarPanel.BackColor = System.Drawing.Color.White;
            this.bottomBarPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bottomBarPanel.Controls.Add(this.backButton);
            this.bottomBarPanel.Controls.Add(this.clearButton);
            this.bottomBarPanel.Controls.Add(this.proceedButton);
            this.bottomBarPanel.Location = new System.Drawing.Point(20, 540);
            this.bottomBarPanel.Name = "bottomBarPanel";
            this.bottomBarPanel.Padding = new System.Windows.Forms.Padding(12);
            this.bottomBarPanel.Size = new System.Drawing.Size(984, 56);
            this.bottomBarPanel.TabIndex = 2;
            // 
            // backButton
            // 
            this.backButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.backButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.backButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.backButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.backButton.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.backButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.backButton.Location = new System.Drawing.Point(12, 10);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(160, 34);
            this.backButton.TabIndex = 2;
            this.backButton.Text = "← Back to Dashboard";
            this.backButton.UseVisualStyleBackColor = false;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clearButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.clearButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.clearButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.clearButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.clearButton.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.clearButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.clearButton.Location = new System.Drawing.Point(620, 10);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(110, 34);
            this.clearButton.TabIndex = 1;
            this.clearButton.Text = "Clear Form";
            this.clearButton.UseVisualStyleBackColor = false;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // proceedButton
            // 
            this.proceedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.proceedButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(148)))), ((int)(((byte)(136)))));
            this.proceedButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.proceedButton.FlatAppearance.BorderSize = 0;
            this.proceedButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.proceedButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.proceedButton.ForeColor = System.Drawing.Color.White;
            this.proceedButton.Location = new System.Drawing.Point(745, 8);
            this.proceedButton.Name = "proceedButton";
            this.proceedButton.Size = new System.Drawing.Size(225, 38);
            this.proceedButton.TabIndex = 0;
            this.proceedButton.Text = "Review & Confirm Booking →";
            this.proceedButton.UseVisualStyleBackColor = false;
            this.proceedButton.Click += new System.EventHandler(this.proceedButton_Click);
            // 
            // estimateCard
            // 
            this.estimateCard.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.estimateCard.BackColor = System.Drawing.Color.White;
            this.estimateCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.estimateCard.Controls.Add(this.estimateStatusLabel);
            this.estimateCard.Controls.Add(this.estimateDividerLine);
            this.estimateCard.Controls.Add(this.estimateTotalValueLabel);
            this.estimateCard.Controls.Add(this.estimateTotalItemLabel);
            this.estimateCard.Controls.Add(this.estimateExtrasValueLabel);
            this.estimateCard.Controls.Add(this.estimateExtrasItemLabel);
            this.estimateCard.Controls.Add(this.estimateFuelTypeValueLabel);
            this.estimateCard.Controls.Add(this.estimateFuelTypeItemLabel);
            this.estimateCard.Controls.Add(this.estimateCarTypeValueLabel);
            this.estimateCard.Controls.Add(this.estimateCarTypeItemLabel);
            this.estimateCard.Controls.Add(this.estimateBaseValueLabel);
            this.estimateCard.Controls.Add(this.estimateBaseItemLabel);
            this.estimateCard.Controls.Add(this.estimateDaysLabel);
            this.estimateCard.Controls.Add(this.estimateHeaderLabel);
            this.estimateCard.Location = new System.Drawing.Point(645, 20);
            this.estimateCard.Name = "estimateCard";
            this.estimateCard.Padding = new System.Windows.Forms.Padding(18);
            this.estimateCard.Size = new System.Drawing.Size(359, 505);
            this.estimateCard.TabIndex = 1;
            // 
            // estimateStatusLabel
            // 
            this.estimateStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.estimateStatusLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.estimateStatusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(119)))), ((int)(((byte)(6)))));
            this.estimateStatusLabel.Location = new System.Drawing.Point(18, 55);
            this.estimateStatusLabel.Name = "estimateStatusLabel";
            this.estimateStatusLabel.Size = new System.Drawing.Size(320, 35);
            this.estimateStatusLabel.TabIndex = 2;
            this.estimateStatusLabel.Text = "▲ Incomplete: Select car category and fuel type";
            // 
            // estimateDividerLine
            // 
            this.estimateDividerLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.estimateDividerLine.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.estimateDividerLine.Location = new System.Drawing.Point(18, 280);
            this.estimateDividerLine.Name = "estimateDividerLine";
            this.estimateDividerLine.Size = new System.Drawing.Size(320, 2);
            this.estimateDividerLine.TabIndex = 12;
            // 
            // estimateTotalValueLabel
            // 
            this.estimateTotalValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.estimateTotalValueLabel.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.estimateTotalValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(148)))), ((int)(((byte)(136)))));
            this.estimateTotalValueLabel.Location = new System.Drawing.Point(155, 330);
            this.estimateTotalValueLabel.Name = "estimateTotalValueLabel";
            this.estimateTotalValueLabel.Size = new System.Drawing.Size(185, 42);
            this.estimateTotalValueLabel.TabIndex = 11;
            this.estimateTotalValueLabel.Text = "£25.00";
            this.estimateTotalValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // estimateTotalItemLabel
            // 
            this.estimateTotalItemLabel.AutoSize = true;
            this.estimateTotalItemLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.estimateTotalItemLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.estimateTotalItemLabel.Location = new System.Drawing.Point(18, 300);
            this.estimateTotalItemLabel.Name = "estimateTotalItemLabel";
            this.estimateTotalItemLabel.Size = new System.Drawing.Size(127, 21);
            this.estimateTotalItemLabel.TabIndex = 10;
            this.estimateTotalItemLabel.Text = "Estimated Total";
            // 
            // estimateExtrasValueLabel
            // 
            this.estimateExtrasValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.estimateExtrasValueLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.estimateExtrasValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.estimateExtrasValueLabel.Location = new System.Drawing.Point(218, 240);
            this.estimateExtrasValueLabel.Name = "estimateExtrasValueLabel";
            this.estimateExtrasValueLabel.Size = new System.Drawing.Size(120, 20);
            this.estimateExtrasValueLabel.TabIndex = 9;
            this.estimateExtrasValueLabel.Text = "£0.00";
            this.estimateExtrasValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // estimateExtrasItemLabel
            // 
            this.estimateExtrasItemLabel.AutoSize = true;
            this.estimateExtrasItemLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.estimateExtrasItemLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.estimateExtrasItemLabel.Location = new System.Drawing.Point(18, 240);
            this.estimateExtrasItemLabel.Name = "estimateExtrasItemLabel";
            this.estimateExtrasItemLabel.Size = new System.Drawing.Size(104, 17);
            this.estimateExtrasItemLabel.TabIndex = 8;
            this.estimateExtrasItemLabel.Text = "Optional Extras:";
            // 
            // estimateFuelTypeValueLabel
            // 
            this.estimateFuelTypeValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.estimateFuelTypeValueLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.estimateFuelTypeValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.estimateFuelTypeValueLabel.Location = new System.Drawing.Point(218, 200);
            this.estimateFuelTypeValueLabel.Name = "estimateFuelTypeValueLabel";
            this.estimateFuelTypeValueLabel.Size = new System.Drawing.Size(120, 20);
            this.estimateFuelTypeValueLabel.TabIndex = 7;
            this.estimateFuelTypeValueLabel.Text = "£0.00";
            this.estimateFuelTypeValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // estimateFuelTypeItemLabel
            // 
            this.estimateFuelTypeItemLabel.AutoSize = true;
            this.estimateFuelTypeItemLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.estimateFuelTypeItemLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.estimateFuelTypeItemLabel.Location = new System.Drawing.Point(18, 200);
            this.estimateFuelTypeItemLabel.Name = "estimateFuelTypeItemLabel";
            this.estimateFuelTypeItemLabel.Size = new System.Drawing.Size(95, 17);
            this.estimateFuelTypeItemLabel.TabIndex = 6;
            this.estimateFuelTypeItemLabel.Text = "Fuel Surcharge:";
            // 
            // estimateCarTypeValueLabel
            // 
            this.estimateCarTypeValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.estimateCarTypeValueLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.estimateCarTypeValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.estimateCarTypeValueLabel.Location = new System.Drawing.Point(218, 160);
            this.estimateCarTypeValueLabel.Name = "estimateCarTypeValueLabel";
            this.estimateCarTypeValueLabel.Size = new System.Drawing.Size(120, 20);
            this.estimateCarTypeValueLabel.TabIndex = 5;
            this.estimateCarTypeValueLabel.Text = "£0.00";
            this.estimateCarTypeValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // estimateCarTypeItemLabel
            // 
            this.estimateCarTypeItemLabel.AutoSize = true;
            this.estimateCarTypeItemLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.estimateCarTypeItemLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.estimateCarTypeItemLabel.Location = new System.Drawing.Point(18, 160);
            this.estimateCarTypeItemLabel.Name = "estimateCarTypeItemLabel";
            this.estimateCarTypeItemLabel.Size = new System.Drawing.Size(113, 17);
            this.estimateCarTypeItemLabel.TabIndex = 4;
            this.estimateCarTypeItemLabel.Text = "Vehicle Surcharge:";
            // 
            // estimateBaseValueLabel
            // 
            this.estimateBaseValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.estimateBaseValueLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.estimateBaseValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.estimateBaseValueLabel.Location = new System.Drawing.Point(218, 120);
            this.estimateBaseValueLabel.Name = "estimateBaseValueLabel";
            this.estimateBaseValueLabel.Size = new System.Drawing.Size(120, 20);
            this.estimateBaseValueLabel.TabIndex = 3;
            this.estimateBaseValueLabel.Text = "£25.00";
            this.estimateBaseValueLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // estimateBaseItemLabel
            // 
            this.estimateBaseItemLabel.AutoSize = true;
            this.estimateBaseItemLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.estimateBaseItemLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.estimateBaseItemLabel.Location = new System.Drawing.Point(18, 120);
            this.estimateBaseItemLabel.Name = "estimateBaseItemLabel";
            this.estimateBaseItemLabel.Size = new System.Drawing.Size(102, 17);
            this.estimateBaseItemLabel.TabIndex = 2;
            this.estimateBaseItemLabel.Text = "Base Rental Hire:";
            // 
            // estimateDaysLabel
            // 
            this.estimateDaysLabel.AutoSize = true;
            this.estimateDaysLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.estimateDaysLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.estimateDaysLabel.Location = new System.Drawing.Point(18, 90);
            this.estimateDaysLabel.Name = "estimateDaysLabel";
            this.estimateDaysLabel.Size = new System.Drawing.Size(147, 15);
            this.estimateDaysLabel.TabIndex = 1;
            this.estimateDaysLabel.Text = "1 Day(s) @ £25.00 per day";
            // 
            // estimateHeaderLabel
            // 
            this.estimateHeaderLabel.AutoSize = true;
            this.estimateHeaderLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.estimateHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.estimateHeaderLabel.Location = new System.Drawing.Point(18, 18);
            this.estimateHeaderLabel.Name = "estimateHeaderLabel";
            this.estimateHeaderLabel.Size = new System.Drawing.Size(149, 21);
            this.estimateHeaderLabel.TabIndex = 0;
            this.estimateHeaderLabel.Text = "Live Price Preview";
            // 
            // leftColumnPanel
            // 
            this.leftColumnPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.leftColumnPanel.AutoScroll = true;
            this.leftColumnPanel.Controls.Add(this.availabilityBadgePanel);
            this.leftColumnPanel.Controls.Add(this.rentalOptionsGroup);
            this.leftColumnPanel.Controls.Add(this.customerGroup);
            this.leftColumnPanel.Location = new System.Drawing.Point(20, 20);
            this.leftColumnPanel.Name = "leftColumnPanel";
            this.leftColumnPanel.Size = new System.Drawing.Size(615, 505);
            this.leftColumnPanel.TabIndex = 0;
            // 
            // availabilityBadgePanel
            // 
            this.availabilityBadgePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.availabilityBadgePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.availabilityBadgePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.availabilityBadgePanel.Controls.Add(this.availabilityStatusLabel);
            this.availabilityBadgePanel.Controls.Add(this.availabilityHeaderLabel);
            this.availabilityBadgePanel.Location = new System.Drawing.Point(3, 420);
            this.availabilityBadgePanel.Name = "availabilityBadgePanel";
            this.availabilityBadgePanel.Padding = new System.Windows.Forms.Padding(12, 8, 12, 8);
            this.availabilityBadgePanel.Size = new System.Drawing.Size(605, 65);
            this.availabilityBadgePanel.TabIndex = 2;
            // 
            // availabilityStatusLabel
            // 
            this.availabilityStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.availabilityStatusLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.availabilityStatusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.availabilityStatusLabel.Location = new System.Drawing.Point(12, 32);
            this.availabilityStatusLabel.Name = "availabilityStatusLabel";
            this.availabilityStatusLabel.Size = new System.Drawing.Size(580, 24);
            this.availabilityStatusLabel.TabIndex = 1;
            this.availabilityStatusLabel.Text = "Select vehicle category and fuel type to check fleet availability.";
            // 
            // availabilityHeaderLabel
            // 
            this.availabilityHeaderLabel.AutoSize = true;
            this.availabilityHeaderLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.availabilityHeaderLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.availabilityHeaderLabel.Location = new System.Drawing.Point(12, 10);
            this.availabilityHeaderLabel.Name = "availabilityHeaderLabel";
            this.availabilityHeaderLabel.Size = new System.Drawing.Size(185, 17);
            this.availabilityHeaderLabel.TabIndex = 0;
            this.availabilityHeaderLabel.Text = "Fleet Vehicle Allocation Status";
            // 
            // rentalOptionsGroup
            // 
            this.rentalOptionsGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rentalOptionsGroup.BackColor = System.Drawing.Color.White;
            this.rentalOptionsGroup.Controls.Add(this.bccheckbox);
            this.rentalOptionsGroup.Controls.Add(this.umcheckbox);
            this.rentalOptionsGroup.Controls.Add(this.extrasLabel);
            this.rentalOptionsGroup.Controls.Add(this.fuelcombobox);
            this.rentalOptionsGroup.Controls.Add(this.fuelTypeLabel);
            this.rentalOptionsGroup.Controls.Add(this.carcombobox);
            this.rentalOptionsGroup.Controls.Add(this.carTypeLabel);
            this.rentalOptionsGroup.Controls.Add(this.daysHintLabel);
            this.rentalOptionsGroup.Controls.Add(this.daysupdown);
            this.rentalOptionsGroup.Controls.Add(this.daysLabel);
            this.rentalOptionsGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.rentalOptionsGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.rentalOptionsGroup.Location = new System.Drawing.Point(3, 215);
            this.rentalOptionsGroup.Name = "rentalOptionsGroup";
            this.rentalOptionsGroup.Padding = new System.Windows.Forms.Padding(16);
            this.rentalOptionsGroup.Size = new System.Drawing.Size(605, 195);
            this.rentalOptionsGroup.TabIndex = 1;
            this.rentalOptionsGroup.TabStop = false;
            this.rentalOptionsGroup.Text = "Rental Options & Vehicle Selection";
            // 
            // bccheckbox
            // 
            this.bccheckbox.AutoSize = true;
            this.bccheckbox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.bccheckbox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.bccheckbox.Location = new System.Drawing.Point(315, 155);
            this.bccheckbox.Name = "bccheckbox";
            this.bccheckbox.Size = new System.Drawing.Size(188, 19);
            this.bccheckbox.TabIndex = 9;
            this.bccheckbox.Text = "Breakdown Cover (+£2.00/day)";
            this.bccheckbox.UseVisualStyleBackColor = true;
            this.bccheckbox.CheckedChanged += new System.EventHandler(this.OptionChanged);
            // 
            // umcheckbox
            // 
            this.umcheckbox.AutoSize = true;
            this.umcheckbox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.umcheckbox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.umcheckbox.Location = new System.Drawing.Point(100, 155);
            this.umcheckbox.Name = "umcheckbox";
            this.umcheckbox.Size = new System.Drawing.Size(193, 19);
            this.umcheckbox.TabIndex = 8;
            this.umcheckbox.Text = "Unlimited Mileage (+£10.00/day)";
            this.umcheckbox.UseVisualStyleBackColor = true;
            this.umcheckbox.CheckedChanged += new System.EventHandler(this.OptionChanged);
            // 
            // extrasLabel
            // 
            this.extrasLabel.AutoSize = true;
            this.extrasLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.extrasLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.extrasLabel.Location = new System.Drawing.Point(16, 156);
            this.extrasLabel.Name = "extrasLabel";
            this.extrasLabel.Size = new System.Drawing.Size(43, 15);
            this.extrasLabel.TabIndex = 7;
            this.extrasLabel.Text = "Extras:";
            // 
            // fuelcombobox
            // 
            this.fuelcombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fuelcombobox.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.fuelcombobox.FormattingEnabled = true;
            this.fuelcombobox.Location = new System.Drawing.Point(145, 110);
            this.fuelcombobox.Name = "fuelcombobox";
            this.fuelcombobox.Size = new System.Drawing.Size(320, 25);
            this.fuelcombobox.TabIndex = 6;
            this.fuelcombobox.SelectedIndexChanged += new System.EventHandler(this.OptionChanged);
            // 
            // fuelTypeLabel
            // 
            this.fuelTypeLabel.AutoSize = true;
            this.fuelTypeLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.fuelTypeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.fuelTypeLabel.Location = new System.Drawing.Point(16, 115);
            this.fuelTypeLabel.Name = "fuelTypeLabel";
            this.fuelTypeLabel.Size = new System.Drawing.Size(63, 15);
            this.fuelTypeLabel.TabIndex = 5;
            this.fuelTypeLabel.Text = "Fuel Type:";
            // 
            // carcombobox
            // 
            this.carcombobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.carcombobox.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.carcombobox.FormattingEnabled = true;
            this.carcombobox.Location = new System.Drawing.Point(145, 70);
            this.carcombobox.Name = "carcombobox";
            this.carcombobox.Size = new System.Drawing.Size(320, 25);
            this.carcombobox.TabIndex = 4;
            this.carcombobox.SelectedIndexChanged += new System.EventHandler(this.OptionChanged);
            // 
            // carTypeLabel
            // 
            this.carTypeLabel.AutoSize = true;
            this.carTypeLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.carTypeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.carTypeLabel.Location = new System.Drawing.Point(16, 75);
            this.carTypeLabel.Name = "carTypeLabel";
            this.carTypeLabel.Size = new System.Drawing.Size(107, 15);
            this.carTypeLabel.TabIndex = 3;
            this.carTypeLabel.Text = "Vehicle Category:";
            // 
            // daysHintLabel
            // 
            this.daysHintLabel.AutoSize = true;
            this.daysHintLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.daysHintLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.daysHintLabel.Location = new System.Drawing.Point(245, 33);
            this.daysHintLabel.Name = "daysHintLabel";
            this.daysHintLabel.Size = new System.Drawing.Size(161, 15);
            this.daysHintLabel.TabIndex = 2;
            this.daysHintLabel.Text = "(Min 1 day, Max 28 days hire)";
            // 
            // daysupdown
            // 
            this.daysupdown.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.daysupdown.Location = new System.Drawing.Point(145, 29);
            this.daysupdown.Maximum = new decimal(new int[] {
            28,
            0,
            0,
            0});
            this.daysupdown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.daysupdown.Name = "daysupdown";
            this.daysupdown.Size = new System.Drawing.Size(85, 24);
            this.daysupdown.TabIndex = 1;
            this.daysupdown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.daysupdown.ValueChanged += new System.EventHandler(this.OptionChanged);
            // 
            // daysLabel
            // 
            this.daysLabel.AutoSize = true;
            this.daysLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.daysLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.daysLabel.Location = new System.Drawing.Point(16, 33);
            this.daysLabel.Name = "daysLabel";
            this.daysLabel.Size = new System.Drawing.Size(77, 15);
            this.daysLabel.TabIndex = 0;
            this.daysLabel.Text = "Rental Days:";
            // 
            // customerGroup
            // 
            this.customerGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.customerGroup.BackColor = System.Drawing.Color.White;
            this.customerGroup.Controls.Add(this.licensePanel);
            this.customerGroup.Controls.Add(this.licenseLabel);
            this.customerGroup.Controls.Add(this.ageHintLabel);
            this.customerGroup.Controls.Add(this.ageupdown);
            this.customerGroup.Controls.Add(this.ageLabel);
            this.customerGroup.Controls.Add(this.addressbox);
            this.customerGroup.Controls.Add(this.addressLabel);
            this.customerGroup.Controls.Add(this.lastnamebox);
            this.customerGroup.Controls.Add(this.surnameLabel);
            this.customerGroup.Controls.Add(this.firstnamebox);
            this.customerGroup.Controls.Add(this.firstNameLabel);
            this.customerGroup.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.customerGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.customerGroup.Location = new System.Drawing.Point(3, 3);
            this.customerGroup.Name = "customerGroup";
            this.customerGroup.Padding = new System.Windows.Forms.Padding(16);
            this.customerGroup.Size = new System.Drawing.Size(605, 200);
            this.customerGroup.TabIndex = 0;
            this.customerGroup.TabStop = false;
            this.customerGroup.Text = "Customer Information";
            // 
            // licensePanel
            // 
            this.licensePanel.Controls.Add(this.noradiobutton);
            this.licensePanel.Controls.Add(this.yesradiobutton);
            this.licensePanel.Location = new System.Drawing.Point(145, 155);
            this.licensePanel.Name = "licensePanel";
            this.licensePanel.Size = new System.Drawing.Size(200, 28);
            this.licensePanel.TabIndex = 10;
            // 
            // noradiobutton
            // 
            this.noradiobutton.AutoSize = true;
            this.noradiobutton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.noradiobutton.Location = new System.Drawing.Point(85, 4);
            this.noradiobutton.Name = "noradiobutton";
            this.noradiobutton.Size = new System.Drawing.Size(41, 19);
            this.noradiobutton.TabIndex = 1;
            this.noradiobutton.Text = "No";
            this.noradiobutton.UseVisualStyleBackColor = true;
            this.noradiobutton.CheckedChanged += new System.EventHandler(this.OptionChanged);
            // 
            // yesradiobutton
            // 
            this.yesradiobutton.AutoSize = true;
            this.yesradiobutton.Checked = true;
            this.yesradiobutton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.yesradiobutton.Location = new System.Drawing.Point(10, 4);
            this.yesradiobutton.Name = "yesradiobutton";
            this.yesradiobutton.Size = new System.Drawing.Size(42, 19);
            this.yesradiobutton.TabIndex = 0;
            this.yesradiobutton.TabStop = true;
            this.yesradiobutton.Text = "Yes";
            this.yesradiobutton.UseVisualStyleBackColor = true;
            this.yesradiobutton.CheckedChanged += new System.EventHandler(this.OptionChanged);
            // 
            // licenseLabel
            // 
            this.licenseLabel.AutoSize = true;
            this.licenseLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.licenseLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.licenseLabel.Location = new System.Drawing.Point(16, 160);
            this.licenseLabel.Name = "licenseLabel";
            this.licenseLabel.Size = new System.Drawing.Size(126, 15);
            this.licenseLabel.TabIndex = 9;
            this.licenseLabel.Text = "Valid Driving Licence:";
            // 
            // ageHintLabel
            // 
            this.ageHintLabel.AutoSize = true;
            this.ageHintLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.ageHintLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.ageHintLabel.Location = new System.Drawing.Point(245, 126);
            this.ageHintLabel.Name = "ageHintLabel";
            this.ageHintLabel.Size = new System.Drawing.Size(175, 15);
            this.ageHintLabel.TabIndex = 8;
            this.ageHintLabel.Text = "(Must be at least 18 years of age)";
            // 
            // ageupdown
            // 
            this.ageupdown.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.ageupdown.Location = new System.Drawing.Point(145, 122);
            this.ageupdown.Minimum = new decimal(new int[] {
            18,
            0,
            0,
            0});
            this.ageupdown.Name = "ageupdown";
            this.ageupdown.Size = new System.Drawing.Size(85, 24);
            this.ageupdown.TabIndex = 7;
            this.ageupdown.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.ageupdown.ValueChanged += new System.EventHandler(this.OptionChanged);
            // 
            // ageLabel
            // 
            this.ageLabel.AutoSize = true;
            this.ageLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.ageLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.ageLabel.Location = new System.Drawing.Point(16, 126);
            this.ageLabel.Name = "ageLabel";
            this.ageLabel.Size = new System.Drawing.Size(89, 15);
            this.ageLabel.TabIndex = 6;
            this.ageLabel.Text = "Customer Age:";
            // 
            // addressbox
            // 
            this.addressbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addressbox.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.addressbox.Location = new System.Drawing.Point(145, 90);
            this.addressbox.MaxLength = 150;
            this.addressbox.Name = "addressbox";
            this.addressbox.Size = new System.Drawing.Size(430, 24);
            this.addressbox.TabIndex = 5;
            this.addressbox.TextChanged += new System.EventHandler(this.OptionChanged);
            // 
            // addressLabel
            // 
            this.addressLabel.AutoSize = true;
            this.addressLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.addressLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.addressLabel.Location = new System.Drawing.Point(16, 94);
            this.addressLabel.Name = "addressLabel";
            this.addressLabel.Size = new System.Drawing.Size(117, 15);
            this.addressLabel.TabIndex = 4;
            this.addressLabel.Text = "Customer Address:";
            // 
            // lastnamebox
            // 
            this.lastnamebox.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lastnamebox.Location = new System.Drawing.Point(145, 58);
            this.lastnamebox.MaxLength = 50;
            this.lastnamebox.Name = "lastnamebox";
            this.lastnamebox.Size = new System.Drawing.Size(260, 24);
            this.lastnamebox.TabIndex = 3;
            this.lastnamebox.TextChanged += new System.EventHandler(this.OptionChanged);
            // 
            // surnameLabel
            // 
            this.surnameLabel.AutoSize = true;
            this.surnameLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.surnameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.surnameLabel.Location = new System.Drawing.Point(16, 62);
            this.surnameLabel.Name = "surnameLabel";
            this.surnameLabel.Size = new System.Drawing.Size(121, 15);
            this.surnameLabel.TabIndex = 2;
            this.surnameLabel.Text = "Surname (Required):";
            // 
            // firstnamebox
            // 
            this.firstnamebox.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.firstnamebox.Location = new System.Drawing.Point(145, 26);
            this.firstnamebox.MaxLength = 50;
            this.firstnamebox.Name = "firstnamebox";
            this.firstnamebox.Size = new System.Drawing.Size(260, 24);
            this.firstnamebox.TabIndex = 1;
            this.firstnamebox.TextChanged += new System.EventHandler(this.OptionChanged);
            // 
            // firstNameLabel
            // 
            this.firstNameLabel.AutoSize = true;
            this.firstNameLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.firstNameLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.firstNameLabel.Location = new System.Drawing.Point(16, 30);
            this.firstNameLabel.Name = "firstNameLabel";
            this.firstNameLabel.Size = new System.Drawing.Size(71, 15);
            this.firstNameLabel.TabIndex = 0;
            this.firstNameLabel.Text = "First Name:";
            // 
            // Details
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(1024, 681);
            this.Controls.Add(this.mainContainer);
            this.Controls.Add(this.headerPanel);
            this.MinimumSize = new System.Drawing.Size(960, 640);
            this.Name = "Details";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "WeAreCars - New Car Booking";
            this.Load += new System.EventHandler(this.Details_Load);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.mainContainer.ResumeLayout(false);
            this.bottomBarPanel.ResumeLayout(false);
            this.estimateCard.ResumeLayout(false);
            this.estimateCard.PerformLayout();
            this.leftColumnPanel.ResumeLayout(false);
            this.availabilityBadgePanel.ResumeLayout(false);
            this.availabilityBadgePanel.PerformLayout();
            this.rentalOptionsGroup.ResumeLayout(false);
            this.rentalOptionsGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.daysupdown)).EndInit();
            this.customerGroup.ResumeLayout(false);
            this.customerGroup.PerformLayout();
            this.licensePanel.ResumeLayout(false);
            this.licensePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ageupdown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label headerBrandLabel;
        private System.Windows.Forms.Label headerSubtitleLabel;
        private System.Windows.Forms.Panel mainContainer;
        private System.Windows.Forms.Panel leftColumnPanel;
        private System.Windows.Forms.GroupBox customerGroup;
        private System.Windows.Forms.Label firstNameLabel;
        private System.Windows.Forms.TextBox firstnamebox;
        private System.Windows.Forms.Label surnameLabel;
        private System.Windows.Forms.TextBox lastnamebox;
        private System.Windows.Forms.Label addressLabel;
        private System.Windows.Forms.TextBox addressbox;
        private System.Windows.Forms.Label ageLabel;
        private System.Windows.Forms.NumericUpDown ageupdown;
        private System.Windows.Forms.Label ageHintLabel;
        private System.Windows.Forms.Label licenseLabel;
        private System.Windows.Forms.Panel licensePanel;
        private System.Windows.Forms.RadioButton yesradiobutton;
        private System.Windows.Forms.RadioButton noradiobutton;
        private System.Windows.Forms.GroupBox rentalOptionsGroup;
        private System.Windows.Forms.Label daysLabel;
        private System.Windows.Forms.NumericUpDown daysupdown;
        private System.Windows.Forms.Label daysHintLabel;
        private System.Windows.Forms.Label carTypeLabel;
        private System.Windows.Forms.ComboBox carcombobox;
        private System.Windows.Forms.Label fuelTypeLabel;
        private System.Windows.Forms.ComboBox fuelcombobox;
        private System.Windows.Forms.Label extrasLabel;
        private System.Windows.Forms.CheckBox umcheckbox;
        private System.Windows.Forms.CheckBox bccheckbox;
        private System.Windows.Forms.Panel availabilityBadgePanel;
        private System.Windows.Forms.Label availabilityHeaderLabel;
        private System.Windows.Forms.Label availabilityStatusLabel;
        private System.Windows.Forms.Panel estimateCard;
        private System.Windows.Forms.Label estimateHeaderLabel;
        private System.Windows.Forms.Label estimateDaysLabel;
        private System.Windows.Forms.Label estimateBaseItemLabel;
        private System.Windows.Forms.Label estimateBaseValueLabel;
        private System.Windows.Forms.Label estimateCarTypeItemLabel;
        private System.Windows.Forms.Label estimateCarTypeValueLabel;
        private System.Windows.Forms.Label estimateFuelTypeItemLabel;
        private System.Windows.Forms.Label estimateFuelTypeValueLabel;
        private System.Windows.Forms.Label estimateExtrasItemLabel;
        private System.Windows.Forms.Label estimateExtrasValueLabel;
        private System.Windows.Forms.Label estimateDividerLine;
        private System.Windows.Forms.Label estimateTotalItemLabel;
        private System.Windows.Forms.Label estimateTotalValueLabel;
        private System.Windows.Forms.Label estimateStatusLabel;
        private System.Windows.Forms.Panel bottomBarPanel;
        private System.Windows.Forms.Button proceedButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button backButton;
    }
}