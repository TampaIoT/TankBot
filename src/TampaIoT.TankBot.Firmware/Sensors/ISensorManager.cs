using System.Threading.Tasks;
using TampaIoT.TankBot.Core.Messages;

namespace TampaIoT.TankBot.Firmware.Sensors
{
    public interface ISensorManager
    {
        Compass5983 Compass { get; }
        Sonar FrontSonar { get; }
        IRProximitySensorArray SensorArray { get; }

        void Dispose();
        Task InitAsync();
        void Start();
        void UpdateSensorData(SensorData sensorData);

        SensorData SensorData {get;}
    }
}