using System;
using System.Collections.Generic;
using System.Text;

namespace TampaIoT.TankBot.mBot.Messages
{
    public class mBlockIncomingMessage : mBlockMessage
    {
        public enum PayloadType
        {
            Byte = 1,
            Float = 2,
            Short = 3,
            String = 4,
            Double = 5
        }

        public Single FloatPayload
        {
            get { return BitConverter.ToSingle(Buffer, 4); }
        }

        public String StringPayload
        {
            get
            {
                return System.Text.UTF8Encoding.UTF8.GetString(Buffer, 5, Buffer[4]);
            }
        }
    }
}