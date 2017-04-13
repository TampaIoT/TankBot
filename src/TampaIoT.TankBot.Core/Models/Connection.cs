using System;
using System.Collections.Generic;
using System.Text;

namespace TampaIoT.TankBot.Core.Models
{
    public class Connection
    {
        public String Id { get; set; }
        public String DeviceName { get; set; }
        public String DeviceSerialNumber { get; set; }
        public String Owner { get; set; }
    }
}
