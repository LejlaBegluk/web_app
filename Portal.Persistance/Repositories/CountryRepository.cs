using Portal.Data.Entities;
using Portal.Persistance.Repositories.Interfaces;

namespace Portal.Persistance.Repositories
{
    public class CountryRepository : Repository<Country>, ICountryRepository
    {
       

        public CountryRepository(PortalDbContext context)
            : base(context)
        {
            
        }

        public PortalDbContext Context { get { return Context as PortalDbContext; } }
    }
}
