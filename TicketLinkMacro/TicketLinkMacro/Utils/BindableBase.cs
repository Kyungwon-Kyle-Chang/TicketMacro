using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TicketLinkMacro.Utils
{
    [Serializable]
    public class BindableBase : INotifyPropertyChanged
    {
        protected void SetProperty<T>(ref T target, T value, [CallerMemberName] string propertyname = null)
        {
            if (Equals(target, value)) return;

            target = value;
            OnPropertyChanged(propertyname);
        }

        [field:NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

