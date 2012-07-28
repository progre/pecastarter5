using System.Windows;

namespace Progressive.Commons.Views.Utils
{
    static class WpfUtils
    {
        public static Window GetRoot(FrameworkElement frameWorkElement)
        {
            var parent = frameWorkElement.Parent;
            if (parent is Window)
            {
                return parent as Window;
            }
            if (parent is FrameworkElement)
            {
                return GetRoot(parent as FrameworkElement);
            }
            return null;
        }
    }
}
