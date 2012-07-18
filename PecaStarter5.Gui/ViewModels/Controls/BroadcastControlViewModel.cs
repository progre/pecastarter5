using System;
using System.Windows.Input;
using Progressive.Commons.ViewModels;
using Progressive.Commons.ViewModels.Commands;
using Progressive.PecaStarter5.Models.Services;
using Progressive.PecaStarter5.Models;

namespace Progressive.PecaStarter5.ViewModels.Controls
{
    public class BroadcastControlViewModel : ViewModelBase
    {
        public BroadcastControlViewModel(MainPanelViewModel parent, PeercastService service, Configuration configuration)
        {
            Configuration = configuration;

            BroadcastCommand = new DelegateCommand(() =>
            {
                var yp = parent.YellowPagesListViewModel.SelectedYellowPages;
                service.BroadcastAsync(yp.Model, yp.AcceptedHash, parent.BroadcastParameter,
                    new Progress<string>(x => { }))
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            // ダイアログ通知
                            parent.Alert = t.Exception.InnerException.Message + t.Exception.StackTrace;
                            System.Windows.MessageBox.Show(t.Exception.InnerException.Message + t.Exception.StackTrace, "仮");
                            return;
                        }
                        Id = t.Result;
                    });
            });

            UpdateCommand = new DelegateCommand(() =>
            {
                service.UpdateAsync(parent.UpdateParameter,
                    new Progress<string>(x => { }))
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            // ダイアログ通知
                            parent.Alert = t.Exception.InnerException.Message + t.Exception.StackTrace;
                            System.Windows.MessageBox.Show(t.Exception.InnerException.Message + t.Exception.StackTrace, "仮");
                            return;
                        }
                    });
            });

            StopCommand = new DelegateCommand(() =>
            {
                service.StopAsync(Id, new Progress<string>(x => { }))
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            // ダイアログ通知
                            parent.Alert = t.Exception.InnerException.Message + t.Exception.StackTrace;
                            System.Windows.MessageBox.Show(t.Exception.InnerException.Message + t.Exception.StackTrace, "仮");
                            return;
                        }
                    });
            });
        }

        public string Id { get; private set; }
        public ICommand BroadcastCommand { get; private set; }
        public ICommand UpdateCommand { get; private set; }
        public ICommand StopCommand { get; private set; }
        public Configuration Configuration { get; private set; }
    }
}
