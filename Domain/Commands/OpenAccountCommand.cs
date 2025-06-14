namespace EventSourcedBankAccountManagement.Domain.Commands;

public class OpenAccountCommand {
    public Guid AccountId { get; set; }
    public decimal InitialDeposit { get; set; }
}
