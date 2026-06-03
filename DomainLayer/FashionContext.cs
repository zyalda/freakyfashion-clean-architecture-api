using DomainLayer.Entites;
using Microsoft.EntityFrameworkCore;

namespace DomainLayer
{
    public class FashionContext:DbContext
    {
        public FashionContext()
        {
        }
        //private readonly string _connectionString;
        public FashionContext(DbContextOptions<FashionContext> options) : base(options)
        {
            //_connectionString = "Data Source=(localdb)\\MSSQLLocalDB; Initial Catalog=BlogDB;Integrated Security=True;TrustServerCertificate=True;";
        }
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(_connectionString);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // This tells Entity Framework to set up a unique index constraints rule in SQL Server
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.UrlSlug)
                .IsUnique();
        }

        //Mappning between tables in database and entites classes.
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
    }
}
