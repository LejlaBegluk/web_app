using Portal.Data.Entities;
using Portal.Data.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Portal.Data.Helpers.HelpersInterfaces
{
    public interface IUserFilterOrderHelper
    {
        Task<IQueryable<User>> GetFilteredUsersAsync(UserManagemenViewModel userParams);
    }
}
