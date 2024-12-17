using MediatR;
using Serilog.Context;

namespace TheFullStackTeam.API.Behaviors;
public class RequestLoggingPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RequestLoggingPipelineBehavior(ILogger<RequestLoggingPipelineBehavior<TRequest, TResponse>> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var correlationId = _httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString();
        var requestName = typeof(TRequest).Name;

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            _logger.LogInformation("Handling {RequestName} with CorrelationId: {CorrelationId}", requestName, correlationId);

            var response = await next();

            _logger.LogInformation("Handled {RequestName} with CorrelationId: {CorrelationId}", requestName, correlationId);
            return response;
        }
    }
}
