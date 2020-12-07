using Microsoft.EntityFrameworkCore;
using Portal.Data.Entities;
using Portal.Persistance;
using Portal.Persistance.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Portal.Tests.Tests
{
    public class FakeUnitOfWorkTests
    {

        public PortalDbContext InitContext()
        {
            var builder = new DbContextOptionsBuilder<PortalDbContext>()
                .UseInMemoryDatabase("DevConnection")
                .Options;

            var context = new PortalDbContext(builder);

            return context;
        }
        public PortalDbContext CreateContext()
        {
            var context = InitContext();

            context.Database.EnsureDeleted();

            var _systemUsers = new List<User>()
            {
               new User{
                Id = Guid.Parse("044d64ce-4fef-4cf3-a3e9-dd60a7168c2e"),
                UserName = "test00",
                Email = "test00@email.com",
                IsActive = true,
                PhoneNumber = "0000000000",
                CreatedOn = DateTime.Now
                },
               new User{
                Id =Guid.Parse( "0879335e-b5d8-4320-aec5-9b0659a8091e"),
                UserName = "test01",
                Email = "test01@email.com",
                IsActive = false,
                PhoneNumber = "0000200000",
                CreatedOn = DateTime.Now
                },
               new User{
                Id = Guid.Parse("221a5544-cc94-4e1d-beb8-243cf15b9242"),
                UserName = "test02",
                Email = "test02@email.com",
                IsActive = false,
                PhoneNumber = "0000200000",
                CreatedOn = DateTime.Now

                },new User{
                Id = Guid.Parse("2dadbf98-ff8e-47e3-9965-dc56a1cf2bfb"),
                UserName = "test03",
                Email = "test03@email.com",
                IsActive = true,
                PhoneNumber = "0030000000",
                CreatedOn = DateTime.Now

                }
            };

            context.Users.AddRange(_systemUsers);
            context.SaveChanges();

            return context;
        }

        private IUnitOfWork GetInMemoryUnitOfWork()
        {
            var context = CreateContext();
            return new UnitOfWork(context);
        }
        [Fact]
        public void GetUsers_WithRepository_Working()
        {
            var unitOfWork = GetInMemoryUnitOfWork();
            var user = unitOfWork.Users.SingleOrDefault(u => u.Id.Equals(Guid.Parse("044d64ce-4fef-4cf3-a3e9-dd60a7168c2e")));

            Assert.Equal(4, unitOfWork.Users.GetAll().ToListAsync().Result.Count);
            Assert.Equal(2, unitOfWork.Users.Find(u => u.IsActive.Equals(true)).Count());
            Assert.Equal(2, unitOfWork.Users.Find(u => u.IsActive.Equals(false)).Count());
            Assert.Equal("test00", unitOfWork.Users.SingleOrDefault(u => u.Id.Equals(Guid.Parse("044d64ce-4fef-4cf3-a3e9-dd60a7168c2e"))).UserName);
            Assert.NotNull(unitOfWork.Users.GetAsync(Guid.Parse("044d64ce-4fef-4cf3-a3e9-dd60a7168c2e")));
            Assert.Equal("test00", unitOfWork.Users.GetAsync(Guid.Parse("044d64ce-4fef-4cf3-a3e9-dd60a7168c2e")).Result.UserName);

        }

        [Fact]
        public void AddUser_WithRepository_Working()
        {
            var unitOfWork = GetInMemoryUnitOfWork();
            Assert.Equal(4, unitOfWork.Users.GetAll().ToListAsync().Result.Count);
            unitOfWork.Users.Add(new User
            {
                Id = Guid.Parse("2dadbf98-ff8e-47e3-8865-dc56a1cf2bfb"),
                UserName = "testRemove",
                Email = "testRemove@email.com",
                IsActive = true,
                PhoneNumber = "0060000000",
                CreatedOn = DateTime.Now

            });
            unitOfWork.CompleteAsync();
            Assert.Equal(5, unitOfWork.Users.GetAll().ToListAsync().Result.Count);
            Assert.Equal(2, unitOfWork.Users.Find(u => u.IsActive.Equals(false)).Count());
            Assert.Equal(3, unitOfWork.Users.Find(u => u.IsActive.Equals(true)).Count());


        }

        [Fact]
        public void RemoveUser_WithRepository_Working()
        {
            var unitOfWork = GetInMemoryUnitOfWork();
            Assert.Equal(4, unitOfWork.Users.GetAll().ToListAsync().Result.Count);
            var user = unitOfWork.Users.SingleOrDefault(u => u.Id.Equals(Guid.Parse("044d64ce-4fef-4cf3-a3e9-dd60a7168c2e")));
            unitOfWork.Users.Remove(user);
            unitOfWork.CompleteAsync();
            Assert.Equal(3, unitOfWork.Users.GetAll().ToListAsync().Result.Count);
        }

        [Fact]
        public void RemoveUsers_WithRepository_Working()
        {
            var unitOfWork = GetInMemoryUnitOfWork();
            var removeRange = unitOfWork.Users.Find(u => u.IsActive.Equals(false));
            Assert.Equal(4, unitOfWork.Users.GetAll().ToListAsync().Result.Count);
            Assert.Equal(2, unitOfWork.Users.Find(u => u.IsActive.Equals(false)).Count());
            Assert.Equal(2, unitOfWork.Users.Find(u => u.IsActive.Equals(true)).Count());
            unitOfWork.Users.RemoveRange(removeRange);
            unitOfWork.CompleteAsync();
            Assert.Equal(2, unitOfWork.Users.GetAll().ToListAsync().Result.Count);
            Assert.Empty(unitOfWork.Users.Find(u => u.IsActive.Equals(false)));
            Assert.Equal(2, unitOfWork.Users.Find(u => u.IsActive.Equals(true)).Count());

        }
        [Fact]
        public void AddUsers_WithRepository_Working()
        {
            var unitOfWork = GetInMemoryUnitOfWork();
            var userRange = new List<User>()
            {
                 new User{
                Id = Guid.Parse("221a3344-cc94-4e1d-beb8-243cf15b9242"),
                UserName = "test32",
                Email = "test32@email.com",
                IsActive = false,
                PhoneNumber = "0000290000",
                CreatedOn = DateTime.Now

                },new User{
                Id = Guid.Parse("2dadba38-ff8e-47e3-9965-dc56a1cf2bfb"),
                UserName = "test07",
                Email = "test07@email.com",
                IsActive = false,
                PhoneNumber = "0039000000",
                CreatedOn = DateTime.Now

                }
            };
            unitOfWork.Users.AddRange(userRange);
            unitOfWork.CompleteAsync();
            Assert.Equal(6, unitOfWork.Users.GetAll().ToListAsync().Result.Count);
            Assert.Equal(4, unitOfWork.Users.Find(u => u.IsActive.Equals(false)).Count());
            Assert.Equal(2, unitOfWork.Users.Find(u => u.IsActive.Equals(true)).Count());
        }
    }
}