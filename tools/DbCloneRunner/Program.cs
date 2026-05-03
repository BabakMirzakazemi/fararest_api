using Npgsql;

const string host = "localhost";
const int port = 5432;
const string user = "postgres";
const string password = "123456";
const string sourceDb = "fararest_refactor";
const string targetDb = "fararest_refactor_dev";

if (args.Length > 0 && string.Equals(args[0], "list", StringComparison.OrdinalIgnoreCase))
{
    var sourceConnectionString = $"Host={host};Port={port};Database={sourceDb};Username={user};Password={password};Pooling=true;Trust Server Certificate=true;";
    await using var sourceConn = new NpgsqlConnection(sourceConnectionString);
    await sourceConn.OpenAsync();

    const string sql = """
                       SELECT table_name
                       FROM information_schema.tables
                       WHERE table_schema = 'public'
                       ORDER BY table_name;
                       """;
    await using var cmd = new NpgsqlCommand(sql, sourceConn);
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
        Console.WriteLine(reader.GetString(0));

    return;
}

if (args.Length > 1 && string.Equals(args[0], "columns", StringComparison.OrdinalIgnoreCase))
{
    var table = args[1];
    var sourceConnectionString = $"Host={host};Port={port};Database={sourceDb};Username={user};Password={password};Pooling=true;Trust Server Certificate=true;";
    await using var sourceConn = new NpgsqlConnection(sourceConnectionString);
    await sourceConn.OpenAsync();

    const string sql = """
                       SELECT column_name, data_type, is_nullable
                       FROM information_schema.columns
                       WHERE table_schema = 'public' AND table_name = @table
                       ORDER BY ordinal_position;
                       """;
    await using var cmd = new NpgsqlCommand(sql, sourceConn);
    cmd.Parameters.AddWithValue("table", table);
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
        Console.WriteLine($"{reader.GetString(0)} | {reader.GetString(1)} | nullable={reader.GetString(2)}");

    return;
}

if (args.Length > 1 && string.Equals(args[0], "viewdef", StringComparison.OrdinalIgnoreCase))
{
    var view = args[1];
    var sourceConnectionString = $"Host={host};Port={port};Database={sourceDb};Username={user};Password={password};Pooling=true;Trust Server Certificate=true;";
    await using var sourceConn = new NpgsqlConnection(sourceConnectionString);
    await sourceConn.OpenAsync();

    const string sql = """
                       SELECT pg_get_viewdef(quote_ident(schemaname) || '.' || quote_ident(viewname), true)
                       FROM pg_views
                       WHERE schemaname = 'public' AND viewname = @view;
                       """;
    await using var cmd = new NpgsqlCommand(sql, sourceConn);
    cmd.Parameters.AddWithValue("view", view);
    var def = await cmd.ExecuteScalarAsync();
    Console.WriteLine(def?.ToString() ?? "<not-found>");
    return;
}

if (args.Length > 1 && string.Equals(args[0], "functions", StringComparison.OrdinalIgnoreCase))
{
    var prefix = args[1];
    var sourceConnectionString = $"Host={host};Port={port};Database={sourceDb};Username={user};Password={password};Pooling=true;Trust Server Certificate=true;";
    await using var sourceConn = new NpgsqlConnection(sourceConnectionString);
    await sourceConn.OpenAsync();

    const string sql = """
                       SELECT proname
                       FROM pg_proc p
                       JOIN pg_namespace n ON n.oid = p.pronamespace
                       WHERE n.nspname = 'public' AND proname LIKE @prefix || '%'
                       ORDER BY proname;
                       """;
    await using var cmd = new NpgsqlCommand(sql, sourceConn);
    cmd.Parameters.AddWithValue("prefix", prefix);
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
        Console.WriteLine(reader.GetString(0));
    return;
}

if (args.Length > 1 && string.Equals(args[0], "functiondef", StringComparison.OrdinalIgnoreCase))
{
    var functionName = args[1];
    var sourceConnectionString = $"Host={host};Port={port};Database={sourceDb};Username={user};Password={password};Pooling=true;Trust Server Certificate=true;";
    await using var sourceConn = new NpgsqlConnection(sourceConnectionString);
    await sourceConn.OpenAsync();

    const string sql = """
                       SELECT pg_get_functiondef(p.oid)
                       FROM pg_proc p
                       JOIN pg_namespace n ON n.oid = p.pronamespace
                       WHERE n.nspname = 'public' AND p.proname = @functionName
                       ORDER BY p.oid
                       LIMIT 1;
                       """;
    await using var cmd = new NpgsqlCommand(sql, sourceConn);
    cmd.Parameters.AddWithValue("functionName", functionName);
    var def = await cmd.ExecuteScalarAsync();
    Console.WriteLine(def?.ToString() ?? "<not-found>");
    return;
}

if (args.Length > 1 && string.Equals(args[0], "triggers", StringComparison.OrdinalIgnoreCase))
{
    var prefix = args[1];
    var sourceConnectionString = $"Host={host};Port={port};Database={sourceDb};Username={user};Password={password};Pooling=true;Trust Server Certificate=true;";
    await using var sourceConn = new NpgsqlConnection(sourceConnectionString);
    await sourceConn.OpenAsync();

    const string sql = """
                       SELECT trigger_name, event_object_table
                       FROM information_schema.triggers
                       WHERE trigger_schema = 'public' AND event_object_table LIKE @prefix || '%'
                       ORDER BY event_object_table, trigger_name;
                       """;
    await using var cmd = new NpgsqlCommand(sql, sourceConn);
    cmd.Parameters.AddWithValue("prefix", prefix);
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
        Console.WriteLine($"{reader.GetString(0)} | {reader.GetString(1)}");
    return;
}

if (args.Length > 1 && string.Equals(args[0], "triggerdef", StringComparison.OrdinalIgnoreCase))
{
    var triggerName = args[1];
    var sourceConnectionString = $"Host={host};Port={port};Database={sourceDb};Username={user};Password={password};Pooling=true;Trust Server Certificate=true;";
    await using var sourceConn = new NpgsqlConnection(sourceConnectionString);
    await sourceConn.OpenAsync();

    const string sql = """
                       SELECT pg_get_triggerdef(t.oid, true)
                       FROM pg_trigger t
                       JOIN pg_class c ON c.oid = t.tgrelid
                       JOIN pg_namespace n ON n.oid = c.relnamespace
                       WHERE n.nspname = 'public' AND t.tgname = @triggerName
                       ORDER BY t.oid
                       LIMIT 1;
                       """;
    await using var cmd = new NpgsqlCommand(sql, sourceConn);
    cmd.Parameters.AddWithValue("triggerName", triggerName);
    var def = await cmd.ExecuteScalarAsync();
    Console.WriteLine(def?.ToString() ?? "<not-found>");
    return;
}

var adminConnectionString = $"Host={host};Port={port};Database=postgres;Username={user};Password={password};Pooling=true;Trust Server Certificate=true;";

await using var conn = new NpgsqlConnection(adminConnectionString);
await conn.OpenAsync();

async Task ExecAsync(string sql)
{
    await using var cmd = new NpgsqlCommand(sql, conn);
    await cmd.ExecuteNonQueryAsync();
}

await ExecAsync($"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = '{targetDb}';");
await ExecAsync($"SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = '{sourceDb}' AND pid <> pg_backend_pid();");
await ExecAsync($"DROP DATABASE IF EXISTS {targetDb};");
await ExecAsync($"CREATE DATABASE {targetDb} TEMPLATE {sourceDb};");

Console.WriteLine($"Cloned '{sourceDb}' into '{targetDb}'.");
