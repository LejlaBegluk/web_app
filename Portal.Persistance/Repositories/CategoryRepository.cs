using Portal.Data.Entities;
using Portal.Persistance.Repositories.Interfaces;

namespace Portal.Persistance.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(PortalDbContext context)
            : base(context)
        {
        }
        public PortalDbContext Context { get { return Context as PortalDbContext; } }
    }
}
