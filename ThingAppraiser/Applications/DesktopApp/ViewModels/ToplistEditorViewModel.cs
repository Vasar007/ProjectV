using System;
using System.Collections.ObjectModel;
using ThingAppraiser.DesktopApp.Models;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    internal class ToplistEditorViewModel : ViewModelBase
    {
        public ObservableCollection<ToplistItem> ToplistItems { get; private set; }


        public ToplistEditorViewModel()
        {
            ToplistItems = new ObservableCollection<ToplistItem>();

            ToplistItems.Add(new ToplistItem("Name1", 1));
            ToplistItems.Add(new ToplistItem("Name2", 2));
            ToplistItems.Add(new ToplistItem("Name3", 3));
            ToplistItems.Add(new ToplistItem("Name4", 4));
            ToplistItems.Add(new ToplistItem("Name5", 5));
        }

        public void Update(string toplistName, string toplistType, string toplistFormat)
        {
            Console.WriteLine(toplistName);
            Console.WriteLine(toplistType);
            Console.WriteLine(toplistFormat);
        }
    }
}
