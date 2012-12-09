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
    public class SceneIntro
    {
        public const string NAME = "introscene";
        public const string HUD_WELCOME = "welcome";
        public const string HUD_CLICKFORHELP = "clickforhelp";
        public const string HUD_CLICKTOCLOSE = "clicktoclose";

        public static HudScene Create( int session)
        {
            Log.Debug("Scenes", "Creating intro scene for {0}", session);
            var scene = new HudScene { Session = session, Name = NAME };

            var hudWelcome = new HudPanel(
                new Texture { Name = "hud-welcome.png" },
                new Metric
                {
                    Rect = new Rectangle(-256, 64, 512, 128),
                    Origin = HudOrigin.Top
                },
                true);

            scene[HUD_WELCOME] = hudWelcome;

            var hudClickForHelp = new HudPanel(
                Lang.Core.Get("ClickForHelp").Replace('|', '\n'), new AW.Color(150, 150, 150),
                new Metric
                {
                    Rect = new Rectangle(-300, -200, 256, 128),
                    Origin = HudOrigin.Bottom,
                },
                true);

            hudClickForHelp.MainHud.Flags |= HudFlag.Highlight;
            hudClickForHelp.Clickable = true;
            hudClickForHelp.Clicked += (s, x, y) =>
            {
                var user = AWHT.BaseApp.GetUser(s);

                if (user.Language != Languages.None)
                    scene.GotoScene( SceneTut1.Create(s) );
                else
                    scene.GotoScene( SceneLanguage.Create(s) );
            };
            scene[HUD_CLICKFORHELP] = hudClickForHelp;

            var hudClickToClose = new HudPanel(
                Lang.Core.Get("ClickToClose").Replace('|', '\n'), new AW.Color(150, 150, 150),
                new Metric
                {
                    Rect = new Rectangle(44, -200, 256, 128),
                    Origin = HudOrigin.Bottom,
                },
                true);

            hudClickToClose.MainHud.Flags |= HudFlag.Highlight; 
            hudClickToClose.Clickable = true;
            hudClickToClose.Clicked += (s, x, y) =>
            {
                scene.Minimize(HudOrigin.BottomLeft,0,0);
            };
            scene[HUD_CLICKTOCLOSE] = hudClickToClose;

            scene[SceneCommon.HUD_ATTRACT_TL] = SceneCommon.Attract(true);
            scene[SceneCommon.HUD_ATTRACT_BR] = SceneCommon.Attract(false);
            scene.MinimizeHud = SceneCommon.CreateMinimizeButton();
            return scene;
        }
    }
}
