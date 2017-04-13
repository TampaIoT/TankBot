using LagoVista.Core.Models.Drawing;
using System;
using TampaIoT.TankBot.Core.Messages;
using TampaIoT.TankBot.Core.Models;

namespace TampaIoT.TankBot.Core.Interfaces
{
    public interface ITankBot
    {
        String Id { get; }
        String Name { get; }
        String DeviceName { get; }
        String FirmwareVersion { get; }
        String DeviceTypeId { get; }
        String APIMode { get; }

        void PlayTone(short frequence);

        void SetLED(byte index, Color color);

        void Move(short speed = 0, short? relativeHeading = 0, short? absoluteHeading = 0, short? duration = 0);

        void Stop();

        void Reset();

        SensorData SensorData { get; set; }

        DateTime? LastBotContact { get; set; }
    }
}
