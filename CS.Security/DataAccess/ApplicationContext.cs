using CS.Security.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CS.Security.DataAccess;

public class ApplicationContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public ApplicationContext() { }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options) { }

}