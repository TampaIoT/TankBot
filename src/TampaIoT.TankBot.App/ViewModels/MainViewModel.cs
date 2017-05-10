using LagoVista.Core.Commanding;
using LagoVista.Core.ViewModels;
using System.Collections.ObjectModel;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.Core.Models;

namespace TampaIoT.TankBot.App.ViewModels
{

    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<Notification> Notifications { get { return TankBotLogger.Notifications; } }

        public MainViewModel()
        {
            TogglePaneVisibilityCommand = new RelayCommand(TagglePaneVisibility);
            SearchCommand = new RelayCommand(Search);
            SearchVM = new SearchTankBotViewModel();
            Message = "Press Search for TankBots to Find Your TankBot.";
        }

        private async void ConnectTankBotAsync(IChannel channel)
        {
            if (channel != null)
            {
                if (await channel.ConnectAsync())
                {
                    if (IsPaneOpen)
                    {
                        IsPaneOpen = false;
                    }

                    if (_currentChannel != null)
                    {
                        _currentChannel.Disconnected -= _currentChannel_Disconnected;
                    }

                    _currentChannel = channel;
                    _currentChannel.Disconnected += _currentChannel_Disconnected;

                    RaisePropertyChanged(nameof(CurrentChannel));

                    switch (channel.ChannelType)
                    {
                        case ChannelTypes.Local: ClientTankBotViewModel = new LocalClientTankBotViewModel(channel, TankBotLogger, App.TheApp.JoyStick); break;
                        case ChannelTypes.Simulated:
                        case ChannelTypes.Remote: ClientTankBotViewModel = new RemoteClientTankBotViewModel(channel, App.TheApp.JoyStick); break;
                    }
                }
            }
        }

        public ITankBotLogger TankBotLogger { get { return App.TheApp.Logger; } }

        IChannel _currentChannel;
        public IChannel CurrentChannel
        {
            get { return _currentChannel; }
            set
            {
                ConnectTankBotAsync(value);
            }
        }

        private void _currentChannel_Disconnected(object sender, string e)
        {
            _currentChannel = null;
            RaisePropertyChanged(nameof(CurrentChannel));
            ClientTankBotViewModel = null;
        }

        public void TagglePaneVisibility()
        {
            IsPaneOpen = !IsPaneOpen;
            if (IsPaneOpen)
            {
                SearchVM.StartSearching();
            }
            else
            {
                SearchVM.StopSearching();
                SearchVM.AvailableChannels.Clear();
            }
        }

        private bool _isPaneOpen = false;
        public bool IsPaneOpen
        {
            get { return _isPaneOpen; }
            set
            {
                _isPaneOpen = value;
                RaisePropertyChanged();
            }
        }

        public void Search()
        {
            if (!IsPaneOpen)
            {
                SearchVM.StartSearching();
                IsPaneOpen = true;
            }
        }

        public SearchTankBotViewModel SearchVM { get; private set; }

        IClientTankBotViewModel _clientTankBotViewModel;
        public IClientTankBotViewModel ClientTankBotViewModel
        {
            get { return _clientTankBotViewModel; }
            set { Set(ref _clientTankBotViewModel, value); }
        }
        public RelayCommand TogglePaneVisibilityCommand { get; private set; }

        public RelayCommand SearchCommand { get; private set; }

        private string _message = null;
        public string Message
        {
            get { return _message; }
            set { Set(ref _message, value); }
        }
    }
}
