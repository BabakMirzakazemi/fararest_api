using AdminPanel.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AdminPanel.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IEntityAdminService _entityAdminService;
        private readonly IOperationCatalogService _operationCatalogService;

        public IndexModel(IEntityAdminService entityAdminService, IOperationCatalogService operationCatalogService)
        {
            _entityAdminService = entityAdminService;
            _operationCatalogService = operationCatalogService;
        }

        public IReadOnlyList<ManagedEntityDescriptor> Entities { get; private set; } = [];
        public IReadOnlyList<OperationMethodDescriptor> Operations { get; private set; } = [];
        public List<DashboardEntityCard> EntityCards { get; private set; } = [];
        public bool DatabaseReachable { get; private set; }

        public void OnGet()
        {
            Entities = _entityAdminService.GetManagedEntities();
            Operations = _operationCatalogService.GetOperations();

            foreach (var entity in Entities)
            {
                try
                {
                    EntityCards.Add(new DashboardEntityCard(entity.Name, entity.DisplayName, _entityAdminService.Count(entity.Name)));
                }
                catch
                {
                    EntityCards.Add(new DashboardEntityCard(entity.Name, entity.DisplayName, -1));
                }
            }

            DatabaseReachable = EntityCards.All(x => x.Count >= 0);
        }
    }

    public sealed record DashboardEntityCard(string Name, string DisplayName, int Count);
}
