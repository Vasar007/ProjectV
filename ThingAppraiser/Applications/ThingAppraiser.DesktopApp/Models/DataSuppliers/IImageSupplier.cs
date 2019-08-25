using ThingAppraiser.Models.Data;

namespace ThingAppraiser.DesktopApp.Models.DataSuppliers
{
    internal interface IImageSupplier
    {
        string GetImageLink(BasicInfo data, ImageSize imageSize);
    }
}
