namespace EventSourcedBankAccountManagement.Domain.Commands;
public class DepositMoneyCommand
{
    public Guid AccountId { get; set; }
    public decimal Amount{ get; set; }
}
