using System;
using System.ComponentModel;
using System.Windows;
using Progressive.Commons.Views;
using Microsoft.CSharp.RuntimeBinder;

namespace Progressive.PecaStarter5.Views
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : GlassWindow
    {
        private TaskTrayIconManager taskTrayIconManager;

        public MainWindow()
        {
            Loaded += OnLoaded;
            InitializeComponent();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                var viewModel = (dynamic)DataContext;
                if (viewModel.IsBusy)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("処理が実行中なので終了できません。", "",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    e.Cancel = true;
                    return;
                }
                if (viewModel.IsBroadcasting)
                {
                    var result = Xceed.Wpf.Toolkit.MessageBox.Show("配信中です。強制的に終了しますか？", "",
                        MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.None);
                    if (result != MessageBoxResult.Yes)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }
            catch (RuntimeBinderException)
            {
                // バインド失敗時は無視
            }
            finally
            {
                base.OnClosing(e);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (taskTrayIconManager != null)
            {
                taskTrayIconManager.Dispose();
                taskTrayIconManager = null;
            }
            var disposeObj = DataContext as IDisposable;
            if (disposeObj != null)
                disposeObj.Dispose();
            base.OnClosed(e);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            taskTrayIconManager = new TaskTrayIconManager(
                (Window)sender, (INotifyPropertyChanged)DataContext, "HasNotifyIcon");
        }
    }
}
