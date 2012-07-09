using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Progressive.PecaStarter5.Model
{
    public class Settings : INotifyPropertyChanged
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        private bool hasNotifyIcon = true;
        public bool HasNotifyIcon
        {
            get { return hasNotifyIcon; }
            set
            {
                if (hasNotifyIcon == value)
                    return;
                hasNotifyIcon = value;
            }
        }

        #region INotifyPropertyChanged メンバー

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;
            if (GetType().GetProperty(propertyName) == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
