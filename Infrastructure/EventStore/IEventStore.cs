namespace EventSourcedBankAccountManagement.Infrastructure.EventStore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
public interface IEventStore
{
    Task<IEnumerable<object>> LoadStream(Guid aggregateId);
    Task AppendToStream(Guid aggregateId, IEnumerable<object> events);
    Task<IEnumerable<(Guid aggregateId, object evt, long position)>> GetAllEvents(long sincePosition);
}
