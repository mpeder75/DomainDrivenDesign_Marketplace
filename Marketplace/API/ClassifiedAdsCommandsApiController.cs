using Marketplace.Api;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Serilog.ILogger;


namespace Marketplace.API
{
    [Route("/ad")]
    public class ClassifiedAdsCommandsApiController : Controller
    {
        private readonly ClassifiedAdsApplicationService _applicationService;
        private static ILogger Log = Serilog.Log.ForContext<ClassifiedAdsCommandsApiController>();
        
        public ClassifiedAdsCommandsApiController(ClassifiedAdsApplicationService applicationService) 
            => _applicationService = applicationService;
        
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Contracts.ClassifiedAds.V1.Create request)
        {
            await _applicationService.Handle(request);

            return Ok();
        }

        [Route("name" )]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Contracts.ClassifiedAds.V1.SetTitle request)
        {
            await _applicationService.Handle(request);
            return Ok();
        }

        [Route("text")]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Contracts.ClassifiedAds.V1.UpdateText request)
        {
            await _applicationService.Handle(request);
            return Ok();
        }

        [Route("price")]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Contracts.ClassifiedAds.V1.UpdatePrice request)
        {
            await _applicationService.Handle(request);
            return Ok();
        }

        [Route("publish")]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Contracts.ClassifiedAds.V1.RequestToPublish request)
        {
            await _applicationService.Handle(request);
            return Ok();
        }

        private async Task<IActionResult> HandleRequest<T>(T request, Func<T, Task> handler)
        {
            try
            {
                Log.Debug("Handling HTTP request of type {type}", typeof(T).Name);
                await handler(request);
                return Ok();
            }
            catch (Exception e)
            {
                Log.Error(e, "Error handling the request");
                return new BadRequestObjectResult(new {error = e.Message, stackTrace = e.StackTrace});
            }
        }
    }
}
