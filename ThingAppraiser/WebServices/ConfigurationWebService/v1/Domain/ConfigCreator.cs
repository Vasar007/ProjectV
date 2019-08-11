using ThingAppraiser.Core;
using ThingAppraiser.Models.Configuration;
using ThingAppraiser.Models.WebService;

namespace ThingAppraiser.ConfigurationWebService.v1.Domain
{
    public class ConfigCreator : IConfigCreator
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
