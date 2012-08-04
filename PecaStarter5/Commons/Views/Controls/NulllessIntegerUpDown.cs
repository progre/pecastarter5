using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xceed.Wpf.Toolkit;

namespace Progressive.Commons.Views.Controls
{
    class NulllessIntegerUpDown : IntegerUpDown
    {
        protected override void OnValueChanged(int? oldValue, int? newValue)
        {
            if (newValue == null)
            {
                Value = oldValue;
                return;
            }

            base.OnValueChanged(oldValue, newValue);
        }
    }
}
