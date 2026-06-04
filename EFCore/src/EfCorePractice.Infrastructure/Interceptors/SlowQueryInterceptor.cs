using System.Data.Common;
using System.Text;
using EfCorePractice.Application.Abstractions;
using EfCorePractice.Application.Models;
using EfCorePractice.Application.Options;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EfCorePractice.Infrastructure.Interceptors;

/// <summary>
/// 命令拦截器链：记录超过阈值的 SQL（可与 <see cref="AuditableEntityInterceptor"/> 并存，职责不同）。
/// </summary>
public sealed class SlowQueryInterceptor(
    IOptions<SlowQueryOptions> options,
    ISlowQueryMetrics metrics,
    ILogger<SlowQueryInterceptor> logger) : DbCommandInterceptor
{
    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        LogIfSlow(command, eventData);
        return base.ReaderExecuted(command, eventData, result);
    }

    public override ValueTask<DbDataReader> ReaderExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result,
        CancellationToken cancellationToken = default)
    {
        LogIfSlow(command, eventData);
        return base.ReaderExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override int NonQueryExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result)
    {
        LogIfSlow(command, eventData);
        return base.NonQueryExecuted(command, eventData, result);
    }

    public override ValueTask<int> NonQueryExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        LogIfSlow(command, eventData);
        return base.NonQueryExecutedAsync(command, eventData, result, cancellationToken);
    }

    public override object? ScalarExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        object? result)
    {
        LogIfSlow(command, eventData);
        return base.ScalarExecuted(command, eventData, result);
    }

    public override ValueTask<object?> ScalarExecutedAsync(
        DbCommand command,
        CommandExecutedEventData eventData,
        object? result,
        CancellationToken cancellationToken = default)
    {
        LogIfSlow(command, eventData);
        return base.ScalarExecutedAsync(command, eventData, result, cancellationToken);
    }

    private void LogIfSlow(DbCommand command, CommandExecutedEventData eventData)
    {
        var threshold = options.Value.ThresholdMs;
        var durationMs = (long)eventData.Duration.TotalMilliseconds;
        if (durationMs < threshold)
        {
            return;
        }

        var contextType = eventData.Context?.GetType().Name ?? "Unknown";
        var sql = TrimSql(command.CommandText);

        var entry = new SlowQueryLogDto(
            DateTime.UtcNow,
            contextType,
            durationMs,
            threshold,
            sql);

        metrics.Record(entry);

        if (options.Value.LogParameters)
        {
            logger.LogWarning(
                "慢 SQL [{DurationMs}ms >= {ThresholdMs}ms] Context={Context} SQL={Sql} Params={Params}",
                durationMs,
                threshold,
                contextType,
                sql,
                FormatParameters(command));
        }
        else
        {
            logger.LogWarning(
                "慢 SQL [{DurationMs}ms >= {ThresholdMs}ms] Context={Context} SQL={Sql}",
                durationMs,
                threshold,
                contextType,
                sql);
        }
    }

    private static string TrimSql(string sql) =>
        sql.Length <= 2000 ? sql : sql[..2000] + "...";

    private static string FormatParameters(DbCommand command)
    {
        if (command.Parameters.Count == 0)
        {
            return "(none)";
        }

        var sb = new StringBuilder();
        foreach (DbParameter parameter in command.Parameters)
        {
            sb.Append(parameter.ParameterName).Append('=').Append(parameter.Value).Append(';');
        }

        return sb.ToString();
    }
}
