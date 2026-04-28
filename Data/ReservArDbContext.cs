using Microsoft.EntityFrameworkCore;
using ReservAr.Models;
namespace ReservAr.Data
{
    public class ReservArDbContext : DbContext
    {
        public ReservArDbContext(DbContextOptions<ReservArDbContext> options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Audit_Log> AuditLogs { get; set; }
        public DbSet<Sector> Sectors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de las entidades y relaciones

            // Tabla Usuario
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasMaxLength(120)
                    .IsRequired();

                entity.Property(e => e.Email)
                    .HasMaxLength(320)
                    .IsRequired();

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(60)
                    .IsRequired();

                entity.ToTable("User");
            });

            // Tabla Audit_Log
            modelBuilder.Entity<Audit_Log>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.Action)
                    .HasMaxLength(120)
                    .IsRequired();

                entity.Property(e => e.EntityType)
                    .HasMaxLength(120)
                    .IsRequired();

                entity.Property(e => e.EntityId)
                    .HasMaxLength(320)
                    .IsRequired();

                entity.Property(e => e.Details)
                    .HasMaxLength(6000);

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict); // --> Revisar si queremos restringir la eliminacion de usuario por tener relaciones dependientes

                entity.ToTable("Audit_Log");
            });

            // Tabla Event
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name)
                    .HasMaxLength(250)
                    .IsRequired();

                entity.Property(e => e.Venue)
                    .HasMaxLength(320)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .HasMaxLength(120)
                    .IsRequired();

                entity.ToTable("Event");
            });

            // Tabla Sector
            modelBuilder.Entity<Sector>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.EventId)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .HasMaxLength(120)
                    .IsRequired();

                entity.Property(e => e.Price)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(e => e.Capacity)
                    .IsRequired();

                entity.HasOne<Event>()
                    .WithMany()
                    .HasForeignKey(e => e.EventId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("Sector");
            });

            // Tabla Seat
            modelBuilder.Entity<Seat>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.SectorId)
                    .IsRequired();

                entity.Property(e => e.RowIdentifier)
                    .HasMaxLength(60)
                    .IsRequired();

                entity.Property(e => e.SeatNumber)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .HasMaxLength(60)
                    .IsRequired();

                entity.Property(e => e.Version)
                    .IsRequired()
                    .HasDefaultValue(0);

                entity.HasOne<Sector>()
                    .WithMany()
                    .HasForeignKey(e => e.SectorId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable("Seat");
            });

            // Tabla Reservation
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.UserId)
                    .IsRequired();

                entity.Property(e => e.SeatId)
                    .IsRequired();

                entity.Property(e => e.Status)
                    .HasMaxLength(120)
                    .IsRequired();

                entity.Property(e => e.ReservedAt)
                    .HasDefaultValueSql("now()")
                    .IsRequired();

                entity.Property(e => e.ExpiresAt)
                    .IsRequired();

                entity.HasOne<User>()
                    .WithMany()
                    .HasForeignKey(e => e.UserId);

                entity.HasOne<Seat>()
                    .WithMany()
                    .HasForeignKey(e => e.SeatId);

                entity.ToTable("Reservation");
            });

            // Data seeding: Users
            //modelBuilder.Entity<User>().HasData(new User { Id = 1, Name = "Admin User", Email = "RA_adminuser@gmail.com", PasswordHash = "" });

            // Data seeding: Events
            modelBuilder.Entity<Event>().HasData(
                new Event
                {
                    Id = 1,
                    Name = "Noche de Rock Amateur",
                    EventDate = new DateTime(2026, 6, 16, 21, 0, 0, DateTimeKind.Utc),
                    Venue = "Niceto Club - CABA",
                    Status = "DISPONIBLE"
                }
            );

            //Data seeding: Sectors
            modelBuilder.Entity<Sector>().HasData(
                new Sector { Id = 1, EventId = 1, Name = "Platea Alta", Price = 25000.00m, Capacity = 50},
                new Sector { Id = 2, EventId = 1, Name = "Platea Baja", Price = 10000.00m, Capacity = 50}
            );

            //Data seeding: Seats
            modelBuilder.Entity<Seat>().HasData(
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), SectorId = 1, RowIdentifier = "A", SeatNumber = 1, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), SectorId = 1, RowIdentifier = "A", SeatNumber = 2, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), SectorId = 1, RowIdentifier = "A", SeatNumber = 3, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), SectorId = 1, RowIdentifier = "A", SeatNumber = 4, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), SectorId = 1, RowIdentifier = "A", SeatNumber = 5, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000006"), SectorId = 1, RowIdentifier = "A", SeatNumber = 6, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000007"), SectorId = 1, RowIdentifier = "A", SeatNumber = 7, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000008"), SectorId = 1, RowIdentifier = "A", SeatNumber = 8, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000009"), SectorId = 1, RowIdentifier = "A", SeatNumber = 9, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000010"), SectorId = 1, RowIdentifier = "A", SeatNumber = 10, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000011"), SectorId = 1, RowIdentifier = "B", SeatNumber = 11, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000012"), SectorId = 1, RowIdentifier = "B", SeatNumber = 12, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000013"), SectorId = 1, RowIdentifier = "B", SeatNumber = 13, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000014"), SectorId = 1, RowIdentifier = "B", SeatNumber = 14, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000015"), SectorId = 1, RowIdentifier = "B", SeatNumber = 15, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000016"), SectorId = 1, RowIdentifier = "B", SeatNumber = 16, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000017"), SectorId = 1, RowIdentifier = "B", SeatNumber = 17, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000018"), SectorId = 1, RowIdentifier = "B", SeatNumber = 18, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000019"), SectorId = 1, RowIdentifier = "B", SeatNumber = 19, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000020"), SectorId = 1, RowIdentifier = "B", SeatNumber = 20, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000021"), SectorId = 1, RowIdentifier = "C", SeatNumber = 21, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000022"), SectorId = 1, RowIdentifier = "C", SeatNumber = 22, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000023"), SectorId = 1, RowIdentifier = "C", SeatNumber = 23, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000024"), SectorId = 1, RowIdentifier = "C", SeatNumber = 24, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000025"), SectorId = 1, RowIdentifier = "C", SeatNumber = 25, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000026"), SectorId = 1, RowIdentifier = "C", SeatNumber = 26, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000027"), SectorId = 1, RowIdentifier = "C", SeatNumber = 27, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000028"), SectorId = 1, RowIdentifier = "C", SeatNumber = 28, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000029"), SectorId = 1, RowIdentifier = "C", SeatNumber = 29, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000030"), SectorId = 1, RowIdentifier = "C", SeatNumber = 30, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000031"), SectorId = 1, RowIdentifier = "D", SeatNumber = 31, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000032"), SectorId = 1, RowIdentifier = "D", SeatNumber = 32, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000033"), SectorId = 1, RowIdentifier = "D", SeatNumber = 33, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000034"), SectorId = 1, RowIdentifier = "D", SeatNumber = 34, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000035"), SectorId = 1, RowIdentifier = "D", SeatNumber = 35, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000036"), SectorId = 1, RowIdentifier = "D", SeatNumber = 36, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000037"), SectorId = 1, RowIdentifier = "D", SeatNumber = 37, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000038"), SectorId = 1, RowIdentifier = "D", SeatNumber = 38, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000039"), SectorId = 1, RowIdentifier = "D", SeatNumber = 39, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000040"), SectorId = 1, RowIdentifier = "D", SeatNumber = 40, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000041"), SectorId = 1, RowIdentifier = "E", SeatNumber = 41, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000042"), SectorId = 1, RowIdentifier = "E", SeatNumber = 42, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000043"), SectorId = 1, RowIdentifier = "E", SeatNumber = 43, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000044"), SectorId = 1, RowIdentifier = "E", SeatNumber = 44, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000045"), SectorId = 1, RowIdentifier = "E", SeatNumber = 45, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000046"), SectorId = 1, RowIdentifier = "E", SeatNumber = 46, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000047"), SectorId = 1, RowIdentifier = "E", SeatNumber = 47, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000048"), SectorId = 1, RowIdentifier = "E", SeatNumber = 48, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000049"), SectorId = 1, RowIdentifier = "E", SeatNumber = 49, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000050"), SectorId = 1, RowIdentifier = "E", SeatNumber = 50, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000051"), SectorId = 2, RowIdentifier = "A", SeatNumber = 1, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000052"), SectorId = 2, RowIdentifier = "A", SeatNumber = 2, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000053"), SectorId = 2, RowIdentifier = "A", SeatNumber = 3, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000054"), SectorId = 2, RowIdentifier = "A", SeatNumber = 4, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000055"), SectorId = 2, RowIdentifier = "A", SeatNumber = 5, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000056"), SectorId = 2, RowIdentifier = "A", SeatNumber = 6, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000057"), SectorId = 2, RowIdentifier = "A", SeatNumber = 7, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000058"), SectorId = 2, RowIdentifier = "A", SeatNumber = 8, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000059"), SectorId = 2, RowIdentifier = "A", SeatNumber = 9, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000060"), SectorId = 2, RowIdentifier = "A", SeatNumber = 10, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000061"), SectorId = 2, RowIdentifier = "B", SeatNumber = 11, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000062"), SectorId = 2, RowIdentifier = "B", SeatNumber = 12, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000063"), SectorId = 2, RowIdentifier = "B", SeatNumber = 13, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000064"), SectorId = 2, RowIdentifier = "B", SeatNumber = 14, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000065"), SectorId = 2, RowIdentifier = "B", SeatNumber = 15, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000066"), SectorId = 2, RowIdentifier = "B", SeatNumber = 16, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000067"), SectorId = 2, RowIdentifier = "B", SeatNumber = 17, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000068"), SectorId = 2, RowIdentifier = "B", SeatNumber = 18, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000069"), SectorId = 2, RowIdentifier = "B", SeatNumber = 19, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000070"), SectorId = 2, RowIdentifier = "B", SeatNumber = 20, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000071"), SectorId = 2, RowIdentifier = "C", SeatNumber = 21, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000072"), SectorId = 2, RowIdentifier = "C", SeatNumber = 22, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000073"), SectorId = 2, RowIdentifier = "C", SeatNumber = 23, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000074"), SectorId = 2, RowIdentifier = "C", SeatNumber = 24, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000075"), SectorId = 2, RowIdentifier = "C", SeatNumber = 25, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000076"), SectorId = 2, RowIdentifier = "C", SeatNumber = 26, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000077"), SectorId = 2, RowIdentifier = "C", SeatNumber = 27, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000078"), SectorId = 2, RowIdentifier = "C", SeatNumber = 28, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000079"), SectorId = 2, RowIdentifier = "C", SeatNumber = 29, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000080"), SectorId = 2, RowIdentifier = "C", SeatNumber = 30, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000081"), SectorId = 2, RowIdentifier = "D", SeatNumber = 31, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000082"), SectorId = 2, RowIdentifier = "D", SeatNumber = 32, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000083"), SectorId = 2, RowIdentifier = "D", SeatNumber = 33, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000084"), SectorId = 2, RowIdentifier = "D", SeatNumber = 34, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000085"), SectorId = 2, RowIdentifier = "D", SeatNumber = 35, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000086"), SectorId = 2, RowIdentifier = "D", SeatNumber = 36, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000087"), SectorId = 2, RowIdentifier = "D", SeatNumber = 37, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000088"), SectorId = 2, RowIdentifier = "D", SeatNumber = 38, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000089"), SectorId = 2, RowIdentifier = "D", SeatNumber = 39, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000090"), SectorId = 2, RowIdentifier = "D", SeatNumber = 40, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000091"), SectorId = 2, RowIdentifier = "E", SeatNumber = 41, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000092"), SectorId = 2, RowIdentifier = "E", SeatNumber = 42, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000093"), SectorId = 2, RowIdentifier = "E", SeatNumber = 43, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000094"), SectorId = 2, RowIdentifier = "E", SeatNumber = 44, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000095"), SectorId = 2, RowIdentifier = "E", SeatNumber = 45, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000096"), SectorId = 2, RowIdentifier = "E", SeatNumber = 46, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000097"), SectorId = 2, RowIdentifier = "E", SeatNumber = 47, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000098"), SectorId = 2, RowIdentifier = "E", SeatNumber = 48, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000099"), SectorId = 2, RowIdentifier = "E", SeatNumber = 49, Status = "Disponible" },
                new Seat { Id = Guid.Parse("00000000-0000-0000-0000-000000000100"), SectorId = 2, RowIdentifier = "E", SeatNumber = 50, Status = "Disponible" }
            );
            
        }
    }
}