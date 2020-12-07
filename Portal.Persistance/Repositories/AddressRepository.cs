using Portal.Data.Entities;
using Portal.Persistance.Repositories.Interfaces;

namespace Portal.Persistance.Repositories
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
     

        public AddressRepository(PortalDbContext context)
            : base(context)
        {
        
        }
        public PortalDbContext Context { get { return Context as PortalDbContext; } }

    }
}
