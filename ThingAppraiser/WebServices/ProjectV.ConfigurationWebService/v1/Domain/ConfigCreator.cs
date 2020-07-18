using ProjectV.Configuration;
using ProjectV.Models.Configuration;
using ProjectV.Models.WebService;

namespace ProjectV.ConfigurationWebService.v1.Domain
{
    public sealed class ConfigCreator : IConfigCreator
    {
        public ConfigCreator()
        {
        }

        #region IConfigCreator Implementation

        public ConfigurationXml CreateConfigBasedOnRequirements(
            ConfigRequirements configRequirements)
        {
            ConfigurationXml configurationXml =
                XmlConfigCreator.CreateXmlConfigBasedOnRequirements(configRequirements);
            return configurationXml;
        }

        #endregion
    }
}
