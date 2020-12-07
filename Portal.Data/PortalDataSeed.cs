using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Portal.Data.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Portal.Data
{
    public class PortalDataSeed
    {

        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly PortalDbContext _portalDbContext;

        public PortalDataSeed(UserManager<User> userManager, RoleManager<Role> roleManager, PortalDbContext portalDbContext)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _portalDbContext = portalDbContext;

        }
        public void SeedTestData()
        {
            var articleOwners = new List<string>();
            if (!_userManager.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("../Portal.Data/Users.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                var employeeData = System.IO.File.ReadAllText("../Portal.Data/Employees.json");
                var employees = JsonConvert.DeserializeObject<List<EmployeeSample>>(employeeData);

                var roles = new List<Role>(){
                    new Role{ Id = Guid.NewGuid() ,Name = "Administrator"},
                    new Role{ Id = Guid.NewGuid() ,Name = "Journalist"},
                    new Role{ Id = Guid.NewGuid() ,Name = "Editor"}
                };
                foreach (var role in roles)
                {
                    _roleManager.CreateAsync(role).Wait();
                }
                var country = new Country { Id = Guid.NewGuid(), Name = "Bosna i Hercegovina", Code = "BH" };
                _portalDbContext.Countries.Add(country);
                _portalDbContext.SaveChanges();
                for (int i = 0; i < users.Count(); i++)
                {

                    users[i].Id = Guid.NewGuid();
                    if (i % 9 == 0) users[i].EmailConfirmed = false; else users[i].EmailConfirmed = true;
                    _userManager.CreateAsync(users[i], "P@$$w0rd").Wait();
                    if (i % 3 == 0)
                    {
                        _userManager.AddToRoleAsync(users[i], "Journalist").Wait();
                        articleOwners.Add(users[i].Id.ToString());
                        var address = new Address() { Id = Guid.NewGuid(), Name = "Test Address", CountryId = country.Id };
                        _portalDbContext.Addresses.Add(address);
                        var employeeObj = new Employee()
                        {
                            Id = Guid.NewGuid(),
                            FirstName = employees[i].FirstName,
                            LastName = employees[i].LastName,
                            UserId = users[i].Id,
                            AddressId = address.Id
                        };

                        _portalDbContext.Employees.Add(employeeObj);


                    }

                    if (i % 7 == 0)
                    {
                        _userManager.AddToRoleAsync(users[i], "Editor").Wait();
                        articleOwners.Add(users[i].Id.ToString());
                        var address = new Address() { Id = Guid.NewGuid(), Name = "Test Address", CountryId = country.Id };
                        _portalDbContext.Addresses.Add(address);
                        var employeeObj = new Employee()
                        {
                            Id = Guid.NewGuid(),
                            FirstName = employees[i].FirstName,
                            LastName = employees[i].LastName,
                            UserId = users[i].Id,
                            AddressId = address.Id
                        };
                        _portalDbContext.Employees.Add(employeeObj);
                    }

                }


                var adminUser = new User
                {
                    Id = Guid.Parse("d4007c81-5d7e-49fd-931f-bff99ab8c157"),
                    UserName = "Admin",
                    Email = "admin@email.com",
                    IsActive = true,
                    EmailConfirmed = true
                };
                IdentityResult result = _userManager.CreateAsync(adminUser, "P@$$w0rd").Result;
                if (result.Succeeded)
                {
                    var admin = _userManager.FindByNameAsync("Admin").Result;
                    _userManager.AddToRolesAsync(admin, roles.Select(r => r.Name)).Wait();
                    var address = new Address() { Id = Guid.NewGuid(), Name = "Test Address", CountryId = country.Id };
                    _portalDbContext.Addresses.Add(address);
                    var employeeObj = new Employee()
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "Super Admin",
                        LastName = "Super Admin",
                        UserId = adminUser.Id,
                        AddressId = address.Id
                    };
                    _portalDbContext.Employees.Add(employeeObj);
                }
                _portalDbContext.SaveChanges();
            }
            if (!_portalDbContext.Categories.Any())
            {
                _portalDbContext.Categories.Add(new Category
                {
                    Id = Guid.Parse("e5a2d73f-c888-4593-8ade-e1d1fee85725"),
                    Name = "Test Category"
                });
                _portalDbContext.SaveChanges();

            }
            if (!_portalDbContext.Articles.Any())
            {
                var articleData = System.IO.File.ReadAllText("../Portal.Data/Articles.json");

                var articles = JsonConvert.DeserializeObject<List<Article>>(articleData);
                var rand = new Random();
                for (int i = 0; i < articles.Count(); i++)
                {
                    articles[i].UserId = Guid.Parse(articleOwners[rand.Next(0, articleOwners.Count())]);
                    _portalDbContext.Articles.Add(articles[i]);

                }
                _portalDbContext.SaveChanges();
            }
        }
    }
    public class EmployeeSample
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
