﻿using Serilog;

namespace KawaekoBot.Util
{
    public static class Support
    {
        public static void StartupOperations()
        {
            LogHandler.CreateLoggerConfiguration();
            DirectorySetup.MakeSureAllDirectoriesExist();
            AppSettings.MakeSureSettingsJsonExists();
            AppSettings.ReadSettingsFromFile();
            Log.Information("Startup Operations complete");
        }
    }
}
