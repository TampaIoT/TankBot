using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.App.Controllers;
using System.Threading.Tasks;

namespace TampaIoT.TankBot.App.ViewModels
{
    public class LocalClientTankBotViewModel : TankBotClientBase, IClientTankBotViewModel
    {
        private ITankBot _mblockTankBot;

        ITankBotLogger _logger;
        IJoyStick _joyStick;
        IChannel _channel;
      
        public LocalClientTankBotViewModel(IChannel channel, ITankBotLogger logger, IJoyStick joyStick) : base(joyStick)
        {
            _channel = channel;
             _mblockTankBot = new mBlockTankBot(channel, logger);
            _logger = logger;
        }
        
        public ChannelTypes ChannelType { get { return ChannelTypes.Remote; } }

        public override void Move(short direction)
        {
            _mblockTankBot.Move(Speed, direction);
        }

        public override void Stop()
        {
            Speed = 0;
            _mblockTankBot.Stop();
        }

        public Task DisconnectAsync()
        {
            return _channel.DisconnectAsync();
        }
    }
}
