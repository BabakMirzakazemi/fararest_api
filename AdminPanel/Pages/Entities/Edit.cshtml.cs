using AdminPanel.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace AdminPanel.Pages.Entities;

public class EditModel : PageModel
{
    private readonly IEntityAdminService _entityAdminService;

    public EditModel(IEntityAdminService entityAdminService)
    {
        _entityAdminService = entityAdminService;
    }

    [BindProperty(SupportsGet = true)]
    public string EntityName { get; set; } = string.Empty;

    [BindProperty(SupportsGet = true)]
    public string? Key { get; set; }

    public ManagedEntityDescriptor Entity { get; private set; } = null!;
    public Dictionary<string, string?> Values { get; private set; } = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, List<LookupOption>> ForeignKeyOptions { get; private set; } = new(StringComparer.OrdinalIgnoreCase);
    public bool IsCreate => string.IsNullOrWhiteSpace(Key);

    public async Task<IActionResult> OnGetAsync(string entityName, string? key, CancellationToken cancellationToken)
    {
        EntityName = entityName;
        Key = key;
        Entity = _entityAdminService.GetManagedEntity(entityName);
        ForeignKeyOptions = await _entityAdminService.GetForeignKeyOptionsAsync(entityName, cancellationToken);

        if (!string.IsNullOrWhiteSpace(key))
        {
            var row = _entityAdminService.FindByKey(entityName, key);
            if (row is null)
            {
                TempData["Error"] = "Requested record was not found.";
                return RedirectToPage("/Entities/List", new { entityName });
            }

            foreach (var property in Entity.EditableProperties)
            {
                var value = property.PropertyInfo.GetValue(row);
                Values[property.Name] = value switch
                {
                    DateTime dt => dt.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture),
                    DateTimeOffset dto => dto.ToString("yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture),
                    _ => value?.ToString()
                };
            }
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string entityName, string? key, CancellationToken cancellationToken)
    {
        EntityName = entityName;
        Key = key;
        Entity = _entityAdminService.GetManagedEntity(entityName);

        var values = Entity.EditableProperties
            .ToDictionary(p => p.Name, p => (string?)Request.Form[p.Name].ToString(), StringComparer.OrdinalIgnoreCase);

        var result = await _entityAdminService.SaveAsync(entityName, key, values, cancellationToken);
        if (!result.Success)
        {
            TempData["Error"] = result.Message;
            ForeignKeyOptions = await _entityAdminService.GetForeignKeyOptionsAsync(entityName, cancellationToken);
            Values = values;
            return Page();
        }

        TempData["Success"] = result.Message;
        return RedirectToPage("/Entities/List", new { entityName });
    }

    public bool IsForeignKey(ManagedPropertyDescriptor property) => ForeignKeyOptions.ContainsKey(property.Name);

    public string GetValue(ManagedPropertyDescriptor property)
    {
        if (Values.TryGetValue(property.Name, out var value))
        {
            return value ?? string.Empty;
        }

        return string.Empty;
    }
}
