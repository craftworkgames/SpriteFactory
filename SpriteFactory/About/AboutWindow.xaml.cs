using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using MahApps.Metro.Controls;

namespace SpriteFactory.About
{
    public partial class AboutWindow : MetroWindow
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            if(sender is Hyperlink hyperlink)
                Process.Start(hyperlink.NavigateUri.ToString());
        }
    }
}
