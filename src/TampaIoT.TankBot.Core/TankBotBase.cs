using LagoVista.Core.Commanding;
using LagoVista.Core.PlatformSupport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace TampaIoT.TankBot.Core
{
    public abstract class TankBotBase : INotifyPropertyChanged
    {
        public const string UNKOWN_VERSION = "??";

        public enum Commands
        {
            Forward,
            Backwards,
            Left,
            Right,
            Stop,
            Reset
        }

        ITimer _sensorRefreshTimer;

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

            _sensorRefreshTimer = Services.TimerFactory.Create(TimeSpan.FromMilliseconds(500));

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
            if (_sensorRefreshTimer != null)
            {
                _sensorRefreshTimer.Interval = TimeSpan.FromMilliseconds(500);
                _sensorRefreshTimer.Tick += _sensorRefreshTimer_Tick;
                StartRefreshTimer();
            }
        }

        protected abstract void RefreshSensors();

        protected abstract void SpeedUpdated(short speed);


        private void _sensorRefreshTimer_Tick(object sender, object e)
        {
            RefreshSensors();
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
            _sensorRefreshTimer.Stop();
        }

        public void StartRefreshTimer()
        {
            _sensorRefreshTimer.Start();
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
