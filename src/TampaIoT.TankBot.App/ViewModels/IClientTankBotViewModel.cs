using LagoVista.Core.Commanding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using TampaIoT.TankBot.Core.Messages;

namespace TampaIoT.TankBot.App.ViewModels
{

    public interface IClientTankBotViewModel
    {
        short Speed { get; set; }
        Task DisconnectAsync();
        SensorData SensorData { get; }
        RelayCommand MoveCommand { get;  }
        RelayCommand StopCommand { get; }
        bool IsConnected { get; }
    }
}
