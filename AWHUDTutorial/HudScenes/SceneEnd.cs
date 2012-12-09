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
    public class SceneEnd
    {
        public const string NAME = "end";
        public const string HUD_TAGLINE = "tagline";
        public const string HUD_BTN_PREV = "btnPrev";
        public const string HUD_BTN_HIDE = "btnHide";
        public const string HUD_BTN_LANG = "btnLang";

        public static HudScene Create(int session)
        {
            Log.Debug("Scenes", "Creating end scene for {0}", session);
            var scene = new HudScene { Session = session, Name = NAME };
            var user = AWHT.BaseApp.GetUser(session);
            var lang = user.Language;

            var hudTagline = new HudPanel(
                Lang.Get(lang, "End"), AW.Color.ColorWhite,
                new Metric
                {
                    Rect = new Rectangle(-512, -80, 1024, 64),
                    Origin = HudOrigin.Center
                },
                true);
            scene[HUD_TAGLINE] = hudTagline;

            scene[SceneCommon.HUD_ATTRACT_TL] = SceneCommon.Attract(true);
            scene[SceneCommon.HUD_ATTRACT_BR] = SceneCommon.Attract(false);
            scene[HUD_BTN_PREV] = HudPanel.CreatePrev(lang);
            scene[HUD_BTN_HIDE] = HudPanel.CreateHide(lang);

            scene[HUD_BTN_HIDE].Metrics = new Metric
            {
                Origin = HudOrigin.Center,
                Rect = new Rectangle(44, 16, 128, 32)
            };

            scene[HUD_BTN_PREV].Metrics = new Metric
            {
                Origin = HudOrigin.Center,
                Rect = new Rectangle(-200, 16, 128, 32)
            };

            scene[HUD_BTN_PREV].Clicked += (s, x, y) => { scene.GotoScene(SceneTut4.Create(s)); };
            scene[HUD_BTN_HIDE].Clicked += (s, x, y) => { scene.Minimize(HudOrigin.BottomLeft, 0, 0); };
            scene.MinimizeHud = SceneCommon.CreateMinimizeButton();
            return scene;
        }
    }
}
