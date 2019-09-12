using System.Windows;
using Serilog;

namespace SpriteFactory
{
    public partial class App : Application
    {
        private ILogger _logger;

        public const string Name = "Sprite Factory";

        protected override void OnStartup(StartupEventArgs e)
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Trace()
                .CreateLogger();

            Log.Logger = _logger;

            _logger.Information("Application started.");

            base.OnStartup(e);
        }
    }
}
