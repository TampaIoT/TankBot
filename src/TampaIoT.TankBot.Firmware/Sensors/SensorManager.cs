using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using System.ComponentModel;

namespace TampaIoT.TankBot.Firmware.Sensors
{
    public class SensorManager 
    {
        Compass5983 _compass;
        IRProximitySensorArray _irSensorArray;
        Sonar _frontSonar;

        public event PropertyChangedEventHandler PropertyChanged;

        public SensorManager()
        {
            _frontSonar = new Sonar();
            _compass = new Compass5983();
        }

        public async Task InitAsync()
        {
            var gpio = GpioController.GetDefault();
            _irSensorArray = new IRProximitySensorArray(gpio);
            await _compass.InitAsync();            
        }

        public void Start()
        {
            _irSensorArray.Start();
            _compass.Start();
        }

        public void Dispose()
        {
            _compass.Dispose();
        }

        public Compass5983 Compass { get { return _compass; } }
        public IRProximitySensorArray SensorArray { get { return _irSensorArray; } }

        public Sonar FrontSonar { get { return _frontSonar; } }
    }
}
