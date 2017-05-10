using LagoVista.Core.Commanding;
using LagoVista.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.Core.Models;
using TampaIoT.TankBot.UWP.Core.Watchers;

namespace TampaIoT.TankBot.App.ViewModels
{
    public class SearchTankBotViewModel : ViewModelBase
    {
        private ObservableCollection<IChannelWatcher> _channelWatchers;
        private ObservableCollection<IChannel> _availableChannels;

        public SearchTankBotViewModel()
        {
            _channelWatchers = new ObservableCollection<IChannelWatcher>();
            _availableChannels = new ObservableCollection<IChannel>();

            StartSearchingCommand = new RelayCommand(StartSearching);
            StopSearchingCommand = new RelayCommand(StopSearching);
            RefreshCommand = new RelayCommand(Refresh);

            if (Debugger.IsAttached && false)
            {
                RegisterChannelWatcher(new FakeWatcher(App.TheApp.Logger));
            }

            RegisterChannelWatcher(new UPNPChannelWatcher(App.TheApp.Logger));
            RegisterChannelWatcher(new BluetoothChannelWatcher(App.TheApp.Logger));
        }

        public void RegisterChannelWatcher(IChannelWatcher channelWatcher)
        {
            channelWatcher.DeviceFoundEvent += ChannelWatcher_DeviceFoundEvent;
            channelWatcher.DeviceRemovedEvent += ChannelWatcher_DeviceRemovedEvent;
            channelWatcher.ClearDevices += ChannelWatcher_ClearDevices;
            _channelWatchers.Add(channelWatcher);
        }

        public void Refresh()
        {
            StopSearching();
            StartSearching();
        }

        public void StartSearching()
        {
            foreach (var watcher in _channelWatchers)
            {
                watcher.Start();
            }
        }

        public void StopSearching()
        {
            foreach (var watcher in _channelWatchers)
            {
                watcher.Stop();
            }

            AvailableChannels.Clear();
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
                    if (!AvailableChannels.Where(channel => channel.Id == e.Id).Any())
                    {
                        AvailableChannels.Add(e);
                    }
                }
            }
        }

        private void ChannelWatcher_ClearDevices(object sender, System.EventArgs e)
        {
            AvailableChannels.Clear();
        }

        public RelayCommand StartSearchingCommand { get; private set; }

        public RelayCommand RefreshCommand { get; private set; }

        public RelayCommand StopSearchingCommand { get; private set; }

       
        public ObservableCollection<IChannel> AvailableChannels { get { return _availableChannels; } }
    }
}
