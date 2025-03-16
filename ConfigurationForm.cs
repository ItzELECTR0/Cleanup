using ELECTRIS;
using Microsoft.Win32;

public class ConfigurationForm : Form
{
    private ListBox lstFiles;
    private ListBox lstDirectories;
    private Button btnAddFile;
    private Button btnAddDirectory;
    private Button btnRemoveFile;
    private Button btnRemoveDirectory;
    private CheckBox chkPersistentMode;
    private Label lblInterval;
    private NumericUpDown numInterval;
    private Button btnSave;
    private CleanupConfig config;

    public ConfigurationForm()
    {
        this.Text = "ELECTRO-Cleanup Configuration";
        this.Icon = ResourceHelper.GetAppIcon();
        this.Size = new Size(600, 500);
        this.StartPosition = FormStartPosition.CenterScreen;

        config = Program.LoadConfig();
        InitializeComponents();
        LoadConfigToForm();
    }

    private void InitializeComponents()
    {
        // Files section
        Label lblFiles = new Label
        {
            Text = "Files to Clean:",
            Location = new Point(20, 20),
            AutoSize = true
        };
        this.Controls.Add(lblFiles);

        lstFiles = new ListBox
        {
            Location = new Point(20, 50),
            Size = new Size(400, 120),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        this.Controls.Add(lstFiles);

        btnAddFile = new Button
        {
            Text = "Add File",
            Location = new Point(430, 50),
            Size = new Size(120, 30),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        btnAddFile.Click += BtnAddFile_Click;
        this.Controls.Add(btnAddFile);

        btnRemoveFile = new Button
        {
            Text = "Remove File",
            Location = new Point(430, 90),
            Size = new Size(120, 30),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        btnRemoveFile.Click += BtnRemoveFile_Click;
        this.Controls.Add(btnRemoveFile);

        // Directories section
        Label lblDirectories = new Label
        {
            Text = "Directories to Clean:",
            Location = new Point(20, 190),
            AutoSize = true
        };
        this.Controls.Add(lblDirectories);

        lstDirectories = new ListBox
        {
            Location = new Point(20, 220),
            Size = new Size(400, 120),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };
        this.Controls.Add(lstDirectories);

        btnAddDirectory = new Button
        {
            Text = "Add Directory",
            Location = new Point(430, 220),
            Size = new Size(120, 30),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        btnAddDirectory.Click += BtnAddDirectory_Click;
        this.Controls.Add(btnAddDirectory);

        btnRemoveDirectory = new Button
        {
            Text = "Remove Directory",
            Location = new Point(430, 260),
            Size = new Size(120, 30),
            Anchor = AnchorStyles.Top | AnchorStyles.Right
        };
        btnRemoveDirectory.Click += BtnRemoveDirectory_Click;
        this.Controls.Add(btnRemoveDirectory);

        // Persistent mode section
        chkPersistentMode = new CheckBox
        {
            Text = "Persistent Mode (Keep monitoring for new files/directories)",
            Location = new Point(20, 360),
            AutoSize = true
        };
        chkPersistentMode.CheckedChanged += ChkPersistentMode_CheckedChanged;
        this.Controls.Add(chkPersistentMode);

        lblInterval = new Label
        {
            Text = "Cleanup Interval (minutes):",
            Location = new Point(40, 390),
            AutoSize = true
        };
        this.Controls.Add(lblInterval);

        numInterval = new NumericUpDown
        {
            Location = new Point(200, 388),
            Size = new Size(80, 22),
            Minimum = 1,
            Maximum = 1440, // Max 24 hours
            Value = 60 // Default 1 hour
        };
        this.Controls.Add(numInterval);

        // Save button
        btnSave = new Button
        {
            Text = "Save Configuration",
            Location = new Point(240, 430),
            Size = new Size(150, 30),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Right
        };
        btnSave.Click += BtnSave_Click;
        this.Controls.Add(btnSave);

        // Set initial state for interval controls
        UpdateIntervalControlsState();
    }

    private void LoadConfigToForm()
    {
        lstFiles.Items.Clear();
        foreach (string file in config.FilesToClean)
        {
            lstFiles.Items.Add(file);
        }

        lstDirectories.Items.Clear();
        foreach (string dir in config.DirectoriesToClean)
        {
            lstDirectories.Items.Add(dir);
        }

        chkPersistentMode.Checked = config.PersistentMode;
        numInterval.Value = config.CleanupIntervalMinutes;
    }

    private void BtnAddFile_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog dialog = new OpenFileDialog())
        {
            dialog.Multiselect = true;
            dialog.Title = "Select Files to Clean";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in dialog.FileNames)
                {
                    if (!lstFiles.Items.Contains(file))
                    {
                        lstFiles.Items.Add(file);
                    }
                }
            }
        }
    }

    private void BtnRemoveFile_Click(object sender, EventArgs e)
    {
        if (lstFiles.SelectedIndex != -1)
        {
            lstFiles.Items.RemoveAt(lstFiles.SelectedIndex);
        }
    }

    private void BtnAddDirectory_Click(object sender, EventArgs e)
    {
        using (FolderBrowserDialog dialog = new FolderBrowserDialog())
        {
            dialog.Description = "Select Directory to Clean";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (!lstDirectories.Items.Contains(dialog.SelectedPath))
                {
                    lstDirectories.Items.Add(dialog.SelectedPath);
                }
            }
        }
    }

    private void BtnRemoveDirectory_Click(object sender, EventArgs e)
    {
        if (lstDirectories.SelectedIndex != -1)
        {
            lstDirectories.Items.RemoveAt(lstDirectories.SelectedIndex);
        }
    }

    private void ChkPersistentMode_CheckedChanged(object sender, EventArgs e)
    {
        UpdateIntervalControlsState();
    }

    private void UpdateIntervalControlsState()
    {
        lblInterval.Enabled = chkPersistentMode.Checked;
        numInterval.Enabled = chkPersistentMode.Checked;
    }

    private void BtnSave_Click(object sender, EventArgs e)
    {
        // Update config from form
        config.FilesToClean.Clear();
        foreach (var item in lstFiles.Items)
        {
            config.FilesToClean.Add(item.ToString());
        }

        config.DirectoriesToClean.Clear();
        foreach (var item in lstDirectories.Items)
        {
            config.DirectoriesToClean.Add(item.ToString());
        }

        config.PersistentMode = chkPersistentMode.Checked;
        config.CleanupIntervalMinutes = (int)numInterval.Value;

        // Save config
        Program.SaveConfig(config);

        // Set up autostart if needed
        SetupAutostart();

        // Close configuration form
        this.Close();

        // Run cleanup immediately
        CleanupManager.ExecuteCleanup();

        // If not in persistent mode, exit application
        if (!config.PersistentMode)
        {
            Application.Exit();
        }
    }

    private void SetupAutostart()
    {
        try
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (config.FilesToClean.Count > 0 || config.DirectoriesToClean.Count > 0)
                {
                    string appPath = Application.ExecutablePath;
                    key.SetValue("ELECTRO-Cleanup", $"\"{appPath}\"");
                }
                else
                {
                    if (key.GetValue("ELECTRO-Cleanup") != null)
                    {
                        key.DeleteValue("ELECTRO-Cleanup", false);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error setting up autostart: {ex.Message}", "ELECTRO-Cleanup",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}