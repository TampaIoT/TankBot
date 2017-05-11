using LagoVista.Core.PlatformSupport;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TampaIoT.TankBot.Firmware.Sensors
{
    public class SensorBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            //TODO: Should be a design time check and not run this.
            Services.DispatcherServices.Invoke(() =>
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName))
            );
        }

        private string _value;
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                LastUpdated = DateTime.Now;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(LastUpdated));
            }
        }

        public DateTime? LastUpdated { get; protected set; }

        private bool _isOnline = false;
        public bool IsOnline
        {
            get { return _isOnline; }
            set
            {
                if (_isOnline != value)
                {
                    _isOnline = value;
                    RaisePropertyChanged();
                }
            }
        }

    }
}
