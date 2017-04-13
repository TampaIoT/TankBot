using LagoVista.Core.PlatformSupport;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TampaIoT.TankBot.Core.Channels;
using TampaIoT.TankBot.Core.Interfaces;

namespace TampaIoT.TankBot.UWP.Core.Channels
{
    class SerialPortChannel : ChannelBase
    {
        ISerialPort _port;
        ITankBotLogger _logger;
        StreamReader _reader;
        StreamWriter _writer;
        const int MAX_BUFFER_SIZE = 255;

        public SerialPortChannel(ISerialPort port, ITankBotLogger logger)
        {
            _port = port;
            _logger = logger;
        }

        public async override void Connect()
        {
            await ConnectAsync();
        }

        public async override Task<bool> ConnectAsync()
        {
            try
            {
                await _port.OpenAsync();
                State = States.Connecting;
                _reader = new StreamReader(_port.InputStream);
                _writer = new StreamWriter(_port.OutputStream);

                StartListening(_reader);
                State = States.Connected;
                return true;
            }
            catch (Exception ex)
            {
                _logger.NotifyUserError("SerialPortChannel_ConnectAsync", ex.Message);
                if (_reader != null)
                {
                    _reader.Dispose();
                    _reader = null;
                }

                if (_writer != null)
                {
                    _writer.Dispose();
                    _writer = null;
                }

                return false;
            }
        }

        private void StartListening(StreamReader reader)
        {
            Task.Run(() =>
            {
                var spinWait = new SpinWait();
                try
                {
                    var buffer = new byte[255];
                    int idx = 0;
                    DateTime? lastByteReceived = null;

                    while (_reader != null)
                    {
                        while (_reader != null && _reader.Peek() == -1)
                        {
                            spinWait.SpinOnce();

                            if (lastByteReceived.HasValue && (DateTime.Now - lastByteReceived).Value.Milliseconds > 10)
                            {
                                var msgBuffer = new byte[idx];
                                for (var ptr = 0; ptr < idx; ++ptr)
                                    msgBuffer[ptr] = buffer[ptr];

                                RaiseMessageReceived(msgBuffer);
                                idx = 0;
                                lastByteReceived = null;
                            }
                        }

                        if (_reader != null)
                        {
                            lastByteReceived = DateTime.Now;
                            buffer[idx++] = (byte)_reader.Read();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.NotifyUserError("SerialPortChannel_ReceiveDataAsync", "Error Receiving Data" + ex.Message);
                    Disconnect();
                }
            });
        }

        public async override void Disconnect()
        {
            await DisconnectAsync();
        }

        public async override Task DisconnectAsync()
        {
            if (State == States.Connected)
            {
                await _port.CloseAsync();

                if (_reader != null)
                {
                    _reader.Dispose();
                    _reader = null;
                }

                if (_writer != null)
                {
                    _writer.Dispose();
                    _writer = null;
                }
                State = States.Disconnected;
                InvokeDisconnected();
            }
        }

        public async override Task WriteBuffer(byte[] buffer)
        {
            try
            {
                await _writer.WriteAsync(buffer.ToCharArray(0, buffer.Length));
            }
            catch (Exception ex)
            {
                _logger.NotifyUserError("SerialPortChannel_ReceiveDataAsync", "Error Sending Buffer: " + ex.Message);
                Disconnect();
            }
        }
    }
}