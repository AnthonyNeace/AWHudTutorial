using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rectangle = System.Drawing.Rectangle;
using AW;

namespace AWHudTutorial.Types
{
    public delegate void HudPanelClick (int session, int x, int y);
    public struct Texture
    {
        public string Name;
        public string Mask;

        public override string ToString()
        {
            return Name + (Mask == null ? "" : "|" + Mask);
        }
    }

    public struct Metric
    {
        public Rectangle Rect;
        public HudOrigin Origin;
    }

    public class HudPanel : IDisposable
    {
        public static Dictionary<int, int> IDPools = new Dictionary<int,int>();

        public static int GetNextID(int session)
        {
            if (IDPools[session]++ > 65535)
                IDPools[session] = 1;

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
        public bool IsDisposed;
        public bool IsVisible;
        public Metric Metrics;
        public Hud MainHud;
        public Hud ShadowHud;

        public int Id { get { return MainHud.Id; } }
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
                MainHud.Id = GetNextID(value);

                if (Shadowed)
                {
                    ShadowHud.Session = value;
                    ShadowHud.Id = GetNextID(value);
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
            this.Metrics = metric;
            MainHud = new Hud
            {
                SizeX = metric.Rect.Width,
                SizeY = metric.Rect.Height,
                X = metric.Rect.Left,
                Y = metric.Rect.Top,
                ZOrder = 1,
                Origin = metric.Origin,
                Opacity = 0.0f,
                Flags = HudFlag.Transition | HudFlag.Stretch | HudFlag.Clamp
            };

            if (shadowed)
            {
                var percentX = (MainHud.SizeX / 100) * 10;
                var percentY = (MainHud.SizeY / 100) * 10;
                ShadowHud = new Hud
                {
                    Type = HudType.Image,
                    Text = "hud-shadow.png",
                    Flags = HudFlag.Stretch | HudFlag.Transition,
                    SizeX = MainHud.SizeX + percentX,
                    SizeY = MainHud.SizeY + percentY,
                    X = MainHud.X - (percentX / 2),
                    Y = MainHud.Y - (percentY / 2),
                    ZOrder = 10,
                    Color = 0x000000,
                    Origin = MainHud.Origin,
                    Opacity = 0.0f,
                    Session = this.Session
                };
            }
        }

        /// <summary>
        /// Initializes an image-based panel with an optional shadow
        /// </summary>
        public HudPanel(Texture asset, Metric metric, bool shadowed = false)
            : this(metric, shadowed)
        {
            MainHud.Type = HudType.Image;
            MainHud.Text = asset.ToString();
            MainHud.Color = Color.ColorWhite;
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
            MainHud.Origin = Metrics.Origin;
            MainHud.X = Metrics.Rect.X;
            MainHud.Y = Metrics.Rect.Y;
            MainHud.SizeX = Metrics.Rect.Width;
            MainHud.SizeY = Metrics.Rect.Height;
            MainHud.Opacity = 1f;
            AWHT.Bot.HudCreate(MainHud);

            if (Shadowed)
            {
                ShadowHud.Origin = Metrics.Origin;
                ShadowHud.X = Metrics.Rect.X - 16;
                ShadowHud.Y = Metrics.Rect.Y - 16;
                ShadowHud.SizeX = Metrics.Rect.Width + 32;
                ShadowHud.SizeY = Metrics.Rect.Height + 32;
                ShadowHud.Opacity = 0.5f;
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
            return new HudPanel(label, AW.Color.ColorWhite,
                new Metric
                {
                    Origin = HudOrigin.Right,
                    Rect = new Rectangle(-160, 0, 128, 32)
                }, true) { Clickable = true };
        }

        public static HudPanel CreatePrev(Languages lang)
        {
            var label = Lang.Get(lang, "Prev");
            return new HudPanel(label, AW.Color.ColorWhite,
                new Metric
                {
                    Origin = HudOrigin.Left,
                    Rect = new Rectangle(32, 0, 128, 32)
                }, true) { Clickable = true };
        }

        public static HudPanel CreateHide(Languages lang)
        {
            var label = Lang.Get(lang, "Hide");
            return new HudPanel(label, AW.Color.ColorWhite,
                new Metric
                {
                    Origin = HudOrigin.Right,
                    Rect = new Rectangle(-160, 64, 128, 32)
                }, true) { Clickable = true };
        }

        public static HudPanel CreateLanguage()
        {
            var label = Lang.Core.Get("Language");
            return new HudPanel(label, AW.Color.ColorWhite,
                new Metric
                {
                    Origin = HudOrigin.Right,
                    Rect = new Rectangle(-160, 128, 128, 32)
                }, true) { Clickable = true };
        }
    }
}
