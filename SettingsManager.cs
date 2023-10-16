using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanningScheduleApp
{
    public class SettingsManager
    {
        private static string settingsFilePath = "config.json";

        public AppSettings LoadSettings()
        {
            if (File.Exists(settingsFilePath))
            {
                string json = File.ReadAllText(settingsFilePath);
                return JsonConvert.DeserializeObject<AppSettings>(json);
            }
            else
            {
                // Если файл настроек не существует, создайте новый объект настроек
                return new AppSettings();
            }
        }

        public void SaveSettings(AppSettings settings)
        {
            string json = JsonConvert.SerializeObject(settings);
            File.WriteAllText(settingsFilePath, json);
        }
    }
}
