using System;
using System.Drawing;
using System.Windows.Forms;

namespace EyeRestApp;

public partial class BreakOverlayForm : Form
{
    private System.Windows.Forms.Timer countdownTimer;
    private int secondsLeft = 20;
    private Label countdownLabel;

    public event EventHandler SkipClicked;

    public BreakOverlayForm()
    {
        // --- Basic Form Setup ---
        this.FormBorderStyle = FormBorderStyle.None;
        this.WindowState = FormWindowState.Maximized;
        this.TopMost = true;
        this.BackColor = Color.Black;
        this.Opacity = 0.85;

        // --- Use a TableLayoutPanel for a Professional, Centered Layout ---
        var mainPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent,
            ColumnCount = 1,
            RowCount = 5,
            GrowStyle = TableLayoutPanelGrowStyle.FixedSize
        };
        mainPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));     // Top margin
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));     // Title
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));     // Main text
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));     // Countdown
        mainPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 15F));     // Bottom margin

        // --- Create a few, high-impact labels ---
        var titleLabel = new Label {
            Text = "EYE BREAK TIME",
            ForeColor = Color.FromArgb(255, 80, 80),
            Font = new Font("Segoe UI", 48, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Padding = new Padding(0, 0, 0, 10)
        };

        var mainTextLabel = new Label {
            Text = "Look Away from the Screen\n\n" +
               "You've been focused for a while — now it's time to rest your eyes.\n\n" +
               "Look at something 20 feet away\n\n" +
               "Hold your gaze for 20 seconds\n\n" +
               "Blink, breathe, and let your eyes relax\n\n" +
               "Your vision matters. Take this moment — it's worth it!",
            ForeColor = Color.Gainsboro,
            Font = new Font("Segoe UI", 20, FontStyle.Regular),
            MaximumSize = new Size(900, 0),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Padding = new Padding(20, 0, 20, 10),
            AutoSize = true
        };
        mainTextLabel.Anchor = AnchorStyles.None;

        countdownLabel = new Label {
            Text = "20",
            ForeColor = Color.LimeGreen,
            Font = new Font("Segoe UI Semibold", 72, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter
        };

        // --- Add Labels to the main panel ---
        mainPanel.Controls.Add(titleLabel, 0, 1);
        mainPanel.Controls.Add(mainTextLabel, 0, 2);
        mainPanel.Controls.Add(countdownLabel, 0, 3);

        this.Controls.Add(mainPanel);

        // --- Skip Button (Stays top-right) ---
        var skipButton = new Button {
            Text = "✕",
            ForeColor = Color.White,
            Font = new Font("Arial", 16),
            FlatStyle = FlatStyle.Flat,
            Size = new Size(40, 40),
            BackColor = Color.FromArgb(50, 255, 255, 255),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        skipButton.FlatAppearance.BorderSize = 0;
        skipButton.Location = new Point(this.ClientSize.Width - skipButton.Width - 10, 10);
        skipButton.Click += (s, e) => {
            SkipClicked?.Invoke(this, EventArgs.Empty);
            this.Close();
        };
        this.Controls.Add(skipButton);
        skipButton.BringToFront();

        // Keep skip button in top-right on resize
        this.Resize += (s, e) => {
            skipButton.Location = new Point(this.ClientSize.Width - skipButton.Width - 10, 10);
        };

        // --- Start the Countdown Timer ---
        countdownTimer = new System.Windows.Forms.Timer { Interval = 1000 };
        countdownTimer.Tick += CountdownTimer_Tick;
        countdownTimer.Start();
    }

    private void CountdownTimer_Tick(object sender, EventArgs e)
    {
        secondsLeft--;
        countdownLabel.Text = secondsLeft.ToString();
        if (secondsLeft <= 0)
        {
            countdownTimer.Stop();
            this.Close();
        }
    }
}
