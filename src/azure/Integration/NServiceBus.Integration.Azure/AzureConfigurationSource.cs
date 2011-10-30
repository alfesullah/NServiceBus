using System;
using System.Configuration;
using System.Linq;
using System.Web.Configuration;
using System.Web.Hosting;
using NServiceBus.Config.ConfigurationSource;

namespace NServiceBus.Integration.Azure
{
    public class AzureConfigurationSource : IConfigurationSource
    {
        private readonly IAzureConfigurationSettings azureConfigurationSettings;

        public AzureConfigurationSource(IAzureConfigurationSettings configurationSettings)
        {
            azureConfigurationSettings = configurationSettings;
        }

        public string ConfigurationPrefix { get; set; }

        T IConfigurationSource.GetConfiguration<T>()
        {
            var sectionName = typeof(T).Name;

            var section = GetConfigurationHandler()
                              .GetSection(sectionName) as T;

            foreach (var property in typeof(T).GetProperties().Where(x => x.DeclaringType == typeof(T)))
            {
                var adjustedPrefix = !string.IsNullOrEmpty(ConfigurationPrefix) ? ConfigurationPrefix + "." : string.Empty;

                var setting = azureConfigurationSettings.GetSetting(adjustedPrefix + sectionName + "." + property.Name);

                if (!string.IsNullOrEmpty(setting))
                {
                    if( section == null) section = new T();

                    property.SetValue(section, Convert.ChangeType(setting, property.PropertyType), null);
                }
            }

            return section;
        }

        private static Configuration GetConfigurationHandler()
        {
            if (IsWebsite()) return WebConfigurationManager.OpenWebConfiguration("/");

            return ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        private static bool IsWebsite()
        {
            return HostingEnvironment.IsHosted;
        }
    }
}