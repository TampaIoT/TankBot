using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.Core.Models;

namespace TampaIoT.TankBot.Firmware.Loggers
{
    public class DebugLogger : ITankBotLogger
    {
        public DebugLogger()
        {
            Notifications = new ObservableCollection<Notification>();
        }

        public ObservableCollection<Notification> Notifications { get; private set; }


        public void Clear()
        {
            Notifications.Clear();
        }

        public void NotifyUserError(string source, string msg)
        {
            Debug.WriteLine($"ERROR: {source} - {msg}");
        }

        public void NotifyUserInfo(Notification notification)
        {
            Notifications.Add(notification);

            Debug.WriteLine($"INFO: {notification.Source} - {notification.Message}");
        }

        public void NotifyUserInfo(string source, string msg)
        {
            Notifications.Add(new Notification(Notification.Levels.Error, source, msg));

            Debug.WriteLine($"INFO: {source} - {msg}");
        }

        public void NotifyUserWarning(string source, string msg)
        {
            Debug.WriteLine($"WARNING: {source} - {msg}");
        }
    }
}
