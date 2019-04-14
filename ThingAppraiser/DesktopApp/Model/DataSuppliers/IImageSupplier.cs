using System;
using ThingAppraiser.Data;

namespace DesktopApp.Model.DataSuppliers
{
    public interface IImageSupplier
    {
        String GetImageLink(CBasicInfo data, EImageSize imageSize);
    }
}
