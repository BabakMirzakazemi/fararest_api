using System.Globalization;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace AdminPanel.Infrastructure;

public interface IOperationCatalogService
{
    IReadOnlyList<OperationMethodDescriptor> GetOperations();
    OperationMethodDescriptor GetOperation(string serviceName, string methodName);
    Task<OperationInvokeResult> InvokeAsync(string serviceName, string methodName, IDictionary<string, string?> values, CancellationToken cancellationToken);
}

public sealed class OperationCatalogService : IOperationCatalogService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IReadOnlyList<OperationMethodDescriptor> _operations;
    private readonly OperationCatalogSecuritySettings _securitySettings;
    private readonly ILogger<OperationCatalogService> _logger;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true, WriteIndented = true };

    public OperationCatalogService(
        IServiceProvider serviceProvider,
        IOptions<AdminPanelSecuritySettings> securityOptions,
        ILogger<OperationCatalogService> logger)
    {
        _serviceProvider = serviceProvider;
        _securitySettings = securityOptions.Value.OperationCatalog;
        _logger = logger;

        if (!_securitySettings.Enabled)
        {
            _operations = [];
            return;
        }

        _operations = DiscoverOperations()
            .Where(IsOperationAllowedByPolicy)
            .Where(IsOperationSafeShape)
            .Where(IsServiceResolvable)
            .ToList();
    }

    public IReadOnlyList<OperationMethodDescriptor> GetOperations() => _operations;

    public OperationMethodDescriptor GetOperation(string serviceName, string methodName)
    {
        var operation = _operations.FirstOrDefault(x =>
            x.ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase) &&
            x.MethodName.Equals(methodName, StringComparison.OrdinalIgnoreCase));

        if (operation is null)
        {
            throw new InvalidOperationException("Service operation not found.");
        }

        return operation;
    }

    public async Task<OperationInvokeResult> InvokeAsync(string serviceName, string methodName, IDictionary<string, string?> values, CancellationToken cancellationToken)
    {
        if (!_securitySettings.Enabled)
            return OperationInvokeResult.Error("Operation catalog is disabled by security policy.");

        var operation = GetOperation(serviceName, methodName);
        var service = _serviceProvider.GetService(operation.ServiceType);
        if (service is null)
        {
            return OperationInvokeResult.Error($"Service '{operation.ServiceName}' is not registered in DI.");
        }

        if (!IsOperationSafeShape(operation))
            return OperationInvokeResult.Error("Selected operation is blocked by security policy.");

        var arguments = new object?[operation.Parameters.Count];
        var errors = new List<string>();

        for (var i = 0; i < operation.Parameters.Count; i++)
        {
            var parameter = operation.Parameters[i];
            if (parameter.IsCancellationToken)
            {
                arguments[i] = cancellationToken;
                continue;
            }

            var raw = values.TryGetValue(parameter.Name, out var candidate) ? candidate : null;
            if (!string.IsNullOrWhiteSpace(raw) && raw.Length > _securitySettings.MaxParameterLength)
            {
                errors.Add($"{parameter.Name}: input is larger than allowed max length ({_securitySettings.MaxParameterLength}).");
                continue;
            }

            if (!TryConvert(raw, parameter.ParameterType, out var converted, out var error))
            {
                errors.Add($"{parameter.Name}: {error}");
                continue;
            }

            if (converted is null && parameter.IsRequired)
            {
                errors.Add($"{parameter.Name}: value is required.");
                continue;
            }

            arguments[i] = converted;
        }

        if (errors.Count > 0)
        {
            return OperationInvokeResult.Error(string.Join(Environment.NewLine, errors));
        }

        try
        {
            var invokeResult = operation.MethodInfo.Invoke(service, arguments);
            if (invokeResult is Task task)
            {
                await task.ConfigureAwait(false);
                object? taskResult = null;
                var taskType = task.GetType();
                if (taskType.IsGenericType)
                {
                    taskResult = taskType.GetProperty("Result")?.GetValue(task);
                }

                return OperationInvokeResult.Ok(SerializeResult(taskResult));
            }

            return OperationInvokeResult.Ok(SerializeResult(invokeResult));
        }
        catch (TargetInvocationException ex)
        {
            _logger.LogWarning(ex.InnerException ?? ex, "Operation invocation failed for {ServiceName}.{MethodName}", serviceName, methodName);
            return OperationInvokeResult.Error(ex.InnerException?.Message ?? ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Operation invocation failed for {ServiceName}.{MethodName}", serviceName, methodName);
            return OperationInvokeResult.Error(ex.Message);
        }
    }

    private static IReadOnlyList<OperationMethodDescriptor> DiscoverOperations()
    {
        var contractAssembly = typeof(Services.Contracts.Notifiers.ISenderService).Assembly;
        var serviceTypes = contractAssembly
            .GetTypes()
            .Where(t => t.IsInterface && t.IsPublic && t.Namespace is not null && t.Namespace.StartsWith("Services.Contracts", StringComparison.Ordinal))
            .OrderBy(t => t.Name)
            .ToList();

        var operations = new List<OperationMethodDescriptor>();
        foreach (var serviceType in serviceTypes)
        {
            var methods = serviceType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => !m.IsSpecialName)
                .OrderBy(m => m.Name)
                .ToList();

            foreach (var method in methods)
            {
                var parameters = method.GetParameters()
                    .Select(p => new OperationParameterDescriptor(
                        p.Name ?? "arg",
                        p.ParameterType,
                        p.ParameterType == typeof(CancellationToken),
                        IsRequired(p.ParameterType)))
                    .ToList();

                operations.Add(new OperationMethodDescriptor(
                    serviceType.Name.TrimStart('I'),
                    serviceType,
                    method.Name,
                    method,
                    parameters));
            }
        }

        return operations;
    }

    private bool IsServiceResolvable(OperationMethodDescriptor operation)
    {
        try
        {
            return _serviceProvider.GetService(operation.ServiceType) is not null;
        }
        catch
        {
            return false;
        }
    }

    private bool IsOperationAllowedByPolicy(OperationMethodDescriptor operation)
    {
        var allowedServices = new HashSet<string>(
            _securitySettings.AllowedServiceContracts.Where(x => !string.IsNullOrWhiteSpace(x)),
            StringComparer.OrdinalIgnoreCase);

        var allowedOperations = new HashSet<string>(
            _securitySettings.AllowedOperationKeys.Where(x => !string.IsNullOrWhiteSpace(x)),
            StringComparer.OrdinalIgnoreCase);

        if (allowedServices.Count == 0 && allowedOperations.Count == 0)
            return false;

        var serviceContractName = operation.ServiceType.Name;
        var operationKey = $"{serviceContractName}.{operation.MethodName}";

        if (allowedOperations.Count > 0)
            return allowedOperations.Contains(operationKey);

        return allowedServices.Contains(serviceContractName);
    }

    private static bool IsOperationSafeShape(OperationMethodDescriptor operation)
    {
        if (operation.MethodInfo.IsGenericMethodDefinition)
            return false;

        var parameters = operation.MethodInfo.GetParameters();
        return parameters.All(p => !p.IsOut && !p.ParameterType.IsByRef);
    }

    private static string SerializeResult(object? result)
    {
        if (result is null)
        {
            return "Operation completed successfully.";
        }

        if (result is string text)
        {
            return text;
        }

        return JsonSerializer.Serialize(result, JsonOptions);
    }

    private static bool IsRequired(Type parameterType)
    {
        var underlying = Nullable.GetUnderlyingType(parameterType);
        if (underlying is not null)
        {
            return false;
        }

        return parameterType.IsValueType || parameterType == typeof(string);
    }

    private static bool TryConvert(string? rawValue, Type targetType, out object? convertedValue, out string error)
    {
        error = string.Empty;
        var nullableType = Nullable.GetUnderlyingType(targetType);
        var effectiveType = nullableType ?? targetType;
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            convertedValue = nullableType is null && effectiveType.IsValueType ? Activator.CreateInstance(effectiveType) : null;
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

            if (effectiveType == typeof(TimeOnly))
            {
                convertedValue = TimeOnly.Parse(rawValue, CultureInfo.CurrentCulture);
                return true;
            }

            if (effectiveType == typeof(DateOnly))
            {
                convertedValue = DateOnly.Parse(rawValue, CultureInfo.CurrentCulture);
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

            if (!effectiveType.IsPrimitive && effectiveType != typeof(decimal))
            {
                convertedValue = JsonSerializer.Deserialize(rawValue, effectiveType, JsonOptions);
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
}

public sealed record OperationMethodDescriptor(
    string ServiceName,
    Type ServiceType,
    string MethodName,
    MethodInfo MethodInfo,
    IReadOnlyList<OperationParameterDescriptor> Parameters)
{
    public string UniqueKey => $"{ServiceName}.{MethodName}";
}

public sealed record OperationParameterDescriptor(
    string Name,
    Type ParameterType,
    bool IsCancellationToken,
    bool IsRequired)
{
    public bool IsEnum => (Nullable.GetUnderlyingType(ParameterType) ?? ParameterType).IsEnum;
    public Type EffectiveType => Nullable.GetUnderlyingType(ParameterType) ?? ParameterType;
    public bool IsComplexJsonInput => !EffectiveType.IsPrimitive && EffectiveType != typeof(decimal) && EffectiveType != typeof(string) && EffectiveType != typeof(Guid) && EffectiveType != typeof(DateTime) && EffectiveType != typeof(DateTimeOffset) && EffectiveType != typeof(TimeOnly) && EffectiveType != typeof(DateOnly) && !EffectiveType.IsEnum;
}

public sealed record OperationInvokeResult(bool Success, string Message)
{
    public static OperationInvokeResult Ok(string message) => new(true, message);
    public static OperationInvokeResult Error(string message) => new(false, message);
}
