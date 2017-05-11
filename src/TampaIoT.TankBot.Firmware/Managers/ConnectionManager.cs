using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.Networking.Models;
using LagoVista.Core.Networking.Services;
using LagoVista.Core.PlatformSupport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TampaIoT.TankBot.Core;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.Firmware.Api;
using TampaIoT.TankBot.Firmware.Networking;
using TampaIoT.TankBot.Firmware.Sensors;

namespace TampaIoT.TankBot.Firmware.Managers
{
    public class ConnectionManager : INotifyPropertyChanged, IConnectionManager
    {
        ISSDPServer _ssdpServer;
        IWebServer _webServer;
        ISensorManager _sensorManager;
        ITankBotLogger _logger;
        ITankBot _tankBot;

        string _tankBotName;
        int _webServerPort;
        int _tcpListenerPort;
        int _uPnPListenerPort;
        string _deviceName;

        static ConnectionManager _instance = new ConnectionManager();


        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            //TODO: Should be a design time check and not run this.
            Services.DispatcherServices.Invoke(() =>
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName))
            );
        }


        public string GetDefaultPageHTML(string message)
        {
            var html = @"<head>
<title>SoccerBot</title>
<link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css"" integrity=""sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u"" crossorigin=""anonymous"">
</head>
<body>
<h1>" + _deviceName + @" TankBot Api Page</h1>
<h2>Tampa IoT Society</h2> 
<h2>Status: " + message + @"</h2> 
<h3>API Mode: " + _tankBot.APIMode + @"</h3>
<h3>Firmware Version: " + _tankBot.FirmwareVersion + @" </h3>
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


        public ConnectionManager()
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
                if (Server != null)
                {
                    Server.Stop();
                    Server = null;
                }
                //TODO: Need to test dropping and reconnecting with TCP Server and Web Server.
            }
        }


        public void Start(String tankBotName, ITankBot tankBot, ITankBotLogger logger, ISensorManager sensorManager, int webServerPort, int tcpListenerPort, int uPnPListenerPort)
        {
            _logger = logger;
            _tankBotName = tankBotName;
            _tankBot = tankBot;
            _sensorManager = sensorManager;
            _webServerPort = webServerPort;
            _tcpListenerPort = tcpListenerPort;
            _uPnPListenerPort = uPnPListenerPort;

            var networkInfo = Windows.Networking.Connectivity.NetworkInformation.GetInternetConnectionProfile();
            App.TheApp.HasInternetConnection = networkInfo.GetNetworkConnectivityLevel() == Windows.Networking.Connectivity.NetworkConnectivityLevel.InternetAccess;

            if (App.TheApp.HasInternetConnection)
            {
                MakeDiscoverable(tankBotName);
                StartWebServer(_webServerPort, _tankBotName);
                StartTCPServer(_tcpListenerPort, _tankBot, _sensorManager);
            }
        }
        
        private void MakeDiscoverable(string name)
        {
            if (_ssdpServer == null)
            {
                var _configuration = new UPNPConfiguration()
                {
                    UdpListnerPort = _uPnPListenerPort,
                    FriendlyName = name,
                    Manufacture = "Tampa IoT Dev",
                    ModelName = Constants.TankBotModelName,
                    DefaultPageHtml = @"<html>
<head>
<title>SoccerBot</title>
<link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css"" integrity=""sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u"" crossorigin=""anonymous"">
</head>
<body>
<h1>Tampa IoT TankBot SSDP (uPnP) Listener Page</h1>
</body>
</html>"
                };

                try
                {
                    _ssdpServer = NetworkServices.GetSSDPServer();
                    _ssdpServer.MakeDiscoverable(9500, _configuration);
                }
                catch(Exception ex)
                {
                    _logger.NotifyUserError("ConnectionManager_MakeDiscoverable", ex.Message);
                }
            }
        }

        private void StartWebServer(int port, string name)
        {
            _deviceName = name;

            try
            {
                _webServer = NetworkServices.GetWebServer();
                _webServer.RegisterAPIHandler(new MotionRestAPI(_tankBot, _logger));
                _webServer.RegisterAPIHandler(new MotionController(this, _logger));
                _webServer.DefaultPageHtml = GetDefaultPageHTML("Ready");
                _webServer.StartServer(port);
            }
            catch(Exception ex)
            {
                _logger.NotifyUserError("ConnectionManager_StartWebServer", ex.Message);
            }
        }

        private void StartTCPServer(int port, ITankBot soccerBot, ISensorManager sensorManager)
        {
            try
            {
                Server = new Server(_logger, port, soccerBot, sensorManager);
                Server.Start();
            }
            catch(Exception ex)
            {
                _logger.NotifyUserError("ConnectionManager_StartTCPServer", ex.Message);
            }
        }

        IServer _server;
        public IServer Server
        {
            get { return _server; }
            set
            {
                _server = value;
                RaisePropertyChanged();
            }
        }

        public IEnumerable<IClient> Clients { get { return Server != null ? Server.Clients.Values : new List<IClient>() ; } }

        public ITankBot TankBot { get { return _tankBot; } }
        public ISensorManager SensorManager { get { return _sensorManager; } }

    }
}
