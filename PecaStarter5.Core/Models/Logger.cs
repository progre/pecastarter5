using System;
using System.IO;
using System.Text;

namespace Progressive.PecaStarter5.Models
{
    public class Logger
    {
        private string path;
        private string name;
        private DateTime startAt;

        public static Logger StartNew(string path, string name)
        {
            return new Logger(path, name, DateTime.Now);
        }

        private Logger(string path, string name, DateTime startAt)
        {
            this.path = path;
            this.name = name;
            this.startAt = startAt;
        }

        public void Insert(string listeners, string relays, string genre, string description, string comment)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            var sb = new StringBuilder();
            sb.Append(DateTime.Now).Append(',');
            sb.Append(listeners).Append(',');
            sb.Append(relays).Append(',');
            sb.Append('"').Append(genre.Replace("\"", "\"\"")).Append('"').Append(',');
            sb.Append('"').Append(description.Replace("\"", "\"\"")).Append('"').Append(',');
            sb.Append('"').Append(comment.Replace("\"", "\"\"")).Append('"');
            sb.Append("\r\n");
            File.AppendAllText(path + System.IO.Path.DirectorySeparatorChar
                + startAt.ToString("yyyyMMdd_HHmmss-") + name + ".csv", sb.ToString());
        }
    }
}
