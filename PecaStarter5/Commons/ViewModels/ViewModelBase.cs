using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Progressive.Commons.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo
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

        #region IDataErrorInfo メンバー

        public string Error
        {
            get
            {
                var results = new List<ValidationResult>();
                if (Validator.TryValidateObject(
                    this,
                    new ValidationContext(this, null, null),
                    results, true))
                {
                    return "";
                }
                return results.Count.ToString() + "個のエラーがあります";
            }
        }

        public string this[string columnName]
        {
            get
            {
                var results = new List<ValidationResult>();
                if (Validator.TryValidateProperty(
                    GetType().GetProperty(columnName).GetValue(this, null),
                    new ValidationContext(this, null, null) { MemberName = columnName },
                    results))
                {
                    return "";
                }
                // エラー時
                return string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage));
            }
        }

        #endregion
    }
}
