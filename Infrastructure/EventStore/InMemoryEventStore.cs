using EventSourcedBankAccountManagement.Infrastructure.EventStore;

public class InMemoryEventStore : IEventStore
{
    private readonly Dictionary<Guid, List<object>> _store = new();
    private readonly List<(Guid agg, object evt)> _allEvents = new();

    public Task<IEnumerable<object>> LoadStream(Guid aggregateId)
    {
        _store.TryGetValue(aggregateId, out var list);
        return Task.FromResult(list?.AsEnumerable() ?? Enumerable.Empty<object>());
    }

    public Task AppendToStream(Guid aggregateId, IEnumerable<object> events)
    {
        if (!_store.TryGetValue(aggregateId, out var list))
        {
            list = new();
            _store[aggregateId] = list;
        }
        foreach (var e in events)
        {
            list.Add(e);
            _allEvents.Add((aggregateId, e));
        }
        return Task.CompletedTask;
    }

    public Task<IEnumerable<(Guid, object, long)>> GetAllEvents(long sincePosition)
    {
        var slice = _allEvents.Skip((int)sincePosition).Select((x, idx) => (x.agg, x.evt, sincePosition + idx));
        return Task.FromResult((IEnumerable<(Guid, object, long)>)slice);
    }
}