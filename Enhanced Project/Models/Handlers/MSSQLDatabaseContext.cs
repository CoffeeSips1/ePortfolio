using Microsoft.EntityFrameworkCore;
using SNHU_Capstone_Project.Models.Entities;

namespace SNHU_Capstone_Project.Models.Handlers
{
    public class MSSQLDatabaseContext : DbContext
    {

        //  Class variables
        // private readonly IConfiguration? _configuration;


        //  Default constructors
        public MSSQLDatabaseContext(DbContextOptions<MSSQLDatabaseContext> options) : base(options) { }


        //  DB sets
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;

        }

}
