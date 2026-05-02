using AdminPanel.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Pages.Entities;

public class DeleteModel : PageModel
{
    private readonly IEntityAdminService _entityAdminService;

    public DeleteModel(IEntityAdminService entityAdminService)
    {
        _entityAdminService = entityAdminService;
    }

    [BindProperty(SupportsGet = true)]
    public string EntityName { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string Key { get; set; } = string.Empty;

    public ManagedEntityDescriptor Entity { get; private set; } = null!;
    public object Row { get; private set; } = null!;

    public IActionResult OnGet(string entityName, string key)
    {
        EntityName = entityName;
        Key = key;
        Entity = _entityAdminService.GetManagedEntity(entityName);
        var row = _entityAdminService.FindByKey(entityName, key);
        if (row is null)
        {
            TempData["Error"] = "رکورد مورد نظر یافت نشد.";
            return RedirectToPage("/Entities/List", new { entityName });
        }

        Row = row;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string entityName, string key, CancellationToken cancellationToken)
    {
        var result = await _entityAdminService.DeleteAsync(entityName, key, cancellationToken);
        TempData[result.Success ? "Success" : "Error"] = result.Message;
        return RedirectToPage("/Entities/List", new { entityName });
    }

    public string ShowValue(ManagedPropertyDescriptor property)
    {
        var value = property.PropertyInfo.GetValue(Row);
        return value?.ToString() ?? "-";
    }
}
