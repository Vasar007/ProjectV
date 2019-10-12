using Prism.Mvvm;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class EnterDataViewModel : BindableBase
    {
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value.ThrowIfNull(nameof(value)));
        }

        public string HintText { get; }
        

        public EnterDataViewModel(string hintText)
        {
            HintText = hintText.ThrowIfNullOrEmpty(nameof(hintText));
        }
    }
}
