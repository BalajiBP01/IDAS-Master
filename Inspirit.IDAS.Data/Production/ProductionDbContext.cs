using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Inspirit.IDAS.Data.Production
{
    public class ProductionDbContext : DbContext
    {
        private static DbContextOptions options;

        public DbSet<Commercial> Commercials { get; set; }
        public DbSet<Consumer> Consumers { get; set; }        
        public DbSet<PropertyDeed> PropertyDeeds { get; set; }

        public DbSet<Auditor> Auditors { get; set; }
        public DbSet<Telephone> Telephones { get; set; }
        public DbSet<Director> Directors { get; set; }

        public DbSet<ConsumerHomeAffair> ConsumerHomeAffairs { get; set; }
        public DbSet<ConsumerAddress> ConsumerAddresses { get; set; }
        public DbSet<ConsumerEmploymentOccupation> ConsumerEmploymentOccupations { get; set; }

        public DbSet<ConsumerTelephone> ConsumerTelephones { get; set; }

        public DbSet<ConsumerEmailConfirmed> ConsumerEmails { get; set; }

        public DbSet<AuditorAddress> AuditorAddresses { get; set; }
        public DbSet<AuditorHistory> AuditorHistory { get; set; }

        public DbSet<CommercialAddress> CommercialAddresses { get; set; }

        public DbSet<CommercialDirector> CommercialDirectors { get; set; }
        public DbSet<CommercialTelephone> CommercialTelephones { get; set; }
        public DbSet<CommercialAuditor> CommercialAuditors { get; set; }

        public DbSet<CommercialJudgement> CommercialJudgements { get; set; }

        public DbSet<DirectorAddress> DirectorAddresses { get; set; }

        public DbSet<DirectorTelephone> DirectorTelephones { get; set; }
        public DbSet<PropertyDeedBuyer> PropertyDeedBuyers { get; set; }

        public DbSet<PropertyDeedSeller> PropertyDeedSellers { get; set; }

        public DbSet<ConsumerDebtReview> ConsumerDebtReviews { get; set; }

        public DbSet<ConsumerJudgement> ConsumerJudgements { get; set; }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Postalcode> Postalcodes { get; set; }
        public DbSet<Provinces> Provinces { get; set; }

        public DbSet<LastETLProcessedDate> LastETLProcessedDate { get; set; }

        public DbSet<Log> Log { get; set; }
        public DbSet<TelephoneCode> TelephoneCodes { get; set; }
        public DbSet<LSM> LSM { get; set; }
        public DbSet<Endorsement> Endorsements { get; set; }
        public DbSet<Dashboard> Dashboards { get; set; }
        public DbSet<ConsumerEmployment> ConsumerEmployments { get; set; }
        public DbSet<ConsumerOccupation> ConsumerOccupations { get; set; }
        public ProductionDbContext(DbContextOptions<ProductionDbContext> options) : base(options)
        {
            // Krishna commented to create db
            //Database.SetCommandTimeout(0);
        }

        public ProductionDbContext(string connectionString) : this(GetOptions(connectionString))
        {

        }

        private static DbContextOptions<ProductionDbContext> GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder<ProductionDbContext>(), connectionString).Options;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Consumer>().HasIndex(b => b.IDNO);
            modelBuilder.Entity<Consumer>().HasIndex(b => b.PassportNo);

            base.OnModelCreating(modelBuilder);
        }
        
    }
}
