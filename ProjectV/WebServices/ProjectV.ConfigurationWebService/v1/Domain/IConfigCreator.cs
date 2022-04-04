using ProjectV.Models.Configuration;
using ProjectV.Models.WebService.Requests;

namespace ProjectV.ConfigurationWebService.v1.Domain
{
    public interface IConfigCreator
    {
        ConfigurationXml CreateConfigBasedOnRequirements(ConfigRequirements configRequirements);
    }
}
