using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Test.BLL;
using Test.Logger;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // logging system initialisation
            string log4NetConfigFile = "log4net.config";
            bool skipDiagnosticLogs = false;
            ILogger logger = new Log4NetLogger(typeof(Program).Namespace, new FileInfo(log4NetConfigFile), skipDiagnosticLogs);

            // configuration initialization
            var switchMappings = new Dictionary<string, string>()
            {
                { "-d", "delay" },
            };
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args, switchMappings);

            var config = builder.Build();

            // get delay parameter 
            string delayParam = config["delay"];
            if (!int.TryParse(delayParam, out int delay))
                delay = 0;
            
            // create business logic class instance
            var manager = new Manager(logger);

            // run process
            var task = manager.Run(args, delay);
            task.Wait();

#if DEBUG
            Console.ReadKey();
#endif
        }
    }
}
