using Acolyte.Assertions;
using Prism.Mvvm;
using ThingAppraiser.DesktopApp.Models;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class EnterDataViewModel : BindableBase
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value.ThrowIfNull(nameof(value)));
        }

        public string HintText { get; }
        

        public EnterDataViewModel(GoogleDriveData googleDriveData)
        {
            googleDriveData.ThrowIfNull(nameof(googleDriveData));

            _name = string.Empty;

            HintText = googleDriveData.HintText.ThrowIfNullOrEmpty(nameof(HintText));
        }
    }
}
