using System;

namespace DesktopApp.ViewModel
{
    public class CProgressDialogViewModel : CViewModelBase
    {
        private Int32 _progressBarSize;

        public Int32 ProgressBarSize
        {
            get => _progressBarSize;
            set => SetProperty(ref _progressBarSize, value);
        }


        public CProgressDialogViewModel(Int32 progressBarSize)
        {
            ProgressBarSize = progressBarSize;
        }

        public CProgressDialogViewModel()
            : this(64)
        {
        }
    }
}
