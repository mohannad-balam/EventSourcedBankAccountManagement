// Domain/Events/AccountOpened.cs
namespace EventSourcedBankAccountManagement.Domain.Events;
public record AccountOpened(Guid AccountId, decimal InitialDeposit);
public record MoneyDeposited(Guid AccountId, decimal Amount);
public record MoneyWithdrawn(Guid AccountId, decimal Amount);
