using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System;
using System.Collections;
using System.Linq;

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

        public PluginSettingsList GetPluginSettings(string assemblyName)
        {
            try
            {
                using (var fileStream
                    = m_externalResource.GetPluginSettingsInputStream(
                        assemblyName + ".xml"))
                {
                    return (PluginSettingsList)CreateDataContractSerializer()
                        .ReadObject(fileStream);
                }
            }
            catch (Exception)
            {
                return new PluginSettingsList()
                {
                    PluginSettings = new Dictionary<string, PluginSettings>()
                };
            }
        }

        public PluginSettings GetPluginSettings(string assemblyName, string pluginName)
        {
            PluginSettings value;
            if (!GetPluginSettings(assemblyName).PluginSettings.TryGetValue(pluginName, out value))
            {
                value = new PluginSettings();
            }
            return value;
        }

        public void PutPluginSettings(string assemblyName, PluginSettingsList pluginSettingsList)
        {
            using (var fileStream = m_externalResource.GetPluginSettingsOutputStream(assemblyName + ".xml"))
            {
                CreateDataContractSerializer().WriteObject(fileStream, pluginSettingsList);
            }
        }

        public void PutPluginSettings(string assemblyName, string pluginName, PluginSettings pluginSettings)
        {
            var list = GetPluginSettings(assemblyName);
            list.PluginSettings[pluginName] = pluginSettings;
            PutPluginSettings(assemblyName, list);
        }

        private DataContractSerializer CreateDataContractSerializer()
        {
            return new DataContractSerializer(typeof(PluginSettingsList));
        }
    }
}
