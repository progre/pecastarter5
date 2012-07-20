using System;
using System.Windows.Input;
using Progressive.Commons.ViewModels;
using Progressive.Commons.ViewModels.Commands;
using Progressive.PecaStarter5.Models.Services;
using Progressive.PecaStarter5.Models;
using System.Threading.Tasks;

namespace Progressive.PecaStarter5.ViewModels.Controls
{
    public class BroadcastControlViewModel : ViewModelBase
    {
        private MainPanelViewModel parent;
        public BroadcastControlViewModel(MainPanelViewModel parent, PeercastService service, Configuration configuration)
        {
            this.parent = parent;
            Configuration = configuration;

            BroadcastCommand = new DelegateCommand(() =>
            {
                var yp = parent.YellowPagesListViewModel.SelectedYellowPages;
                service.BroadcastAsync(yp.Model, yp.AcceptedHash, parent.BroadcastParameter,
                    new Progress<string>(x => parent.Feedback = x))
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            // ダイアログ通知
                            parent.Feedback = "中止";
                            parent.Alert = t.Exception.InnerException.Message + t.Exception.StackTrace;
                            return;
                        }
                        Id = t.Result;
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }, () => !IsBroadcasting);

            UpdateCommand = new DelegateCommand(() =>
            {
                service.UpdateAsync(parent.UpdateParameter,
                    new Progress<string>(x => parent.Feedback = x))
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            // ダイアログ通知
                            parent.Feedback = "中止";
                            parent.Alert = t.Exception.InnerException.Message + t.Exception.StackTrace;
                            return;
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }, () => IsBroadcasting);

            StopCommand = new DelegateCommand(() =>
            {
                service.StopAsync(Id,
                    new Progress<string>(x => parent.Feedback = x))
                    .ContinueWith(t =>
                    {
                        if (t.IsFaulted)
                        {
                            // ダイアログ通知
                            parent.Feedback = "中止";
                            parent.Alert = t.Exception.InnerException.Message + t.Exception.StackTrace;
                            return;
                        }
                        Id = "";
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }, () => IsBroadcasting);
        }

        private string id = "";
        public string Id
        {
            get { return id; }
            set
            {
                if (!SetProperty("Id", ref id, value))
                    return;
                if (string.IsNullOrEmpty(id))
                {
                    parent.ExternalSourceViewModel.IsBroadcasting = false;
                }
                else
                {
                    parent.ExternalSourceViewModel.IsBroadcasting = true;
                }
                OnPropertyChanged("IsBroadcasting");
                BroadcastCommand.OnCanExecuteChanged();
                UpdateCommand.OnCanExecuteChanged();
                StopCommand.OnCanExecuteChanged();
            }
        }

        public bool IsBroadcasting { get { return !string.IsNullOrEmpty(Id); } }

        public DelegateCommand BroadcastCommand { get; private set; }
        public DelegateCommand UpdateCommand { get; private set; }
        public DelegateCommand StopCommand { get; private set; }
        public Configuration Configuration { get; private set; }
    }
}
