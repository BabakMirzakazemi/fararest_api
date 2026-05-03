using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Data.Database.SqlObjects;

public static class DatabaseSqlObjectsInstaller
{
    private static readonly string[] ScriptApplyOrder =
    [
        "accounts_functions.sql",
        "accounts_triggers.sql",
        "phase5_db_objects_up.sql"
    ];

    public static async Task ApplyAsync(ApplicationDbContext dbContext, CancellationToken cancellationToken = default)
    {
        // SQL object scripts are PostgreSQL-specific (functions/triggers/views/sequences).
        var providerName = dbContext.Database.ProviderName;
        if (providerName is null || !providerName.Contains("Npgsql", StringComparison.OrdinalIgnoreCase))
            return;

        foreach (var script in ScriptApplyOrder)
        {
            var sql = ReadEmbeddedSql(script);
            if (string.IsNullOrWhiteSpace(sql))
                continue;

            sql = NormalizeSql(sql);
            await ExecuteSqlScriptAsync(dbContext, sql, cancellationToken);
        }
    }

    private static string ReadEmbeddedSql(string fileName)
    {
        var assembly = typeof(DatabaseSqlObjectsInstaller).Assembly;
        var resourceName = $"Data.Migrations.Sql.{fileName}";
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Embedded SQL resource not found: {resourceName}");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static async Task ExecuteSqlScriptAsync(ApplicationDbContext dbContext, string sql, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            await using var command = dbContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = sql;
            command.Transaction = transaction.GetDbTransaction();

            if (command.Connection?.State != System.Data.ConnectionState.Open)
                await command.Connection!.OpenAsync(cancellationToken);

            await command.ExecuteNonQueryAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        });
    }

    private static string NormalizeSql(string sql)
    {
        // Ensure function blocks are statement-terminated.
        sql = Regex.Replace(sql, @"(?m)^\$function\$\s*$", "$function$;");

        // Make one-line trigger creation idempotent to support repeated startup execution.
        sql = Regex.Replace(
            sql,
            @"(?im)^\s*CREATE\s+TRIGGER\s+([a-zA-Z0-9_]+)\s+.*?\s+ON\s+([a-zA-Z0-9_.]+)\s+FOR\s+EACH\s+ROW\s+EXECUTE\s+FUNCTION\s+.*?;\s*$",
            m => $"DROP TRIGGER IF EXISTS {m.Groups[1].Value} ON {m.Groups[2].Value};{Environment.NewLine}{m.Value}");

        return sql;
    }
}
