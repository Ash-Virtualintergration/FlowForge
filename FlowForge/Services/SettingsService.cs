using System;
using System.IO;
using FlowForge.Models;
using Newtonsoft.Json;


namespace FlowForge.Services
{
    public static class SettingsService
    {
        private static string BaseFolder => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "FlowForge");


        private static string SettingsPath => Path.Combine(BaseFolder, "appsettings.json");


        public static void SaveSettings(AppSettings settings)
        {
            Directory.CreateDirectory(BaseFolder);
            var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(SettingsPath, json);
        }


        public static AppSettings LoadSettings()
        {
            try
            {
                if (!File.Exists(SettingsPath)) return new AppSettings();
                var json = File.ReadAllText(SettingsPath);
                var settings = JsonConvert.DeserializeObject<AppSettings>(json);
                return settings ?? new AppSettings();
            }
            catch
            {
                return new AppSettings();
            }
        }
    }
}