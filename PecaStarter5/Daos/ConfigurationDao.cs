using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Progressive.PecaStarter5.Models;
using System.Xml.Serialization;
using System.IO;

namespace Progressive.PecaStarter5.Daos
{
    class ConfigurationDao
    {
        public string FilePath { get; set; }

        public Configuration Get()
        {
            try
            {
                using (var fileStream = new FileStream(FilePath, FileMode.Open))
                {
                    return (Configuration)new XmlSerializer(typeof(Configuration)).Deserialize(fileStream);
                }
            }
            catch (FileNotFoundException)
            {
                return new Configuration();
            }
        }

        public void Put(Configuration entity)
        {
            using (var fileStream = new FileStream(FilePath, FileMode.Create))
            {
                new XmlSerializer(typeof(Configuration)).Serialize(fileStream, entity);
            }
        }
    }
}
