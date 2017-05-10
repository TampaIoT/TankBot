using LagoVista.Core.Commanding;
using LagoVista.Core.PlatformSupport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace TampaIoT.TankBot.Core
{
    public abstract class TankBotBase : INotifyPropertyChanged
    {
        public const string UNKOWN_VERSION = "??";

        System.Threading.Timer _sensorRefreshTimer;
        private bool _sensorTimeEnabled = false;

        public enum Commands
        {
            Forward,
            Backwards,
            Left,
            Right,
            Stop,
            Reset
        }

        public String Id { get; set; }
        public String Name { get; set; }
        public String DeviceName { get; set; }
        public String DeviceTypeId { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            Services.DispatcherServices.Invoke(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            });
        }

        public TankBotBase()
        {
            ForwardCommand = RelayCommand.Create(SendCommand, Commands.Forward);
            StopCommand = RelayCommand.Create(SendCommand, Commands.Stop);
            BackwardsCommand = RelayCommand.Create(SendCommand, Commands.Backwards);
            LeftCommand = RelayCommand.Create(SendCommand, Commands.Left);
            RightCommand = RelayCommand.Create(SendCommand, Commands.Right);
            ResetCommand = RelayCommand.Create(SendCommand, Commands.Reset);

            _sensorRefreshTimer = new Timer(_sensorRefreshTimer_Tick, null, Timeout.Infinite, Timeout.Infinite);

            FirmwareVersion = UNKOWN_VERSION;
        }

        private String _firmwareVersion;
        public String FirmwareVersion
        {
            get { return _firmwareVersion; }
            set
            {
                _firmwareVersion = value;
                RaisePropertyChanged();

                if (_firmwareVersion != UNKOWN_VERSION)
                {
                    StartSensorRefreshTimer();
                }
            }
        }

        protected abstract void SendCommand(object cmd);

        public RelayCommand RefreshSensorsCommand { get; private set; }

        public RelayCommand ForwardCommand { get; private set; }
        public RelayCommand StopCommand { get; private set; }
        public RelayCommand BackwardsCommand { get; private set; }

        public RelayCommand LeftCommand { get; private set; }
        public RelayCommand RightCommand { get; private set; }

        public RelayCommand ResetCommand { get; private set; }

        public void StartSensorRefreshTimer()
        {
            _sensorTimeEnabled = true;
            _sensorRefreshTimer.Change(0, 1000);            
        }

        protected abstract void RefreshSensors();

        protected abstract void SpeedUpdated(short speed);


        private void _sensorRefreshTimer_Tick(object state)
        {
            if (_sensorTimeEnabled)
            {
                RefreshSensors();
            }
        }

        private short _speed = 100;
        public short Speed
        {
            get { return _speed; }
            set
            {
                _speed = value;
                RaisePropertyChanged();
            }
        }

        private DateTime? _lastBotContact;
        public DateTime? LastBotContact
        {
            get { return _lastBotContact; }
            set
            {
                _lastBotContact = value;
                RaisePropertyChanged();
            }
        }

        public void PauseRefreshTimer()
        {
            _sensorTimeEnabled = false;
            _sensorRefreshTimer.Change(0, Timeout.Infinite);
        }

        public void StartRefreshTimer()
        {
            _sensorTimeEnabled = true;
            _sensorRefreshTimer.Change(0, 1000);
        }

        private String _apiMode = "Uknown/Not Connected";
        public string APIMode
        {
            get { return _apiMode; }
            set
            {
                _apiMode = value;
                RaisePropertyChanged();
            }
        }
    }
}
