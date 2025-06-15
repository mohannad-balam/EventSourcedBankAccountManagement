using EventSourcedBankAccountManagement.Domain.Aggregates;
using EventSourcedBankAccountManagement.Domain.Commands;
using EventSourcedBankAccountManagement.Infrastructure.EventStore;

namespace EventSourcedBankAccountManagement.Applications.Handlers
{
    public class DipositMoneyHandler(IEventStore store)
    {
        private readonly IEventStore _store = store;

        public async Task Handle(DepositMoneyCommand cmd)
        {
            var history = await _store.LoadStream(cmd.AccountId);
            var acct = new Account(history);
            acct.Deposit(cmd.Amount);
            await _store.AppendToStream(cmd.AccountId, acct.UncommittedEvents);
        }
    }
}

