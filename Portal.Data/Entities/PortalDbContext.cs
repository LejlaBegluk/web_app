using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Portal.Data.Entities
{
    public class PortalDbContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>,
    UserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {

        public DbSet<Photo> UserPhotos { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Poll> Polls { get; set; }
        public DbSet<PollAnswer> PollAnswer { get; set; }
        public DbSet<ArticlePhoto> ArticlePhotos { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Country> Countries { get; set; }

        public PortalDbContext(DbContextOptions<PortalDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.Entity<IdentityUserClaim<Guid>>(b =>
            {

                b.ToTable("Claims");
            });

            builder.Entity<IdentityUserLogin<Guid>>(b =>
            {

                b.ToTable("Logins");
            });

            builder.Entity<IdentityUserToken<Guid>>(b =>
            {

                b.ToTable("Tokens");
            });

            builder.Entity<IdentityRoleClaim<Guid>>(b =>
            {
                b.ToTable("RolesClaims");
            });

            builder.Entity<User>(b =>
            {
                b.ToTable("Users");

                b.HasMany(u => u.Articles)
                    .WithOne(a => a.User)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired(false);
                b.HasMany(u => u.Comments)
                    .WithOne(c => c.User)
                    .HasForeignKey(c => c.UserId)
                    .IsRequired(false);
                b.HasOne(u => u.Photo)
                    .WithOne(p => p.User)
                    .HasForeignKey<Photo>(p => p.UserId);
                b.HasOne(u => u.Editor)
                     .WithOne(p => p.UserEditor)
                     .HasForeignKey<User>(p => p.EditorId)
                     .IsRequired(false);
                b.HasIndex(e => e.EditorId)
                    .IsUnique(false);
                b.HasOne(u => u.Employee)
                    .WithOne(p => p.User)
                    .HasForeignKey<Employee>(p => p.UserId);
                
                b.HasMany(e => e.Claims)
                    .WithOne(c => c.User)
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                b.HasMany(e => e.Logins)
                    .WithOne(e => e.User)
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                b.HasMany(e => e.Tokens)
                    .WithOne(e => e.User)
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<UserRole>(b =>
            {
                b.ToTable("UsersRoles");
                b.HasKey(ur => new { ur.UserId, ur.RoleId });

                b.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                b.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<Role>(b =>
            {
                b.ToTable("Roles");
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                b.HasMany(e => e.RoleClaims)
                    .WithOne(e => e.Role)
                    .HasForeignKey(rc => rc.RoleId)
                    .IsRequired();
            });

            builder.Entity<Article>(b =>
            {
                b.HasMany(a => a.Comments)
               .WithOne(c => c.Article)
               .HasForeignKey(a => a.ArticleId)
               .IsRequired(false);
            });
            builder.Entity<Category>(b =>
            {
                b.HasMany(c => c.Articles)
                .WithOne(a => a.Category)
                .HasForeignKey(a => a.CategoryId)
                .IsRequired();
            });
            builder.Entity<Employee>(b =>
            {
                b.HasOne(c => c.Address)
                .WithOne(a => a.Employee)
                .HasForeignKey<Employee>(a => a.AddressId)
                .IsRequired();
            });
        }
    }
}
