using System.Text;
using EventStore.ClientAPI;
using Marketplace.Framework;
using Newtonsoft.Json;

namespace Marketplace.Infrastructure;

public class EsAggregateStore : IAggregateStore
{
    private readonly IEventStoreConnection _connection;

    public EsAggregateStore(IEventStoreConnection connection)
    {
        _connection = connection;
    }

    public async Task<bool> Exists<T, TId>(TId aggregateId)
        where TId : Value<TId>  // Add this constraint
    {
        var stream = GetStreamName<T, TId>(aggregateId);
        var result = await _connection.ReadEventAsync(stream, 1, false);
        return result.Status != EventReadStatus.NoStream;
    }

    public async Task Save<T, TId>(T aggregate)
        where T : AggregateRoot<TId>
        where TId : Value<TId>  // Add this constraint
    {
        if (aggregate == null)
            throw new ArgumentNullException(nameof(aggregate));

        var changes = aggregate.GetChanges()
            .Select(@event =>
                new EventData(
                    Guid.NewGuid(),
                    @event.GetType().Name,
                    true,
                    Serialize(@event),
                    Serialize(new EventMetadata
                    {
                        ClrType = @event.GetType().AssemblyQualifiedName
                    })
                ))
            .ToArray();

        if (!changes.Any()) return;

        var streamName = GetStreamName<T, TId>(aggregate);
        await _connection.AppendToStreamAsync(
            streamName,
            aggregate.Version,
            changes);

        aggregate.ClearChanges();
    }

    public async Task<T> Load<T, TId>(TId aggregateId)
        where T : AggregateRoot<TId>
        where TId : Value<TId>  // Add this constraint
    {
        if (aggregateId == null)
            throw new ArgumentNullException(nameof(aggregateId));

        var stream = GetStreamName<T, TId>(aggregateId);
        var aggregate = (T)Activator.CreateInstance(typeof(T), true);

        var page = await _connection.ReadStreamEventsForwardAsync(
            stream, 0, 1024, false);

        aggregate.Load(page.Events.Select(resolvedEvent =>
        {
            var meta = JsonConvert.DeserializeObject<EventMetadata>(
                Encoding.UTF8.GetString(resolvedEvent.Event.Metadata));
            var dataType = Type.GetType(meta.ClrType);
            var jsonData =
                Encoding.UTF8.GetString(resolvedEvent.Event.Data);
            var data = JsonConvert.DeserializeObject(jsonData, dataType);
            return data;
        }).ToArray());

        return aggregate;
    }

    private static byte[] Serialize(object data)
    {
        return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
    }

    private static string GetStreamName<T, TId>(TId aggregateId)
        where TId : Value<TId>  // Add this constraint
    {
        return $"{typeof(T).Name}-{aggregateId}";
    }

    private static string GetStreamName<T, TId>(T aggregate)
        where T : AggregateRoot<TId>
        where TId : Value<TId>  // Add this constraint
    {
        return $"{typeof(T).Name}-{aggregate.Id}";
    }

    private class EventMetadata
    {
        public string ClrType { get; set; }
    }
}