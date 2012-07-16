using System.ComponentModel;
using System.Diagnostics.Contracts;

namespace Progressive.Commons.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged メンバー

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void OnPropertyChanged(string propertyName)
        {
            Contract.Requires(GetType().GetProperty(propertyName) != null);

            if (PropertyChanged == null)
                return;

            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(string propertyName, ref T obj, T value)
        {
            if (obj != null && obj.Equals(value))
                return false;
            obj = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
