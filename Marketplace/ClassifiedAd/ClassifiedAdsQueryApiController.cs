using Marketplace.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Marketplace.ClassifiedAd;

[Route("/ad")]
public class ClassifiedAdsQueryApiController : Controller
{
    private static readonly ILogger _log = Log.ForContext<ClassifiedAdsQueryApiController>();
    private readonly IAsyncDocumentSession _session;

    public ClassifiedAdsQueryApiController(IAsyncDocumentSession session)
    {
        _session = session;
    }

    [HttpGet]
    [Route("list")]
    public Task<IActionResult> Get(QueryModels.GetPublishedClassifiedAds request)
    {
        return RequestHandler.HandleQuery(() => _session.Query(request), _log);
    }

    [HttpGet]
    [Route("myads")]
    public Task<IActionResult> Get(QueryModels.GetOwnersClassifiedAd request)
    {
        return RequestHandler.HandleQuery(() => _session.Query(request), _log);
    }

    [HttpGet]
    public Task<IActionResult> Get(QueryModels.GetPublicClassifiedAd request)
    {
        return RequestHandler.HandleQuery(() => _session.Query(request), _log);
    }
}