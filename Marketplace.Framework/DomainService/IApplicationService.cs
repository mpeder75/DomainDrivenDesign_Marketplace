namespace Marketplace.Framework.DomainService;

public interface IApplicationService
{
    Task Handle(object command);
}