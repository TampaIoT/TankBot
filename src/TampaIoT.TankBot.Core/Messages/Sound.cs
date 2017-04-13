using System;
using System.Collections.Generic;
using System.Text;

namespace TampaIoT.TankBot.Core.Messages
{
    public class Sound
    {
        public const int MessageTypeId = 60;

        public short Frequency { get; set; }
    }
}
