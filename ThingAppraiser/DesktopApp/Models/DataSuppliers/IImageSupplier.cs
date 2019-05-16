using ThingAppraiser.Data;

namespace ThingAppraiser.DesktopApp.Models.DataSuppliers
{
    public interface IImageSupplier
    {
        string GetImageLink(BasicInfo data, ImageSize imageSize);
    }
}
