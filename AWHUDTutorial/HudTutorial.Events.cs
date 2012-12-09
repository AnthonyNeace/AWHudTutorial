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
        #region Event management
        void setupEvents()
        {
            AWBot.EventAvatarAdd += onEnter;
            AWBot.EventAvatarDelete += onLeave;
            AWBot.EventUniverseDisconnect += onDisconnect;
            AWBot.EventWorldDisconnect += onDisconnect;
        }

        void removeEvents()
        {
            AWBot.EventAvatarAdd -= onEnter;
            AWBot.EventAvatarDelete -= onLeave;
            AWBot.EventUniverseDisconnect -= onDisconnect;
            AWBot.EventWorldDisconnect -= onDisconnect;
        }
        #endregion
    }
}
