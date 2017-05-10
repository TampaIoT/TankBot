using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using System.ComponentModel;
using TampaIoT.TankBot.Core.Messages;
using System.Runtime.CompilerServices;
using LagoVista.Core.PlatformSupport;

namespace TampaIoT.TankBot.Firmware.Sensors
{
    public class SensorManager : ISensorManager, INotifyPropertyChanged
    {
        Compass5983 _compass;
        IRProximitySensorArray _irSensorArray;
        Sonar _frontSonar;


        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            //TODO: Should be a design time check and not run this.
            Services.DispatcherServices.Invoke(() =>
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName))
            );
        }

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

        public void UpdateSensorData(SensorData sensorData)
        {
            sensorData.FrontSonar = FrontSonar;
            sensorData.Compass = Compass;
            sensorData.CompassRawX = Compass.RawX;
            sensorData.CompassRawY = Compass.RawY;

            sensorData.RightIR = SensorArray.Right;

            sensorData.FrontRightIR = SensorArray.FrontRight;
            sensorData.FrontIR = SensorArray.Front;
            sensorData.FrontLeftIR = SensorArray.FrontLeft;

            sensorData.LeftIR = SensorArray.Left;

            sensorData.RearRightIR = SensorArray.RearRight;
            sensorData.RearIR = SensorArray.Rear;
            sensorData.RearLeftIR = SensorArray.RearLeft;
        }

        public Compass5983 Compass { get { return _compass; } }
        public IRProximitySensorArray SensorArray { get { return _irSensorArray; } }

        public Sonar FrontSonar { get { return _frontSonar; } }

        private SensorData _sensorData;
        public SensorData SensorData
        {
            get { return _sensorData; }
            set
            {
                _sensorData = value;
                RaisePropertyChanged();
            }
        }
    }
}
