using System;
using System.ComponentModel;
using System.Windows;
using Progressive.PecaStarter5.Properties;

namespace Progressive.Commons.Views
{
    class TaskTrayIconManager : IDisposable
    {
        private TaskTrayIcon taskTrayIcon;
        private Window root;
        private INotifyPropertyChanged dataSource;
        private string propertyName;

        public bool IsEnabled { get { return taskTrayIcon != null; } }
        private bool IsSourceEnabled
        {
            get
            {
                return (bool)dataSource.GetType().GetProperty(propertyName).GetValue(dataSource, null);
            }
        }

        public TaskTrayIconManager(Window root, INotifyPropertyChanged source, string propertyName)
        {
            this.root = root;
            this.dataSource = source;
            this.propertyName = propertyName;
            dataSource.PropertyChanged += source_PropertyChanged;
            if (IsSourceEnabled)
            {
                Enable();
            }
        }
        ~TaskTrayIconManager()
        {
            Dispose();
        }

        public void Dispose()
        {
            dataSource.PropertyChanged -= source_PropertyChanged;
            Disable();
            GC.SuppressFinalize(this);
        }

        public void Enable()
        {
            if (taskTrayIcon == null)
            {
                taskTrayIcon = new TaskTrayIcon(root);
                taskTrayIcon.Icon = Resources.icon;
                taskTrayIcon.Text = root.Title;
                taskTrayIcon.BalloonTipText = "";
                taskTrayIcon.BalloonTipTitle = root.Title;
            }
        }
        public void Disable()
        {
            if (taskTrayIcon != null)
            {
                taskTrayIcon.Dispose();
                taskTrayIcon = null;
            }
        }
        private void source_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != propertyName)
            {
                return;
            }

            if (IsSourceEnabled)
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }
    }
}
