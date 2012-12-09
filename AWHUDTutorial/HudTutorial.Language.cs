using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nini.Config;

namespace AWHudTutorial
{
    public enum Languages
    {
        None = -1,
        English = 0,
        Spanish,
        Russian,
        German,
        French,
        ChineseSimplified
    }

    public static class Lang
    {
        public static IniConfigSource Ini;
        public static IConfig Core;

        public static void Load()
        {
            Ini = new IniConfigSource("Languages.ini");
            Core = Ini.Configs["Core"];
        }

        public static string Get(Languages lang, string key)
        {
            return Ini
                .Configs[lang.ToString()]
                .Get(key)
                .Replace('|', '\n');
        }
    }
}
