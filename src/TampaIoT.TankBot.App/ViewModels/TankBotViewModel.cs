using LagoVista.Core.Models.Drawing;
using LagoVista.Core.ViewModels;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.Core.Messages;
using TampaIoT.TankBot.Core.Models;
using TampaIoT.TankBot.UWP.Core.Channels;
using static TampaIoT.TankBot.Core.TankBotBase;

namespace TampaIoT.TankBot.App.ViewModels
{
    public enum ChannelType
    {
        NotConnected,
        Local,
        Remote
    }

    public class TankBotViewModel : ViewModelBase
    {

        ChannelType _channelType = ChannelType.NotConnected;
        IChannel _channel;
        ITankBotLogger _logger;
        ITankBot _tankBot;

        Commands _currentCommand = Commands.Stop;

        public TankBotViewModel(ITankBotLogger logger)
        {
            _logger = logger;
        }

        public void SetChannel(IChannel channel)
        {
            _channel = channel;
            if (channel is TCPIPChannel)
            {
                _channelType = ChannelType.Remote;
                ((TCPIPChannel)_channel).NetworkMessageReceived += _channel_NetworkMessageReceived;
            }
            else if(channel is BluetoothChannel)
            {
                _channelType = ChannelType.Local;
                _tankBot = new mBlockSoccerBot(_channel, _logger);
            }
        }

        private void _channel_NetworkMessageReceived(object sender, NetworkMessage e)
        {
            if (e.MessageTypeCode == Core.Messages.SensorData.MessageTypeId)
            {
                SensorData = e.DeserializePayload<SensorData>();
            }
        }

        SensorData _sensorData;
        public SensorData SensorData
        {
            get { return _sensorData; }
            set { Set(ref _sensorData, value); }
        }

        public async void Move(short speed = 0, short? relativeHeading = 0, short? absoluteHeading = 0, short? duration = 0)
        {
            var moveMessage = Core.Messages.Move.Create(speed, relativeHeading, absoluteHeading, duration);
            await _channel.WriteBuffer(moveMessage.GetBuffer());
        }

        public void PlayTone(short frequency)
        {

        }

        public void Reset()
        {

        }

        public void SetLED(byte index, Color color)
        {

        }

        public async void Stop()
        {
            var moveMessage = Core.Messages.Stop.Create();
            await _channel.WriteBuffer(moveMessage.GetBuffer());
        }

        protected void RefreshSensors()
        {

        }

        private short _speed;
        public short Speed
        {
            get { return _speed; }
            set { Set(ref _speed, value); }
        }

        protected void SendCommand(object inputCmd)
        {
            var cmd = (Commands)inputCmd;
            _currentCommand = cmd;
            switch (cmd)
            {
                case Commands.Forward: Move(Speed, 0); break;
                case Commands.Stop: Speed = 0; Move(0, 0); break;
                case Commands.Left: Move(Speed, 270); break;
                case Commands.Right: Move(Speed, 90); break;
                case Commands.Backwards: Move(Speed, 180); break;
                case Commands.Reset: Reset(); break;
            }
        }

        public ChannelType ChannelType
        {
            get { return _channelType; }
            set { Set(ref _channelType, value); }

        }

        protected void SpeedUpdated(short speed)
        {
            Speed = speed;
            SendCommand(_currentCommand);
        }
    }
}
