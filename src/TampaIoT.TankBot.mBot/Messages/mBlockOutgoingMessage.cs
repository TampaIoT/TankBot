﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TampaIoT.TankBot.mBot.Messages
{
    public class mBlockOutgoingMessage : mBlockMessage
    {
        /* According to the mbot protocol from review of the source at: https://github.com/Makeblock-official/Makeblock-Firmware
         * This is the structure of the message:
         * 
         * ff 55 len idx action device port slot data a
         * 0  1  2   3   4      5      6    7    8
         * 
         * 
         */

        public static byte MessageIndexCounter { get; protected set; }


        byte[] _payload = null;

        private mBlockOutgoingMessage()
        {
            MessageSerialNumber = MessageIndexCounter++;
            /* The mbot recycles message serial numbers at 0x7f */
            if (MessageSerialNumber == 0x80)
            {
                MessageSerialNumber = 0x00;
                MessageIndexCounter = 0x00;
            }

            DateStamp = DateTime.Now;
        }

        const byte HEADER_LENGTH = 3;

        public override byte[] Buffer
        {
            get
            {
                byte length = 6;

                if (Slot.HasValue && !Port.HasValue)
                    throw new Exception("If you specify a slot you must also specify a port");

                if (Data.HasValue && (!Port.HasValue || !Slot.HasValue))
                    throw new Exception("If you specify data, you must also specify port and slot");

                if (Port.HasValue) length++;
                if (Slot.HasValue) length++;
                if (Data.HasValue) length++;
                if (Parameter.HasValue) length += 2;

                if (_payload != null && _payload.Length > 0)
                {
                    length += (byte)_payload.Length;
                }

                var position = 0;
                var buffer = new byte[length];
                buffer[position++] = 0xFF;
                buffer[position++] = 0x55;
                buffer[position++] = Convert.ToByte(length - HEADER_LENGTH);
                buffer[position++] = (byte)(MessageSerialNumber);
                buffer[position++] = Convert.ToByte(CommandType);
                buffer[position++] = Convert.ToByte(Device);
                if (Parameter.HasValue)
                {
                    buffer[position++] = (byte)(Parameter.Value & 0xFF);
                    buffer[position++] = (byte)(Parameter.Value >> 8);
                }
                else if (Port.HasValue) buffer[position++] = Convert.ToByte(Port.Value);
                if (Slot.HasValue) buffer[position++] = Slot.Value;
                if (Data.HasValue) buffer[position++] = Data.Value;

                if (_payload != null)
                {
                    for (var idx = 0; idx < _payload.Length; ++idx)
                    {
                        buffer[position++] = _payload[idx];
                    }
                }

                return buffer;
            }
        }

        public static mBlockOutgoingMessage CreateMessage(CommandTypes command, Devices device)
        {
            return new mBlockOutgoingMessage()
            {
                CommandType = command,
                Device = device
            };
        }

        public static mBlockOutgoingMessage CreateMessage(CommandTypes command, Devices device, Ports port)
        {
            return new mBlockOutgoingMessage()
            {
                CommandType = command,
                Device = device,
                Port = (int)port
            };
        }

        public static mBlockOutgoingMessage CreateMessage(CommandTypes command, Devices device, short value)
        {
            return new mBlockOutgoingMessage()
            {
                CommandType = command,
                Device = device,
                Parameter = value
            };
        }


        public static mBlockOutgoingMessage CreateMessage(CommandTypes command, Devices device, Ports port, byte[] payload)
        {
            return new mBlockOutgoingMessage()
            {
                CommandType = command,
                Device = device,
                Port = (int)port,
                _payload = payload
            };
        }

        public static mBlockOutgoingMessage CreateMessage(CommandTypes command, Devices device, int port, byte[] payload)
        {
            return new mBlockOutgoingMessage()
            {
                CommandType = command,
                Device = device,
                Port = port,
                _payload = payload
            };
        }

        public static mBlockOutgoingMessage CreateMessage(CommandTypes command, Devices device, Ports port, byte slot)
        {
            return new mBlockOutgoingMessage()
            {
                CommandType = command,
                Device = device,
                Port = (int)port,
                Slot = slot
            };
        }

        public static mBlockOutgoingMessage CreateMessage(CommandTypes command, Devices device, Ports port, byte slot, byte data)
        {
            return new mBlockOutgoingMessage()
            {
                CommandType = command,
                Device = device,
                Port = (int)port,
                Slot = slot,
                Data = data
            };
        }

        public Action<mBlockIncomingMessage> Handler { get; set; }
    }
}
