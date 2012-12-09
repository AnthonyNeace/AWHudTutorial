using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nini.Config;

namespace AWHudTutorial
{
    public static class Settings
    {
        public static IniConfigSource Ini;
        public static IConfig Core;
        public static IConfig Network;
        public static IConfig Users;

        public static void Load()
        {
            Ini = new IniConfigSource("Settings.ini");
            Core = Ini.Configs["Core"];
            Network = Ini.Configs["Network"];
            Users = Ini.Configs["Users"];
        }
    }
}
