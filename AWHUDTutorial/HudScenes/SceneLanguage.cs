using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using AWHudTutorial.Types;
using AW;

namespace AWHudTutorial
{
    public class SceneLanguage
    {
        public const string NAME = "language";
        public const string HUD_CHOOSE = "choose";

        public static HudScene Create(int session)
        {
            Log.Debug("Scenes", "Creating language selection scene for {0}", session);
            var scene = new HudScene { Session = session, Name = NAME };

            var hudChoose = new HudPanel(
                "Select your language", AW.Color.ColorWhite,
                new Metric
                {
                    Rect = new Rectangle(-256, 64, 512, 64),
                    Origin = HudOrigin.Top
                },
                true);

            scene[HUD_CHOOSE] = hudChoose;

            var langs = Enum.GetNames(typeof(Languages));
            for (var i = 0; i < 6; i++)
            {
                var left = i % 2 == 0 ? -300 : 44;
                var lang = langs[i];
                var langLabel = Lang.Ini.Configs[lang].Get("Name");
                var langButton = new HudPanel(
                    langLabel, AW.Color.ColorWhite,
                    new Metric
                    {
                        Rect = new Rectangle(left, 64 * (i/2), 256, 32),
                        Origin = HudOrigin.Center,
                    },
                    true);

                langButton.Clickable = true;
                langButton.Clicked += (s, x, y) =>
                {
                    Log.Fine("Language Scene", "Session {0} has chosen {1}", s, lang);
                    AWHT.BaseApp.GetUser(s).Language = (Languages) Enum.Parse(typeof(Languages), lang);

                    scene.GotoScene(SceneTut1.Create(s));
                };
                scene[lang] = langButton;
            }

            return scene;
        }
    }
}
