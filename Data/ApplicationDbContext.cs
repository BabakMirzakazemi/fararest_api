using Common.Utilities.Extensions;
using Entities.Common;
using Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Data
{
    public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        // zamani constructor zir ra minevisim ke mikhahim tanzimate coonection string ra az laye digar mese laye api daryaft konim
        // bedin shekl tanzimat az laye birooni be in sazande pass dade mishavad va in sazande ham option ha ra be pedar khod
        // be vaslie : base(options) miferestad
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=MyApiDb;Integrated Security=true");
        //    base.OnConfiguring(optionsBuilder);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var entitiesAssembly = typeof(IEntity).Assembly;

            modelBuilder.RegisterAllEntities<IEntity>(entitiesAssembly); // register automatic entity ha
            modelBuilder.RegisterEntityTypeConfiguration(entitiesAssembly); // register automatic fluent api haye entities ( masalan :builder.Property(p => p.Name).IsRequired().HasMaxLength(200);)
            modelBuilder.AddRestrictDeleteBehaviorConvention(); // jologiry az delete shodan cascade ( agar parenty child darad hazf nashavad ta zamani ke child ha ham hazf shode bashand
            modelBuilder.AddSequentialGuidForIdConvention(); // agar primary key az noe guid ast afzayeshi va index pazir bashad jahate optimiz boodan. guid tasadofi generat nashavad)
            modelBuilder.AddPluralizingTableNameConvention(); //  name table ha ke az entity ha eijad mishavad ra jam mibandad. yani entity user ba table users sakhte mishavad.
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    foreach (var property in entityType.GetProperties())
                    {
                        if (property.ClrType == typeof(DateTime))
                            property.SetColumnType("timestamp without time zone");
                    }
                }
            }
        }

        public override int SaveChanges()
        {
            _cleanString();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            _cleanString();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            _cleanString();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            _cleanString();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void _cleanString()
        {
            var changedEntities = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
            foreach (var item in changedEntities)
            {
                if (item.Entity == null)
                    continue;

                var properties = item.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

                foreach (var property in properties)
                {
                    var val = property.GetValue(item.Entity, null) as string;

                    if (!string.IsNullOrWhiteSpace(val))
                    {
                        var newVal = val.Fa2En().FixPersianChars();
                        if (newVal == val)
                            continue;
                        property.SetValue(item.Entity, newVal, null);
                    }
                }
            }
        }
    }
}

