namespace EyeRestApp;

public partial class BreakOverlayForm : Form
{
    private System.Windows.Forms.Timer countdownTimer;
    private int secondsLeft = 20;
    private Label countdownLabel;

    public BreakOverlayForm()
    {
        this.FormBorderStyle = FormBorderStyle.None;
        this.WindowState = FormWindowState.Maximized;
        this.TopMost = true;
        this.BackColor = Color.Black;
        this.Opacity = 0.85;

        var messageLabel = new Label { Text = "Time for a break!", ForeColor = Color.White, Font = new Font("Arial", 48, FontStyle.Bold), AutoSize = true };
        countdownLabel = new Label { Text = "20", ForeColor = Color.LimeGreen, Font = new Font("Arial", 80, FontStyle.Bold), AutoSize = true };
        var skipButton = new Button { Text = "✕", ForeColor = Color.White, Font = new Font("Arial", 16), FlatStyle = FlatStyle.Flat, Size = new Size(40, 40) };
        skipButton.FlatAppearance.BorderSize = 0;
        
        this.Controls.Add(messageLabel);
        this.Controls.Add(countdownLabel);
        this.Controls.Add(skipButton);

        this.Load += (s, e) => {
            messageLabel.Location = new Point((this.Width - messageLabel.Width) / 2, this.Height / 2 - 100);
            countdownLabel.Location = new Point((this.Width - countdownLabel.Width) / 2, this.Height / 2);
            skipButton.Location = new Point(this.Width - 50, 10);
        };
        
        skipButton.Click += (s, e) => this.Close();

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