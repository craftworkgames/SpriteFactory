namespace SpriteFactory.About
{
    public class AboutWindowViewModel : ViewModel
    {
        public AboutWindowViewModel()
        {
        }

        public override string Title => $"About {App.Name}";

        public string Name => App.Name;
        public string Version => $"version {App.Version}";
    }
}
