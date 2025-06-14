// Infrastructure/Projections/AccountProjections.cs
using EventSourcedBankAccountManagement.Domain.Events;
using EventSourcedBankAccountManagement.Infrastructure.EventStore;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace EventSourcedBankAccountManagement.Infrastructure.Projections
{
    public class AccountBalanceProjection
    {
        public long LastPosition { get; private set; } = 0;
        public ConcurrentDictionary<Guid, decimal> Balances { get; } = new();
        public ConcurrentBag<(Guid accountId, string type, decimal amount, long position)> History = new();

        public void Project(Guid aggregateId, object evt, long pos)
        {
            switch (evt)
            {
                case AccountOpened x:
                    Balances[x.AccountId] = x.InitialDeposit;
                    History.Add((x.AccountId, nameof(AccountOpened), x.InitialDeposit, pos));
                    break;
                case MoneyDeposited x:
                    Balances.AddOrUpdate(x.AccountId, x.Amount, (_, old) => old + x.Amount);
                    History.Add((x.AccountId, nameof(MoneyDeposited), x.Amount, pos));
                    break;
                case MoneyWithdrawn x:
                    Balances.AddOrUpdate(x.AccountId, 0, (_, old) => old - x.Amount);
                    History.Add((x.AccountId, nameof(MoneyWithdrawn), x.Amount, pos));
                    break;
            }

            LastPosition = pos + 1;
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
