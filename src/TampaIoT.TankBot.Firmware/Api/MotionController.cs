using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.Networking.Models;
using TampaIoT.TankBot.Core.Interfaces;
using TampaIoT.TankBot.Firmware.Managers;

namespace TampaIoT.TankBot.Firmware.Api
{
    /// <summary>
    /// Will return full HTML pages in response to URLs to control motion.
    /// </summary>
    public class MotionController : IApiHandler
    {
        ITankBotLogger _logger;
        IConnectionManager _connectionManager;

        public MotionController(IConnectionManager connectionManager, ITankBotLogger logger)
        {
            _connectionManager = connectionManager;
            _logger = logger;
        }

        [MethodHandler(MethodHandlerAttribute.MethodTypes.GET, FullPath = "/reset")]
        public HttpResponseMessage Reset(HttpRequestMessage msg)
        {
            _connectionManager.TankBot.Reset();

            var response = msg.GetResponseMessage();
            response.ContentType = "text/html";
            response.Content = _connectionManager.GetDefaultPageHTML("Ok - Resetting");
            return response;
        }

        [MethodHandler(MethodHandlerAttribute.MethodTypes.GET, FullPath = "/motion/forward/{speed}")]
        public HttpResponseMessage Forward(HttpRequestMessage msg, int speed)
        {
            _connectionManager.TankBot.Move((short)speed, absoluteHeading: 0);

            var response = msg.GetResponseMessage();
            response.ContentType = "text/html";
            response.Content = _connectionManager.GetDefaultPageHTML("Ok - starting forward");
            return response;
        }

        [MethodHandler(MethodHandlerAttribute.MethodTypes.GET, FullPath = "/motion/left/{speed}")]
        public HttpResponseMessage Left(HttpRequestMessage msg, int speed)
        {
            _connectionManager.TankBot.Move((short)speed, absoluteHeading: 270);

            var response = msg.GetResponseMessage();
            response.ContentType = "text/html";
            response.Content = _connectionManager.GetDefaultPageHTML("Ok - starting left");
            return response;
        }

        [MethodHandler(MethodHandlerAttribute.MethodTypes.GET, FullPath = "/motion/right/{speed}")]
        public HttpResponseMessage Right(HttpRequestMessage msg, int speed)
        {
            _connectionManager.TankBot.Move((short)speed, absoluteHeading: 90);

            var response = msg.GetResponseMessage();
            response.ContentType = "text/html";
            response.Content = _connectionManager.GetDefaultPageHTML("Ok - starting right");
            return response;
        }

        [MethodHandler(MethodHandlerAttribute.MethodTypes.GET, FullPath = "/motion/backwards/{speed}")]
        public HttpResponseMessage Backwards(HttpRequestMessage msg, int speed)
        {
            _connectionManager.TankBot.Move((short)speed, absoluteHeading: 180);

            var response = msg.GetResponseMessage();
            response.ContentType = "text/html";
            response.Content = _connectionManager.GetDefaultPageHTML("Ok - starting backwards");
            return response;
        }

        [MethodHandler(MethodHandlerAttribute.MethodTypes.GET, FullPath = "/motion/stop")]
        public HttpResponseMessage Stop(HttpRequestMessage msg)
        {
            _connectionManager.TankBot.Stop();

            var response = msg.GetResponseMessage();
            response.ContentType = "text/html";
            response.Content = _connectionManager.GetDefaultPageHTML("Ok - stopping");
            return response;
        }
    }
}
