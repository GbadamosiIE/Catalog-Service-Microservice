using Microsoft.EntityFrameworkCore;
using Play.Catalog.Entities;

namespace Play.Catalog.Service.Context
{
    public class ApplicationContext:DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options):base(options)
        {
            
        }
        public DbSet<Item> Items {get;set;}
    }
}