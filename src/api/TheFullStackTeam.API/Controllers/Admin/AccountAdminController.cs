using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TheFullStackTeam.Application.Accounts.Models;
using TheFullStackTeam.Application.Admin.Commands;
using TheFullStackTeam.Application.Admin.Queries;
using TheFullStackTeam.Common.Constants;

namespace TheFullStackTeam.API.Controllers.Admin;

[ApiExplorerSettings(GroupName = UserRoles.Admin)]
[Area(UserRoles.Admin)]
[Route("admin/account")]
[ApiController]
[Authorize(Roles = UserRoles.Admin)]
public class AccountAdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AccountAdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("")]
    public async Task<ActionResult<List<AccountModel>>> GetAllAccounts()
    {
        var accounts = await _mediator.Send(new GetAllUsersQuery());
        return Ok(accounts);
    }

    [HttpPut("{accountId}/enable")]
    public async Task<ActionResult<AccountModel>> EnableAccount(string accountId)
    {
        var account = await _mediator.Send(new EnableAccountCommand(accountId));
        return Ok(account);
    }

    [HttpDelete("{accountId}")]
    public async Task<IActionResult> DeleteAccount(string accountId)
    {
        await _mediator.Send(new DeleteUserCommand(accountId));
        return NoContent();
    }

    [HttpPut("{accountId}/assign-role/{role}")]
    public async Task<ActionResult<AccountModel>> AssignRole(string accountId, string role)
    {
        var account = await _mediator.Send(new AssignRoleCommand(accountId, role));
        return Ok(account);
    }
}
