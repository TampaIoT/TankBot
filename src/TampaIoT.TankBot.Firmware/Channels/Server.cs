using System;
using System.Linq;
using System.Collections.Generic;
using Windows.Networking.Sockets;
using System.Diagnostics;
using LagoVista.Core.Models.Drawing;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.Firmware.Sensors;
using TampaIoT.TankBot.Core.Models;
using TampaIoT.TankBot.Core.Messages;

namespace TampaIoT.TankBot.Firmware.Channels
{
    public class Server
    {
        ITankBotLogger _logger;
        ITankBot _soccerBot;
        SensorManager _sensorManager;
        List<Client> _clients;
        TCPListener _listener;

        System.Threading.Timer _watchDog;
        System.Threading.Timer _sensorUpdateTimer;
        int _port;

        Object _clientAccessLocker = new object();


        public Server(ITankBotLogger logger, int port, ITankBot soccerBot, SensorManager sensorManager)
        {
            _port = port;
            _logger = logger;
            _soccerBot = soccerBot;

            _sensorManager = sensorManager;

            _clients = new List<Client>();

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

            foreach(var client in _clients)
            {
                client.Disconnect();
                client.Dispose();
            }

            _clients.Clear();
        }

        private async void _sensorUpdateTimer_Tick(object sender)
        {
            var sensorMessage = new SensorData()
            {
                Version = _soccerBot.FirmwareVersion,
                DeviceName = _soccerBot.DeviceName,
                FrontSonar = _sensorManager.FrontSonar,
                Compass = _sensorManager.Compass,
                CompassRawX = _sensorManager.Compass.RawX,
                CompassRawY = _sensorManager.Compass.RawY,

                RightIR = _sensorManager.SensorArray.Right,

                FrontRightIR = _sensorManager.SensorArray.FrontRight,
                FrontIR = _sensorManager.SensorArray.Front,
                FrontLeftIR = _sensorManager.SensorArray.FrontLeft,

                LeftIR = _sensorManager.SensorArray.Left,

                RearRightIR = _sensorManager.SensorArray.RearRight,
                RearIR = _sensorManager.SensorArray.Rear,
                RearLeftIR = _sensorManager.SensorArray.RearLeft
            };

            var msg = NetworkMessage.CreateJSONMessage(sensorMessage, Core.Messages.SensorData.MessageTypeId);
            var connectedClients = _clients.Where(clnt => clnt.IsConnected == true).ToList();

            foreach (var client in connectedClients)
            {
                await client.Write(msg.GetBuffer());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        private void _watchDog_Tick(object state)
        {
            var clientsToRemove = new List<Client>();
            foreach (var client in _clients)
            {
                if (client.IsConnected == false)
                    clientsToRemove.Add(client);
            }

            if (clientsToRemove.Count > 0 && _clients.Count > 0)
            {
                if (_clients.Count == 1)
                {
                    if (_soccerBot.LastBotContact.HasValue && ((DateTime.Now - _soccerBot.LastBotContact) < TimeSpan.FromSeconds(10)))
                    {
                        _soccerBot.SetLED(0, NamedColors.Yellow);
                    }
                    else
                        _soccerBot.SetLED(0, NamedColors.Red);
                }
                _soccerBot.PlayTone(200);
            }

            foreach (var client in clientsToRemove)
            {
                try
                {
                    _clients.Remove(client);
                    client.Disconnect();
                    client.Dispose();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
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
                _clients.Add(client);
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
    }
}
