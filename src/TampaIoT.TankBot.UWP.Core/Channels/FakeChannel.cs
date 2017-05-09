using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TampaIoT.TankBot.Core.Channels;

namespace TampaIoT.TankBot.UWP.Core.Channels
{
    public class FakeChannel : ChannelBase
    {
        Timer _timer;

        public override void Connect()
        {
            _timer = new Timer(SendUpdate, null, 0, 1000);
        }

        private void SendUpdate(object state)
        {
            this.
        }

        public override Task<bool> ConnectAsync()
        {
            return Task.FromResult(true);
        }

        public override void Disconnect()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _timer.Dispose();
            _timer = null;
        }

        public override Task DisconnectAsync()
        {
            Disconnect();
            return Task.FromResult(default(object));
        }

        public override Task WriteBuffer(byte[] buffer)
        {
            return Task.FromResult(default(object));
        }
    }
}
