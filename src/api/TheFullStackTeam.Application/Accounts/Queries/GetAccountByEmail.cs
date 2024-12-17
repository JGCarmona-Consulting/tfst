using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TheFullStackTeam.Application.Accounts.Models;
using TheFullStackTeam.Domain.Events;
using TheFullStackTeam.Domain.Repositories.Command;
using TheFullStackTeam.Domain.Services;

namespace TheFullStackTeam.Application.Accounts.Queries;

public class GetAccountByEmailQuery : IRequest<AccountModel>
{
    public string Email { get; set; }
    public string ViewedByProfileId { get; set; }

    public GetAccountByEmailQuery(string accountEmail, string viewedByProfileId)
    {
        Email = accountEmail;
        ViewedByProfileId = viewedByProfileId;
    }
}

public class GetAccountByEmailQueryHandler : IRequestHandler<GetAccountByEmailQuery, AccountModel>
{
    private readonly IAccountCommandRepository _accountRepository;
    private readonly IEventDispatcher _eventDispatcher;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<GetAccountByEmailQueryHandler> _logger;

    public GetAccountByEmailQueryHandler(
        IAccountCommandRepository accountRepository,
        ILogger<GetAccountByEmailQueryHandler> logger,
        IEventDispatcher eventDispatcher,
        IHttpContextAccessor httpContextAccessor)
    {
        _accountRepository = accountRepository;
        _eventDispatcher = eventDispatcher;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public async Task<AccountModel> Handle(GetAccountByEmailQuery request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetByEmailAsync(request.Email);
        if (account == null)
        {
            throw new Exception($"No account found for email: {request.Email}");
        }

        var correlationId = _httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString() ?? string.Empty;

        // Dispatch the event
        var accountViewedEvent = new AccountViewedEvent(account.Id.ToString(), request.ViewedByProfileId)
        {
            CorrelationId = correlationId
        };

        await _eventDispatcher.DispatchAsync(accountViewedEvent);

        return AccountModel.FromEntity(account);
    }
}