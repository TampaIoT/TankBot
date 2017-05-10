using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.System.Diagnostics.DevicePortal;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace TampaIoT.TankBot.Firmware
{
    public sealed class PortalProvider : IBackgroundTask
    {
        BackgroundTaskDeferral _taskDeferral;
        DevicePortalConnection _devicePortalConnection;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Take a deferral to allow the background task to continue executing 
            this._taskDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += TaskInstance_Canceled;

            // Create a DevicePortal client from an AppServiceConnection 
            var details = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            var appServiceConnection = details.AppServiceConnection;
            _devicePortalConnection = DevicePortalConnection.GetForAppServiceConnection(appServiceConnection);

            // Add Closed, RequestReceived handlers 
            _devicePortalConnection.Closed += _devicePortalConnection_Closed;
            _devicePortalConnection.RequestReceived += DevicePortalConnection_RequestReceived;
        }

        private void _devicePortalConnection_Closed(DevicePortalConnection sender, DevicePortalConnectionClosedEventArgs args)
        {
            
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            
        }

        private void DevicePortalConnection_RequestReceived(DevicePortalConnection sender, DevicePortalConnectionRequestReceivedEventArgs args)
        {
            var req = args.RequestMessage;
            var res = args.ResponseMessage;

            if (req.RequestUri.AbsolutePath.EndsWith("/echo"))
            {
                // construct an html response message
                string con = "<h1>" + req.RequestUri.AbsoluteUri + "</h1><br/>";
                var proc = Windows.System.Diagnostics.ProcessDiagnosticInfo.GetForCurrentProcess();
                con += String.Format("This process is consuming {0} bytes (Working Set)<br/>", proc.MemoryUsage.GetReport().WorkingSetSizeInBytes);
                con += String.Format("The process PID is {0}<br/>", proc.ProcessId);
                con += String.Format("The executable filename is {0}", proc.ExecutableFileName);
                res.Content = new HttpStringContent(con);
                res.Content.Headers.ContentType = new HttpMediaTypeHeaderValue("text/html");
                res.StatusCode = HttpStatusCode.Ok;
            }
            //...
        }
    }
}
