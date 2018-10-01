using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestWebAp.Models;

namespace TestWebAp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Ignore<IdentityUserClaim<string>>();
            //builder.Ignore<IdentityUserRole<string>>();
            //builder.Ignore<IdentityRole>();

            builder.Ignore<IdentityUserToken<string>>();
            builder.Ignore<IdentityUserLogin<string>>();
            builder.Ignore<IdentityRoleClaim<string>>();


            builder.Entity<ApplicationUser>().Ignore(c => c.TwoFactorEnabled)
                                             .Ignore(c => c.PhoneNumber)
                                             .Ignore(c => c.PhoneNumberConfirmed);

            builder.Entity<ApplicationUser>().ToTable("users");
        }
    }
}
