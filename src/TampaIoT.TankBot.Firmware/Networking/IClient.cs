using System;
using System.Threading.Tasks;
using TampaIoT.TankBot.Core.Models;

namespace TampaIoT.TankBot.Firmware.Networking
{
    public interface IClient
    {
        string Id { get; }
        bool IsConnected { get; }

        event EventHandler<NetworkMessage> MessageRecevied;

        void Disconnect();
        void Dispose();
        void StartListening();
        Task Write(byte[] buffer);
    }
}