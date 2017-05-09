using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace TampaIoT.TankBot.App.Controllers
{
    public interface IJoyStick
    {
        event EventHandler<Point> JoyStickUpdated;
    }
}
