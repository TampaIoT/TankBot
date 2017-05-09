using System;
using System.Collections.Generic;
using System.Text;
using TampaIoT.TankBot.Core.Models;

namespace TampaIoT.TankBot.Core.Interfaces
{
    public interface INetworkChannel : IChannel
    {
        event EventHandler<NetworkMessage> NetworkMessageReceived;
    }
}
