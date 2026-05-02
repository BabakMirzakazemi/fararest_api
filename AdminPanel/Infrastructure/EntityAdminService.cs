using Data;
using Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace AdminPanel.Infrastructure;

public interface IEntityAdminService
{
    IReadOnlyList<ManagedEntityDescriptor> GetManagedEntities();
    ManagedEntityDescriptor GetManagedEntity(string entityName);
    int Count(string entityName);
    IReadOnlyList<object> GetPage(string entityName, int page, int pageSize, string? search);
    object? FindByKey(string entityName, string keyToken);
    string BuildKeyToken(object entity, ManagedEntityDescriptor descriptor);
    Task<Dictionary<string, List<LookupOption>>> GetForeignKeyOptionsAsync(string entityName, CancellationToken cancellationToken);
    Task<(bool Success, string Message)> SaveAsync(string entityName, string? keyToken, IDictionary<string, string?> values, CancellationToken cancellationToken);
    Task<(bool Success, string Message)> DeleteAsync(string entityName, string keyToken, CancellationToken cancellationToken);
}

public sealed class EntityAdminService : IEntityAdminService
{
    private const string CompositeKeySeparator = "||";
    private static readonly Type[] BuiltInSimpleTypes = [typeof(string), typeof(decimal), typeof(DateTime), typeof(DateTimeOffset), typeof(TimeOnly), typeof(DateOnly), typeof(Guid)];
    private static readonly MethodInfo SetMethod = typeof(DbContext).GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Single(m => m.Name == nameof(DbContext.Set) && m.IsGenericMethod && m.GetParameters().Length == 0);
    private static readonly MethodInfo AsNoTrackingMethod = typeof(EntityFrameworkQueryableExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static)
        .Single(m => m.Name == nameof(EntityFrameworkQueryableExtensions.AsNoTracking) && m.IsGenericMethod && m.GetParameters().Length == 1);
    private readonly ApplicationDbContext _db;
    private readonly IReadOnlyList<ManagedEntityDescriptor> _entities;

    public EntityAdminService(ApplicationDbContext db)
    {
        _db = db;
        _entities = BuildEntityDescriptors(_db.Model);
    }

    public IReadOnlyList<ManagedEntityDescriptor> GetManagedEntities() => _entities;

    public ManagedEntityDescriptor GetManagedEntity(string entityName)
    {
        var entity = _entities.FirstOrDefault(x => x.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase));
        if (entity is null)
        {
            throw new InvalidOperationException($"Entity '{entityName}' was not found.");
        }

        return entity;
    }

    public int Count(string entityName)
    {
        var descriptor = GetManagedEntity(entityName);
        return AsNoTracking(GetQueryable(descriptor.ClrType), descriptor.ClrType).Cast<object>().Count();
    }

    public IReadOnlyList<object> GetPage(string entityName, int page, int pageSize, string? search)
    {
        var descriptor = GetManagedEntity(entityName);
        var query = AsNoTracking(GetQueryable(descriptor.ClrType), descriptor.ClrType);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = ApplySearch(query, descriptor, search.Trim());
        }

        query = ApplyDefaultOrder(query, descriptor);

        var skip = Math.Max(page - 1, 0) * pageSize;
        return query.Cast<object>().Skip(skip).Take(pageSize).ToList();
    }

    public object? FindByKey(string entityName, string keyToken)
    {
        var descriptor = GetManagedEntity(entityName);
        var keyValues = ParseKeyToken(descriptor, keyToken);
        return _db.Find(descriptor.ClrType, keyValues);
    }

    public string BuildKeyToken(object entity, ManagedEntityDescriptor descriptor)
    {
        var keyValues = descriptor.KeyProperties.Select(k => k.PropertyInfo.GetValue(entity)).ToList();
        var encoded = keyValues.Select(v => Uri.EscapeDataString(FormatValue(v)));
        return string.Join(CompositeKeySeparator, encoded);
    }

    public async Task<Dictionary<string, List<LookupOption>>> GetForeignKeyOptionsAsync(string entityName, CancellationToken cancellationToken)
    {
        var descriptor = GetManagedEntity(entityName);
        var result = new Dictionary<string, List<LookupOption>>(StringComparer.OrdinalIgnoreCase);

        foreach (var fk in descriptor.ForeignKeys)
        {
            var principalSet = GetQueryable(fk.PrincipalEntityType);
            var entities = AsNoTracking(principalSet, fk.PrincipalEntityType).Cast<object>().Take(200).ToList();
            var options = new List<LookupOption>();
            foreach (var entity in entities)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var keyValue = fk.PrincipalKeyProperty.PropertyInfo.GetValue(entity);
                if (keyValue is null)
                {
                    continue;
                }

                options.Add(new LookupOption(FormatValue(keyValue), BuildLookupLabel(entity, fk)));
            }

            result[fk.ForeignKeyProperty.Name] = options;
        }

        await Task.CompletedTask;
        return result;
    }

    public async Task<(bool Success, string Message)> SaveAsync(string entityName, string? keyToken, IDictionary<string, string?> values, CancellationToken cancellationToken)
    {
        var descriptor = GetManagedEntity(entityName);
        var isCreate = string.IsNullOrWhiteSpace(keyToken);

        object entity;
        if (isCreate)
        {
            entity = Activator.CreateInstance(descriptor.ClrType)
                ?? throw new InvalidOperationException($"Cannot create instance for '{descriptor.Name}'.");
            _db.Add(entity);
        }
        else
        {
            var keyValues = ParseKeyToken(descriptor, keyToken!);
            entity = await FindAsync(descriptor.ClrType, keyValues, cancellationToken)
                ?? throw new InvalidOperationException("Entity instance was not found.");
        }

        var errors = new List<string>();
        foreach (var property in descriptor.EditableProperties)
        {
            var rawValue = values.TryGetValue(property.Name, out var candidate) ? candidate : null;
            if (!TryConvert(rawValue, property.PropertyType, out var convertedValue, out var convertError))
            {
                errors.Add($"{property.DisplayName}: {convertError}");
                continue;
            }

            if (convertedValue is null && property.IsRequired)
            {
                errors.Add($"{property.DisplayName}: value is required.");
                continue;
            }

            property.PropertyInfo.SetValue(entity, convertedValue);
        }

        if (errors.Count > 0)
        {
            return (false, string.Join(Environment.NewLine, errors));
        }

        var context = new ValidationContext(entity);
        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(entity, context, validationResults, validateAllProperties: true))
        {
            return (false, string.Join(Environment.NewLine, validationResults.Select(v => v.ErrorMessage)));
        }

        await _db.SaveChangesAsync(cancellationToken);
        return (true, isCreate ? "Record created successfully." : "Record updated successfully.");
    }

    public async Task<(bool Success, string Message)> DeleteAsync(string entityName, string keyToken, CancellationToken cancellationToken)
    {
        var descriptor = GetManagedEntity(entityName);
        var keyValues = ParseKeyToken(descriptor, keyToken);
        var entity = await FindAsync(descriptor.ClrType, keyValues, cancellationToken);
        if (entity is null)
        {
            return (false, "Requested record was not found.");
        }

        _db.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return (true, "Record deleted successfully.");
    }

    private static IReadOnlyList<ManagedEntityDescriptor> BuildEntityDescriptors(IModel model)
    {
        var result = new List<ManagedEntityDescriptor>();
        foreach (var entityType in model.GetEntityTypes().Where(e => typeof(IEntity).IsAssignableFrom(e.ClrType)))
        {
            var key = entityType.FindPrimaryKey();
            if (key is null)
            {
                continue;
            }

            var scalarProperties = entityType
                .GetProperties()
                .Where(p => p.PropertyInfo is not null)
                .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

            var keyProperties = key.Properties
                .Select(p => CreatePropertyDescriptor(p))
                .ToList();

            var editableProperties = scalarProperties.Values
                .Where(p => !key.Properties.Contains(p))
                .Where(IsEditable)
                .Select(CreatePropertyDescriptor)
                .ToList();

            var foreignKeys = entityType
                .GetForeignKeys()
                .Where(fk => fk.Properties.Count == 1 && fk.PrincipalKey.Properties.Count == 1)
                .Select(fk =>
                {
                    var fkProp = CreatePropertyDescriptor(fk.Properties.Single());
                    var principalProp = CreatePropertyDescriptor(fk.PrincipalKey.Properties.Single());
                    return new ForeignKeyDescriptor(
                        fkProp,
                        fk.PrincipalEntityType.ClrType,
                        principalProp,
                        GuessDisplayProperty(fk.PrincipalEntityType.ClrType));
                })
                .ToList();

            result.Add(new ManagedEntityDescriptor(
                entityType.ClrType.Name,
                entityType.ClrType.Name,
                entityType.ClrType,
                keyProperties,
                editableProperties,
                foreignKeys));
        }

        return result.OrderBy(e => e.DisplayName).ToList();
    }

    private static bool IsEditable(IProperty property)
    {
        var type = Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType;
        if (property.PropertyInfo is null)
        {
            return false;
        }

        if (property.IsShadowProperty())
        {
            return false;
        }

        if (property.ValueGenerated == ValueGenerated.OnAdd && (type == typeof(int) || type == typeof(long) || type == typeof(Guid)))
        {
            return false;
        }

        return IsSimpleType(type) || type.IsEnum;
    }

    private static ManagedPropertyDescriptor CreatePropertyDescriptor(IProperty property)
    {
        return new ManagedPropertyDescriptor(
            property.Name,
            property.PropertyInfo!,
            property.ClrType,
            property.IsNullable is false,
            property.GetMaxLength(),
            property.ClrType.IsEnum || (Nullable.GetUnderlyingType(property.ClrType)?.IsEnum ?? false),
            property.GetColumnType());
    }

    private static bool IsSimpleType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        return type.IsPrimitive || BuiltInSimpleTypes.Contains(type);
    }

    private static string FormatValue(object? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        return value switch
        {
            DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture),
            DateTimeOffset dto => dto.ToString("yyyy-MM-dd HH:mm:ss zzz", CultureInfo.InvariantCulture),
            _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty
        };
    }

    private static string BuildLookupLabel(object entity, ForeignKeyDescriptor fk)
    {
        var idValue = fk.PrincipalKeyProperty.PropertyInfo.GetValue(entity);
        var label = idValue is null ? string.Empty : FormatValue(idValue);
        if (fk.DisplayProperty is null)
        {
            return label;
        }

        var displayValue = fk.DisplayProperty.GetValue(entity);
        if (displayValue is null || string.IsNullOrWhiteSpace(displayValue.ToString()))
        {
            return label;
        }

        return $"{displayValue} ({label})";
    }

    private static PropertyInfo? GuessDisplayProperty(Type type)
    {
        var candidates = new[] { "Name", "Title", "Subject", "UserName", "FullName", "Email", "Receiver" };
        return candidates
            .Select(name => type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase))
            .FirstOrDefault(p => p is not null && p.PropertyType == typeof(string));
    }

    private static bool TryConvert(string? rawValue, Type targetType, out object? convertedValue, out string error)
    {
        error = string.Empty;
        var nullableType = Nullable.GetUnderlyingType(targetType);
        var effectiveType = nullableType ?? targetType;
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            convertedValue = nullableType is not null || targetType == typeof(string) ? null : Activator.CreateInstance(effectiveType);
            return true;
        }

        try
        {
            if (effectiveType == typeof(string))
            {
                convertedValue = rawValue;
                return true;
            }

            if (effectiveType == typeof(Guid))
            {
                convertedValue = Guid.Parse(rawValue);
                return true;
            }

            if (effectiveType == typeof(DateTime))
            {
                convertedValue = DateTime.Parse(rawValue, CultureInfo.CurrentCulture);
                return true;
            }

            if (effectiveType == typeof(DateTimeOffset))
            {
                convertedValue = DateTimeOffset.Parse(rawValue, CultureInfo.CurrentCulture);
                return true;
            }

            if (effectiveType == typeof(bool))
            {
                convertedValue = rawValue.Equals("true", StringComparison.OrdinalIgnoreCase) || rawValue.Equals("on", StringComparison.OrdinalIgnoreCase);
                return true;
            }

            if (effectiveType.IsEnum)
            {
                convertedValue = Enum.Parse(effectiveType, rawValue, true);
                return true;
            }

            convertedValue = Convert.ChangeType(rawValue, effectiveType, CultureInfo.CurrentCulture);
            return true;
        }
        catch (Exception ex)
        {
            convertedValue = null;
            error = ex.Message;
            return false;
        }
    }

    private static object[] ParseKeyToken(ManagedEntityDescriptor descriptor, string keyToken)
    {
        var segments = keyToken.Split(CompositeKeySeparator, StringSplitOptions.None)
            .Select(Uri.UnescapeDataString)
            .ToArray();

        if (segments.Length != descriptor.KeyProperties.Count)
        {
            throw new InvalidOperationException("Invalid key format.");
        }

        var keyValues = new object[descriptor.KeyProperties.Count];
        for (var i = 0; i < descriptor.KeyProperties.Count; i++)
        {
            if (!TryConvert(segments[i], descriptor.KeyProperties[i].PropertyType, out var converted, out var error))
            {
                throw new InvalidOperationException($"Key value is invalid: {error}");
            }

            keyValues[i] = converted!;
        }

        return keyValues;
    }

    private static IQueryable ApplySearch(IQueryable query, ManagedEntityDescriptor descriptor, string search)
    {
        var searchableProperties = descriptor.EditableProperties
            .Where(p => (Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType) == typeof(string))
            .ToList();

        if (searchableProperties.Count == 0)
        {
            return query;
        }

        var parameter = Expression.Parameter(descriptor.ClrType, "x");
        Expression? body = null;
        var toLowerMethod = typeof(string).GetMethod(nameof(string.ToLower), Type.EmptyTypes)!;
        var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
        var searchValueExpr = Expression.Constant(search.ToLower());

        foreach (var property in searchableProperties)
        {
            var propertyExpr = Expression.Property(parameter, property.PropertyInfo);
            var nullSafe = Expression.Coalesce(propertyExpr, Expression.Constant(string.Empty));
            var loweredExpr = Expression.Call(nullSafe, toLowerMethod);
            var containsExpr = Expression.Call(loweredExpr, containsMethod, searchValueExpr);
            body = body is null ? containsExpr : Expression.OrElse(body, containsExpr);
        }

        if (body is null)
        {
            return query;
        }

        var lambda = Expression.Lambda(body, parameter);
        var whereMethod = typeof(Queryable).GetMethods()
            .First(m => m.Name == nameof(Queryable.Where) && m.GetParameters().Length == 2)
            .MakeGenericMethod(descriptor.ClrType);

        return (IQueryable)whereMethod.Invoke(null, [query, lambda])!;
    }

    private static IQueryable ApplyDefaultOrder(IQueryable query, ManagedEntityDescriptor descriptor)
    {
        var firstKey = descriptor.KeyProperties.FirstOrDefault();
        if (firstKey is null)
        {
            return query;
        }

        var parameter = Expression.Parameter(descriptor.ClrType, "x");
        var keyExpr = Expression.Property(parameter, firstKey.PropertyInfo);
        var lambda = Expression.Lambda(keyExpr, parameter);

        var method = typeof(Queryable).GetMethods()
            .First(m => m.Name == nameof(Queryable.OrderBy) && m.GetParameters().Length == 2)
            .MakeGenericMethod(descriptor.ClrType, firstKey.PropertyType);

        return (IQueryable)method.Invoke(null, [query, lambda])!;
    }

    private IQueryable GetQueryable(Type entityType)
    {
        return (IQueryable)SetMethod.MakeGenericMethod(entityType).Invoke(_db, null)!;
    }

    private static IQueryable AsNoTracking(IQueryable query, Type entityType)
    {
        return (IQueryable)AsNoTrackingMethod.MakeGenericMethod(entityType).Invoke(null, [query])!;
    }

    private async Task<object?> FindAsync(Type entityType, object[] keyValues, CancellationToken cancellationToken)
    {
        return await _db.FindAsync(entityType, keyValues, cancellationToken);
    }
}

public sealed record ManagedEntityDescriptor(
    string Name,
    string DisplayName,
    Type ClrType,
    IReadOnlyList<ManagedPropertyDescriptor> KeyProperties,
    IReadOnlyList<ManagedPropertyDescriptor> EditableProperties,
    IReadOnlyList<ForeignKeyDescriptor> ForeignKeys);

public sealed record ManagedPropertyDescriptor(
    string Name,
    PropertyInfo PropertyInfo,
    Type PropertyType,
    bool IsRequired,
    int? MaxLength,
    bool IsEnum,
    string? ColumnType)
{
    public string DisplayName => Name;
}

public sealed record ForeignKeyDescriptor(
    ManagedPropertyDescriptor ForeignKeyProperty,
    Type PrincipalEntityType,
    ManagedPropertyDescriptor PrincipalKeyProperty,
    PropertyInfo? DisplayProperty);

public sealed record LookupOption(string Value, string Label);
