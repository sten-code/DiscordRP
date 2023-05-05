using System;

namespace DiscordRP
{
    public class Config
    {
        public static string RoamingPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\DiscordRP";
        public static string SettingsPath = $"{RoamingPath}\\settings.json";
        public static string UpdaterPath = $"{RoamingPath}\\DiscordRPUpdater.exe";
    }
}
