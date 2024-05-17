using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Technical_support.Models;

namespace Technical_support.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Request> Request { get; set; }
        public DbSet<StatusRequest> StatusRequests { get; set; }
        public DbSet<ThemeRequest> ThemeRequests { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<Prioritet> Prioritets { get; set; }
    }
}