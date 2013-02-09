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
        public const string NAME               = "introscene";
        public const string HUD_WELCOME        = "welcome";
        public const string HUD_CLICKFORHELP   = "clickforhelp";
        public const string HUD_CLICKFORHELPBG = "clickforhelpbg";
        public const string HUD_CLICKTOCLOSE   = "clicktoclose";
        public const string HUD_BG             = "bgfill";

        public static HudScene Create( int session)
        {
            Log.Debug("Scenes", "Creating intro scene for {0}", session);
            var scene = new HudScene { Session = session, Name = NAME };

            var background = SceneCommon.CreateBackground();
            var i = 0;
            foreach (var bg in background)
            {
                bg.MainHud.Color  = new AW.Color(0xA8, 0xC0, 0xFF);
                scene[HUD_BG + i] = bg;
                i++;
            }

            var hudWelcome = new HudPanel(
                new Texture { Name = "hud-welcome.png" },
                new Metric
                {
                    Rectangle = new Rectangle(-256, -200, 512, 128),
                    Origin = HudOrigin.Center
                });

            scene[HUD_WELCOME] = hudWelcome;

            var hudClickForHelp = new HudPanel(
                Lang.Core.Get("ClickForHelp").Replace('|', '\n'), Colors.White,
                new Metric
                {
                    Rectangle = new Rectangle(-150, -50, 300, 200),
                    Origin    = HudOrigin.Center,
                });

            var hudHelpBanner = new HudPanel(
                new Texture { Name = "clr_white" },
                new Metric
                {
                    Rectangle = new Rectangle(-1024, -50, 2048, 200),
                    Origin    = HudOrigin.Center
                });

            hudHelpBanner.MainHud.Color    = Colors.OrangeRed;
            hudClickForHelp.MainHud.Flags |= HudFlag.Highlight;
            hudClickForHelp.Clickable      = true;
            hudClickForHelp.Clicked       += (s, x, y) =>
            {
                var user = AWHT.BaseApp.GetUser(s);

                if (user.Language != Languages.None)
                    scene.GotoScene( SceneTut1.Create(s) );
                else
                    scene.GotoScene( SceneLanguage.Create(s) );
            };
            scene[HUD_CLICKFORHELP]   = hudClickForHelp;
            scene[HUD_CLICKFORHELPBG] = hudHelpBanner;

            var hudClickToClose = new HudPanel(
                "X", new AW.Color(255, 255, 255),
                new Metric
                {
                    Rectangle = new Rectangle(16, -64 - 16, 64, 64),
                    Origin    = HudOrigin.BottomLeft,
                });

            hudClickToClose.Clickable  = true;
            hudClickToClose.Clicked   += (s, x, y) =>
            {
                scene.Minimize(HudOrigin.BottomLeft,0,0);
            };
            scene[HUD_CLICKTOCLOSE] = hudClickToClose;

            scene.MinimizeHud = SceneCommon.CreateMinimizeButton();
            return scene;
        }
    }
}
