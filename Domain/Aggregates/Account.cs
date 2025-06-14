// Domain/Aggregates/Account.cs
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Aggregates
{
    public class Account
    {
        public Guid Id { get; private set; }
        public decimal Balance { get; private set; }
        private readonly List<object> _uncommitted = new();

        public IEnumerable<object> UncommittedEvents => _uncommitted.ToList();

        public Account(IEnumerable<object> history)
        {
            foreach (var e in history) Apply(e);
        }

        public Account() { } // empty for new

        public void Open(Guid id, decimal initialDeposit)
        {
            if (initialDeposit < 0) throw new Exception("Initial deposit must be >= 0");
            Raise(new AccountOpened(id, initialDeposit));
        }

        public void Deposit(decimal amount)
        {
            if (amount <= 0) throw new Exception("Deposit amount must be > 0");
            Raise(new MoneyDeposited(Id, amount));
        }

        public void Withdraw(decimal amount)
        {
            if (amount <= 0) throw new Exception("Withdraw amount must be > 0");
            if (Balance - amount < 0) throw new Exception("Insufficient funds");
            Raise(new MoneyWithdrawn(Id, amount));
        }

        private void Raise(object e)
        {
            Apply(e);
            _uncommitted.Add(e);
        }

        private void Apply(object e)
        {
            switch (e)
            {
                case AccountOpened x:
                    Id = x.AccountId;
                    Balance = x.InitialDeposit;
                    break;
                case MoneyDeposited x:
                    Balance += x.Amount;
                    break;
                case MoneyWithdrawn x:
                    Balance -= x.Amount;
                    break;
            }
        }
    }
}
