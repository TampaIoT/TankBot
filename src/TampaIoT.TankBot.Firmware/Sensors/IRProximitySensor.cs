using System;
using System.ComponentModel;
using TampaIoT.TankBot.Core.Interfaces;
using Windows.Devices.Gpio;

namespace TampaIoT.TankBot.Firmware.Sensors
{
    public class IRProximitySensor : SensorBase, ISensor
    {
        GpioPin _input;

        public IRProximitySensor(GpioController gpio, int pin)
        {
            _input = gpio.OpenPin(pin);
            _input.SetDriveMode(GpioPinDriveMode.Input);
            Value = "?";
        }


        public void Read()
        {
            if (_input != null)
            {

                Value = (_input.Read() == GpioPinValue.High) ? "Off" : "On";

                LastUpdated = DateTime.Now;
                IsOnline = true;
            }
            else
            {
                Value = "Offline";
                IsOnline = false;
            }
        }

        public void Dispose()
        {
            _input.Dispose();
            _input = null;
            IsOnline = false;
            
        }

    }
}
