using AW;
using AWHudTutorial.Types;
using System;
using System.Drawing;

namespace AWHudTutorial
{
    public class SceneTut4
    {
        public const string NAME = "tut4";
        public const string HUD_TAGLINE = "tagline";
        public const string HUD_BTN_NEXT = "btnNext";
        public const string HUD_BTN_PREV = "btnPrev";
        public const string HUD_BTN_HIDE = "btnHide";
        public const string HUD_BTN_LANG = "btnLang";

        public static HudScene Create(int session)
        {
            Log.Debug("Scenes", "Creating tutorial 4 scene for {0}", session);
            var scene = new HudScene { Session = session, Name = NAME };
            var user  = AWHT.Instance.GetUser(session);
            var lang  = user.Language;

            var hudTagline = new HudPanel(
                Lang.Get(lang, "Tut4a"), Colors.White,
                new Metric
                {
                    Rectangle = new Rectangle(-256, -100, 512, 64),
                    Origin = HudOrigin.Bottom
                },
                true);
            scene[HUD_TAGLINE] = hudTagline;

            var hudDiagramA = new HudPanel(
                new Texture { Name = "hud-tut4b.png" },
                new Metric
                {
                    Rectangle = new Rectangle(222, 32, 64, 128),
                    Origin = HudOrigin.TopLeft
                });

            hudDiagramA.MainHud.Flags |= HudFlag.Highlight;
            hudDiagramA.MainHud.Color  = Colors.Red;
            scene["diagramA"]          = hudDiagramA;

            var hudDiagramB = new HudPanel(
                new Texture { Name = "hud-tut4a.png" },
                new Metric
                {
                    Rectangle = new Rectangle(64, 0, 64, 128),
                    Origin = HudOrigin.Center
                });

            scene["diagramB"] = hudDiagramB;

            scene[HUD_BTN_NEXT] = HudPanel.CreateNext(lang);
            scene[HUD_BTN_PREV] = HudPanel.CreatePrev(lang);
            scene[HUD_BTN_HIDE] = HudPanel.CreateHide(lang);
            scene[HUD_BTN_LANG] = HudPanel.CreateLanguage();

            scene[HUD_BTN_NEXT].Clicked += (s, x, y) => { scene.GotoScene(SceneEnd.Create(s)); };
            scene[HUD_BTN_PREV].Clicked += (s, x, y) => { scene.GotoScene(SceneTut3.Create(s)); };
            scene[HUD_BTN_HIDE].Clicked += (s, x, y) => { scene.Minimize(HudOrigin.BottomLeft, 0, 0); };
            scene[HUD_BTN_LANG].Clicked += (s, x, y) => { scene.GotoScene(SceneLanguage.Create(s)); };
            scene.MinimizeHud = SceneCommon.CreateMinimizeButton();
            return scene;
        }
    }
}
