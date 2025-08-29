using System.Text.Json;

namespace PdfCutter
{
    public class PrinterSettings
    {
        public string? LastUsedPrinter { get; set; }
        public string? LastUsedPaperName { get; set; }
        public bool Landscape { get; set; }

        private static string SettingsPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PdfCutter",
            "printersettings.json"
        );

        public static PrinterSettings Load()
        {
            try
            {
                if (File.Exists(SettingsPath))
                {
                    string json = File.ReadAllText(SettingsPath);
                    return JsonSerializer.Deserialize<PrinterSettings>(json) ?? new PrinterSettings();
                }
            }
            catch
            {
                // If there's any error, return default settings
            }
            return new PrinterSettings();
        }

        public void Save()
        {
            try
            {
                string directoryPath = Path.GetDirectoryName(SettingsPath)!;
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                string json = JsonSerializer.Serialize(this);
                File.WriteAllText(SettingsPath, json);
            }
            catch
            {
                // Ignore save errors
            }
        }
    }
}