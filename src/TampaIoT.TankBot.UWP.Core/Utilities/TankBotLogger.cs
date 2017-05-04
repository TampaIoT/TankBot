using System;
using System.Collections.ObjectModel;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.Core.Models;

namespace TampaIoT.TankBot.UWP.Core.Utilities
{
    public class TankBotLogger : ITankBotLogger
    {
        public ObservableCollection<Notification> Notifications { get; private set; }

        public TankBotLogger()
        {
            Notifications = new ObservableCollection<Notification>();
        }

        public void NotifyUserInfo(Notification notification)
        {
            LagoVista.Core.PlatformSupport.Services.DispatcherServices.Invoke(() =>
            {
                Notifications.Insert(0, notification);
            });
        }

        public void NotifyUserInfo(String source, String msg)
        {
            NotifyUserInfo(Notification.CreateInfo(source, msg));
        }

        public void NotifyUserWarning(String source, String msg)
        {
            NotifyUserInfo(Notification.CreateWarning(source, msg));
        }

        public void NotifyUserError(String source, String msg)
        {
            NotifyUserInfo(Notification.CreateError(source, msg));
        }

        public void Clear()
        {
            Notifications.Clear();
        }
    }
}