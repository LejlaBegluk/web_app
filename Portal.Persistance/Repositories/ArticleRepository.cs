using Portal.Data.Entities;
using Portal.Persistance.Repositories.Interfaces;

namespace Portal.Persistance.Repositories
{
    public class ArticleRepository : Repository<Article>, IArticleRepository
    {
        
        public ArticleRepository(PortalDbContext context)
            : base(context)
        {
        }
        public PortalDbContext Context { get { return Context as PortalDbContext; } }
    }
}
