using System;
using System.Drawing;
using System.Windows.Forms;

namespace RentOnCars.UI
{
    /// <summary>
    /// Design system and theme constants for WeAreCars desktop interface.
    /// Provides curated color palettes, Segoe UI typography, and component styling helpers.
    /// </summary>
    public static class Theme
    {
        // Colors
        public static readonly Color HeaderDarkNavy = Color.FromArgb(15, 23, 42);      // #0F172A
        public static readonly Color NavNavy = Color.FromArgb(30, 41, 59);             // #1E293B
        public static readonly Color PageBackground = Color.FromArgb(248, 250, 252);     // #F8FAFC
        public static readonly Color CardBackground = Color.FromArgb(255, 255, 255);    // #FFFFFF
        public static readonly Color CardBorder = Color.FromArgb(226, 232, 240);        // #E2E8F0
        public static readonly Color BorderSubtle = Color.FromArgb(203, 213, 225);      // #CBD5E1

        public static readonly Color AccentPrimary = Color.FromArgb(13, 148, 136);      // #0D9488 (Teal)
        public static readonly Color AccentPrimaryHover = Color.FromArgb(15, 118, 110); // #0F766E
        public static readonly Color AccentBlue = Color.FromArgb(2, 132, 199);          // #0284C7 (Sky Blue)
        public static readonly Color AccentBlueHover = Color.FromArgb(3, 105, 161);     // #0369A1

        public static readonly Color TextPrimary = Color.FromArgb(15, 23, 42);         // #0F172A
        public static readonly Color TextSecondary = Color.FromArgb(71, 85, 105);       // #475569
        public static readonly Color TextMuted = Color.FromArgb(100, 116, 139);         // #64748B
        public static readonly Color TextWhite = Color.FromArgb(255, 255, 255);

        public static readonly Color SuccessGreen = Color.FromArgb(5, 150, 105);        // #059669
        public static readonly Color SuccessGreenBg = Color.FromArgb(236, 253, 245);    // #ECFDF5
        public static readonly Color WarningAmber = Color.FromArgb(217, 119, 6);        // #D97706
        public static readonly Color WarningAmberBg = Color.FromArgb(254, 243, 199);    // #FEF3C7
        public static readonly Color DangerRed = Color.FromArgb(220, 38, 38);           // #DC2626
        public static readonly Color DangerRedBg = Color.FromArgb(254, 242, 242);       // #FEF2F2

        // Typography (Segoe UI)
        public static readonly Font FontHeadingTitle = new Font("Segoe UI", 16f, FontStyle.Bold);
        public static readonly Font FontSectionHeader = new Font("Segoe UI", 12f, FontStyle.Bold);
        public static readonly Font FontCardTitle = new Font("Segoe UI", 11f, FontStyle.Bold);
        public static readonly Font FontKpiValue = new Font("Segoe UI", 24f, FontStyle.Bold);
        public static readonly Font FontKpiLabel = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        public static readonly Font FontBody = new Font("Segoe UI", 9.5f, FontStyle.Regular);
        public static readonly Font FontBodyBold = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        public static readonly Font FontMuted = new Font("Segoe UI", 8.5f, FontStyle.Regular);
        public static readonly Font FontPriceBig = new Font("Segoe UI", 20f, FontStyle.Bold);

        /// <summary>
        /// Styles a button as a primary action (Teal/Blue with white text and smooth flat appearance).
        /// </summary>
        public static void ApplyPrimaryButton(Button btn)
        {
            if (btn == null) return;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = AccentPrimary;
            btn.ForeColor = TextWhite;
            btn.Font = FontBodyBold;
            btn.Cursor = Cursors.Hand;
            btn.Height = Math.Max(btn.Height, 38);
        }

        /// <summary>
        /// Styles a button as a secondary action (Light neutral outline with dark text).
        /// </summary>
        public static void ApplySecondaryButton(Button btn)
        {
            if (btn == null) return;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = BorderSubtle;
            btn.FlatAppearance.BorderSize = 1;
            btn.BackColor = Color.FromArgb(241, 245, 249);
            btn.ForeColor = TextPrimary;
            btn.Font = FontBody;
            btn.Cursor = Cursors.Hand;
            btn.Height = Math.Max(btn.Height, 36);
        }

        /// <summary>
        /// Styles a button as a destructive / cancel action.
        /// </summary>
        public static void ApplyDangerButton(Button btn)
        {
            if (btn == null) return;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = DangerRed;
            btn.ForeColor = TextWhite;
            btn.Font = FontBodyBold;
            btn.Cursor = Cursors.Hand;
            btn.Height = Math.Max(btn.Height, 36);
        }

        /// <summary>
        /// Styles a panel as a clean white card with subtle border and padding.
        /// </summary>
        public static void ApplyCardPanel(Panel panel)
        {
            if (panel == null) return;
            panel.BackColor = CardBackground;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Padding = new Padding(16);
        }

        /// <summary>
        /// Styles a DataGridView with modern Segoe UI typography, alternating rows, and clean headers.
        /// </summary>
        public static void ApplyDataGrid(DataGridView dgv)
        {
            if (dgv == null) return;

            dgv.BackgroundColor = CardBackground;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = CardBorder;
            dgv.EnableHeadersVisualStyles = false;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;

            // Column Header Style
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = NavNavy;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextWhite;
            dgv.ColumnHeadersDefaultCellStyle.Font = FontBodyBold;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 6, 8, 6);
            dgv.ColumnHeadersHeight = 38;

            // Row Styles
            dgv.DefaultCellStyle.Font = FontBody;
            dgv.DefaultCellStyle.ForeColor = TextPrimary;
            dgv.DefaultCellStyle.BackColor = CardBackground;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 242, 254); // Light blue selection
            dgv.DefaultCellStyle.SelectionForeColor = TextPrimary;
            dgv.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);
            dgv.RowTemplate.Height = 34;

            // Alternating Row Style
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = TextPrimary;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 242, 254);
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = TextPrimary;

            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        /// <summary>
        /// Creates a styled top navigation / branding header for WeAreCars forms.
        /// </summary>
        public static Panel CreateTopHeader(string screenTitle, string subtitle = null, Action onLogout = null)
        {
            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = HeaderDarkNavy,
                Padding = new Padding(24, 12, 24, 12)
            };

            // Brand Logo & Title Label
            Label brandLabel = new Label
            {
                Text = "WeAreCars",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 212, 191), // Bright Teal
                AutoSize = true,
                Location = new Point(24, 12)
            };

            Label screenLabel = new Label
            {
                Text = "• " + screenTitle,
                Font = new Font("Segoe UI", 12f, FontStyle.Regular),
                ForeColor = Color.FromArgb(226, 232, 240),
                AutoSize = true,
                Location = new Point(148, 14)
            };

            header.Controls.Add(brandLabel);
            header.Controls.Add(screenLabel);

            if (!string.IsNullOrEmpty(subtitle))
            {
                Label subLabel = new Label
                {
                    Text = subtitle,
                    Font = FontMuted,
                    ForeColor = Color.FromArgb(148, 163, 184),
                    AutoSize = true,
                    Location = new Point(24, 38)
                };
                header.Controls.Add(subLabel);
            }

            if (onLogout != null)
            {
                Button logoutBtn = new Button
                {
                    Text = "Logout (sta001)",
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(51, 65, 85),
                    ForeColor = Color.FromArgb(241, 245, 249),
                    Font = FontBody,
                    Cursor = Cursors.Hand,
                    Size = new Size(130, 32),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Location = new Point(header.Width - 154, 16)
                };
                logoutBtn.FlatAppearance.BorderSize = 0;
                logoutBtn.Click += (s, e) => onLogout();
                header.Controls.Add(logoutBtn);
            }

            return header;
        }
    }
}
