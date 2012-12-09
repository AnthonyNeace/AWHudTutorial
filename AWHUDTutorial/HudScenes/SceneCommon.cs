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
    public class SceneCommon
    {
        public const string HUD_ATTRACT_TL = "atttl";
        public const string HUD_ATTRACT_BR = "attbr";

        public static Metric METRIC_TLATTRACT = new Metric
        {
            Rect = new Rectangle(0, 0, 512, 512),
            Origin = HudOrigin.TopLeft
        };

        public static Metric METRIC_BRATTRACT = new Metric
        {
            Rect = new Rectangle(-512, -512, 512, 512),
            Origin = HudOrigin.BottomRight
        };

        static Metric METRIC_MINBUTTON = new Metric
            {
                Rect = new Rectangle(16, -128 - 16, 128, 128),
                Origin = HudOrigin.BottomLeft,
            };

        public static HudPanel CreateMinimizeButton()
        {
            return new HudPanel(
                new Texture { Name = "hud-minimized.png" },
                METRIC_MINBUTTON, false);
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
