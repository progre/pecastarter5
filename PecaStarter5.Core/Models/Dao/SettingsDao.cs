using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Progressive.PecaStarter5.Model;
using System.Xml.Serialization;
using System.IO;

namespace Progressive.PecaStarter5.Models.Dao
{
    class SettingsDao
    {
        public string FilePath { get; set; }

        public Settings Get()
        {
            try
            {
                using (var fileStream = new FileStream(FilePath, FileMode.Open))
                {
                    return (Settings)new XmlSerializer(typeof(Settings)).Deserialize(fileStream);
                }
            }
            catch (FileNotFoundException)
            {
                return new Settings();
            }
        }

        public void Put(Settings entity)
        {
            using (var fileStream = new FileStream(FilePath, FileMode.OpenOrCreate))
            {
                new XmlSerializer(typeof(Settings)).Serialize(fileStream, entity);
            }
        }
    }
}
