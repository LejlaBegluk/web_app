using System;
using System.Threading.Tasks;

namespace Portal.Persistance.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        IUserRepository Users { get; }
        IUserPhotosRepository UserPhotos { get; }
        public IArticleRepository Articles { get; }
        public ICategoryRepository Categories { get; }
        public ICommentRepository Comments { get; }
        public IEmployeeRepository Employees { get; }
        public ICountryRepository Countries { get; }
        public IAddressRepository Addresses { get; }
        public IPollRepository Polls { get; }

        public IArticlePhotoRepository ArticlePhotos { get; }
        Task<int> CompleteAsync();


    }
}
