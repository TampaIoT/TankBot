using System.Collections.Generic;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.Firmware.Networking;
using TampaIoT.TankBot.Firmware.Sensors;

namespace TampaIoT.TankBot.Firmware.Managers
{
    public interface IConnectionManager
    {
        string GetDefaultPageHTML(string message);
        void Start(string tankBotName, ITankBot tankBot, ITankBotLogger logger, ISensorManager sensorManager, int webServerPort, int tcpListenerPort);

        ITankBot TankBot { get; }
        ISensorManager SensorManager { get; }
        IEnumerable<IClient> Clients { get; }

    }
}