using System;
using System.Threading;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using AW;
using AWHudTutorial.Types;
using Nini;

namespace AWHudTutorial
{
    partial class AWHT
    {
        public static Random Random = new Random();
        public static AWHT BaseApp;
        static ConsoleLogger Logger;

        public static Instance Bot { get { return BaseApp.AWBot; } }

        static void Main()
        {
            Console.Title = "AWHudTutorial";
            Thread.CurrentThread.Priority = ThreadPriority.Lowest;

            while (true)
            {
                try
                {
                    BaseApp = new AWHT();
                    BaseApp.start();
                    BaseApp.loop();
                }
                catch (Exception e)
                {
                    Log.FullStackTrace(e);
                    Log.Severe("AWHT", "Bot crashed; restarting...");

                    if (BaseApp.AWBot != null)
                        BaseApp.AWBot.Dispose();
                }
            }
        }

        ~AWHT()
        {
            Users = null;
        }

        /// <summary>
        /// Start the AWHT application
        /// </summary>
        void start()
        {
            if (Logger == null)
                Logger = new ConsoleLogger();

            // Load settings files
            Lang.Load();
            Settings.Load();
            Log.LogLevel = (LogLevels) Enum.Parse(
                typeof(LogLevels),
                Settings.Core.Get("LogLevel") );

            // Finally connect
            Connect();
            AWBot.CallbackHudResult += (i,j) => { };
        }

        /// <summary>
        /// Main update loop of the bot
        /// </summary>
        void loop()
        {
            while (true) { Utility.Wait(500); }
        }

        /// <summary>
        /// Gets a user by name from memory or disk, otherwise returns a new one
        /// </summary>
        public User GetUser(string name)
        {
            // Memory
            foreach (var user in Users)
                if (user.Name == name)
                    return user;
            
            // Disk
            if (Settings.Users.Contains(name))
            {
                var user = User.FromString(Settings.Users.Get(name));
                var maxAge = Settings.Core.GetInt("MaxHoursPersist");

                // Forget setting if last seen a long time ago
                if (DateTime.Now.Subtract(user.LastSeen).TotalHours < maxAge)
                {
                    Users.Add(user);
                    return user;
                }
            }

            // Create
            var newUser = new User { Name = name };
            Users.Add(newUser);
            return newUser;
        }

        /// <summary>
        /// Gets a user by session; does not create a new one if not found
        /// </summary>
        public User GetUser(int session)
        {
            foreach (var user in Users)
                if (user.Session == session)
                    return user;

            return null;
        }
        
        #region Event handlers
        void onEnter(IInstance sender, EventCancelToken token)
        {
            var name = sender.Attributes.AvatarName;
            var session = sender.Attributes.AvatarSession;

            if (Settings.Core.GetBoolean("TouristsOnly") && !User.IsTourist(name))
                return;

            var user = GetUser(name);

            user.Session = session;
            user.LastSeen = DateTime.Now;
            user.Save();

            Log.Info("HUD", "Generating HUD for {0} (session {1})", name, session);
            AWBot.HudClear(session);
            user.Scene = SceneIntro.Create(session);

            if (user.Hidden)
                user.Scene.MinimizeHud.Show();
            else
                user.Scene.Show();
        }

        void onLeave(IInstance sender, EventCancelToken token)
        {
            var name = sender.Attributes.AvatarName;
            var user = GetUser(name);
            user.Session = User.OFFLINE;
            user.Save();
        }
        #endregion
    }
}
