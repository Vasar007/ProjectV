﻿using System.Windows.Controls;
using ProjectV.DesktopApp.ViewModels;

namespace ProjectV.DesktopApp.Views
{
    /// <summary>
    /// Interaction logic for CreateToplistView.xaml
    /// </summary>
    public sealed partial class CreateToplistView : UserControl
    {
        public CreateToplistView()
        {
            InitializeComponent();
        }

        public void Clear()
        {
            if (DataContext is not CreateToplistViewModel createToplistViewModel) return;

            createToplistViewModel.ToplistName = string.Empty;
            ToplistNameTextBox.Clear();
        }
    }
}
