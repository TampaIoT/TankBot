using System;
using LagoVista.Core.Commanding;
using TampaIoT.TankBot.Core.Messages;
using LagoVista.Core.ViewModels;
using TampaIoT.TankBot.App.Controllers;
using TampaIoT.TankBot.Core.Interfaces;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TampaIoT.TankBot.App.ViewModels
{

    public class RemoteClientTankBotViewModel : TankBotClientBase, IClientTankBotViewModel
    {
        INetworkChannel _networkChannel;

        public RemoteClientTankBotViewModel(IChannel channel, IJoyStick joyStick) : base(joyStick)
        {
            _networkChannel = channel as INetworkChannel;
            _networkChannel.Disconnected += _networkChannel_Disconnected;
            if (_networkChannel == null)
                throw new Exception("Must provide something that implements INetworkChannel to RemoteClientTankBotViewModel");

            _networkChannel.NetworkMessageReceived += _networkChannel_NetworkMessageReceived;

        }

        private void _networkChannel_Disconnected(object sender, string e)
        {
            IsConnected = false;
        }

        private void _networkChannel_NetworkMessageReceived(object sender, Core.Models.NetworkMessage e)
        {
            try
            {
                switch (e.MessageTypeCode)
                {
                    case SensorData.MessageTypeId:
                        SensorData = e.DeserializePayload<SensorData>();
                        break;
                }
            }
            catch(Exception ex)
            {
                Logger.LogException("RemoteClient_NetworkMessageReceived", ex);
            }
        }
 
        public override void Move(short direction)
        {
            _networkChannel.SendAsync(Core.Messages.Move.Create(Speed, direction));
        }

        public override void Stop()
        {
            _networkChannel.SendAsync(Core.Messages.Stop.Create());
        }

        public Task DisconnectAsync()
        {
            return _networkChannel.DisconnectAsync();
        }
    }
}