using ProjectV.Models.Data;

namespace ProjectV.DesktopApp.Models.DataSuppliers
{
    internal interface IImageSupplier
    {
        string GetImageLink(BasicInfo data, ImageSize imageSize);
    }
}
