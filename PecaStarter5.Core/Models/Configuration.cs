using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;

namespace Progressive.PecaStarter5.Models
{
    public class Configuration : INotifyPropertyChanged
    {
        public Configuration()
        {
            Port = 7144;
            YellowPagesList = new List<YellowPages>();
            NameHistory = new Collection<string>();
            GenreHistory = new Collection<string>();
            DescriptionHistory = new Collection<string>();
            CommentHistory = new Collection<string>();
        }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public string SelectedYellowPages { get; set; }
        public List<YellowPages> YellowPagesList { get; set; }
        public string StreamUrl { get; set; }
        public Collection<string> NameHistory { get; set; }
        public Collection<string> GenreHistory { get; set; }
        public Collection<string> DescriptionHistory { get; set; }
        public string ContactUrl { get; set; }
        public Collection<string> CommentHistory { get; set; }

        private bool hasNotifyIcon;
        public bool HasNotifyIcon
        {
            get { return hasNotifyIcon; }
            set
            {
                if (hasNotifyIcon == value)
                    return;
                hasNotifyIcon = value;
                OnPropertyChanged("HasNotifyIcon");
            }
        }

        public bool IsSavePosition { get; set; }
        public bool Logging { get; set; }
        private string m_logPath;
        public string LogPath
        {
            get { return m_logPath; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                    m_logPath = value;
                else
                    m_logPath = DefaultLogPath;
            }
        }
        public int Delay { get; set; }
        public int Port { get; set; }

        [XmlIgnore]
        private string m_defaultLogPath;
        public string DefaultLogPath
        {
            get { return m_defaultLogPath; }
            set
            {
                m_defaultLogPath = value;
                LogPath = LogPath;
            }
        }

        #region INotifyPropertyChanged メンバー

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged == null)
                return;
            if (GetType().GetProperty(propertyName) == null)
                return;
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public class YellowPages
        {
            public string Name { get; set; }
            public int? AcceptedHash { get; set; }
            public string Prefix { get; set; }
        }
    }
}
