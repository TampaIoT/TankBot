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
            MoveCommand = new RelayCommand((param) => Move((short)param));
            StopCommand = new RelayCommand(Stop);
            _joyStick = joyStick;

            _joyStick.JoyStickUpdated += _joyStick_JoyStickUpdated; ;
        }

        private void _joyStick_JoyStickUpdated(object sender, Windows.Foundation.Point e)
        {
            throw new NotImplementedException();
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
            set { Set(ref _sensorData, value); }
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

    }
}
