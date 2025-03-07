using System.Data.Common;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace ProductManagement.Application.Common;

public class RetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly ILogger<RetryBehavior<TRequest, TResponse>> _logger;

    public RetryBehavior(ILogger<RetryBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;

        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .Or<TimeoutException>()
            .Or<DbException>()
            .Or<SqlException>(IsTransientSqlError)
            .WaitAndRetryAsync(
                retryCount: 3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        exception,
                        "Retry {RetryCount} after {Delay} due to: {ExceptionMessage}",
                        retryCount, timeSpan, exception.Message);
                });
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        return await _retryPolicy.ExecuteAsync(() => next());
    }

    private static bool IsTransientSqlError(SqlException ex)
    {
        int[] transientErrors =
        [
            -1,    // Transport Level Error
            20,    // Instance Does Not Support Encryption
            64,    // Connected But Login Failed
            233,   // Unable To Establish Connection
            4060,  // Cannot open database / Database Limit Reached
            10053, // Transport Level Error Receiving Result
            10054, // Transport Level Error When Sending Request To Server
            10060, // Network Related Error During Connect
            11001, // Server Not Found Or Not Accessible
            10928, // Resource Limit Reached
            10929, // Too Many Requests
            40197, // Service restarting
            40501, // Service Busy
            40540, // Service Request Process Fail
            40545, // Service Experiencing A Problem
            40613, // Database Unavailable
            40627, // Operation In Progress
            49918, // SQL Azure throttling error
            49919, // SQL Azure throttling error
            49920  // SQL Azure throttling error
        ];

        return ex.Errors.Cast<SqlError>().Any(err => transientErrors.Contains(err.Number));
    }
}
