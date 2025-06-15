using EventSourcedBankAccountManagement.Domain.Aggregates;
using EventSourcedBankAccountManagement.Domain.Commands;
using EventSourcedBankAccountManagement.Infrastructure.EventStore;

namespace EventSourcedBankAccountManagement.Applications.Handlers
{
    public class OpenAccountHandler(IEventStore store)
    {
        private readonly IEventStore _store = store;

        public async Task Handle(OpenAccountCommand cmd)
        {
            var history = await _store.LoadStream(cmd.AccountId);
            var acct = new Account(history);
            acct.Open(cmd.AccountId, cmd.InitialDeposit);
            await _store.AppendToStream(cmd.AccountId, acct.UncommittedEvents);
        }
    }
}

