using System;
using System.Collections.ObjectModel;
using TampaIoT.TankBot.Core.Models;

namespace TampaIoT.TankBot.Core.Interfaces
{
    /// <summary>
    /// Interface to define classes to send notificatin messages to users.
    /// </summary>
    public interface ITankBotLogger
    {
        ObservableCollection<Notification> Notifications { get; }
        void NotifyUserInfo(Notification notification);
        void NotifyUserInfo(String source, String msg);
        void NotifyUserWarning(String source, String msg);
        void NotifyUserError(String source, String msg);
        void Clear();

    }
}
