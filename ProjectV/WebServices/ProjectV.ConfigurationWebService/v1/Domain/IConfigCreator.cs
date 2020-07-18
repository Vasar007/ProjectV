using ProjectV.Models.Configuration;
using ProjectV.Models.WebService;

namespace ProjectV.ConfigurationWebService.v1.Domain
{
    public interface IConfigCreator
    {
        ConfigurationXml CreateConfigBasedOnRequirements(ConfigRequirements configRequirements);
    }
}
