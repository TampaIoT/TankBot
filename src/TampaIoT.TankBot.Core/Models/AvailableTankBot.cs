using System;
using System.Collections.Generic;
using System.Text;
using TampaIoT.TankBot.Core.Interfaces;

namespace TampaIoT.TankBot.Core.Models
{
    /// <summary>
    /// A FoundTankBot
    /// </summary>
    public class AvailableTankBot
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IChannel Channel {get; set; }

    }
}
