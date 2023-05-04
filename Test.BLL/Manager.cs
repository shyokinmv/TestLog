using Microsoft.Extensions.Logging;

namespace Test.BLL
{
    public class Manager
    {
        private ILogger _logger;

        public Manager(ILogger logger)
        {
            _logger = logger;
        }

        public async Task Run(string[] args, int delay)
        {
            string argStr = string.Join(" ", args);
            string msg = $"Run with parameters: {argStr}";
            _logger.LogDebug(msg);

            if (delay > 0)
                Thread.Sleep(delay * 1000);

            _logger.LogDebug($"Task done in {delay} seconds");
        }
    }
}