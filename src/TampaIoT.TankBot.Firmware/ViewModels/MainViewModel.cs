using LagoVista.Core.Commanding;
using LagoVista.Core.ViewModels;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.Firmware.Networking;
using TampaIoT.TankBot.Firmware.Managers;
using System.Collections.Generic;
using System;
using System.Linq;
using LagoVista.Core.UWP.Services;
using Windows.Networking.Connectivity;

namespace TampaIoT.TankBot.Firmware.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        IConnectionManager _connectionManager;
        System.Threading.Timer _timer;

        public MainViewModel(IConnectionManager connectionManager)
        {
            StartCompassCalibrationCommand = new RelayCommand(StartCompassCalibrarion);
            _connectionManager = connectionManager;

            MoveCommand = new RelayCommand((param) => Move(Convert.ToInt16(param)));
            StopCommand = new RelayCommand(Stop);

            _timer = new System.Threading.Timer(RefreshTimerTick, null, 0, 1000);
        }

        private void StartCompassCalibrarion()
        {

        }

        private string GetLocalIp()
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            if (icp?.NetworkAdapter == null) return null;
            var hostname =
                NetworkInformation.GetHostNames()
                    .SingleOrDefault(
                        hn =>
                            hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                            == icp.NetworkAdapter.NetworkAdapterId);

            // the ip address
            return hostname?.CanonicalName;
        }


        private void RefreshTimerTick(object state)
        {
            DispatcherServices.Invoke(() =>
            {
                RaisePropertyChanged(nameof(IsmBotConnected));
                RaisePropertyChanged(nameof(IsNetworkConnected));
                RaisePropertyChanged(nameof(IsCompassConnected));
                RaisePropertyChanged(nameof(AreClientsConnected));
                if (IsCompassConnected)
                {
                    CompassMessage = $"Online: {_connectionManager.SensorManager.Compass.RawX.Value}x{_connectionManager.SensorManager.Compass.RawY.Value}";
                }
                else
                {
                    CompassMessage = "Compass Offline";
                }

                if (ConnectionManager.Server != null)
                {
                    ClientsMessage = $"{ConnectionManager.Server.Clients.Count} Clients Connected";
                }
                else
                {
                    ClientsMessage = "Not Connected";
                }

                if(IsNetworkConnected && ConnectionManager.Server != null)
                {
                    NetworkMessage = "IP Address: " + GetLocalIp();
                }
                else
                {
                    NetworkMessage = "Network Offline";
                }
            });
        }

        public void Move(short direction)
        {
            ConnectionManager.TankBot.Move(Speed, direction);
        }

        public void Stop()
        {
            ConnectionManager.TankBot.Stop();
        }

        public ITankBot TankBot { get { return _connectionManager.TankBot; } }

        public IEnumerable<IClient> Clients { get { return _connectionManager.Clients; } }


        public RelayCommand StartCompassCalibrationCommand { get; private set; }

        public bool IsmBotConnected
        {
            get { return App.TheApp.HasMBotConnection; }
        }

        public bool AreClientsConnected
        {
            get { return ConnectionManager.Server == null ? false : ConnectionManager.Server.Clients.Count > 0; }
        }

        public new bool IsNetworkConnected
        {
            get { return App.TheApp.HasInternetConnection; }
        }

        public bool IsCompassConnected
        {
            get { return ConnectionManager.SensorManager == null ? false : ConnectionManager.SensorManager.Compass.IsOnline; }
        }

        private String _clientMessage;
        public string ClientsMessage
        {
            get { return _clientMessage; }
            set { Set(ref _clientMessage, value); }
        }

        private String _networkMessage;
        public string NetworkMessage
        {
            get { return _networkMessage; }
            set { Set(ref _networkMessage, value); }
        } 

        private String _compassMessage;
        public string CompassMessage
        {
            get { return _compassMessage; }
            set { Set(ref _compassMessage, value); }
        } 

        public IConnectionManager ConnectionManager
        {
            get{ return _connectionManager; }
        }

        private short _speed;
        public short Speed
        {
            get { return _speed; }
            set { Set(ref _speed, value); }
        }


        public RelayCommand MoveCommand { get; private set; }

        public RelayCommand StopCommand { get; private set; }
    }
}
