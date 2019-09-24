using System;
using System.Collections.Generic;
using System.Linq;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class ContentFinderViewModel : ViewModelBase
    {
        public object DialogIdentifier { get; }

        public ContentFinderViewModel(object dialogIdentifier)
        {
            DialogIdentifier = dialogIdentifier.ThrowIfNull(nameof(dialogIdentifier));
        }
    }
}
