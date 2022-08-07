using ProjectV.Models.Configuration;
using ProjectV.Models.WebServices.Requests;

namespace ProjectV.ConfigurationWebService.v1.Domain
{
    public interface IConfigCreator
    {
        ConfigurationXml CreateConfigBasedOnRequirements(ConfigRequirements configRequirements);
    }
}
