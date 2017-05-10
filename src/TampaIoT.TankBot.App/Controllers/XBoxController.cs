using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Gaming.Input;
using Windows.UI.Core;

namespace TampaIoT.TankBot.App.Controllers
{
    public class XBoxController : IJoyStick
    {
        private Gamepad _gamePad = null;
        private GamepadReading? _lastReading = null;

        public event EventHandler<Point> JoyStickUpdated;

        Point? _lastJoyStick;

        public async void StartListening(CoreDispatcher dispatcher)
        {
            if (Gamepad.Gamepads.Any())
            {
                _gamePad = Gamepad.Gamepads.First();
            }

            Gamepad.GamepadAdded += Gamepad_GamepadAdded;
            Gamepad.GamepadRemoved += Gamepad_GamepadRemoved;

            while (true)
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (_gamePad != null)
                    {

                        var reading = _gamePad.GetCurrentReading();

                        var thisJoyStick = new Point(reading.LeftThumbstickX, reading.LeftThumbstickY);

                        if (_lastJoyStick.HasValue && (_lastJoyStick.Value.X != thisJoyStick.X || _lastJoyStick.Value.Y != thisJoyStick.Y))
                        {
                            if ((Math.Abs(thisJoyStick.X) < 0.05) && (Math.Abs(thisJoyStick.Y) < 0.05))
                            {
                                JoyStickUpdated?.Invoke(_gamePad, new Point(0,0));
                            }
                            else
                            {
                                JoyStickUpdated?.Invoke(_gamePad, thisJoyStick);
                            }
                        }

                        _lastJoyStick = thisJoyStick;

                        _lastReading = reading;
                    }
                });

                await Task.Delay(TimeSpan.FromMilliseconds(250));
            }
        }

        private void Gamepad_GamepadRemoved(object sender, Gamepad e)
        {
            _gamePad = null;
        }

        private void Gamepad_GamepadAdded(object sender, Gamepad e)
        {
            _gamePad = e;
        }
    }
}
