using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using TampaIoT.TankBot.Core.Messages;
using Windows.Networking.Sockets;

namespace TampaIoT.TankBot.Firmware.Networking
{
    public interface IServer : INotifyPropertyChanged
    {
        void ClientConnected(StreamSocket socket);
        void Start();
        void Stop();

        SensorData SensorData { get; }

        ConcurrentDictionary<String, IClient> Clients { get; }
    }
}