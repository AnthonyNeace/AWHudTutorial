using Nini.Config;
using System;
using System.Linq;
using AWHudTutorial.Types;

namespace AWHudTutorial
{
    public static class Settings
    {
        public static IniConfigSource Ini;
        public static IConfig         Core;
        public static IConfig         Network;
        public static IConfig         Users;
        
        public static void Load()
        {
            Ini     = new IniConfigSource("Settings.ini");
            Core    = Ini.Configs["Core"];
            Network = Ini.Configs["Network"];
            Users   = Ini.Configs["Users"];

            // Cleanup
            Log.Debug("Settings", "Cleaning up expired entries");
            foreach (var key in Users.GetKeys())
                if (IsUserExpired(key))
                    Users.Remove(key);

            Ini.Save();
            Log.Debug("Settings", "Saved cleaned up settings");
        }

        public static bool IsUserExpired(string name)
        {
            if (!Settings.Users.Contains(name))
                return true;

            var maxAge = Settings.Core.GetInt("MaxHoursPersist");
            var data   = Settings.Users.Get(name);
            var user   = new User( data );

            return (user.LastSeen.SecondsToNow() / 60) > maxAge;
        }
    }
}
