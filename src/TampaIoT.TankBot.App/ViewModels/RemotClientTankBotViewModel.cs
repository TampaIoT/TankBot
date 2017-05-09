using System;
using LagoVista.Core.Commanding;
using TampaIoT.TankBot.Core.Messages;
using LagoVista.Core.ViewModels;
using TampaIoT.TankBot.App.Controllers;
using TampaIoT.TankBot.Core.Interfaces;

namespace TampaIoT.TankBot.App.ViewModels
{
    public class RemotClientTankBotViewModel : ViewModelBase, IClientTankBotViewModel
    {
        IJoyStick _joyStick;
        INetworkChannel _networkChannel;

        public RemotClientTankBotViewModel(IChannel channel, IJoyStick joyStick)
        {
            _networkChannel = channel as INetworkChannel;
            _networkChannel.Disconnected += _networkChannel_Disconnected;
            if (_networkChannel == null)
                throw new Exception("Must provide something that implements INetworkChannel to RemoteClientTankBotViewModel");

            _networkChannel.NetworkMessageReceived += _networkChannel_NetworkMessageReceived;

            MoveCommand = new RelayCommand((param) => Move((int)param));
            _joyStick.JoyStickUpdated += _joyStick_JoyStickUpdated;                
        }

        private void _networkChannel_Disconnected(object sender, string e)
        {
            
        }

        public ChannelTypes ChannelType { get { return ChannelTypes.Remote; } }

        private void _networkChannel_NetworkMessageReceived(object sender, Core.Models.NetworkMessage e)
        {
            
        }

        private void _joyStick_JoyStickUpdated(object sender, Windows.Foundation.Point e)
        {
            
        }

        private SensorData _sensorData;
        public SensorData SensorData
        {
            get { return _sensorData; }
            set { Set(ref _sensorData, value); }
        }
        
        public void Move(int direction)
        {

        }

        public void Stop()
        {

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

        public void Disconnect()
        {
            throw new NotImplementedException();
        }

        public void SetChannel(IChannel channel)
        {
            throw new NotImplementedException();
        }
    }
}
