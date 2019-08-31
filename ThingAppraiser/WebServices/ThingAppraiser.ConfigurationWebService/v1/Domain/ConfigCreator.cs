using ThingAppraiser.Configuration;
using ThingAppraiser.Models.Configuration;
using ThingAppraiser.Models.WebService;

namespace ThingAppraiser.ConfigurationWebService.v1.Domain
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
