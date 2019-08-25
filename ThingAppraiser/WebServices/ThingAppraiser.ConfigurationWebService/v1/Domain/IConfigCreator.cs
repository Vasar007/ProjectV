using ThingAppraiser.Models.Configuration;
using ThingAppraiser.Models.WebService;

namespace ThingAppraiser.ConfigurationWebService.v1.Domain
{
    public interface IConfigCreator
    {
        ConfigurationXml CreateConfigBasedOnRequirements(ConfigRequirements configRequirements);
    }
}
