namespace ELECTRIS
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using System.Drawing;
    using System.Timers;
    using System.Xml.Serialization;
    using Microsoft.Win32;
    using System.Diagnostics;
    using Microsoft.VisualBasic.FileIO;

    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Check if this is first run or config mode
            if (args.Contains("--config") || !File.Exists(GetConfigFilePath()))
            {
                Application.Run(new ConfigurationForm());
            }
            else
            {
                // Run cleanup task
                CleanupManager.ExecuteCleanup();

                // Check if persistent mode is enabled
                CleanupConfig config = LoadConfig();
                if (config.PersistentMode)
                {
                    Application.Run(new TrayApplicationContext());
                }
                else
                {
                    // Exit application
                    Application.Exit();
                }
            }
        }

        public static string GetConfigFilePath()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "ELECTRIS");
            Directory.CreateDirectory(tempPath);
            return Path.Combine(tempPath, "electro-cleanup.config");
        }

        public static CleanupConfig LoadConfig()
        {
            string configPath = GetConfigFilePath();
            if (File.Exists(configPath))
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(CleanupConfig));
                    using (FileStream fs = new FileStream(configPath, FileMode.Open))
                    {
                        return (CleanupConfig)serializer.Deserialize(fs);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading configuration: {ex.Message}", "ELECTRO-Cleanup",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return new CleanupConfig();
        }

        public static void SaveConfig(CleanupConfig config)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(CleanupConfig));
                using (FileStream fs = new FileStream(GetConfigFilePath(), FileMode.Create))
                {
                    serializer.Serialize(fs, config);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving configuration: {ex.Message}", "ELECTRO-Cleanup",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    [Serializable]
    public class CleanupConfig
    {
        public List<string> FilesToClean { get; set; } = new List<string>();
        public List<string> DirectoriesToClean { get; set; } = new List<string>();
        public bool PersistentMode { get; set; } = false;
        public int CleanupIntervalMinutes { get; set; } = 60; // Default to hourly
    }

    public static class ResourceHelper
    {
        private static Icon _appIcon = null;

        public static Icon GetAppIcon()
        {
            if (_appIcon == null)
            {
                // Load the embedded icon resource
                using (Stream iconStream = typeof(ResourceHelper).Assembly.GetManifestResourceStream("ELECTRIS.Properties.Resources.AppIcon.ico"))
                {
                    if (iconStream != null)
                    {
                        _appIcon = new Icon(iconStream);
                    }
                    else
                    {
                        // Fallback to a default system icon if our resource isn't found
                        _appIcon = SystemIcons.Application;
                    }
                }
            }
            return _appIcon;
        }
    }
}