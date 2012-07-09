using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Progressive.Commons.Views.Utils;

namespace Progressive.PecaStarter.View.Control
{
    class Alert : ContentControl
    {
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property.Name != "Content")
            {
                return;
            }
            if (e.NewValue as string == "")
            {
                return;
            }
            Xceed.Wpf.Toolkit.MessageBox.Show(WpfUtils.GetRoot(this), e.NewValue as string, "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
