using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Progressive.Commons.Views.Controls
{
    public class AllSelectableTextBox : TextBox
    {
        public AllSelectableTextBox()
        {
            PreviewMouseLeftButtonDown += (sender, e) =>
            {
                var self = (TextBoxBase)sender;
                if (!self.IsKeyboardFocused)
                {
                    self.Focus();
                    e.Handled = true;
                }
            };
        }

        protected override void OnGotKeyboardFocus(System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            SelectAll();
            base.OnGotKeyboardFocus(e);
        }
    }
}
