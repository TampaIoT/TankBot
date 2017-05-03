using LagoVista.Core.Networking.Interfaces;
using LagoVista.Core.Networking.Models;
using TampaIoT.TankBot.Core.Interfaces;

namespace TampaIoT.TankBot.Firmware.Api
{
    public class MotionRestAPI : IApiHandler
    {
        ITankBot _tankBot;
        ITankBotLogger _logger;

        public MotionRestAPI(ITankBot tankBot, ITankBotLogger logger)
        {
            _tankBot = tankBot;
            _logger = logger;
        }

        [MethodHandler(MethodHandlerAttribute.MethodTypes.GET, FullPath = "/api/reset")]
        public HttpResponseMessage Reset(HttpRequestMessage msg)
        {
            _tankBot.Reset();

            var response = msg.GetResponseMessage();
            response.ContentType = "application/json";
            response.Content = "{'status':'ok'}";
            return response;
        }

        [MethodHandler(MethodHandlerAttribute.MethodTypes.GET, FullPath = "/api/motion/{direction}/{speed}")]
        public HttpResponseMessage Move(HttpRequestMessage msg, int direction, int speed)
        {
            _tankBot.Move((short)speed, absoluteHeading: (short)direction);

            var response = msg.GetResponseMessage();
            response.ContentType = "application/json";
            response.Content = "{'status':'ok'}";
            return response;
        }

        [MethodHandler(MethodHandlerAttribute.MethodTypes.GET, FullPath = "/api/motion/forward/{speed}")]
        public HttpResponseMessage Forward(HttpRequestMessage msg, int speed)
        {
            _tankBot.Move((short)speed, absoluteHeading: 0);

            var response = msg.GetResponseMessage();
            response.ContentType = "application/json";
            response.Content = "{'status':'ok'}";
            return response;
        }

        [MethodHandler(MethodHandlerAttribute.MethodTypes.GET, FullPath = "/api/motion/left/{speed}")]
        public HttpResponseMessage Left(HttpRequestMessage msg, int speed)
        {
            _tankBot.Move((short)speed, absoluteHeading: 270);

            var response = msg.GetResponseMessage();
            response.ContentType = "application/json";
            response.Content = "{'status':'ok'}";
            return response;
        }

        [MethodHandler(MethodHandlerAttribute.MethodTypes.GET, FullPath = "/api/motion/right/{speed}")]
        public HttpResponseMessage Right(HttpRequestMessage msg, int speed)
        {
            _tankBot.Move((short)speed, absoluteHeading: 90);

            var response = msg.GetResponseMessage();
            response.ContentType = "application/json";
            response.Content = "{'status':'ok'}";
            return response;
        }

        [MethodHandler(MethodHandlerAttribute.MethodTypes.GET, FullPath = "/api/motion/backwards/{speed}")]
        public HttpResponseMessage Backwards(HttpRequestMessage msg, int speed)
        {
            _tankBot.Move((short)speed, absoluteHeading: 180);

            var response = msg.GetResponseMessage();
            response.ContentType = "application/json";
            response.Content = "{'status':'ok'}";
            return response;
        }

        [MethodHandler(MethodHandlerAttribute.MethodTypes.GET, FullPath = "/api/motion/stop")]
        public HttpResponseMessage Stop(HttpRequestMessage msg)
        {
            _tankBot.Stop();

            var response = msg.GetResponseMessage();
            response.ContentType = "application/json";
            response.Content = "{'status':'ok'}";
            return response;
        }
    }
}
