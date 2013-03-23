using AW;
using AWHudTutorial.Types;
using System.Drawing;

namespace AWHudTutorial
{
    public class SceneCommon
    {
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
    }
}
