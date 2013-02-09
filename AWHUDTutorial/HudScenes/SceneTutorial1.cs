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
    public class SceneTut1
    {
        public const string NAME         = "tut1";
        public const string HUD_TAGLINE  = "tagline";
        public const string HUD_BTN_NEXT = "btnNext";
        public const string HUD_BTN_HIDE = "btnHide";
        public const string HUD_BTN_LANG = "btnLang";

        public static HudScene Create(int session)
        {
            Log.Debug("Scenes", "Creating tutorial 1 scene for {0}", session);
            var scene = new HudScene { Session = session, Name = NAME };
            var user = AWHT.BaseApp.GetUser(session);
            var lang = user.Language;

            var hudTagline = new HudPanel(
                Lang.Get(lang, "Tut1a"), Colors.White,
                new Metric
                {
                    Rectangle = new Rectangle(-512, 64, 1024, 64),
                    Origin = HudOrigin.Top
                },
                true);
            scene[HUD_TAGLINE] = hudTagline;

            var hudDiagramA = new HudPanel(
                new Texture { Name = "hud-tut1a.png" },
                new Metric
                {
                    Rectangle = new Rectangle(-300, 0, 256, 128),
                    Origin    = HudOrigin.Center
                });
            scene["diagramA"] = hudDiagramA;

            scene[HUD_BTN_NEXT] = HudPanel.CreateNext(lang);
            scene[HUD_BTN_HIDE] = HudPanel.CreateHide(lang);
            scene[HUD_BTN_LANG] = HudPanel.CreateLanguage();

            scene[HUD_BTN_NEXT].Clicked += (s, x, y) => { scene.GotoScene(SceneTut2.Create(s));       };
            scene[HUD_BTN_HIDE].Clicked += (s, x, y) => { scene.Minimize(HudOrigin.BottomLeft, 0, 0); };
            scene[HUD_BTN_LANG].Clicked += (s, x, y) => { scene.GotoScene(SceneLanguage.Create(s));   };
            scene.MinimizeHud = SceneCommon.CreateMinimizeButton();
            return scene;
        }
    }
}
