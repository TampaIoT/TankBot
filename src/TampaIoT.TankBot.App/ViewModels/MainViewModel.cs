using LagoVista.Core.Commanding;
using LagoVista.Core.PlatformSupport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.Core.Models;

namespace TampaIoT.TankBot.App.ViewModels
{

    public class MainViewModel : INotifyPropertyChanged
    {
        ITankBotLogger _logger;

        public event EventHandler<IChannel> ChannelConnected;

   //     private ObservableCollection<ISoccerBotCommands> _connectedDevices;
        private ObservableCollection<IChannelWatcher> _channelWatchers;
        private ObservableCollection<IChannel> _availableChannels;
        public ObservableCollection<Notification> Notifications { get { return _logger.Notifications; } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            //TODO: Should be a design time check and not run this.
            Services.DispatcherServices.Invoke(() =>
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName))
            );
        }

        public MainViewModel(ITankBotLogger logger)
        {
            _logger = logger;
            _availableChannels = new ObservableCollection<IChannel>();
            _channelWatchers = new ObservableCollection<IChannelWatcher>();
     //       _connectedDevices = new ObservableCollection<ISoccerBotCommands>();
            StartWatchersCommand = new RelayCommand(StartWatchers);
            StopWatchersCommand = new RelayCommand(StopWatchers);
        }

        public ITankBotLogger Logger { get { return _logger; } set { _logger = value; } }

        public void RegisterChannelWatcher(IChannelWatcher channelWatcher)
        {
            channelWatcher.DeviceFoundEvent += ChannelWatcher_DeviceFoundEvent;
            channelWatcher.DeviceRemovedEvent += ChannelWatcher_DeviceRemovedEvent;
            channelWatcher.ClearDevices += ChannelWatcher_ClearDevices;
            _channelWatchers.Add(channelWatcher);
        }

        private void ChannelWatcher_ClearDevices(object sender, System.EventArgs e)
        {
            AvailableChannels.Clear();
           // ConnectedDevices.Clear();
        }

        private void ChannelWatcher_DeviceRemovedEvent(object sender, IChannel e)
        {
            lock (this)
            {
                if (AvailableChannels.Contains(e))
                {
                    AvailableChannels.Remove(e);
                }
            }
        }

        private void ChannelWatcher_DeviceFoundEvent(object sender, IChannel e)
        {
            lock (this)
            {
                if (!AvailableChannels.Contains(e))
                {
                    e.Connected += channel_Connected;
                    AvailableChannels.Add(e);
                }
            }
        }

        private void channel_Connected(object sender, IChannel device)
        {
            ChannelConnected?.Invoke(this, device);
        }

        ITankBot _activeDevice;
        public ITankBot ActiveDevice
        {
            get { return _activeDevice; }
            set
            {
                _activeDevice = value;
                RaisePropertyChanged();
            }
        }

        ITankBot _activeRemoteDevice;
        public ITankBot ActiveRemoteDevice
        {
            get { return _activeRemoteDevice; }
            set
            {
                _activeRemoteDevice = value;
                RaisePropertyChanged();
            }
        }


        public void StartWatchers()
        {
            foreach (var watcher in _channelWatchers)
            {
                watcher.StartWatcherCommand.Execute(null);
            }

            StartWatchersCommand.Enabled = false;
            StopWatchersCommand.Enabled = true;
        }

        public void StopWatchers()
        {
            foreach (var watcher in _channelWatchers)
            {
                watcher.StopWatcherCommand.Execute(null);
            }

            StartWatchersCommand.Enabled = true;
            StopWatchersCommand.Enabled = false;
        }


        public RelayCommand StartWatchersCommand { get; private set; }
        public RelayCommand StopWatchersCommand { get; private set; }

       // public ObservableCollection<ISoccerBotCommands> ConnectedDevices { get { return _connectedDevices; } }

        public ObservableCollection<IChannel> AvailableChannels { get { return _availableChannels; } }

    }
}
