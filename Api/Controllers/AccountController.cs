// API/Controllers/AccountController.cs
using Microsoft.AspNetCore.Mvc;
using EventSourcedBankAccountManagement.Applications.Handlers;
using EventSourcedBankAccountManagement.Infrastructure.Projections;
using static System.Net.Mime.MediaTypeNames;
using EventSourcedBankAccountManagement.Domain.Commands;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController(
      OpenAccountHandler open,
      DipositMoneyHandler dep,
      WithdarwMoneyHandler wdr,
      AccountBalanceProjection proj) : ControllerBase
    {
        private readonly OpenAccountHandler _open = open;
        private readonly DipositMoneyHandler _deposit = dep;
        private readonly WithdarwMoneyHandler _withdraw = wdr;
        private readonly AccountBalanceProjection _proj = proj;

        [HttpPost("open")]
        public async Task<IActionResult> OpenAccount([FromBody] OpenAccountCommand cmd)
        {
            await _open.Handle(cmd);
            return CreatedAtAction(nameof(GetBalance), new { id = cmd.AccountId }, null);
        }

        [HttpPost("{id}/deposit")]
        public async Task<IActionResult> Deposit(Guid id, [FromBody] decimal amount)
        {
            await _deposit.Handle(new DepositMoneyCommand { AccountId = id, Amount = amount });
            return Ok();
        }

        [HttpPost("{id}/withdraw")]
        public async Task<IActionResult> Withdraw(Guid id, [FromBody] decimal amount)
        {
            await _withdraw.Handle(new WithdrawMoneyCommand { AccountId = id, Amount = amount });
            return Ok();
        }

        [HttpGet("{id}/balance")]
        public IActionResult GetBalance(Guid id)
        {
            if (_proj.Balances.TryGetValue(id, out var bal))
                return Ok(new { id, balance = bal });
            return NotFound();
        }

        [HttpGet("{id}/history")]
        public IActionResult GetHistory(Guid id)
        {
            if (_proj.History.TryGetValue(id, out var list))
                return Ok(list);

            return NotFound($"No history found for account ID: {id}");
        }
    }
}
