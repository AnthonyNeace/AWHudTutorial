using AW;
using System;
using System.Collections.Generic;
using Rectangle = System.Drawing.Rectangle;

namespace AWHudTutorial.Types
{
    public delegate void HudPanelClick (int session, int x, int y);

    /// <summary>
    /// Defines an AW texture pair with texture name and mask
    /// </summary>
    public struct Texture
    {
        public string Name;
        public string Mask;

        public Texture(string name, string mask = null)
        {
            this.Name = name;
            this.Mask = mask;
        }

        public override string ToString()
        {
            return Name + (Mask == null ? "" : "|" + Mask);
        }
    }

    /// <summary>
    /// Defines measurements for a hud panel
    /// </summary>
    public struct Metric
    {
        public Rectangle Rectangle;
        public HudOrigin Origin;

        public Metric(Rectangle rect, HudOrigin origin)
        {
            this.Rectangle = rect;
            this.Origin    = origin;
        }

        public Metric(int x, int y, int w, int h, HudOrigin origin)
        {
            this.Rectangle = new Rectangle(x, y, w, h);
            this.Origin    = origin;
        }
    }

    public class HudPanel : IDisposable
    {
        public static Dictionary<int, int> IDPools = new Dictionary<int,int>();

        static string tag = "HudPanel";

        public static int GetNextId(int session)
        {
            if (IDPools[session]++ > 1000)
                IDPools[session] = 1;

            Log.Fine( tag, "Next ID for session '{0}' is '{1}'", session, IDPools[session] );
            return IDPools[session];
        }

        event HudPanelClick clicked;
        public event HudPanelClick Clicked
        {
            add {
                if (clicked != null) return;

                AWHT.Bot.EventHudClick += OnClick;
                clicked += value;
            }

            remove {
                clicked -= value;

                if (clicked == null)
                    AWHT.Bot.EventHudClick -= OnClick;
            }
        }

        public string Name;
        public bool   IsDisposed;
        public bool   IsVisible;
        public Hud    MainHud;
        public Hud    ShadowHud;

        public int  Id       { get { return MainHud.Id; } }
        public bool Shadowed { get { return ShadowHud != null; } }

        /// <summary>
        /// Gets or sets the Z order of this panel
        /// </summary>
        public int ZOrder {
            get { return MainHud.ZOrder; }
            set
            {
                MainHud.ZOrder = value;
                if (Shadowed) ShadowHud.ZOrder = value + 1;
            }
        }

        /// <summary>
        /// Gets or sets the session (along with ID) this panel is attached to
        /// </summary>
        public int Session
        {
            get { return MainHud.Session; }
            set
            {
                MainHud.Session = value;
                MainHud.Id = GetNextId(value);

                if (Shadowed)
                {
                    ShadowHud.Session = value;
                    ShadowHud.Id = GetNextId(value);
                }
            }
        }

        public Metric Metrics
        {
            set
            {
                MainHud.SizeX  = value.Rectangle.Width;
                MainHud.SizeY  = value.Rectangle.Height;
                MainHud.X      = value.Rectangle.Left;
                MainHud.Y      = value.Rectangle.Top;
                MainHud.Origin = value.Origin;

                if ( Shadowed )
                {
                    ShadowHud.SizeX  = value.Rectangle.Width + 32;
                    ShadowHud.SizeY  = value.Rectangle.Height + 32;
                    ShadowHud.X      = value.Rectangle.Left - 16;
                    ShadowHud.Y      = value.Rectangle.Top - 16;
                    ShadowHud.Origin = value.Origin;
                }
            }
        }

        /// <summary>
        /// Gets or sets hud clickability
        /// </summary>
        public bool Clickable
        {
            set
            {
                if (value)
                    MainHud.Flags = MainHud.Flags | HudFlag.Clicks;
                else
                    MainHud.Flags = MainHud.Flags ^ HudFlag.Clicks;
            }

            get { return (MainHud.Flags & HudFlag.Clicks) == HudFlag.Clicks; }
        }

        HudPanel(Metric metric, bool shadowed)
        {
            MainHud = new Hud
            {
                ZOrder  = 1,
                Opacity = 0.0f,
                Flags   = HudFlag.Transition | HudFlag.Stretch | HudFlag.Clamp
            };

            if (shadowed)
            {
                ShadowHud = new Hud
                {
                    Type    = HudType.Image,
                    Text    = "clr_white",
                    Flags   = HudFlag.Stretch | HudFlag.Transition,                    
                    ZOrder  = 10,
                    Color   = new AW.Color(0xA8, 0xC0, 0xFF),
                    Opacity = 0.0f,
                    Session = this.Session
                };
            }

            this.Metrics = metric;
        }

        /// <summary>
        /// Initializes an image-based panel with an optional shadow
        /// </summary>
        public HudPanel(Texture asset, Metric metric, bool shadowed = false)
            : this(metric, shadowed)
        {
            MainHud.Type = HudType.Image;
            MainHud.Text = asset.ToString();
            MainHud.Color = Colors.White;
        }

        /// <summary>
        /// Initializes a text-based panel with an optional shadow
        /// </summary>
        public HudPanel(string message, Color color, Metric metric, bool shadowed = false)
            : this(metric, shadowed)
        {
            MainHud.Type = HudType.Text;
            MainHud.Text = message;
            MainHud.Color = color;
        }

        public void OnClick(IInstance sender, EventCancelToken token)
        {
            if (IsDisposed || token.Cancelled) return;

            // Security
            var id = AWHT.Bot.Attributes.HudElementId;
            var session = AWHT.Bot.Attributes.HudElementSession;
            if (id != this.Id || session != this.Session)
                return;

            // Fire
            var x = AWHT.Bot.Attributes.HudElementClickX;
            var y = AWHT.Bot.Attributes.HudElementClickY;
            token.Cancelled = true;
            clicked(session, x, y);
        }

        public void Show()
        {
            if (IsDisposed) return;
            MainHud.Opacity = 1f;
            AWHT.Bot.HudCreate(MainHud);

            if (Shadowed)
            {
                ShadowHud.Opacity = 1f;
                AWHT.Bot.HudCreate(ShadowHud);
            }

            IsVisible = true;
        }

        public void Hide(HudOrigin origin = HudOrigin.BottomLeft, int x = 0, int y = 0)
        {
            if (IsDisposed) return;
            IsVisible = false;
            AWHT.Bot.HudDestroy(Session, MainHud.Id);
            if (Shadowed) AWHT.Bot.HudDestroy(Session, ShadowHud.Id);
        }

        public void Dispose()
        {
            // Destroy elements
            Hide();
            IsDisposed = true;
            MainHud = null;
            if (Shadowed) ShadowHud = null;

            // Delink bot
            AWHT.Bot.EventHudClick -= OnClick;
            clicked = null;
        }

        public static HudPanel CreateNext(Languages lang)
        {
            var label = Lang.Get(lang, "Next");
            var panel = new HudPanel(label, Colors.White,
                new Metric
                {
                    Origin = HudOrigin.Right,
                    Rectangle = new Rectangle(-160, 0, 128, 32)
                }, true);

            panel.Clickable       = true;
            panel.ShadowHud.Color = Colors.OrangeRed;
            return panel;
        }

        public static HudPanel CreatePrev(Languages lang)
        {
            var label = Lang.Get(lang, "Prev");
            var panel = new HudPanel(label, Colors.White,
                new Metric
                {
                    Origin = HudOrigin.Left,
                    Rectangle = new Rectangle(32, 0, 128, 32)
                }, true);

            panel.Clickable       = true;
            panel.ShadowHud.Color = Colors.OrangeRed;
            return panel;
        }

        public static HudPanel CreateHide(Languages lang)
        {
            var label = Lang.Get(lang, "Hide");
            return new HudPanel(label, Colors.White,
                new Metric
                {
                    Origin = HudOrigin.Right,
                    Rectangle = new Rectangle(-160, 64, 128, 32)
                }, true) { Clickable = true };
        }

        public static HudPanel CreateLanguage()
        {
            var label = Lang.Core.Get("Language");
            return new HudPanel(label, Colors.White,
                new Metric
                {
                    Origin = HudOrigin.Right,
                    Rectangle = new Rectangle(-160, 128, 128, 32)
                }, true) { Clickable = true };
        }
    }
}
