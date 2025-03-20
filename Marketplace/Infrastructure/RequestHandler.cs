using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Marketplace.Infrastructure
{
    public class RequestHandler
    {
        // Metode der håndterer requests til commands
        public static async Task<IActionResult> HandleCommand<T>(
            T request, Func<T, Task> handler, ILogger log)
        {
            try
            {
                log.Debug("Handling HTTP request of type {type}", typeof(T).Name);
                await handler(request);
                return new OkResult();
            }
            catch (Exception e)
            {
                log.Error(e, "Error handling the command");
                return new BadRequestObjectResult(new
                {
                    error = e.Message, stackTrace = e.StackTrace
                });
            }
        }
        // Metode der håndterer requests til reads
        public static async Task<IActionResult> HandleQuery<TModel>(
            Func<Task<TModel>> query, ILogger log)
        {
            try
            {
                return new OkObjectResult(await query());
            }
            catch (Exception e)
            {
                log.Error(e, "Error handling the query");
                return new BadRequestObjectResult(new
                {
                    error = e.Message, stackTrace = e.StackTrace
                });
            }
        }
    }
}
