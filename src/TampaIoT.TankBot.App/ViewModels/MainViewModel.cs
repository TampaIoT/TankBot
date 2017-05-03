using LagoVista.Core.Commanding;
using LagoVista.Core.PlatformSupport;
using LagoVista.Core.ViewModels;
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

    public class MainViewModel : ViewModelBase
    {
        ITankBotLogger _logger;

        public ObservableCollection<Notification> Notifications { get { return _logger.Notifications; } }
        
        public MainViewModel()
        {
            TogglePaneVisibilityCommand = new RelayCommand(TagglePaneVisibility);
        }

        public ITankBotLogger TankBotLogger { get { return _logger; } set { _logger = value; } }

        ITankBot _activeDevice;
        public ITankBot CurrentDevice
        {
            get { return _activeDevice; }
            set
            {
                _activeDevice = value;
                RaisePropertyChanged();
            }
        }

        public void TagglePaneVisibility()
        {
            IsPaneOpen = !IsPaneOpen;  
        }

        public RelayCommand TogglePaneVisibilityCommand { get; private set; }

        private bool _isPaneOpen = false;
        public bool IsPaneOpen
        {
            get { return _isPaneOpen; }
            set { _isPaneOpen = value; RaisePropertyChanged(); }
        }
    }
}
