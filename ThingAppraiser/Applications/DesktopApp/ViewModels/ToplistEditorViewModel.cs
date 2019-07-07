using System;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal class ToplistEditorViewModel : ViewModelBase
    {
        private int _progressBarSize;

        public int ProgressBarSize
        {
            get => _progressBarSize;
            set => SetProperty(ref _progressBarSize, value);
        }


        public ToplistEditorViewModel(int progressBarSize)
        {
            ProgressBarSize = progressBarSize;
        }

        public ToplistEditorViewModel()
            : this(64)
        {
        }

        public void Update(string toplistName, string toplistType, string toplistFormat)
        {
            Console.WriteLine(toplistName);
            Console.WriteLine(toplistType);
            Console.WriteLine(toplistFormat);
        }
    }
}
