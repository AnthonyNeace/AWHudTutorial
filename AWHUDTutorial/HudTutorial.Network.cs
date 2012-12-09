using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AW;
using AWHudTutorial.Types;

namespace AWHudTutorial
{
    partial class AWHT
    {
        public Instance AWBot;
        int connectionAttempts = 1;

        public void Connect()
        {
            try
            {
                if (AWBot == null)
                    AWBot = new Instance();

                // Universe
                Log.Info("Network", "Connecting to universe");
                AWBot.Attributes.LoginOwner = Settings.Network.GetInt("CitizenNumber");
                AWBot.Attributes.LoginPrivilegePassword = Settings.Network.Get("CitizenPrivilegePassword");
                AWBot.Attributes.LoginName = "HudTutorial";
                AWBot.Login();

                // World
                setupEvents();
                var world = Settings.Network.Get("World");
                Log.Info("Network", "Entering world {0} in global mode", world);
                AWBot.Attributes.EnterGlobal = true;


                AWBot.CallbackEnter += (o, r) =>
                {
                    if (r != Result.Success && r != Result.Timeout)
                        Log.Severe("Network", "Connection did not succeed: {0}", r.ToString());
                    else if (!AWBot.Attributes.WorldCaretakerCapability)
                        Log.Severe("Network", "No caretaker ability available in {0}", world);
                    else
                        return;

                    Reconnect();
                };
                AWBot.Enter( world );

                // Sanity check
                /// 09/12/2012 XXX: Commented out due to bug with aw_enter timing out on global mode
                /// with registered avatar events
                //if (result != Result.Success)
                //    throw new Exception(result.ToString());
                //else if (!AWBot.Attributes.WorldCaretakerCapability)
                //    throw new Exception("No caretaker ability");

                Log.Info("Network", "Connected to world {0}", world);
                connectionAttempts = 1;
            }
            catch (Exception ex)
            {
                if (connectionAttempts > 10)
                    throw new Exception("Could not connect: " + ex.Message, ex);
                else
                {
                    Log.Severe("Network", ex.ToString());
                    Log.Warn("Network", "Failed to connect, attempt {0} out of 10", connectionAttempts);
                    connectionAttempts++;
                    Connect();
                }
            }
        }

        public void Reconnect()
        {
            // Destroy current instance (less hassle)
            removeEvents();
            AWBot.Dispose();
            AWBot = null;

            // Recreate and resume
            Connect();
        }

        /// <summary>
        /// Destroys the vpInstance and attempts reconnection upon disconnect
        /// </summary>
        void onDisconnect(IInstance sender, EventCancelToken token)
        {
            Log.Severe("Warn", "Disconnected! Killing bot and reconnecting...");
            Reconnect();
        }
    }
}
