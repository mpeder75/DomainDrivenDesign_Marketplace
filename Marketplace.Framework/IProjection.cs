namespace Marketplace.Framework;

public interface IProjection
{
    Task Project(object @event);
}