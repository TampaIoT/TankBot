using LagoVista.Core.Commanding;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.ViewModels;
using System.Collections.ObjectModel;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.Core.Models;

namespace TampaIoT.TankBot.App.ViewModels
{

    public class MainViewModel : ViewModelBase
    {
        public ObservableCollection<Notification> Notifications { get { return  TankBotLogger.Notifications; } }

        public MainViewModel()
        {
            TogglePaneVisibilityCommand = new RelayCommand(TagglePaneVisibility);
            SearchVM = new SearchTankBotViewModel();
            TankBotVM = new TankBotViewModel(TankBotLogger);
        }

        private async void ConnectTankBotAsync(IChannel channel)
        {
            if (await channel.ConnectAsync())
            {
                TankBotVM.SetChannel(channel);
                if (IsPaneOpen)
                {
                    IsPaneOpen = false;
                }
            }
        }

        public ITankBotLogger TankBotLogger { get { return App.TheApp.Logger; } }
        IChannel _currentChannel;
        public IChannel CurrentChannel
        {
            get { return _currentChannel; }
            set { ConnectTankBotAsync(value); }
        }

        private ITankBot _tankBot;
        public ITankBot TankBot
        {
            get { return _tankBot;}
            set{ Set(ref _tankBot, value);}
        }

        public void TagglePaneVisibility()
        {
            IsPaneOpen = !IsPaneOpen;
            if(IsPaneOpen)
            {
                SearchVM.StartSearching();
            }
            else
            {
                SearchVM.StopSearching();
                SearchVM.AvailableChannels.Clear();
            }
        }

        public RelayCommand TogglePaneVisibilityCommand { get; private set; }

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

        public SearchTankBotViewModel SearchVM { get; private set; }
        public TankBotViewModel TankBotVM { get; private set; }
    }
}
