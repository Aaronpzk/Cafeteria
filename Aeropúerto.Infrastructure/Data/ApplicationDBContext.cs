using Cafeteria.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cafeteria.Infrastructure.Data
{
    /// <summary>
    /// Contexto principal de Entity Framework Core para la base de datos.
    /// Mapea las entidades de cafetería hacia las tablas correspondientes en la base de datos.
    /// </summary>
    public class ApplicationDBContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public ApplicationDBContext(
            DbContextOptions<ApplicationDBContext> options ) : base(options) { }
        public DbSet<Cafe> Cafes { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cafe>().Property(c => c.Precio).HasPrecision(10, 2);
            modelBuilder.Entity<Pedido>().Property(p => p.PrecioUnitario).HasPrecision(10, 2);
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Cafe)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(p => p.CafeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

