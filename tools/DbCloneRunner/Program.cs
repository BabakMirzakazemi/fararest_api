using System.Text.RegularExpressions;
using Npgsql;

const string host = "localhost";
const int port = 5432;
const string user = "postgres";
const string password = "123456";
const string sourceDb = "fararest_refactor";
const string targetDb = "fararest_refactor_dev";

string Cs(string db) => $"Host={host};Port={port};Database={db};Username={user};Password={password};Pooling=true;Trust Server Certificate=true;";

async Task<HashSet<string>> GetTablesAsync(string db)
{
    const string sql = """
                       SELECT table_name
                       FROM information_schema.tables
                       WHERE table_schema = 'public' AND table_type='BASE TABLE'
                       ORDER BY table_name;
                       """;
    await using var conn = new NpgsqlConnection(Cs(db));
    await conn.OpenAsync();
    await using var cmd = new NpgsqlCommand(sql, conn);
    await using var r = await cmd.ExecuteReaderAsync();
    var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    while (await r.ReadAsync()) set.Add(r.GetString(0));
    return set;
}

async Task<HashSet<string>> GetFunctionsAsync(string db)
{
    const string sql = """
                       SELECT p.proname
                       FROM pg_proc p
                       JOIN pg_namespace n ON n.oid = p.pronamespace
                       WHERE n.nspname = 'public'
                       ORDER BY p.proname;
                       """;
    await using var conn = new NpgsqlConnection(Cs(db));
    await conn.OpenAsync();
    await using var cmd = new NpgsqlCommand(sql, conn);
    await using var r = await cmd.ExecuteReaderAsync();
    var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    while (await r.ReadAsync()) set.Add(r.GetString(0));
    return set;
}

async Task<HashSet<string>> GetTriggersAsync(string db)
{
    const string sql = """
                       SELECT t.tgname || '|' || c.relname
                       FROM pg_trigger t
                       JOIN pg_class c ON c.oid = t.tgrelid
                       JOIN pg_namespace n ON n.oid = c.relnamespace
                       WHERE n.nspname='public' AND NOT t.tgisinternal
                       ORDER BY c.relname, t.tgname;
                       """;
    await using var conn = new NpgsqlConnection(Cs(db));
    await conn.OpenAsync();
    await using var cmd = new NpgsqlCommand(sql, conn);
    await using var r = await cmd.ExecuteReaderAsync();
    var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    while (await r.ReadAsync()) set.Add(r.GetString(0));
    return set;
}

async Task<List<string>> GetMissingFunctionDefsAsync()
{
    const string sql = """
                       SELECT pg_get_functiondef(p.oid)
                       FROM pg_proc p
                       JOIN pg_namespace n ON n.oid = p.pronamespace
                       WHERE n.nspname='public'
                       ORDER BY p.proname, p.oid;
                       """;
    var target = await GetFunctionsAsync(targetDb);
    await using var conn = new NpgsqlConnection(Cs(sourceDb));
    await conn.OpenAsync();
    await using var cmd = new NpgsqlCommand(sql, conn);
    await using var r = await cmd.ExecuteReaderAsync();
    var defs = new List<string>();
    while (await r.ReadAsync())
    {
        var def = r.GetString(0);
        var m = Regex.Match(def, @"FUNCTION\s+[^.]+\.([a-zA-Z0-9_]+)\s*\(", RegexOptions.IgnoreCase);
        if (m.Success && !target.Contains(m.Groups[1].Value)) defs.Add(def.TrimEnd() + ";");
    }
    return defs;
}

async Task<List<string>> GetMissingTriggerDefsAsync()
{
    const string sql = """
                       SELECT pg_get_triggerdef(t.oid, true)
                       FROM pg_trigger t
                       JOIN pg_class c ON c.oid = t.tgrelid
                       JOIN pg_namespace n ON n.oid = c.relnamespace
                       WHERE n.nspname='public' AND NOT t.tgisinternal
                       ORDER BY c.relname, t.tgname;
                       """;
    var target = await GetTriggersAsync(targetDb);
    await using var conn = new NpgsqlConnection(Cs(sourceDb));
    await conn.OpenAsync();
    await using var cmd = new NpgsqlCommand(sql, conn);
    await using var r = await cmd.ExecuteReaderAsync();
    var defs = new List<string>();
    while (await r.ReadAsync())
    {
        var def = r.GetString(0).TrimEnd(';');
        var nameMatch = Regex.Match(def, @"CREATE\s+TRIGGER\s+([a-zA-Z0-9_]+)", RegexOptions.IgnoreCase);
        var tableMatch = Regex.Match(def, @"ON\s+([a-zA-Z0-9_\.""]+)", RegexOptions.IgnoreCase);
        if (!nameMatch.Success || !tableMatch.Success) continue;
        var key = $"{nameMatch.Groups[1].Value}|{tableMatch.Groups[1].Value.Replace("public.", string.Empty).Replace("\"", string.Empty)}";
        if (target.Contains(key)) continue;
        defs.Add($"DROP TRIGGER IF EXISTS {nameMatch.Groups[1].Value} ON {tableMatch.Groups[1].Value};\n{def};");
    }
    return defs;
}

if (args.Length > 0 && string.Equals(args[0], "compare-summary", StringComparison.OrdinalIgnoreCase))
{
    var sTables = await GetTablesAsync(sourceDb);
    var tTables = await GetTablesAsync(targetDb);
    var sFuncs = await GetFunctionsAsync(sourceDb);
    var tFuncs = await GetFunctionsAsync(targetDb);
    var sTrgs = await GetTriggersAsync(sourceDb);
    var tTrgs = await GetTriggersAsync(targetDb);

    Console.WriteLine($"tables: source={sTables.Count} target={tTables.Count}");
    Console.WriteLine($"functions: source={sFuncs.Count} target={tFuncs.Count}");
    Console.WriteLine($"triggers: source={sTrgs.Count} target={tTrgs.Count}");
    return;
}

if (args.Length > 1 && string.Equals(args[0], "columns", StringComparison.OrdinalIgnoreCase))
{
    var table = args[1];
    const string sql = """
                       SELECT c.column_name, c.data_type, c.is_nullable, c.column_default
                       FROM information_schema.columns c
                       WHERE c.table_schema='public' AND c.table_name=@table
                       ORDER BY c.ordinal_position;
                       """;
    await using var conn = new NpgsqlConnection(Cs(sourceDb));
    await conn.OpenAsync();
    await using var cmd = new NpgsqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("table", table);
    await using var r = await cmd.ExecuteReaderAsync();
    while (await r.ReadAsync())
        Console.WriteLine($"{r.GetString(0)}|{r.GetString(1)}|{r.GetString(2)}|{(r.IsDBNull(3) ? "" : r.GetString(3))}");
    return;
}

if (args.Length > 1 && string.Equals(args[0], "functiondefs", StringComparison.OrdinalIgnoreCase))
{
    var prefix = args[1];
    const string sql = """
                       SELECT pg_get_functiondef(p.oid)
                       FROM pg_proc p
                       JOIN pg_namespace n ON n.oid = p.pronamespace
                       WHERE n.nspname='public' AND p.proname LIKE @prefix || '%'
                       ORDER BY p.proname, p.oid;
                       """;
    await using var conn = new NpgsqlConnection(Cs(sourceDb));
    await conn.OpenAsync();
    await using var cmd = new NpgsqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("prefix", prefix);
    await using var r = await cmd.ExecuteReaderAsync();
    while (await r.ReadAsync())
    {
        Console.WriteLine(r.GetString(0) + ";");
        Console.WriteLine();
    }
    return;
}

if (args.Length > 1 && string.Equals(args[0], "triggerdefs", StringComparison.OrdinalIgnoreCase))
{
    var prefix = args[1];
    const string sql = """
                       SELECT pg_get_triggerdef(t.oid, true)
                       FROM pg_trigger t
                       JOIN pg_class c ON c.oid = t.tgrelid
                       JOIN pg_namespace n ON n.oid = c.relnamespace
                       WHERE n.nspname='public' AND NOT t.tgisinternal AND c.relname LIKE @prefix || '%'
                       ORDER BY c.relname, t.tgname;
                       """;
    await using var conn = new NpgsqlConnection(Cs(sourceDb));
    await conn.OpenAsync();
    await using var cmd = new NpgsqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("prefix", prefix);
    await using var r = await cmd.ExecuteReaderAsync();
    while (await r.ReadAsync())
        Console.WriteLine(r.GetString(0) + ";");
    return;
}

if (args.Length > 0 && string.Equals(args[0], "compare-missing", StringComparison.OrdinalIgnoreCase))
{
    var missingTables = (await GetTablesAsync(sourceDb)).Except(await GetTablesAsync(targetDb), StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();
    var missingFuncs = (await GetFunctionsAsync(sourceDb)).Except(await GetFunctionsAsync(targetDb), StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();
    var missingTrgs = (await GetTriggersAsync(sourceDb)).Except(await GetTriggersAsync(targetDb), StringComparer.OrdinalIgnoreCase).OrderBy(x => x).ToList();

    Console.WriteLine("[missing tables]");
    foreach (var x in missingTables) Console.WriteLine(x);
    Console.WriteLine("[missing functions]");
    foreach (var x in missingFuncs) Console.WriteLine(x);
    Console.WriteLine("[missing triggers]");
    foreach (var x in missingTrgs) Console.WriteLine(x);
    return;
}

if (args.Length > 0 && string.Equals(args[0], "sync-missing-func-trg", StringComparison.OrdinalIgnoreCase))
{
    var functionDefs = await GetMissingFunctionDefsAsync();
    var triggerDefs = await GetMissingTriggerDefsAsync();

    await using var conn = new NpgsqlConnection(Cs(targetDb));
    await conn.OpenAsync();
    await using var tx = await conn.BeginTransactionAsync();

    // Safety base requirements for helper functions exposed by source extensions.
    await using (var pre = new NpgsqlCommand("CREATE EXTENSION IF NOT EXISTS pgcrypto; CREATE EXTENSION IF NOT EXISTS btree_gist;", conn, tx))
        await pre.ExecuteNonQueryAsync();

    foreach (var def in functionDefs)
    {
        await using var cmd = new NpgsqlCommand(def, conn, tx);
        await cmd.ExecuteNonQueryAsync();
    }

    foreach (var def in triggerDefs)
    {
        await using var cmd = new NpgsqlCommand(def, conn, tx);
        await cmd.ExecuteNonQueryAsync();
    }

    await tx.CommitAsync();
    Console.WriteLine($"synced missing functions={functionDefs.Count}, triggers={triggerDefs.Count}");
    return;
}

if (args.Length > 0 && string.Equals(args[0], "clone", StringComparison.OrdinalIgnoreCase))
{
    var adminConnectionString = Cs("postgres");
    await using var admin = new NpgsqlConnection(adminConnectionString);
    await admin.OpenAsync();

    async Task ExecAsync(string sql)
    {
        await using var cmd = new NpgsqlCommand(sql, admin);
        await cmd.ExecuteNonQueryAsync();
    }

    await ExecAsync($"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = '{targetDb}';");
    await ExecAsync($"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = '{sourceDb}' AND pid <> pg_backend_pid();");
    await ExecAsync($"DROP DATABASE IF EXISTS {targetDb};");
    await ExecAsync($"CREATE DATABASE {targetDb} TEMPLATE {sourceDb};");

    Console.WriteLine($"Cloned '{sourceDb}' into '{targetDb}'.");
    return;
}

Console.WriteLine("Unknown command. Use: compare-summary | compare-missing | sync-missing-func-trg | clone");
