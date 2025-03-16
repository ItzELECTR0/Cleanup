using System.Diagnostics;
using ELECTRIS;
using Microsoft.VisualBasic.FileIO;

public class CleanupManager
{
    public static void ExecuteCleanup()
    {
        CleanupConfig config = Program.LoadConfig();

        foreach (string filePath in config.FilesToClean)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    FileSystem.DeleteFile(filePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting file {filePath}: {ex.Message}");
            }
        }

        foreach (string dirPath in config.DirectoriesToClean)
        {
            try
            {
                if (Directory.Exists(dirPath))
                {
                    FileSystem.DeleteDirectory(dirPath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting directory {dirPath}: {ex.Message}");
            }
        }
    }
}