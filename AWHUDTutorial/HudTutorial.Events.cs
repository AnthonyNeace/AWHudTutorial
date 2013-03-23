
namespace AWHudTutorial
{
    partial class AWHT
    {
        #region Event management
        void setupEvents()
        {
            AWBot.EventAvatarAdd          += onEnter;
            AWBot.EventAvatarDelete       += onLeave;
            AWBot.EventUniverseDisconnect += onDisconnect;
            AWBot.EventWorldDisconnect    += onDisconnect;
        }

        void removeEvents()
        {
            AWBot.EventAvatarAdd          -= onEnter;
            AWBot.EventAvatarDelete       -= onLeave;
            AWBot.EventUniverseDisconnect -= onDisconnect;
            AWBot.EventWorldDisconnect    -= onDisconnect;
        }
        #endregion
    }
}
