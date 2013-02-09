using AW;
using AWHudTutorial.Types;
using System;
using System.Drawing;

namespace AWHudTutorial
{
    public class SceneLanguage
    {
        public const string NAME       = "language";
        public const string HUD_CHOOSE = "choose";
        public const string HUD_BG     = "bgfill";

        public static HudScene Create(int session)
        {
            Log.Debug("Scenes", "Creating language selection scene for {0}", session);
            var scene = new HudScene { Session = session, Name = NAME };

            var background = SceneCommon.CreateBackground();
            var b = 0;
            foreach (var bg in background)
            {
                scene[HUD_BG + b] = bg;
                b++;
            }

            var hudChoose = new HudPanel(
                "Select your language", Colors.White,
                new Metric
                {
                    Rectangle = new Rectangle(-256, -128, 512, 64),
                    Origin    = HudOrigin.Center
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
                    langLabel, Colors.White,
                    new Metric
                    {
                        Rectangle = new Rectangle(left, 64 * (i/2), 256, 32),
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
