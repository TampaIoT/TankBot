using LagoVista.Core.PlatformSupport;
using System;
using System.Linq;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Networking.Connectivity;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using TampaIoT.TankBot.Firmware.Sensors;
using TampaIoT.TankBot;
using TampaIoT.TankBot.Core.Interfaces;
using Windows.System.Profile;
using TampaIoT.TankBot.UWP.Core.Channels;
using TampaIoT.TankBot.Core.Simulators;
using Windows.Networking;
using TampaIoT.TankBot.Firmware.Networking;
using TampaIoT.TankBot.Firmware.Managers;

namespace TampaIoT.TankBot.Firmware
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        private static App _app;

        ITankBotLogger _logger;
        ISensorManager _sensorManager;
        ITankBot _tankBot;
        IConnectionManager _connectionManager;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;

            _app = this;
        }

        public static App TheApp { get { return _app; } }


        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                InitSockerBot();

                Window.Current.Activate();
            }
        }

        private async void InitSockerBot()
        {
            var hostNames = NetworkInformation.GetHostNames();
            var computerName = hostNames.FirstOrDefault(name => name.Type == HostNameType.DomainName)?.DisplayName ?? "???";

            _connectionManager = new ConnectionManager();

            var pin = await LagoVista.Core.PlatformSupport.Services.Storage.GetKVPAsync<string>("PIN");
            if (String.IsNullOrEmpty(pin))
            {
                var rnd = new Random();
                pin = rnd.Next(1000, 9999).ToString();
                await LagoVista.Core.PlatformSupport.Services.Storage.StoreKVP("PIN", pin);
            }

            Debug.Write("========================================");
            Debug.Write("NOTE: NOTE: NOTE: Your PIN is: " + pin);
            Debug.Write("========================================");

            _logger = new Loggers.DebugLogger();

            switch (AnalyticsInfo.VersionInfo.DeviceFamily)
            {
                case "Windows.IoT":
                    var ports = (await LagoVista.Core.PlatformSupport.Services.DeviceManager.GetSerialPortsAsync());
                    if (ports.Count == 0)
                    {
                        throw new Exception("Could not find any serial ports, a serial port is required.");
                    }
                    else if (ports.Count > 1)
                    {
                        throw new Exception("Found more than one serial port, please add additional logic to select the serial port the mBot is connected to.");
                    }

                    var serialPortChannel = new SerialPortChannel(ports.First().Id, _logger);
                    await serialPortChannel.ConnectAsync();
                    _tankBot = new mBlockTankBot(serialPortChannel, _logger, pin);
                    _sensorManager = new SensorManager();
                    await _sensorManager.InitAsync();
                    _sensorManager.Start();

                    break;
                case "Windows.Desktop":
                    _tankBot = new SimulatedSoccerBot();

                    break;
            }

            ConnectionManager.Start(computerName, _tankBot, _logger, _sensorManager, 80, 9001);
        }

        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            deferral.Complete();
        }

        public bool HasInternetConnection { get; set; }

        public bool HasMBotConnection { get; set; }

        public bool ConnectedToCloud { get; set; }

        public ISensorManager SensorManager { get { return _sensorManager; } }
        public ITankBot TankBot { get { return _tankBot; } }
        public IConnectionManager ConnectionManager { get { return _connectionManager; } }
    }
}
