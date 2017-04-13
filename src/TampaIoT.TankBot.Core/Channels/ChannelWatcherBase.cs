using LagoVista.Core.Commanding;
using LagoVista.Core.PlatformSupport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using TampaIoT.TankBot.Core.Interfaces;

namespace TampaIoT.TankBot.Core.Channels
{
    public abstract class ChannelWatcherBase : IChannelWatcher
    {
        public event PropertyChangedEventHandler PropertyChanged;

        ITankBotLogger _logger;

        public event EventHandler<IChannel> DeviceFoundEvent;
        public event EventHandler<IChannel> DeviceRemovedEvent;
        public event EventHandler<IChannel> DeviceConnectedEvent;

        public event EventHandler ClearDevices;

        public ChannelWatcherBase(ITankBotLogger logger)
        {
            _logger = logger;

            StartWatcherCommand = RelayCommand.Create(StartWatcher);
            StopWatcherCommand = RelayCommand.Create(StopWatcher);
            StopWatcherCommand.Enabled = false;
        }

        public void RaiseDeviceFoundEvent(IChannel channel)
        {
            Services.DispatcherServices.Invoke(() => DeviceFoundEvent?.Invoke(this, channel));
        }

        public void RaiseDeviceRemovedEvent(IChannel channel)
        {
            Services.DispatcherServices.Invoke(() => DeviceRemovedEvent?.Invoke(this, channel));
        }

        public void RaiseDeviceConnectedEvent(IChannel channel)
        {
            Services.DispatcherServices.Invoke(() => DeviceConnectedEvent?.Invoke(this, channel));
        }

        public void RaiseClearDevicesEvent()
        {
            Services.DispatcherServices.Invoke(() => ClearDevices?.Invoke(this, null));
        }

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            Services.DispatcherServices.Invoke(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)));
        }

        protected abstract void StartWatcher();
        protected abstract void StopWatcher();

        public RelayCommand StartWatcherCommand { get; private set; }
        public RelayCommand StopWatcherCommand { get; private set; }

        public ObservableCollection<IChannel> Channels { get; private set; }
    }
}
