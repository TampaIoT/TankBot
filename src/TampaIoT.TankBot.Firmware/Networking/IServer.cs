using System;
using System.Collections.Concurrent;
using Windows.Networking.Sockets;

namespace TampaIoT.TankBot.Firmware.Networking
{
    public interface IServer
    {
        void ClientConnected(StreamSocket socket);
        void Start();
        void Stop();

        ConcurrentDictionary<String, IClient> Clients { get; }
    }
}