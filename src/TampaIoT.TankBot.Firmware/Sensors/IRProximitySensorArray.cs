using System;
using System.Threading.Tasks;
using Windows.Devices.Gpio;

namespace TampaIoT.TankBot.Firmware.Sensors
{
    public class IRProximitySensorArray
    {
        public const int BANK_ONE_OUTPUT = 12;
        public const int BANK_TWO_OUTPUT = 26;

        public const int FRONT_LEFT = 17;
        public const int FRONT = 18;
        public const int FRONT_RIGHT = 23;

        public const int LEFT = 27;
        public const int RIGHT = 24;

        public const int REAR_LEFT = 20;
        public const int REAR = 21;
        public const int REAR_RIGHT = 16;

        GpioPin _bank1;
        GpioPin _bank2;

        IRProximitySensor _front;
        IRProximitySensor _frontRight;
        IRProximitySensor _frontLeft;

        IRProximitySensor _leftSide;
        IRProximitySensor _rightSide;

        IRProximitySensor _rear;
        IRProximitySensor _rearLeft;
        IRProximitySensor _rearRight;

        public IRProximitySensorArray(GpioController gpioController)
        {
            _frontLeft = new IRProximitySensor(gpioController, FRONT_LEFT);
            _front = new IRProximitySensor(gpioController, FRONT);
            _frontRight = new IRProximitySensor(gpioController, FRONT_RIGHT);

            _leftSide = new IRProximitySensor(gpioController, LEFT);
            _rightSide = new IRProximitySensor(gpioController, RIGHT);


            _rearLeft = new IRProximitySensor(gpioController, REAR_LEFT);
            _rear = new IRProximitySensor(gpioController, REAR);
            _rearRight = new IRProximitySensor(gpioController, REAR_RIGHT);

            _bank1 = gpioController.OpenPin(BANK_ONE_OUTPUT);
            _bank1.SetDriveMode(GpioPinDriveMode.Output);
            _bank2 = gpioController.OpenPin(BANK_TWO_OUTPUT);
            _bank2.SetDriveMode(GpioPinDriveMode.Output);
        }

        public void Start()
        {
            var rnd = new Random();
            Task.Run(async () =>
            {
                while (true)
                {
                    if (!App.TheApp.HasInternetConnection)
                    {
                        for (var idx = 0; idx < 5; ++idx)
                        {
                            _bank1.Write(GpioPinValue.High);
                            _bank2.Write(GpioPinValue.High);
                            await Task.Delay(50);
                            _bank1.Write(GpioPinValue.Low);
                            _bank2.Write(GpioPinValue.Low);
                            await Task.Delay(50);
                        }


                        await Task.Delay(1500);

                    }
                    else if (!App.TheApp.HasMBotConnection)
                    {
                        for (var idx = 0; idx < 2; ++idx)
                        {
                            _bank1.Write(GpioPinValue.High);
                            _bank2.Write(GpioPinValue.High);
                            await Task.Delay(50);
                            _bank1.Write(GpioPinValue.Low);
                            _bank2.Write(GpioPinValue.Low);
                            await Task.Delay(50);
                        }

                        _bank1.Write(GpioPinValue.Low);
                        _bank2.Write(GpioPinValue.Low);

                        await Task.Delay(1500);
                    }
                    else 
                    {
                        _bank1.Write(GpioPinValue.High);
                        _bank2.Write(GpioPinValue.High);
                        await Task.Delay(10);

                        _front.Read();
                        _frontLeft.Read();
                        _frontRight.Read();

                        _leftSide.Read();
                        _rightSide.Read();

                        _rear.Read();
                        _rearLeft.Read();
                        _rearRight.Read();


                        _bank1.Write(GpioPinValue.Low);
                        _bank2.Write(GpioPinValue.Low);
                        await Task.Delay(rnd.Next(100,200));
                    }
                }
            });
        }

        public IRProximitySensor FrontLeft { get { return _frontLeft; } }
        public IRProximitySensor Front { get { return _front; } }
        public IRProximitySensor FrontRight { get { return _frontRight; } }

        public IRProximitySensor Right { get { return _rightSide; } }
        public IRProximitySensor Left { get { return _leftSide; } }

        public IRProximitySensor RearLeft { get { return _rearLeft; } }
        public IRProximitySensor Rear { get { return _rear; } }
        public IRProximitySensor RearRight { get { return _rearRight; } }

    }
}
