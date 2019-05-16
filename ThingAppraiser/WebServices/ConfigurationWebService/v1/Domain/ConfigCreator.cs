using ThingAppraiser.Core;
using ThingAppraiser.Data.Configuration;
using ThingAppraiser.Data.Models;

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
