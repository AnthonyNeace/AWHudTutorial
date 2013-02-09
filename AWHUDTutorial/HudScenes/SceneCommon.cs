using AW;
using AWHudTutorial.Types;
using System.Drawing;

namespace AWHudTutorial
{
    public class SceneCommon
    {
        public const string HUD_ATTRACT_TL = "atttl";
        public const string HUD_ATTRACT_BR = "attbr";

        public static Metric METRIC_TLATTRACT = new Metric
        {
            Rectangle = new Rectangle(0, 0, 512, 512),
            Origin = HudOrigin.TopLeft
        };

        public static Metric METRIC_BRATTRACT = new Metric
        {
            Rectangle = new Rectangle(-512, -512, 512, 512),
            Origin = HudOrigin.BottomRight
        };

        public static Metric METRIC_MINBUTTON = new Metric
        {
            Rectangle = new Rectangle(16, -64 - 16, 64, 64),
            Origin = HudOrigin.BottomLeft,
        };

        public static HudPanel CreateMinimizeButton()
        {
            return new HudPanel(
                new Texture { Name = "hud-minimized.png" },
                METRIC_MINBUTTON, false);
        }

        public static HudPanel[] CreateBackground()
        {
            var panels = new HudPanel[4];

            panels[0] = new HudPanel(
                new Texture("hud-fill.png"),
                new Metric(0, 0, 2048, 2048, HudOrigin.TopLeft));

            panels[1] = new HudPanel(
                new Texture("hud-fill.png"),
                new Metric(2048, 0, 2048, 2048, HudOrigin.TopLeft));

            panels[2] = new HudPanel(
                new Texture("hud-fill.png"),
                new Metric(0, 2048, 2048, 2048, HudOrigin.TopLeft));

            panels[3] = new HudPanel(
                new Texture("hud-fill.png"),
                new Metric(2048, 2048, 2048, 2048, HudOrigin.TopLeft));

            foreach (var pan in panels)
            {
                pan.MainHud.Color  = new AW.Color(0xE8, 0xEF, 0xFF);
                //pan.MainHud.Color  = new AW.Color(0xA8, 0xC0, 0xFF);
                pan.MainHud.ZOrder = 200;
            }

            return panels;
        }

        /// <summary>
        /// Generates an attractor-style graphic of a world screenshot on a specified
        /// corner
        /// </summary>
        public static HudPanel Attract(bool topleft)
        {
            var maxAttracts = Settings.Core.GetInt("NumberOfAttracts") + 1;
            var attract = "hud-attract" + AWHT.Random.Next(1, maxAttracts) + ".jpg";
            return new HudPanel(
                new Texture
                {
                    Name = attract,
                    Mask = topleft ? "hud-attract1m.bmp" : "hud-attract2m.bmp"
                },
                topleft
                    ? METRIC_TLATTRACT
                    : METRIC_BRATTRACT) { ZOrder = 50 };
        }
    }
}
