using System.Timers;
using ELECTRIS;

public class TrayApplicationContext : ApplicationContext
{
    private NotifyIcon trayIcon;
    private System.Timers.Timer cleanupTimer;
    private CleanupConfig config;

    public TrayApplicationContext()
    {
        config = Program.LoadConfig();

        // Initialize tray icon
        trayIcon = new NotifyIcon()
        {
            Icon = ResourceHelper.GetAppIcon(),
            Visible = true,
            Text = "ELECTRO-Cleanup"
        };

        // Create context menu
        trayIcon.ContextMenuStrip = new ContextMenuStrip();
        trayIcon.ContextMenuStrip.Items.Add("Configuration", null, OpenConfiguration);
        trayIcon.ContextMenuStrip.Items.Add("Disable Persistent Mode", null, DisablePersistentMode);
        trayIcon.ContextMenuStrip.Items.Add("Exit", null, Exit);

        // Set up cleanup timer
        cleanupTimer = new System.Timers.Timer(config.CleanupIntervalMinutes * 60 * 1000); // Convert minutes to milliseconds
        cleanupTimer.Elapsed += OnTimerElapsed;
        cleanupTimer.Start();
    }

    private void OnTimerElapsed(object sender, ElapsedEventArgs e)
    {
        CleanupManager.ExecuteCleanup();
    }

    private void OpenConfiguration(object sender, EventArgs e)
    {
        Form configForm = new ConfigurationForm();
        configForm.Show();
    }

    private void DisablePersistentMode(object sender, EventArgs e)
    {
        // Update config
        config.PersistentMode = false;
        Program.SaveConfig(config);

        // Run cleanup task
        CleanupManager.ExecuteCleanup();

        // Exit
        Exit(sender, e);
    }

    private void Exit(object sender, EventArgs e)
    {
        // Clean up before exiting
        trayIcon.Visible = false;
        cleanupTimer.Stop();
        cleanupTimer.Dispose();

        // Exit application
        Application.Exit();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            trayIcon?.Dispose();
            cleanupTimer?.Dispose();
        }

        base.Dispose(disposing);
    }
}