using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.Networking.Models;
using LagoVista.Core.Networking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.Firmware.Api;
using TampaIoT.TankBot.Firmware.Channels;
using TampaIoT.TankBot.Firmware.Sensors;

namespace TampaIoT.TankBot.Firmware.Managers
{
    public class ConnectionManager
    {
        ISSDPServer _ssdpServer;
        IWebServer _webServer;
        Server _tcpServer;

        ITankBot _soccerBot;
        ITankBotLogger _logger;

        private string _deviceName;

        static ConnectionManager _instance = new ConnectionManager();

        public string GetDefaultPageHTML(string message)
        {
            var html = @"<head>
<title>SoccerBot</title>
<link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css"" integrity=""sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u"" crossorigin=""anonymous"">
</head>
<body>
<h1>" + _deviceName + @" Soccer Bot Api Page</h1>
<h2>Tampa IoT Society</h2> 
<h2>Status: " + message + @"</h2> 
<h3>API Mode: " + _soccerBot.APIMode + @"</h3>
<h3>Firmware Version: " + _soccerBot.FirmwareVersion + @" </h3>
<img src='https://raw.githubusercontent.com/bytemaster-0xff/WinIoTSoccerBot/master/Documentation/BasicVersion.jpg' />
<div class='row'>
<div class='col-md-1'><a class='btn btn-success' href='/reset' >Reset</a></div>
<div class='col-md-1'><a class='btn btn-success' href='/motion/forward/150' >Forward</a></div>
<div class='col-md-1'><a class='btn btn-success' href='/motion/backwards/150' >Back</a></div>
<div class='col-md-1'><a class='btn btn-success' href='/motion/left/150' >Left</a></div>
<div class='col-md-1'><a class='btn btn-success' href='/motion/right/150' >Right</a></div>
<div class='col-md-1'><a class='btn btn-success' href='/motion/stop/150' >Stop</a></div>
<div class='col-md-5'></div>
</div>
</body>
</html>";

            return html;
        }


        private ConnectionManager()
        {
            Windows.Networking.Connectivity.NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;
        }

        private void NetworkInformation_NetworkStatusChanged(object sender)
        {
            var temp = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            var previousInternetConnectionStatus = App.TheApp.HasInternetConnection;
            App.TheApp.HasInternetConnection = temp.GetNetworkConnectivityLevel() == Windows.Networking.Connectivity.NetworkConnectivityLevel.InternetAccess;

            if (App.TheApp.HasInternetConnection)
            {
                MakeDiscoverable(_tankBotName);
                StartWebServer(_webServerPort, _tankBotName);
                StartTCPServer(_tcpListenerPort, _tankBot, _sensorManager);
            }
            else
            {
                _tcpServer.Stop();
                //TODO: Need to test dropping and reconnecting with TCP Server and Web Server.
            }
        }

        private string _tankBotName;
        private ITankBot _tankBot;
        private SensorManager _sensorManager;
        private int _webServerPort;
        private int _tcpListenerPort;

        public void Start(String tankBotName, ITankBot tankBot, ITankBotLogger logger, SensorManager sensorManager, int webServerPort, int tcpListenerPort)
        {
            _logger = logger;
            _tankBotName = tankBotName;
            _tankBot = tankBot;
            _sensorManager = sensorManager;
            _webServerPort = webServerPort;
            _tcpListenerPort = tcpListenerPort;

            var networkInfo = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            App.TheApp.HasInternetConnection = networkInfo.GetNetworkConnectivityLevel() == Windows.Networking.Connectivity.NetworkConnectivityLevel.InternetAccess;

            if (App.TheApp.HasInternetConnection)
            {
                MakeDiscoverable(tankBotName);
                StartWebServer(_webServerPort, _tankBotName);
                StartTCPServer(_tcpListenerPort, _tankBot, _sensorManager);
            }
        }

        public static ConnectionManager Instance { get { return _instance; } }

        public void MakeDiscoverable(string name)
        {
            var _configuration = new UPNPConfiguration()
            {
                UdpListnerPort = 1900,
                FriendlyName = name,
                Manufacture = "Tampa IoT Dev",
                ModelName = "SoccerBot-mBot",
                DefaultPageHtml = @"<html>
<head>
<title>SoccerBot</title>
<link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css"" integrity=""sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u"" crossorigin=""anonymous"">
</head>
<body>
<h1>Soccer Bot SSDP (uPnP) Listener Page</h1>
</body>
</html>"
            };

            _ssdpServer = NetworkServices.GetSSDPServer();
            _ssdpServer.MakeDiscoverable(9500, _configuration);
        }

        public void StartWebServer(int port, string name)
        {
            _deviceName = name;

            _webServer = NetworkServices.GetWebServer();
            _webServer.RegisterAPIHandler(new MotionRestAPI(_soccerBot, _logger));
            _webServer.RegisterAPIHandler(new MotionController(_soccerBot, _logger));
            _webServer.DefaultPageHtml = GetDefaultPageHTML("Ready");
            _webServer.StartServer(port);
        }

        public void StartTCPServer(int port, ITankBot soccerBot, SensorManager sensorManager)
        {
            _tcpServer = new Channels.Server(_logger, port, soccerBot, sensorManager);
            _tcpServer.Start();
        }

        public Channels.Server TCPIPServer { get { return _tcpServer; } }
    }
}
