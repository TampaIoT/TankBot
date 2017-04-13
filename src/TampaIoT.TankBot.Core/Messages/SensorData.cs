using Newtonsoft.Json;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.Core.Models;

namespace TampaIoT.TankBot.Core.Messages
{
    public class SensorData
    {
        /* Using JsonConstructor allows us to deserialize to proerties defined as interfaces */
        [JsonConstructor]
        public SensorData(Sensor compass, Sensor compassRawX, Sensor compassRawY,
                  Sensor frontSonar,
                  Sensor frontLeftIR, Sensor frontIR, Sensor frontRightIR,
                  Sensor leftIR, Sensor rightIR,
                  Sensor rearLeftIR, Sensor rearIR, Sensor rearRightIR)
        {
            Compass = compass;
            CompassRawX = compassRawX;
            CompassRawY = compassRawY;

            FrontSonar = frontSonar;

            FrontLeftIR = frontLeftIR;
            FrontIR = frontIR;
            FrontRightIR = frontRightIR;

            LeftIR = leftIR;
            RightIR = rightIR;

            RearLeftIR = rearLeftIR;
            RearIR = rearIR;
            RearRightIR = rearRightIR;
        }

        public SensorData()
        {

        }

        public const int MessageTypeId = 130;

        public string DeviceName { get; set; }
        public string Version { get; set; }

        public ISensor Compass { get; set; }
        public ISensor CompassRawX { get; set; }
        public ISensor CompassRawY { get; set; }

        public ISensor FrontSonar { get; set; }

        public ISensor LeftIR { get; set; }

        public ISensor FrontRightIR { get; set; }
        public ISensor FrontIR { get; set; }
        public ISensor FrontLeftIR { get; set; }

        public ISensor RightIR { get; set; }


        public ISensor RearRightIR { get; set; }
        public ISensor RearIR { get; set; }
        public ISensor RearLeftIR { get; set; }
    }
}
