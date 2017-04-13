using LagoVista.Core.Commanding;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace TampaIoT.TankBot.Core.Interfaces
{
    /// <summary>
    /// Channel Watcher is used to Monitor an External Channel and look for Robots
    /// that become available to connect.
    /// </summary>
    public interface IChannelWatcher : INotifyPropertyChanged
    {
        event EventHandler<IChannel> DeviceFoundEvent;
        event EventHandler<IChannel> DeviceRemovedEvent;
        event EventHandler<IChannel> DeviceConnectedEvent;

        event EventHandler ClearDevices;

        ObservableCollection<IChannel> Channels { get; }

        RelayCommand StartWatcherCommand { get; }
        RelayCommand StopWatcherCommand { get; }
    }
}
