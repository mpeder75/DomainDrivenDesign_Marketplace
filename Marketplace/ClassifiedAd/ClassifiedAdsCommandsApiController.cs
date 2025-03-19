using Microsoft.AspNetCore.Mvc;
using ILogger = Serilog.ILogger;

namespace Marketplace.ClassifiedAd;

[Route("/ad")]
public class ClassifiedAdsCommandsApiController : Controller
{
    private static readonly ILogger Log = Serilog.Log.ForContext<ClassifiedAdsCommandsApiController>();
    private readonly ClassifiedAdsApplicationService _applicationService;

    public ClassifiedAdsCommandsApiController(
        ClassifiedAdsApplicationService applicationService)
    {
        _applicationService = applicationService;
    }

    [HttpPost]
    public Task<IActionResult> Post(Contracts.V1.Create request)
    {
        return HandleRequest(request, _applicationService.Handle);
    }

    [Route("name")]
    [HttpPut]
    public Task<IActionResult> Put(Contracts.V1.SetTitle request)
    {
        return HandleRequest(request, _applicationService.Handle);
    }

    [Route("text")]
    [HttpPut]
    public Task<IActionResult> Put(Contracts.V1.UpdateText request)
    {
        return HandleRequest(request, _applicationService.Handle);
    }

    [Route("price")]
    [HttpPut]
    public Task<IActionResult> Put(Contracts.V1.UpdatePrice request)
    {
        return HandleRequest(request, _applicationService.Handle);
    }

    [Route("publish")]
    [HttpPut]
    public Task<IActionResult> Put(Contracts.V1.RequestToPublish request)
    {
        return HandleRequest(request, _applicationService.Handle);
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
            return new BadRequestObjectResult(new { error = e.Message, stackTrace = e.StackTrace });
        }
    }
}