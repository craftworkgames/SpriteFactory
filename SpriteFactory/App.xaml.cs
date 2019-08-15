using System.Windows;
using Serilog;

namespace SpriteFactory
{
    public partial class App : Application
    {
        private ILogger _logger;

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
