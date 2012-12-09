using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWHudTutorial.Types
{
    public class User
    {
        public const string TOKEN_QUOTES = "%QUOTE";
        public const int OFFLINE = -1;

        public static bool IsTourist(string name)
        {
            return name.StartsWith("\"") && name.EndsWith("\"");
        }

        public string Name;
        public Languages Language = Languages.None;
        public int Session;
        public bool Hidden;
        public DateTime LastSeen;
        public bool Online { get { return Session != OFFLINE; } }

        public HudScene scene;
        public HudScene Scene
        {
            get { return scene; }
            set
            {
                if (scene != null)
                    scene.Dispose();

                scene = value;
            }

        }

        public static User FromString(string data)
        {
            var parts = data.Split(new[] { ',' }, StringSplitOptions.None);
            return new User
            {
                Name = parts[0].Replace(TOKEN_QUOTES, "\""),
                Hidden = bool.Parse(parts[1]),
                LastSeen = DateTime.Parse(parts[2])
            };
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}",
                Name.Replace("\"", TOKEN_QUOTES),
                Hidden,
                LastSeen.ToString());
        }

        public void Save()
        {
            Log.Debug("Users", "Saving settings for {0}", Name);
            Settings.Users.Set(Name, ToString());
            Settings.Ini.Save();
        }
    }
}
