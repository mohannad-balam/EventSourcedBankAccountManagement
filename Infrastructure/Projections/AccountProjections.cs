using EventSourcedBankAccountManagement.Domain.Events;
using EventSourcedBankAccountManagement.Infrastructure.EventStore;
using System.Collections.Concurrent;
namespace EventSourcedBankAccountManagement.Infrastructure.Projections
{
    public class AccountBalanceProjection
    {
        public long LastPosition { get; private set; } = 0;
        public ConcurrentDictionary<Guid, decimal> Balances { get; } = new();
        public ConcurrentDictionary<Guid, List<string>> History { get; } = new();

        public void Project(Guid aggregateId, object evt, long pos)
        {
            switch (evt)
            {
                case AccountOpened e:
                    Balances[e.AccountId] = e.InitialDeposit;
                    AppendHistory(aggregateId, $"Account opened with {e.InitialDeposit}");
                    break;

                case MoneyDeposited e:
                    Balances.AddOrUpdate(e.AccountId, e.Amount, (_, old) => old + e.Amount);
                    AppendHistory(aggregateId, $"Deposited {e.Amount}");
                    break;

                case MoneyWithdrawn e:
                    Balances.AddOrUpdate(e.AccountId, 0, (_, old) => old - e.Amount);
                    AppendHistory(aggregateId, $"Withdrew {e.Amount}");
                    break;
            }

            LastPosition = pos + 1;
        }

        private void AppendHistory(Guid id, string message)
        {
            if (!History.TryGetValue(id, out var list))
            {
                list = [];
                History[id] = list;
            }
            list.Add(message);
        }
    }


    public class ProjectionWorker(IEventStore store, AccountBalanceProjection proj)
    {
        private readonly IEventStore _store = store;
        private readonly AccountBalanceProjection _proj = proj;

        public async Task Start(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var events = await _store.GetAllEvents(_proj.LastPosition);
                foreach (var (agg, evt, pos) in events)
                {
                    _proj.Project(agg, evt, pos);
                }
                await Task.Delay(500, token);
            }
        }
    }
}
