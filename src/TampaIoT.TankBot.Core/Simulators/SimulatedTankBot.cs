using LagoVista.Core.Models.Drawing;
using System;
using System.Collections.Generic;
using System.Text;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.Core.Messages;

namespace TampaIoT.TankBot.Core.Simulators
{
    public class SimulatedSoccerBot : TankBotBase, ITankBot
    {
        public ISensor FrontSonar
        {
            get; set;
        }

        public ISensor Compass
        {
            get; set;
        }
        public SensorData SensorData { get; set; }

        public void PlayTone(short frequency)
        {

        }

        protected override void RefreshSensors()
        {

        }

        protected override void SendCommand(object cmd)
        {

        }

        protected override void SpeedUpdated(short speed)
        {

        }

        public void Reset()
        {

        }

        public void SetLED(byte index, Color color)
        {

        }

        public void Move(short speed = 0, short? relativeHeading = 0, short? absoluteHeading = 0, short? duration = 0)
        {

        }

        public void Stop()
        {

        }
    }
}
