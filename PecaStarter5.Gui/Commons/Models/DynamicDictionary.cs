using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace Progressive.PecaStarter5.Commons.Models
{
    // The class derived from DynamicObject.
    public class DynamicDictionary<T> : DynamicObject
    {
        public DynamicDictionary()
        {
            Dictionary = new Dictionary<string, T>();
        }

        // The inner dictionary.
        public Dictionary<string, T> Dictionary { get; private set; }

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
            T tResult;
            bool isSucceed = Dictionary.TryGetValue(name, out tResult);
            result = tResult;
            return isSucceed;
        }

        // If you try to set a value of a property that is
        // not defined in the class, this method is called.
        public override bool TrySetMember(
            SetMemberBinder binder, object value)
        {
            // Converting the property name to lowercase
            // so that property names become case-insensitive.
            Dictionary[binder.Name.ToLower()] = (T)value;

            // You can always add a value to a dictionary,
            // so this method always returns true.
            return true;
        }
    }
}
