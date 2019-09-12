using System.ComponentModel;
using System.Windows;
using MahApps.Metro.Controls;

namespace SpriteFactory
{
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
                await viewModel.InitializeViewModelAsync();
        }

        private async void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (DataContext is MainWindowViewModel viewModel)
                e.Cancel = !await viewModel.ConfirmSave();
        }

    }
}
