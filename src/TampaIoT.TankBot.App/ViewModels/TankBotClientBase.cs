using LagoVista.Core.Commanding;
using LagoVista.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TampaIoT.TankBot.App.Controllers;
using TampaIoT.TankBot.Core.Messages;

namespace TampaIoT.TankBot.App.ViewModels
{
    public abstract class TankBotClientBase : ViewModelBase
    {
        IJoyStick _joyStick;
        public TankBotClientBase(IJoyStick joyStick)
        {
            MoveCommand = new RelayCommand((param) => Move(Convert.ToInt16(param)));
            StopCommand = new RelayCommand(Stop);
            _joyStick = joyStick;

            _joyStick.JoyStickUpdated += _joyStick_JoyStickUpdated; ;
        }

        private void _joyStick_JoyStickUpdated(object sender, Windows.Foundation.Point e)
        {
            JoystickDisplayX = e.X * 100;
            JoystickDisplayY = (-e.Y * 100);
        }

        private void _networkChannel_Disconnected(object sender, string e)
        {

        }

        public RelayCommand MoveCommand { get; private set; }

        public RelayCommand StopCommand { get; private set; }

        public abstract void Move(short direction);


        public abstract void Stop();

        private SensorData _sensorData;
        public SensorData SensorData
        {
            get { return _sensorData; }
            set
            {
                _sensorData = value;
                DispatcherServices.Invoke(() =>
                {
                    RaisePropertyChanged();
                });
            }
        }

        bool _isConnected;
        public bool IsConnected
        {
            get { return _isConnected; }
            set { Set(ref _isConnected, value); }
        }

        private short _speed;
        public short Speed
        {
            get { return _speed; }
            set { Set(ref _speed, value); }
        }


        private double _joyStickDisplayX = 100;
        public double JoystickDisplayX
        {
            get { return _joyStickDisplayX; }
            set
            {
                /* Add 100 to center it in the control */
                Set(ref _joyStickDisplayX, value + 100);
            }
        }


        private double _joyStickDisplayY = 100;
        public double JoystickDisplayY
        {
            get { return _joyStickDisplayY; }
            set
            {     /* Add 100 to center it in the control */
                Set(ref _joyStickDisplayY, value + 100);
            }
        }
    }
}
