using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.Networking.Services;
using TampaIoT.TankBot.Core.Channels;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.UWP.Core.Channels;

namespace TampaIoT.TankBot.UWP.Core.Watchers
{
    public class UPNPChannelWatcher : ChannelWatcherBase, IChannelWatcher
    {
        ISSDPClient _ssdpClient;
        ITankBotLogger _logger;

        public UPNPChannelWatcher(ITankBotLogger logger) : base(logger)
        {
            _logger = logger;
        }

        public override void Start()
        {
            _logger.NotifyUserInfo("TCPIP Mgr", $"Started Watcher");

            _ssdpClient = NetworkServices.GetSSDPClient();
            _ssdpClient.ShowDiagnostics = true;
            _ssdpClient.NewDeviceFound += _ssdpClient_NewDeviceFound;
            _ssdpClient.SsdpQueryAsync(port: 1900);
        }

        private void _ssdpClient_NewDeviceFound(object sender, LagoVista.Core.Networking.Models.uPnPDevice device)
        {
            if (device.ModelName == "SoccerBot-mBot")
            {
                _logger.NotifyUserInfo("TCPIP Mgr", "Found Channel =>: " + device.FriendlyName);
                RaiseDeviceFoundEvent(new TCPIPChannel(device, _logger));
            }
        }

        public override void Stop()
        {
            _logger.NotifyUserInfo("TCPIP Mgr", $"Stopped Watcher");
            _ssdpClient.Cancel();
        }
    }
}
