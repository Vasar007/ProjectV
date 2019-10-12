using System;
using System.Collections.Generic;
using System.Linq;
using Prism.Mvvm;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class ContentFinderViewModel : BindableBase
    {
        public object DialogIdentifier { get; }


        public ContentFinderViewModel(object dialogIdentifier)
        {
            DialogIdentifier = dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));
        }
    }
}
