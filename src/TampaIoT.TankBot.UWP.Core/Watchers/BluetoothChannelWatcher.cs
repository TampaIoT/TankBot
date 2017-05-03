using LagoVista.Core.PlatformSupport;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TampaIoT.TankBot.Core.Channels;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.UWP.Core.Channels;
using Windows.Devices.Enumeration;
using Windows.Foundation;

namespace TampaIoT.TankBot.UWP.Core.Watchers
{
    public class BluetoothChannelWatcher : ChannelWatcherBase
    {
        ITankBotLogger _logger;
        DeviceWatcher _deviceWatcher = null;

        private ObservableCollection<IChannel> _channels = new ObservableCollection<IChannel>();

        public BluetoothChannelWatcher(ITankBotLogger logger) : base(logger)
        {
            _logger = logger;
        }

        public  override void Stop()
        {
            if (null != _deviceWatcher && (DeviceWatcherStatus.Started == _deviceWatcher.Status ||
                    DeviceWatcherStatus.EnumerationCompleted == _deviceWatcher.Status))
            {
                _deviceWatcher.Stop();

            }
        }

        public override void Start()
        {
            // Request additional properties
            string[] requestedProperties = new string[] { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            _deviceWatcher = DeviceInformation.CreateWatcher("(System.Devices.Aep.ProtocolId:=\"{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}\")",
                                                            requestedProperties,
                                                            DeviceInformationKind.AssociationEndpoint);

            _logger.NotifyUserInfo("BT Mgr", "Watcher Started");

            // Hook up handlers for the watcher events before starting the watcher
            _deviceWatcher.Added += new TypedEventHandler<DeviceWatcher, DeviceInformation>((watcher, deviceInfo) =>
            {
                // Since we have the collection databound to a UI element, we need to update the collection on the UI thread.
                Services.DispatcherServices.Invoke(() => {
                    // Make sure device name isn't blank
                    if (deviceInfo.Name != "")
                    {
                        _logger.NotifyUserInfo("BT Mgr", $"Found Channel => " + deviceInfo.Name);
                        RaiseDeviceFoundEvent(new BluetoothChannel(deviceInfo, _logger));
                    }
                });
            });

            _deviceWatcher.Updated += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>((watcher, deviceInfoUpdate) =>
            {
                Services.DispatcherServices.Invoke(() => {
                    _logger.NotifyUserInfo("BT Mgr", $"Device Updated => {deviceInfoUpdate.Id}.");

                    var updatedChannel = _channels.Where(itm => itm.Id == deviceInfoUpdate.Id).FirstOrDefault();
                    if (updatedChannel != null)
                    {
                        //TODO: Maybe introduce again.
                        //updatedChannel.Update(deviceInfoUpdate);
                    }
                });
            });

            _deviceWatcher.EnumerationCompleted += new TypedEventHandler<DeviceWatcher, Object>((watcher, obj) =>
            {
                _logger.NotifyUserInfo("BT Mgr", $"{_channels.Count} devices found. Enumeration completed. Watching for updates...");
            });

            _deviceWatcher.Removed += new TypedEventHandler<DeviceWatcher, DeviceInformationUpdate>((watcher, deviceInfoUpdate) =>
            {
                // Since we have the collection databound to a UI element, we need to update the collection on the UI thread.
                Services.DispatcherServices.Invoke(() => {
                    _logger.NotifyUserInfo("BT Mgr", $"Device Removed => {deviceInfoUpdate.Id}.");
                    var removedDevice = _channels.Where(itm => itm.Id == deviceInfoUpdate.Id).FirstOrDefault();
                    if (removedDevice != null)
                    {
                        _channels.Remove(removedDevice);
                    }
                });
            });

            _deviceWatcher.Stopped += new TypedEventHandler<DeviceWatcher, Object>((watcher, obj) =>
            {
                Services.DispatcherServices.Invoke(() => {
                    var status = (watcher.Status == DeviceWatcherStatus.Aborted ? "aborted" : "stopped");
                    _logger.NotifyUserInfo("BT Mgr", $"BT Manager State Change: {status}.");
                    _channels.Clear();
                });
            });

            _deviceWatcher.Start();
        }
    }
}
