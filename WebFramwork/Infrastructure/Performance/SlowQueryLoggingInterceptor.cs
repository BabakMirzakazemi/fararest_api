using Common.Configurations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Diagnostics;

namespace WebFramework.Infrastructure.Performance;

// Logs slow SQL commands to help systematic index/query tuning.
// This is infrastructure-level observability and should be always safe (fail-open).
public class SlowQueryLoggingInterceptor(
    IOptions<PerformanceSettings> performanceOptions,
    ILogger<SlowQueryLoggingInterceptor> logger) : DbCommandInterceptor
{
    private readonly QueryTuningSettings _settings = performanceOptions.Value.QueryTuning;
    private readonly ConcurrentDictionary<Guid, Stopwatch> _timers = new();

    public override InterceptionResult<DbDataReader> ReaderExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<DbDataReader> result)
    {
        StartTiming(eventData.CommandId);
        return base.ReaderExecuting(command, eventData, result);
    }

    public override DbDataReader ReaderExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        DbDataReader result)
    {
        StopAndLogIfSlow(command, eventData);
        return base.ReaderExecuted(command, eventData, result);
    }

    public override InterceptionResult<object> ScalarExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<object> result)
    {
        StartTiming(eventData.CommandId);
        return base.ScalarExecuting(command, eventData, result);
    }

    public override object? ScalarExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        object? result)
    {
        StopAndLogIfSlow(command, eventData);
        return base.ScalarExecuted(command, eventData, result);
    }

    public override InterceptionResult<int> NonQueryExecuting(
        DbCommand command,
        CommandEventData eventData,
        InterceptionResult<int> result)
    {
        StartTiming(eventData.CommandId);
        return base.NonQueryExecuting(command, eventData, result);
    }

    public override int NonQueryExecuted(
        DbCommand command,
        CommandExecutedEventData eventData,
        int result)
    {
        StopAndLogIfSlow(command, eventData);
        return base.NonQueryExecuted(command, eventData, result);
    }

    public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
    {
        _timers.TryRemove(eventData.CommandId, out _);
        base.CommandFailed(command, eventData);
    }

    private void StartTiming(Guid commandId)
    {
        if (!_settings.Enabled)
            return;

        var sw = Stopwatch.StartNew();
        _timers[commandId] = sw;
    }

    private void StopAndLogIfSlow(DbCommand command, CommandExecutedEventData eventData)
    {
        if (!_settings.Enabled)
            return;

        if (!_timers.TryRemove(eventData.CommandId, out var sw))
            return;

        sw.Stop();
        if (sw.ElapsedMilliseconds < _settings.SlowQueryThresholdMs)
            return;

        if (_settings.LogCommandText)
        {
            logger.LogWarning(
                "Slow query detected ({ElapsedMs}ms). CommandType: {CommandType}, DataSource: {DataSource}, CommandText: {CommandText}",
                sw.ElapsedMilliseconds,
                command.CommandType,
                command.Connection?.DataSource,
                command.CommandText);
        }
        else
        {
            logger.LogWarning(
                "Slow query detected ({ElapsedMs}ms). CommandType: {CommandType}, DataSource: {DataSource}",
                sw.ElapsedMilliseconds,
                command.CommandType,
                command.Connection?.DataSource);
        }
    }
}
