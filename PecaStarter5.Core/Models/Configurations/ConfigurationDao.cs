using System.IO;
using System.Xml.Serialization;

namespace Progressive.PecaStarter5.Models.Configurations
{
    class ConfigurationDao
    {
        private IExternalResource m_externalResource;

        public ConfigurationDao(IExternalResource externalResource)
        {
            m_externalResource = externalResource;
        }

        public Configuration Get()
        {
            try
            {
                using (var fileStream = m_externalResource.GetConfigurationInputStream())
                {
                    // TODO: DataContractSerializerに置き換え
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
            using (var fileStream = m_externalResource.GetConfigurationOutputStream())
            {
                new XmlSerializer(typeof(Configuration)).Serialize(fileStream, entity);
            }
        }
    }
}
