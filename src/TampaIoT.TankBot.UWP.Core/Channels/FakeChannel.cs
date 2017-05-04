using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using TampaIoT.TankBot.Core.Channels;

namespace TampaIoT.TankBot.UWP.Core.Channels
{
    public class FakeChannel : ChannelBase
    {
        public override void Connect()
        {
          
        }

        public override Task<bool> ConnectAsync()
        {
            return Task.FromResult(true);
        }

        public override void Disconnect()
        {

        }

        public override Task DisconnectAsync()
        {
            return Task.FromResult(default(object));
        }

        public override Task WriteBuffer(byte[] buffer)
        {
            return Task.FromResult(default(object));
        }
    }
}
