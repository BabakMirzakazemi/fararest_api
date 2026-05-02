using AdminPanel.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Pages.Entities;

public class ListModel : PageModel
{
    private readonly IEntityAdminService _entityAdminService;
    private const int PageSize = 20;

    public ListModel(IEntityAdminService entityAdminService)
    {
        _entityAdminService = entityAdminService;
    }

    public ManagedEntityDescriptor Entity { get; private set; } = null!;
    public IReadOnlyList<object> Rows { get; private set; } = [];
    public IReadOnlyList<ManagedPropertyDescriptor> Columns { get; private set; } = [];

    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public int PageNumber { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public string EntityName { get; set; } = string.Empty;

    public int TotalRecords { get; private set; }
    public int TotalPages => Math.Max((int)Math.Ceiling((double)TotalRecords / PageSize), 1);

    public IActionResult OnGet(string entityName)
    {
        EntityName = entityName;
        if (string.IsNullOrWhiteSpace(entityName))
        {
            return RedirectToPage("/Entities/Index");
        }

        Entity = _entityAdminService.GetManagedEntity(entityName);
        TotalRecords = _entityAdminService.Count(entityName);
        PageNumber = Math.Clamp(PageNumber, 1, TotalPages);
        Rows = _entityAdminService.GetPage(entityName, PageNumber, PageSize, Search);
        Columns = BuildColumns(Entity);
        return Page();
    }

    public string BuildKey(object row) => _entityAdminService.BuildKeyToken(row, Entity);

    public string GetCell(object row, ManagedPropertyDescriptor property)
    {
        var value = property.PropertyInfo.GetValue(row);
        if (value is null)
        {
            return "-";
        }

        return value switch
        {
            DateTime dt => dt.ToString("yyyy-MM-dd HH:mm"),
            DateTimeOffset dto => dto.ToString("yyyy-MM-dd HH:mm zzz"),
            _ => value.ToString() ?? "-"
        };
    }

    private static IReadOnlyList<ManagedPropertyDescriptor> BuildColumns(ManagedEntityDescriptor entity)
    {
        var result = new List<ManagedPropertyDescriptor>();
        result.AddRange(entity.KeyProperties);
        result.AddRange(entity.EditableProperties
            .Where(p => !result.Any(x => x.Name == p.Name))
            .Take(6));
        return result;
    }
}
