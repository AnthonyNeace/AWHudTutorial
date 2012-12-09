using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AW;

namespace AWHudTutorial.Types
{
    public class HudScene : HashSet<HudPanel>, IDisposable
    {
        public string Name = "Scene";

        int session;
        /// <summary>
        /// Gets the session ID this scene is attached to. Setting one mass-sets the
        /// session ID to all panels.
        /// </summary>
        public int Session
        {
            get { return session; }
            set
            {
                session = value;
                foreach (var pan in this)
                    pan.Session = value;
            }
        }

        HudPanel minimizeHud;
        /// <summary>
        /// Sets a HudPanel that will be clicked to unminimize a minimized scene.
        /// HudPanels set will have their Clickable attribute set to true and an event
        /// created and set.
        /// </summary>
        public HudPanel MinimizeHud
        {
            set
            {
                if (minimizeHud != null && value != null)
                    minimizeHud.Dispose();

                if (value != null)
                {
                    value.Clickable = true;
                    value.Clicked += onMinClicked;
                    value.Session = session;
                }

                minimizeHud = value;
            }

            get { return minimizeHud; }
        }

        /// <summary>
        /// Gets or sets a HudPanel in this scene.
        /// Set HudPanels' session numbers are automatically set to this scene's number
        /// </summary>
        public HudPanel this[string name]
        {
            get {
                foreach (var panel in this)
                    if (panel.Name != null && panel.Name == name)
                        return panel;

                return null;
            }

            set
            {
                value.Name = name;
                value.Session = Session;
                this.Add(value);
            }
        }

        public void Show()
        {
            Log.Fine("Scene", "Showing scene {0} for {1}", Name, Session);
            if (minimizeHud != null) minimizeHud.Hide();

            foreach (var panel in this)
                panel.Show();
        }

        public void Minimize(HudOrigin origin, int x, int y)
        {
            Log.Fine("Scene", "Minimizing scene {0} for {1}", Name, Session);
            if (minimizeHud != null) minimizeHud.Show();

            AWHT.BaseApp.GetUser(session).Hidden = true;
            AWHT.BaseApp.GetUser(session).Save();

            foreach (var panel in this)
                panel.Hide(origin, x, y);
        }

        public void Hide()
        {
            Log.Fine("Scene", "Hiding scene {0} for {1}", Name, Session);
            foreach (var panel in this)
                panel.Hide();
        }

        public void GotoScene(HudScene scn)
        {
            AWHT.BaseApp.GetUser(session).Scene = scn;
            scn.Show();
        }

        public void Dispose()
        {
            foreach (var panel in this)
                panel.Dispose();

            if (MinimizeHud != null)
            {
                MinimizeHud.Dispose();
                MinimizeHud = null;
            }
        }

        void onMinClicked(int s, int x, int y) {
            AWHT.BaseApp.GetUser(s).Hidden = false;
            AWHT.BaseApp.GetUser(s).Save();
            this.Show();
        }
    }
}
