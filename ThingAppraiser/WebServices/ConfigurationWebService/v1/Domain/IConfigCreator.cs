using ThingAppraiser.Data.Configuration;
using ThingAppraiser.Data.Models;

namespace ThingAppraiser.ConfigurationWebService.v1.Domain
{
    public interface IConfigCreator
    {
        ConfigurationXml CreateConfigBasedOnRequirements(ConfigRequirements configRequirements);
    }
}
