using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TampaIoT.TankBot.Core.Interfaces
{
    public enum States
    {
        Connecting,
        Connected,
        Disconnected,
    }

    public enum ChannelTypes
    {
        Simulated,
        Local,
        Remote
    }


    public interface IChannel
    {
        String Id { get; set; }

        String DeviceName { get; set; }

        ChannelTypes ChannelType { get; }

        DateTime? LastMessageReceived { get; }

        event EventHandler<IChannel> Connected;
        event EventHandler<string> Disconnected;
        event EventHandler<byte[]> MessageReceived;
       
        Task WriteBuffer(byte[] buffer);

        States State { get; set; }

        Task<bool> ConnectAsync();
        Task DisconnectAsync();

        bool IsRemote { get; }
        bool IsLocal { get; }
    }
}
