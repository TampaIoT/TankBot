using LagoVista.Core.Commanding;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.Core.Messages;
using LagoVista.Core.ViewModels;
using TampaIoT.TankBot.App.Controllers;

namespace TampaIoT.TankBot.App.ViewModels
{
    public class LocalClientTankBotViewModel : ViewModelBase, IClientTankBotViewModel
    {
        private mBlockTankBot _mblockTankBot;
        ITankBotLogger _logger;
        IJoyStick _joyStick;
        int _direction;

        public LocalClientTankBotViewModel(IChannel channel, ITankBotLogger logger, IJoyStick joyStick)
        {
            _mblockTankBot = new mBlockTankBot(channel, logger);
            _joyStick = joyStick;
            _logger = logger;
            _joyStick.JoyStickUpdated += _joyStick_JoyStickUpdated;
        }

        private void _joyStick_JoyStickUpdated(object sender, Windows.Foundation.Point e)
        {
            
        }

        public ChannelTypes ChannelType { get { return ChannelTypes.Remote; } }

        public void Move(int direction)
        {
            _direction = direction;

        }

        public void Stop()
        {
            Speed = 0;
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

        public RelayCommand MoveCommand { get; private set; }

        public RelayCommand StopCommand { get; private set; }

        SensorData _sensorData;
        public SensorData SensorData
        {
            get { return _sensorData; }
            set { Set(ref _sensorData, value); }
        }

        public void Disconnect()
        {
            
        }
    }
}
