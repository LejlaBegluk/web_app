using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Portal.Data.Entities;
using Portal.Data.Helpers;
using Portal.Data.ViewModels;
using Portal.Persistance.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Persistance.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {

        public UserRepository(PortalDbContext context)
            : base(context)
        {

        }
        public PortalDbContext Context { get { return Context as PortalDbContext; } }

    }

}