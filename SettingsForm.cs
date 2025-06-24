using System.IO;
using System.Reflection;
using System.Media;
using Microsoft.Win32;

namespace EyeRestApp;

public partial class SettingsForm : Form
{
    // App Constants
    private const string AppName = "EyeRestApp";
    private const string RegistryRunPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    private NumericUpDown intervalNumericUpDown;
    private Button pauseButton;
    private Label statusLabel;
    private CheckBox autostartCheckbox;
    private System.Windows.Forms.Timer breakTimer;
    private NotifyIcon trayIcon;
    private ContextMenuStrip trayMenu;
    private SoundPlayer soundPlayer;

    private System.Windows.Forms.Timer uiUpdateTimer;
    private TimeSpan timeUntilBreak;

    private bool isPaused = false;

    public SettingsForm()
    {
        InitializeComponent();
        InitializeAppLogic();
        LoadSettings();
        StartBreakTimer();
        this.WindowState = FormWindowState.Minimized;
        this.ShowInTaskbar = false;
    }

    private void InitializeAppLogic()
    {
        breakTimer = new System.Windows.Forms.Timer();
        breakTimer.Tick += BreakTimer_Tick;

        uiUpdateTimer = new System.Windows.Forms.Timer();
        uiUpdateTimer.Interval = 1000;
        uiUpdateTimer.Tick += UiUpdateTimer_Tick;

        trayIcon = new NotifyIcon();
        var iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EyeRestApp.Resources.ilogo.ico");
        this.Icon = (iconStream != null) ? new Icon(iconStream) : SystemIcons.Application;
        trayIcon.Icon = this.Icon;
        trayIcon.Visible = true;

        trayMenu = new ContextMenuStrip();
        trayMenu.Items.Add("Show Settings", null, OnShowSettings);
        trayMenu.Items.Add("Pause", null, (s, e) => TogglePause());
        trayMenu.Items.Add("-");
        trayMenu.Items.Add("Quit", null, OnQuit);
        trayIcon.ContextMenuStrip = trayMenu;
        
        trayIcon.DoubleClick += OnShowSettings;
    }

    private void StartBreakTimer()
    {
        int intervalMinutes = (int)intervalNumericUpDown.Value;
        breakTimer.Interval = intervalMinutes * 60 * 1000;
        
        timeUntilBreak = TimeSpan.FromMinutes(intervalMinutes);
        breakTimer.Start();
        uiUpdateTimer.Start();

        isPaused = false;
        UpdateUI();
    }

    // --- MODIFIED: This timer tick now just calls UpdateUI for a cleaner design ---
    private void UiUpdateTimer_Tick(object sender, EventArgs e)
    {
        if (!isPaused)
        {
            timeUntilBreak = timeUntilBreak.Subtract(TimeSpan.FromSeconds(1));
            if (timeUntilBreak.TotalSeconds < 0)
            {
                timeUntilBreak = TimeSpan.Zero;
            }
            // Update all UI elements that show the countdown
            UpdateUI();
        }
    }
    
    private void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            breakTimer.Stop();
            uiUpdateTimer.Stop();
        }
        else
        {
            breakTimer.Start();
            uiUpdateTimer.Start();
        }
        UpdateUI();
    }

    private void BreakTimer_Tick(object sender, EventArgs e)
    {
        timeUntilBreak = TimeSpan.FromMinutes((int)intervalNumericUpDown.Value);

        try
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EyeRestApp.Resources.mosimosi.wav"))
            {
                if (stream != null)
                {
                    soundPlayer = new SoundPlayer(stream);
                    soundPlayer.Play();
                }
            }
        }
        catch {}

        using (var overlay = new BreakOverlayForm())
        {
            overlay.SkipClicked += OnOverlaySkipClicked;
            overlay.ShowDialog();
        }
        
        soundPlayer?.Stop();
    }

    private void OnOverlaySkipClicked(object sender, EventArgs e)
    {
        soundPlayer?.Stop();
    }

    // --- MODIFIED: The UpdateUI method now handles all countdown text updates ---
    private void UpdateUI()
    {
        if (isPaused)
        {
            statusLabel.Text = "Status: Paused";
            pauseButton.Text = "Resume";
            trayMenu.Items[1].Text = "Resume";
            trayIcon.Text = "EyeRest (Paused)";
        }
        else
        {
            string countdownText = $"{timeUntilBreak:mm\\:ss}";
            statusLabel.Text = $"Status: Running (Next break in: {countdownText})";
            trayIcon.Text = $"Next break in: {countdownText}";

            pauseButton.Text = "Pause";
            trayMenu.Items[1].Text = "Pause";
        }
    }

    private void OnShowSettings(object sender, EventArgs e)
    {
        this.Show();
        this.WindowState = FormWindowState.Normal;
        this.ShowInTaskbar = true;
        this.Activate();
    }

    private void OnQuit(object sender, EventArgs e)
    {
        trayIcon.Visible = false;
        Application.Exit();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            this.ShowInTaskbar = false;
            this.Hide();
        } else { base.OnFormClosing(e); }
    }

    private void SetAutostart(bool enabled)
    {
        try
        {
            using (RegistryKey rk = Registry.CurrentUser.OpenSubKey(RegistryRunPath, true))
            {
                if (enabled) rk.SetValue(AppName, Application.ExecutablePath);
                else rk.DeleteValue(AppName, false);
            }
        }
        catch {}
    }

    private void SaveSettings()
    {
        string settingsPath = Path.Combine(AppContext.BaseDirectory, "settings.txt");
        File.WriteAllText(settingsPath, $"{intervalNumericUpDown.Value},{autostartCheckbox.Checked}");
    }
    
    private void LoadSettings()
    {
        try
        {
            string settingsPath = Path.Combine(AppContext.BaseDirectory, "settings.txt");
            if (File.Exists(settingsPath))
            {
                var parts = File.ReadAllText(settingsPath).Split(',');
                intervalNumericUpDown.Value = int.Parse(parts[0]);
                autostartCheckbox.Checked = bool.Parse(parts[1]);
            }
        }
        catch {}
    }

    private void InitializeComponent()
    {
        this.Text = "Eye Rest Settings";
        this.ClientSize = new System.Drawing.Size(300, 220);
        var intervalLabel = new Label { Text = "Reminder Interval (minutes):", Location = new System.Drawing.Point(12, 15), AutoSize=true };
        intervalNumericUpDown = new NumericUpDown { Location = new System.Drawing.Point(15, 35), Width = 100, Minimum = 1, Maximum = 60, Value = 12 };
        pauseButton = new Button { Text = "Pause", Location = new System.Drawing.Point(100, 80), Width=100 };
        statusLabel = new Label { Text = "Status: Running", Location = new System.Drawing.Point(12, 120), AutoSize=true, TextAlign=ContentAlignment.MiddleCenter, Width=276 };
        autostartCheckbox = new CheckBox { Text = "Start with Windows", Location = new System.Drawing.Point(15, 160), AutoSize=true };
        this.Controls.Add(intervalLabel);
        this.Controls.Add(intervalNumericUpDown);
        this.Controls.Add(pauseButton);
        this.Controls.Add(statusLabel);
        this.Controls.Add(autostartCheckbox);
        pauseButton.Click += (s, e) => TogglePause();
        intervalNumericUpDown.ValueChanged += (s, e) => { breakTimer.Stop(); uiUpdateTimer.Stop(); StartBreakTimer(); SaveSettings(); };
        autostartCheckbox.CheckedChanged += (s, e) => { SetAutostart(autostartCheckbox.Checked); SaveSettings(); };
    }
}