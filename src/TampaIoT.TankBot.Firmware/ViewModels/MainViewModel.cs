using LagoVista.Core.Commanding;
using LagoVista.Core.ViewModels;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.Firmware.Networking;
using TampaIoT.TankBot.Firmware.Managers;
using System.Collections.Generic;

namespace TampaIoT.TankBot.Firmware.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        IConnectionManager _connectionManager;

        public MainViewModel(IConnectionManager connectionManager)
        {
            StartCompassCalibrationCommand = new RelayCommand(StartCompassCalibrarion);
            _connectionManager = connectionManager;
        }

        private void StartCompassCalibrarion()
        {
            
        }

        public ITankBot TankBot { get { return _connectionManager.TankBot; } }

        public IEnumerable<IClient> Clients { get { return _connectionManager.Clients; } }


        public RelayCommand StartCompassCalibrationCommand { get; private set; }
    }
}
