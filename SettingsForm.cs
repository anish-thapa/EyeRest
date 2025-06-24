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

    // UI Controls
    private NumericUpDown intervalNumericUpDown;
    private Button pauseButton;
    private Label statusLabel;
    private CheckBox autostartCheckbox;
    private System.Windows.Forms.Timer breakTimer;
    private NotifyIcon trayIcon;
    private ContextMenuStrip trayMenu;

    private bool isPaused = false;

    public SettingsForm()
    {
        InitializeComponent();
        InitializeAppLogic();
        LoadSettings();
        StartBreakTimer();
        // Start hidden
        this.WindowState = FormWindowState.Minimized;
        this.ShowInTaskbar = false;
    }

    private void InitializeAppLogic()
    {
        // --- Setup the Main Timer ---
        breakTimer = new System.Windows.Forms.Timer();
        breakTimer.Tick += BreakTimer_Tick;

        // --- Setup the System Tray Icon ---
        trayIcon = new NotifyIcon();
        // --- THIS IS THE CORRECTED CODE ---
        var iconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EyeRestApp.Resources.ilogo.ico");
        this.Icon = (iconStream != null) ? new Icon(iconStream) : SystemIcons.Application;
        // --- END OF CORRECTION ---
        trayIcon.Icon = this.Icon;
        trayIcon.Text = "Eye Rest Reminder";
        trayIcon.Visible = true;

        // --- Setup the Tray Menu ---
        trayMenu = new ContextMenuStrip();
        trayMenu.Items.Add("Show Settings", null, OnShowSettings);
        trayMenu.Items.Add("Pause", null, (s, e) => TogglePause());
        trayMenu.Items.Add("-"); // Separator
        trayMenu.Items.Add("Quit", null, OnQuit);
        trayIcon.ContextMenuStrip = trayMenu;
        
        trayIcon.DoubleClick += OnShowSettings;
    }

    private void StartBreakTimer()
    {
        int intervalMinutes = (int)intervalNumericUpDown.Value;
        breakTimer.Interval = intervalMinutes * 60 * 1000;
        breakTimer.Start();
        isPaused = false;
        UpdateUI();
    }
    
    private void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused) breakTimer.Stop();
        else breakTimer.Start();
        UpdateUI();
    }

    private void BreakTimer_Tick(object sender, EventArgs e)
    {
        // Play sound
        try
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("EyeRestApp.Resources.mosimosi.wav"))
            {
                if (stream != null)
                {
                    SoundPlayer player = new SoundPlayer(stream);
                    player.Play();
                }
            }
        }
        catch {}

        // Show overlay
        using (var overlay = new BreakOverlayForm())
        {
            overlay.ShowDialog();
        }
    }

    private void UpdateUI()
    {
        if (isPaused)
        {
            statusLabel.Text = "Status: Paused";
            pauseButton.Text = "Resume";
            trayMenu.Items[1].Text = "Resume";
        }
        else
        {
            statusLabel.Text = $"Status: Running";
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
        }
        else
        {
            base.OnFormClosing(e);
        }
    }

    // Settings and Autostart
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
        // A simple way to save settings without a full config file
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
        catch { /* Ignore errors, use defaults */ }
    }

    // This method replaces the visual designer
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
        
        // Connect events
        pauseButton.Click += (s, e) => TogglePause();
        intervalNumericUpDown.ValueChanged += (s, e) => { breakTimer.Stop(); StartBreakTimer(); SaveSettings(); };
        autostartCheckbox.CheckedChanged += (s, e) => { SetAutostart(autostartCheckbox.Checked); SaveSettings(); };
    }
}