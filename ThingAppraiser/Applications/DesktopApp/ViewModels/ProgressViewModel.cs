namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal sealed class ProgressViewModel : ViewModelBase
    {
        private int _progressBarSize;

        public int ProgressBarSize
        {
            get => _progressBarSize;
            set => SetProperty(ref _progressBarSize, value);
        }


        public ProgressViewModel(int progressBarSize)
        {
            ProgressBarSize = progressBarSize;
        }

        public ProgressViewModel()
            : this(64)
        {
        }
    }
}
