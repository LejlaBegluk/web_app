using Portal.Data.Entities;
using Portal.Persistance.Repositories;
using Portal.Persistance.Repositories.Interfaces;
using System;
using System.Threading.Tasks;

namespace Portal.Persistance
{
    public class UnitOfWork : IUnitOfWork
    {
        //Portal Context
        private readonly PortalDbContext _context;


        public IUserRepository Users { get; private set; }
        public IUserPhotosRepository UserPhotos { get; private set; }
        public IArticleRepository Articles { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public ICommentRepository Comments { get; private set; }
        public IEmployeeRepository Employees { get; private set; }
        public ICountryRepository Countries { get; private set; }
        public IAddressRepository Addresses { get; private set; }
        public IArticlePhotoRepository ArticlePhotos { get; private set; }
        public IPollRepository Polls { get; private set; }
        public ICorrespondentArticleRepository CorrespondentArticles { get; private set; }
        public ICorrespondentArticlePhotoRepository CorrespondentArticlePhotos { get; private set; }

        public UnitOfWork(PortalDbContext context)
        {
            _context = context;
            Users = new UserRepository(_context);
            UserPhotos = new UserPhotosRepository(_context);
            Articles = new ArticleRepository(_context);
            Categories = new CategoryRepository(_context);
            Comments = new CommentRepository(_context);
            Employees = new EmployeeRepository(_context);
            Countries = new CountryRepository(_context);
            Addresses = new AddressRepository(_context);
            ArticlePhotos = new ArticlePhotoRepository(_context);
            Polls = new PollRepository(_context);
            CorrespondentArticles=new CorrespondentArticleRepository(_context);
            CorrespondentArticlePhotos = new CorrespondentArticlePhotoRepository(_context);


        }



        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
