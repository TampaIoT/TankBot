namespace TampaIoT.TankBot.Firmware.Networking
{
    public interface ITCPListener
    {
        void Close();
        void Dispose();
        void StartListening();
    }
}