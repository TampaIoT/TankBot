using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TampaIoT.TankBot.Core.Channels;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.UWP.Core.Channels;

namespace TampaIoT.TankBot.UWP.Core.Watchers
{
    public class FakeWatcher : ChannelWatcherBase, IChannelWatcher
    {
        Timer _timer;
        bool _active;
        int _fakeDeviceIndex = 1;

        public FakeWatcher(ITankBotLogger logger) : base(logger)
        {
            _timer = new Timer(Tick, null, 0, 2500);
        }

        public void Tick(object state)
        {
            if(_active)
            {
                RaiseDeviceFoundEvent(new FakeChannel()
                {
                    Id = _fakeDeviceIndex.ToString(),
                    DeviceName = $"Fake Device {_fakeDeviceIndex++}"
                });
            }
        }

        public override void Start()
        {
            _active = true;
        }

        public override void Stop()
        {
            _active = false;
        }
    }
}
