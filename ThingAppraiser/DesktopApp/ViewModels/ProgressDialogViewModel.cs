namespace ThingAppraiser.DesktopApp.ViewModels
{
    public class ProgressDialogViewModel : ViewModelBase
    {
        private int _progressBarSize;

        public int ProgressBarSize
        {
            get => _progressBarSize;
            set => SetProperty(ref _progressBarSize, value);
        }


        public ProgressDialogViewModel(int progressBarSize)
        {
            ProgressBarSize = progressBarSize;
        }

        public ProgressDialogViewModel()
            : this(64)
        {
        }
    }
}
