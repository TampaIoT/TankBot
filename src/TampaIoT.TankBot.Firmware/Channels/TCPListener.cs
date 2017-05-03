using System;
using TampaIoT.TankBot.Core.Interfaces;

namespace TampaIoT.TankBot.Firmware.Channels
{
    public class TCPListener : IDisposable
    {
        Windows.Networking.Sockets.StreamSocketListener _listener;

        Server _server;
        ITankBotLogger _logger;
        int _port;

        public TCPListener(ITankBotLogger logger, Server server, int port)
        {
            _server = server;
            _logger = logger;
            _port = port;
            _logger.NotifyUserInfo("TCPIP Listener", $"Created Listener");
        }

        public async void StartListening()
        {
            try
            {
                _listener = new Windows.Networking.Sockets.StreamSocketListener();
                _listener.ConnectionReceived += _listener_ConnectionReceived;

                _logger.NotifyUserInfo("TCPIP Listener", $"Started Listening on Port {_port}");
                await _listener.BindServiceNameAsync(_port.ToString());
            }
            catch(Exception ex)
            {
                if (_listener != null)
                {
                    _listener.Dispose();
                    _listener = null;
                }
                _logger.NotifyUserError("TCPIP Listener", ex.Message);
            }
        }

        private void _listener_ConnectionReceived(Windows.Networking.Sockets.StreamSocketListener sender, Windows.Networking.Sockets.StreamSocketListenerConnectionReceivedEventArgs args)
        {
            _server.ClientConnected(args.Socket);
        }

        public void Close()
        {
            _listener.Dispose();
            _listener = null;
        }

        public void Dispose()
        {
            _listener.Dispose();
            _listener = null;
        }
    }
}
