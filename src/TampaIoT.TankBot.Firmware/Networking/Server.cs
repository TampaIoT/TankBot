using System;
using System.Linq;
using System.Collections.Generic;
using Windows.Networking.Sockets;
using LagoVista.Core.Models.Drawing;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.Firmware.Sensors;
using TampaIoT.TankBot.Core.Models;
using TampaIoT.TankBot.Core.Messages;
using System.Collections.Concurrent;
using System.Threading;

namespace TampaIoT.TankBot.Firmware.Networking
{
    public class Server : IServer
    {
        ITankBotLogger _logger;
        ITankBot _soccerBot;
        ISensorManager _sensorManager;
        ConcurrentDictionary<String, IClient> _clients;
        ITCPListener _listener;

        System.Threading.Timer _watchDog;
        System.Threading.Timer _sensorUpdateTimer;
        int _port;

        Object _clientAccessLocker = new object();


        public Server(ITankBotLogger logger, int port, ITankBot soccerBot, ISensorManager sensorManager)
        {
            _port = port;
            _logger = logger;
            _soccerBot = soccerBot;

            _sensorManager = sensorManager;

            _clients = new ConcurrentDictionary<string, IClient>();

            _watchDog = new System.Threading.Timer(_watchDog_Tick, null, 0, 2500);
            _sensorUpdateTimer = new System.Threading.Timer(_sensorUpdateTimer_Tick, null, 0, 1000);
        }

        public void Start()
        {
            _listener = new TCPListener(_logger, this, _port);
            _listener.StartListening();
        }

        public void Stop()
        {
            if (_listener != null)
            {
                _listener.Close();
                _listener = null;
            }

            foreach (var client in _clients)
            {
                client.Value.Disconnect();
                client.Value.Dispose();
            }

            _clients.Clear();
        }

        private async void _sensorUpdateTimer_Tick(object sender)
        {
            var sensorMessage = new SensorData()
            {
                Version = _soccerBot.FirmwareVersion,
                DeviceName = _soccerBot.DeviceName,
            };

            _sensorManager.UpdateSensorData(sensorMessage);

            var msg = NetworkMessage.CreateJSONMessage(sensorMessage, Core.Messages.SensorData.MessageTypeId);
            var connectedClients = _clients.Where(clnt => clnt.Value.IsConnected == true).ToList();

            foreach (var client in connectedClients)
            {
                await client.Value.Write(msg.GetBuffer());
            }
        }


        private void _watchDog_Tick(object state)
        {
            var clientsToRemove = new List<IClient>();
            foreach (var client in _clients)
            {
                if (client.Value.IsConnected == false)
                    clientsToRemove.Add(client.Value);
            }

            /* Run if a client has dropped */
            if (clientsToRemove.Count > 0 && _clients.Count > 0)
            {
                /* If we only have one client left, turn LEDs red */
                if (_clients.Count == 1)
                {
                    _soccerBot.SetLED(0, NamedColors.Red);
                }

                /* Play tone if client dropped */
                _soccerBot.PlayTone(200);
            }

            foreach (var client in clientsToRemove)
            {
                if (_clients.TryRemove(client.Id, out IClient removedClient))
                {
                    client.Disconnect();
                    client.Dispose();
                }
                /* else - if this fails it will just be picke up next time */
            }
        }

        public void ClientConnected(StreamSocket socket)
        {
            _soccerBot.PlayTone(400);
            _soccerBot.SetLED(0, NamedColors.Green);

            lock (_clients)
            {
                var client = Client.Create(socket, _logger);
                client.MessageRecevied += Client_MessageRecevied;
                int attempts = 0;
                while(!_clients.TryAdd(client.Id,client) && attempts++ < 5)
                {
                    SpinWait.SpinUntil(() => false, 5);
                }

                if(attempts == 5)
                {
                    _logger.NotifyUserError("Server_ClientConnected", "Could not add client after 5 attempts.");
                }

                client.StartListening();
            }
        }

        private void Client_MessageRecevied(object sender, NetworkMessage e)
        {
            switch (e.MessageTypeCode)
            {
                case Move.MessageTypeId:
                    {
                        var movePayload = e.DeserializePayload<Move>();
                        _soccerBot.Move(movePayload.Speed, movePayload.RelativeHeading, movePayload.AbsoluteHeading, movePayload.Duration);
                    }
                    break;
                case Core.Messages.Stop.MessageTypeId:
                    _soccerBot.Stop();
                    break;
            }
        }

        public ConcurrentDictionary<String, IClient> Clients { get { return _clients; } }

    }
}
