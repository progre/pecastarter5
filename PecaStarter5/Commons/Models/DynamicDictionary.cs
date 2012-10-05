using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Progressive.PecaStarter5.Commons.Models
{
    // The class derived from DynamicObject.
    internal class DynamicStringDictionary : DynamicObject, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged メンバー

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public DynamicStringDictionary()
        {
            Dictionary = new Dictionary<string, string>();
        }

        // The inner dictionary.
        public Dictionary<string, string> Dictionary { get; set; }

        public string this[string key]
        {
            get { return Dictionary[key]; }
            set
            {
                Dictionary[key] = value;
                OnPropertyChanged(key);
            }
        }

        // If you try to get a value of a property 
        // not defined in the class, this method is called.
        public override bool TryGetMember(
            GetMemberBinder binder, out object result)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            string name = binder.Name.ToLower();

            // If the property name is found in a dictionary,
            // set the result parameter to the property value and return true.
            // Otherwise, return false.
            string tResult;
            bool isSucceed = Dictionary.TryGetValue(name, out tResult);
            result = tResult;
            return isSucceed;
        }

        // If you try to set a value of a property that is
        // not defined in the class, this method is called.
        public override bool TrySetMember(
            SetMemberBinder binder, object value)
        {
            var key = binder.Name.ToLower();
            var stringValue = value.ToString();
            if (Dictionary[key] == stringValue)
                return true;
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            Dictionary[key] = value.ToString();

            OnPropertyChanged(key);

            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
