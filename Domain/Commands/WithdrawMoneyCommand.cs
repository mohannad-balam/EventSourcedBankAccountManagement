namespace EventSourcedBankAccountManagement.Domain.Commands;
public class WithdrawMoneyCommand
{
    public Guid AccountId { get; set; }
    public decimal Amount { get; set; }
}
