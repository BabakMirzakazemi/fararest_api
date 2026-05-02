using AdminPanel.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Pages.Operations;

public class IndexModel : PageModel
{
    private readonly IOperationCatalogService _operationCatalogService;

    public IndexModel(IOperationCatalogService operationCatalogService)
    {
        _operationCatalogService = operationCatalogService;
    }

    public IReadOnlyList<IGrouping<string, OperationMethodDescriptor>> OperationsByService { get; private set; } = [];

    public void OnGet()
    {
        OperationsByService = _operationCatalogService
            .GetOperations()
            .GroupBy(x => x.ServiceName)
            .OrderBy(x => x.Key)
            .ToList();
    }
}
