using EventSourcedBankAccountManagement.Domain.Aggregates;
using EventSourcedBankAccountManagement.Domain.Commands;
using EventSourcedBankAccountManagement.Infrastructure.EventStore;

namespace EventSourcedBankAccountManagement.Applications.Handlers
{
    public class WithdarwMoneyHandler(IEventStore store)
    {
        private readonly IEventStore _store = store;

        public async Task Handle(WithdrawMoneyCommand cmd)
        {
            var history = await _store.LoadStream(cmd.AccountId);
            var acct = new Account(history);
            acct.Withdraw(cmd.Amount);
            await _store.AppendToStream(cmd.AccountId, acct.UncommittedEvents);
        }
    }
}