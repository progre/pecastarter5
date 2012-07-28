using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Progressive.PecaStarter.View.Control
{
    /// <summary>
    /// TextBoxWithHistory.xaml の相互作用ロジック
    /// </summary>
    public partial class TextBoxWithHistory : UserControl
    {
        public TextBoxWithHistory()
        {
            InitializeComponent();
        }

        private void ComboBoxItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ComboBoxItem;
            switch (e.ChangedButton)
            {
                case MouseButton.Middle:
                    var command = (DataContext as dynamic).Command as ICommand;
                    if (command.CanExecute(DataContext))
                    {
                        command.Execute(Tuple.Create(DataContext, item.Content));
                    }
                    break;
            }
        }
    }
}
