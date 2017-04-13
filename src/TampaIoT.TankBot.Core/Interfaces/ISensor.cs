using System;
using System.Collections.Generic;
using System.Text;

namespace TampaIoT.TankBot.Core.Interfaces
{
    public interface ISensor
    {
        string Value { get; set; }
        DateTime? LastUpdated { get; }
        bool IsOnline { get; }
    }
}
