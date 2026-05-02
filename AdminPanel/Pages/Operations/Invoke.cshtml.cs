using AdminPanel.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Pages.Operations;

public class InvokeModel : PageModel
{
    private readonly IOperationCatalogService _operationCatalogService;

    public InvokeModel(IOperationCatalogService operationCatalogService)
    {
        _operationCatalogService = operationCatalogService;
    }

    [BindProperty(SupportsGet = true)]
    public string ServiceName { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string MethodName { get; set; } = string.Empty;

    public OperationMethodDescriptor Operation { get; private set; } = null!;
    public Dictionary<string, string?> Values { get; private set; } = new(StringComparer.OrdinalIgnoreCase);
    public string? ResultMessage { get; private set; }
    public bool? ResultSuccess { get; private set; }

    public IActionResult OnGet(string serviceName, string methodName)
    {
        ServiceName = serviceName;
        MethodName = methodName;
        Operation = _operationCatalogService.GetOperation(serviceName, methodName);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string serviceName, string methodName, CancellationToken cancellationToken)
    {
        ServiceName = serviceName;
        MethodName = methodName;
        Operation = _operationCatalogService.GetOperation(serviceName, methodName);

        Values = Operation.Parameters
            .Where(p => !p.IsCancellationToken)
            .ToDictionary(p => p.Name, p => (string?)Request.Form[p.Name].ToString(), StringComparer.OrdinalIgnoreCase);

        var result = await _operationCatalogService.InvokeAsync(serviceName, methodName, Values, cancellationToken);
        ResultMessage = result.Message;
        ResultSuccess = result.Success;

        return Page();
    }

    public string GetValue(string parameterName)
    {
        if (Values.TryGetValue(parameterName, out var value))
        {
            return value ?? string.Empty;
        }

        return string.Empty;
    }
}
