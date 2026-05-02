using AdminPanel.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Pages.Entities;

public class IndexModel : PageModel
{
    private readonly IEntityAdminService _entityAdminService;

    public IndexModel(IEntityAdminService entityAdminService)
    {
        _entityAdminService = entityAdminService;
    }

    public IReadOnlyList<ManagedEntityDescriptor> Entities { get; private set; } = [];

    public void OnGet()
    {
        Entities = _entityAdminService.GetManagedEntities();
    }
}
