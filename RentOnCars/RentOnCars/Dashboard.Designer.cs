namespace RentOnCars
{
    partial class Dashboard
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
            this.staffBadgeLabel = new System.Windows.Forms.Label();
            this.logoutButton = new System.Windows.Forms.Button();
            this.headerSubtitleLabel = new System.Windows.Forms.Label();
            this.headerBrandLabel = new System.Windows.Forms.Label();
            this.contentPanel = new System.Windows.Forms.Panel();
            this.actionsGroup = new System.Windows.Forms.GroupBox();
            this.bookingHistoryDescLabel = new System.Windows.Forms.Label();
            this.bookingHistoryButton = new System.Windows.Forms.Button();
            this.rentedCarsDescLabel = new System.Windows.Forms.Label();
            this.rentedCarsButton = new System.Windows.Forms.Button();
            this.newBookingDescLabel = new System.Windows.Forms.Label();
            this.newBookingButton = new System.Windows.Forms.Button();
            this.kpiTableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.bookingsCard = new System.Windows.Forms.Panel();
            this.totalBookingsValueLabel = new System.Windows.Forms.Label();
            this.totalBookingsTitleLabel = new System.Windows.Forms.Label();
            this.rentedCard = new System.Windows.Forms.Panel();
            this.rentedCarsValueLabel = new System.Windows.Forms.Label();
            this.rentedCarsTitleLabel = new System.Windows.Forms.Label();
            this.availableCard = new System.Windows.Forms.Panel();
            this.availableCarsValueLabel = new System.Windows.Forms.Label();
            this.availableCarsTitleLabel = new System.Windows.Forms.Label();
            this.totalCard = new System.Windows.Forms.Panel();
            this.totalCarsValueLabel = new System.Windows.Forms.Label();
            this.totalCarsTitleLabel = new System.Windows.Forms.Label();
            this.dbStatusPanel = new System.Windows.Forms.Panel();
            this.refreshButton = new System.Windows.Forms.Button();
            this.retryDbButton = new System.Windows.Forms.Button();
            this.dbGuidanceLabel = new System.Windows.Forms.Label();
            this.dbStatusLabel = new System.Windows.Forms.Label();
            this.headerPanel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.actionsGroup.SuspendLayout();
            this.kpiTableLayout.SuspendLayout();
            this.bookingsCard.SuspendLayout();
            this.rentedCard.SuspendLayout();
            this.availableCard.SuspendLayout();
            this.totalCard.SuspendLayout();
            this.dbStatusPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.headerPanel.Controls.Add(this.staffBadgeLabel);
            this.headerPanel.Controls.Add(this.logoutButton);
            this.headerPanel.Controls.Add(this.headerSubtitleLabel);
            this.headerPanel.Controls.Add(this.headerBrandLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Padding = new System.Windows.Forms.Padding(24, 12, 24, 12);
            this.headerPanel.Size = new System.Drawing.Size(984, 70);
            this.headerPanel.TabIndex = 0;
            // 
            // staffBadgeLabel
            // 
            this.staffBadgeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.staffBadgeLabel.AutoSize = true;
            this.staffBadgeLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.staffBadgeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.staffBadgeLabel.Location = new System.Drawing.Point(685, 26);
            this.staffBadgeLabel.Name = "staffBadgeLabel";
            this.staffBadgeLabel.Size = new System.Drawing.Size(155, 17);
            this.staffBadgeLabel.TabIndex = 2;
            this.staffBadgeLabel.Text = "Staff Member: sta001";
            // 
            // logoutButton
            // 
            this.logoutButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.logoutButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            this.logoutButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.logoutButton.FlatAppearance.BorderSize = 0;
            this.logoutButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.logoutButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.logoutButton.ForeColor = System.Drawing.Color.White;
            this.logoutButton.Location = new System.Drawing.Point(854, 19);
            this.logoutButton.Name = "logoutButton";
            this.logoutButton.Size = new System.Drawing.Size(100, 32);
            this.logoutButton.TabIndex = 3;
            this.logoutButton.Text = "Logout";
            this.logoutButton.UseVisualStyleBackColor = false;
            this.logoutButton.Click += new System.EventHandler(this.logoutButton_Click);
            // 
            // headerSubtitleLabel
            // 
            this.headerSubtitleLabel.AutoSize = true;
            this.headerSubtitleLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.headerSubtitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.headerSubtitleLabel.Location = new System.Drawing.Point(180, 26);
            this.headerSubtitleLabel.Name = "headerSubtitleLabel";
            this.headerSubtitleLabel.Size = new System.Drawing.Size(306, 17);
            this.headerSubtitleLabel.TabIndex = 1;
            this.headerSubtitleLabel.Text = "• Sunderland Branch • Fleet Management Dashboard";
            // 
            // headerBrandLabel
            // 
            this.headerBrandLabel.AutoSize = true;
            this.headerBrandLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.headerBrandLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(212)))), ((int)(((byte)(191)))));
            this.headerBrandLabel.Location = new System.Drawing.Point(24, 16);
            this.headerBrandLabel.Name = "headerBrandLabel";
            this.headerBrandLabel.Size = new System.Drawing.Size(129, 30);
            this.headerBrandLabel.TabIndex = 0;
            this.headerBrandLabel.Text = "WeAreCars";
            // 
            // contentPanel
            // 
            this.contentPanel.AutoScroll = true;
            this.contentPanel.Controls.Add(this.actionsGroup);
            this.contentPanel.Controls.Add(this.kpiTableLayout);
            this.contentPanel.Controls.Add(this.dbStatusPanel);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 70);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Padding = new System.Windows.Forms.Padding(24);
            this.contentPanel.Size = new System.Drawing.Size(984, 591);
            this.contentPanel.TabIndex = 1;
            // 
            // actionsGroup
            // 
            this.actionsGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.actionsGroup.BackColor = System.Drawing.Color.White;
            this.actionsGroup.Controls.Add(this.bookingHistoryDescLabel);
            this.actionsGroup.Controls.Add(this.bookingHistoryButton);
            this.actionsGroup.Controls.Add(this.rentedCarsDescLabel);
            this.actionsGroup.Controls.Add(this.rentedCarsButton);
            this.actionsGroup.Controls.Add(this.newBookingDescLabel);
            this.actionsGroup.Controls.Add(this.newBookingButton);
            this.actionsGroup.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.actionsGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.actionsGroup.Location = new System.Drawing.Point(24, 235);
            this.actionsGroup.Name = "actionsGroup";
            this.actionsGroup.Size = new System.Drawing.Size(936, 260);
            this.actionsGroup.TabIndex = 2;
            this.actionsGroup.TabStop = false;
            this.actionsGroup.Text = "Operations & Workflows";
            // 
            // bookingHistoryDescLabel
            // 
            this.bookingHistoryDescLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.bookingHistoryDescLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.bookingHistoryDescLabel.Location = new System.Drawing.Point(28, 195);
            this.bookingHistoryDescLabel.Name = "bookingHistoryDescLabel";
            this.bookingHistoryDescLabel.Size = new System.Drawing.Size(580, 36);
            this.bookingHistoryDescLabel.TabIndex = 5;
            this.bookingHistoryDescLabel.Text = "View historical booking transactions, stored totals, customer details, and saved optional extras.";
            // 
            // bookingHistoryButton
            // 
            this.bookingHistoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.bookingHistoryButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.bookingHistoryButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bookingHistoryButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.bookingHistoryButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bookingHistoryButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.bookingHistoryButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.bookingHistoryButton.Location = new System.Drawing.Point(660, 188);
            this.bookingHistoryButton.Name = "bookingHistoryButton";
            this.bookingHistoryButton.Size = new System.Drawing.Size(240, 42);
            this.bookingHistoryButton.TabIndex = 4;
            this.bookingHistoryButton.Text = "Booking History & Details →";
            this.bookingHistoryButton.UseVisualStyleBackColor = false;
            this.bookingHistoryButton.Click += new System.EventHandler(this.bookingHistoryButton_Click);
            // 
            // rentedCarsDescLabel
            // 
            this.rentedCarsDescLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.rentedCarsDescLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.rentedCarsDescLabel.Location = new System.Drawing.Point(28, 122);
            this.rentedCarsDescLabel.Name = "rentedCarsDescLabel";
            this.rentedCarsDescLabel.Size = new System.Drawing.Size(580, 36);
            this.rentedCarsDescLabel.TabIndex = 3;
            this.rentedCarsDescLabel.Text = "Inspect currently active vehicle rentals, registrations, categories, and fuel types in the fleet.";
            // 
            // rentedCarsButton
            // 
            this.rentedCarsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.rentedCarsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.rentedCarsButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.rentedCarsButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.rentedCarsButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rentedCarsButton.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.rentedCarsButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.rentedCarsButton.Location = new System.Drawing.Point(660, 115);
            this.rentedCarsButton.Name = "rentedCarsButton";
            this.rentedCarsButton.Size = new System.Drawing.Size(240, 42);
            this.rentedCarsButton.TabIndex = 2;
            this.rentedCarsButton.Text = "View Rented Cars →";
            this.rentedCarsButton.UseVisualStyleBackColor = false;
            this.rentedCarsButton.Click += new System.EventHandler(this.rentedCarsButton_Click);
            // 
            // newBookingDescLabel
            // 
            this.newBookingDescLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.newBookingDescLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.newBookingDescLabel.Location = new System.Drawing.Point(28, 48);
            this.newBookingDescLabel.Name = "newBookingDescLabel";
            this.newBookingDescLabel.Size = new System.Drawing.Size(580, 36);
            this.newBookingDescLabel.TabIndex = 1;
            this.newBookingDescLabel.Text = "Collect customer details, select category, fuel type and extras with live price preview and explicit review confirmation.";
            // 
            // newBookingButton
            // 
            this.newBookingButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.newBookingButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(148)))), ((int)(((byte)(136)))));
            this.newBookingButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.newBookingButton.FlatAppearance.BorderSize = 0;
            this.newBookingButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.newBookingButton.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            this.newBookingButton.ForeColor = System.Drawing.Color.White;
            this.newBookingButton.Location = new System.Drawing.Point(660, 42);
            this.newBookingButton.Name = "newBookingButton";
            this.newBookingButton.Size = new System.Drawing.Size(240, 46);
            this.newBookingButton.TabIndex = 0;
            this.newBookingButton.Text = "+ New Car Booking";
            this.newBookingButton.UseVisualStyleBackColor = false;
            this.newBookingButton.Click += new System.EventHandler(this.newBookingButton_Click);
            // 
            // kpiTableLayout
            // 
            this.kpiTableLayout.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.kpiTableLayout.ColumnCount = 4;
            this.kpiTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.kpiTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.kpiTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.kpiTableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.kpiTableLayout.Controls.Add(this.bookingsCard, 3, 0);
            this.kpiTableLayout.Controls.Add(this.rentedCard, 2, 0);
            this.kpiTableLayout.Controls.Add(this.availableCard, 1, 0);
            this.kpiTableLayout.Controls.Add(this.totalCard, 0, 0);
            this.kpiTableLayout.Location = new System.Drawing.Point(24, 100);
            this.kpiTableLayout.Name = "kpiTableLayout";
            this.kpiTableLayout.RowCount = 1;
            this.kpiTableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.kpiTableLayout.Size = new System.Drawing.Size(936, 115);
            this.kpiTableLayout.TabIndex = 1;
            // 
            // bookingsCard
            // 
            this.bookingsCard.BackColor = System.Drawing.Color.White;
            this.bookingsCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.bookingsCard.Controls.Add(this.totalBookingsValueLabel);
            this.bookingsCard.Controls.Add(this.totalBookingsTitleLabel);
            this.bookingsCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bookingsCard.Location = new System.Drawing.Point(705, 3);
            this.bookingsCard.Name = "bookingsCard";
            this.bookingsCard.Padding = new System.Windows.Forms.Padding(12);
            this.bookingsCard.Size = new System.Drawing.Size(228, 109);
            this.bookingsCard.TabIndex = 3;
            // 
            // totalBookingsValueLabel
            // 
            this.totalBookingsValueLabel.AutoSize = true;
            this.totalBookingsValueLabel.Font = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.totalBookingsValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(40)))), ((int)(((byte)(217)))));
            this.totalBookingsValueLabel.Location = new System.Drawing.Point(12, 40);
            this.totalBookingsValueLabel.Name = "totalBookingsValueLabel";
            this.totalBookingsValueLabel.Size = new System.Drawing.Size(50, 47);
            this.totalBookingsValueLabel.TabIndex = 1;
            this.totalBookingsValueLabel.Text = "--";
            // 
            // totalBookingsTitleLabel
            // 
            this.totalBookingsTitleLabel.AutoSize = true;
            this.totalBookingsTitleLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.totalBookingsTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.totalBookingsTitleLabel.Location = new System.Drawing.Point(12, 14);
            this.totalBookingsTitleLabel.Name = "totalBookingsTitleLabel";
            this.totalBookingsTitleLabel.Size = new System.Drawing.Size(123, 17);
            this.totalBookingsTitleLabel.TabIndex = 0;
            this.totalBookingsTitleLabel.Text = "TOTAL BOOKINGS";
            // 
            // rentedCard
            // 
            this.rentedCard.BackColor = System.Drawing.Color.White;
            this.rentedCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rentedCard.Controls.Add(this.rentedCarsValueLabel);
            this.rentedCard.Controls.Add(this.rentedCarsTitleLabel);
            this.rentedCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rentedCard.Location = new System.Drawing.Point(471, 3);
            this.rentedCard.Name = "rentedCard";
            this.rentedCard.Padding = new System.Windows.Forms.Padding(12);
            this.rentedCard.Size = new System.Drawing.Size(228, 109);
            this.rentedCard.TabIndex = 2;
            // 
            // rentedCarsValueLabel
            // 
            this.rentedCarsValueLabel.AutoSize = true;
            this.rentedCarsValueLabel.Font = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.rentedCarsValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(2)))), ((int)(((byte)(132)))), ((int)(((byte)(199)))));
            this.rentedCarsValueLabel.Location = new System.Drawing.Point(12, 40);
            this.rentedCarsValueLabel.Name = "rentedCarsValueLabel";
            this.rentedCarsValueLabel.Size = new System.Drawing.Size(50, 47);
            this.rentedCarsValueLabel.TabIndex = 1;
            this.rentedCarsValueLabel.Text = "--";
            // 
            // rentedCarsTitleLabel
            // 
            this.rentedCarsTitleLabel.AutoSize = true;
            this.rentedCarsTitleLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.rentedCarsTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.rentedCarsTitleLabel.Location = new System.Drawing.Point(12, 14);
            this.rentedCarsTitleLabel.Name = "rentedCarsTitleLabel";
            this.rentedCarsTitleLabel.Size = new System.Drawing.Size(126, 17);
            this.rentedCarsTitleLabel.TabIndex = 0;
            this.rentedCarsTitleLabel.Text = "CURRENTLY HIRED";
            // 
            // availableCard
            // 
            this.availableCard.BackColor = System.Drawing.Color.White;
            this.availableCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.availableCard.Controls.Add(this.availableCarsValueLabel);
            this.availableCard.Controls.Add(this.availableCarsTitleLabel);
            this.availableCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.availableCard.Location = new System.Drawing.Point(237, 3);
            this.availableCard.Name = "availableCard";
            this.availableCard.Padding = new System.Windows.Forms.Padding(12);
            this.availableCard.Size = new System.Drawing.Size(228, 109);
            this.availableCard.TabIndex = 1;
            // 
            // availableCarsValueLabel
            // 
            this.availableCarsValueLabel.AutoSize = true;
            this.availableCarsValueLabel.Font = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.availableCarsValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.availableCarsValueLabel.Location = new System.Drawing.Point(12, 40);
            this.availableCarsValueLabel.Name = "availableCarsValueLabel";
            this.availableCarsValueLabel.Size = new System.Drawing.Size(50, 47);
            this.availableCarsValueLabel.TabIndex = 1;
            this.availableCarsValueLabel.Text = "--";
            // 
            // availableCarsTitleLabel
            // 
            this.availableCarsTitleLabel.AutoSize = true;
            this.availableCarsTitleLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.availableCarsTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.availableCarsTitleLabel.Location = new System.Drawing.Point(12, 14);
            this.availableCarsTitleLabel.Name = "availableCarsTitleLabel";
            this.availableCarsTitleLabel.Size = new System.Drawing.Size(146, 17);
            this.availableCarsTitleLabel.TabIndex = 0;
            this.availableCarsTitleLabel.Text = "AVAILABLE FOR HIRE";
            // 
            // totalCard
            // 
            this.totalCard.BackColor = System.Drawing.Color.White;
            this.totalCard.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.totalCard.Controls.Add(this.totalCarsValueLabel);
            this.totalCard.Controls.Add(this.totalCarsTitleLabel);
            this.totalCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.totalCard.Location = new System.Drawing.Point(3, 3);
            this.totalCard.Name = "totalCard";
            this.totalCard.Padding = new System.Windows.Forms.Padding(12);
            this.totalCard.Size = new System.Drawing.Size(228, 109);
            this.totalCard.TabIndex = 0;
            // 
            // totalCarsValueLabel
            // 
            this.totalCarsValueLabel.AutoSize = true;
            this.totalCarsValueLabel.Font = new System.Drawing.Font("Segoe UI", 26F, System.Drawing.FontStyle.Bold);
            this.totalCarsValueLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.totalCarsValueLabel.Location = new System.Drawing.Point(12, 40);
            this.totalCarsValueLabel.Name = "totalCarsValueLabel";
            this.totalCarsValueLabel.Size = new System.Drawing.Size(50, 47);
            this.totalCarsValueLabel.TabIndex = 1;
            this.totalCarsValueLabel.Text = "--";
            // 
            // totalCarsTitleLabel
            // 
            this.totalCarsTitleLabel.AutoSize = true;
            this.totalCarsTitleLabel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.totalCarsTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.totalCarsTitleLabel.Location = new System.Drawing.Point(12, 14);
            this.totalCarsTitleLabel.Name = "totalCarsTitleLabel";
            this.totalCarsTitleLabel.Size = new System.Drawing.Size(117, 17);
            this.totalCarsTitleLabel.TabIndex = 0;
            this.totalCarsTitleLabel.Text = "TOTAL VEHICLES";
            // 
            // dbStatusPanel
            // 
            this.dbStatusPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dbStatusPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(253)))), ((int)(((byte)(245)))));
            this.dbStatusPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dbStatusPanel.Controls.Add(this.refreshButton);
            this.dbStatusPanel.Controls.Add(this.retryDbButton);
            this.dbStatusPanel.Controls.Add(this.dbGuidanceLabel);
            this.dbStatusPanel.Controls.Add(this.dbStatusLabel);
            this.dbStatusPanel.Location = new System.Drawing.Point(24, 16);
            this.dbStatusPanel.Name = "dbStatusPanel";
            this.dbStatusPanel.Padding = new System.Windows.Forms.Padding(16, 10, 16, 10);
            this.dbStatusPanel.Size = new System.Drawing.Size(936, 68);
            this.dbStatusPanel.TabIndex = 0;
            // 
            // refreshButton
            // 
            this.refreshButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshButton.BackColor = System.Drawing.Color.White;
            this.refreshButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.refreshButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.refreshButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refreshButton.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.refreshButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.refreshButton.Location = new System.Drawing.Point(825, 14);
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(90, 36);
            this.refreshButton.TabIndex = 3;
            this.refreshButton.Text = "Refresh";
            this.refreshButton.UseVisualStyleBackColor = false;
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // retryDbButton
            // 
            this.retryDbButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.retryDbButton.BackColor = System.Drawing.Color.White;
            this.retryDbButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.retryDbButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(213)))), ((int)(((byte)(225)))));
            this.retryDbButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.retryDbButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.retryDbButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.retryDbButton.Location = new System.Drawing.Point(685, 14);
            this.retryDbButton.Name = "retryDbButton";
            this.retryDbButton.Size = new System.Drawing.Size(130, 36);
            this.retryDbButton.TabIndex = 2;
            this.retryDbButton.Text = "Retry LocalDB";
            this.retryDbButton.UseVisualStyleBackColor = false;
            this.retryDbButton.Visible = false;
            this.retryDbButton.Click += new System.EventHandler(this.retryDbButton_Click);
            // 
            // dbGuidanceLabel
            // 
            this.dbGuidanceLabel.AutoSize = true;
            this.dbGuidanceLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.dbGuidanceLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            this.dbGuidanceLabel.Location = new System.Drawing.Point(16, 36);
            this.dbGuidanceLabel.Name = "dbGuidanceLabel";
            this.dbGuidanceLabel.Size = new System.Drawing.Size(262, 15);
            this.dbGuidanceLabel.TabIndex = 1;
            this.dbGuidanceLabel.Text = "Database guidance and troubleshooting details...";
            this.dbGuidanceLabel.Visible = false;
            // 
            // dbStatusLabel
            // 
            this.dbStatusLabel.AutoSize = true;
            this.dbStatusLabel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.dbStatusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(5)))), ((int)(((byte)(150)))), ((int)(((byte)(105)))));
            this.dbStatusLabel.Location = new System.Drawing.Point(16, 12);
            this.dbStatusLabel.Name = "dbStatusLabel";
            this.dbStatusLabel.Size = new System.Drawing.Size(437, 19);
            this.dbStatusLabel.TabIndex = 0;
            this.dbStatusLabel.Text = "● Database Online: Connected to SQL Server LocalDB (RentOnCarsDB)";
            // 
            // Dashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.ClientSize = new System.Drawing.Size(984, 661);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.headerPanel);
            this.MinimumSize = new System.Drawing.Size(880, 560);
            this.Name = "Dashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WeAreCars - Management Dashboard";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Dashboard_FormClosed);
            this.Load += new System.EventHandler(this.Dashboard_Load);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.contentPanel.ResumeLayout(false);
            this.actionsGroup.ResumeLayout(false);
            this.kpiTableLayout.ResumeLayout(false);
            this.bookingsCard.ResumeLayout(false);
            this.bookingsCard.PerformLayout();
            this.rentedCard.ResumeLayout(false);
            this.rentedCard.PerformLayout();
            this.availableCard.ResumeLayout(false);
            this.availableCard.PerformLayout();
            this.totalCard.ResumeLayout(false);
            this.totalCard.PerformLayout();
            this.dbStatusPanel.ResumeLayout(false);
            this.dbStatusPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label headerBrandLabel;
        private System.Windows.Forms.Label headerSubtitleLabel;
        private System.Windows.Forms.Label staffBadgeLabel;
        private System.Windows.Forms.Button logoutButton;
        private System.Windows.Forms.Panel contentPanel;
        private System.Windows.Forms.Panel dbStatusPanel;
        private System.Windows.Forms.Label dbStatusLabel;
        private System.Windows.Forms.Label dbGuidanceLabel;
        private System.Windows.Forms.Button retryDbButton;
        private System.Windows.Forms.Button refreshButton;
        private System.Windows.Forms.TableLayoutPanel kpiTableLayout;
        private System.Windows.Forms.Panel totalCard;
        private System.Windows.Forms.Label totalCarsTitleLabel;
        private System.Windows.Forms.Label totalCarsValueLabel;
        private System.Windows.Forms.Panel availableCard;
        private System.Windows.Forms.Label availableCarsTitleLabel;
        private System.Windows.Forms.Label availableCarsValueLabel;
        private System.Windows.Forms.Panel rentedCard;
        private System.Windows.Forms.Label rentedCarsTitleLabel;
        private System.Windows.Forms.Label rentedCarsValueLabel;
        private System.Windows.Forms.Panel bookingsCard;
        private System.Windows.Forms.Label totalBookingsTitleLabel;
        private System.Windows.Forms.Label totalBookingsValueLabel;
        private System.Windows.Forms.GroupBox actionsGroup;
        private System.Windows.Forms.Button newBookingButton;
        private System.Windows.Forms.Label newBookingDescLabel;
        private System.Windows.Forms.Button rentedCarsButton;
        private System.Windows.Forms.Label rentedCarsDescLabel;
        private System.Windows.Forms.Button bookingHistoryButton;
        private System.Windows.Forms.Label bookingHistoryDescLabel;
    }
}