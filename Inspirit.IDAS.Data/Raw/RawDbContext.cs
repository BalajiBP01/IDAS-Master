using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Inspirit.IDAS.Data.Raw
{
    public partial class RawDbContext : DbContext
    {
        public RawDbContext(DbContextOptions<RawDbContext> options)
            : base(options)
        {

        }
        public RawDbContext(string connectionString) : this(GetOptions(connectionString))
        {

        }

        private static DbContextOptions<RawDbContext> GetOptions(string connectionString)
        {
            return SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder<RawDbContext>(), connectionString).Options;
        }

        public virtual DbSet<AuditorAddress> AuditorAddress { get; set; }
        public virtual DbSet<AuditorHistory> AuditorHistory { get; set; }
        public virtual DbSet<Buyer> Buyer { get; set; }
        public virtual DbSet<Commercial> Commercial { get; set; }
        public virtual DbSet<CommercialAddress> CommercialAddress { get; set; }
        public virtual DbSet<CommercialAuditor> CommercialAuditor { get; set; }
        public virtual DbSet<CommercialCapital> CommercialCapital { get; set; }
        public virtual DbSet<CommercialDirector> CommercialDirector { get; set; }
        public virtual DbSet<CommercialJudgement> CommercialJudgement { get; set; }
        public virtual DbSet<CommercialName> CommercialName { get; set; }
        public virtual DbSet<CommercialTelephone> CommercialTelephone { get; set; }
        public virtual DbSet<CommercialVATInfo> CommercialVatinfo { get; set; }
        public virtual DbSet<Consumer> Consumer { get; set; }
        public virtual DbSet<ConsumerAddress> ConsumerAddress { get; set; }

        public virtual DbSet<ConsumerDebtReview> ConsumerDebtReview { get; set; }
        public virtual DbSet<ConsumerEmailConfirmed> ConsumerEmailConfirmed { get; set; }
        public virtual DbSet<ConsumerEmployment> ConsumerEmployment { get; set; }
        public virtual DbSet<ConsumerEmploymentOccupation> ConsumerEmploymentOccupation { get; set; }
        public virtual DbSet<ConsumerJudgement> ConsumerJudgement { get; set; }
        public virtual DbSet<ConsumerName> ConsumerName { get; set; }
        public virtual DbSet<ConsumerTelephone> ConsumerTelephone { get; set; }
        public virtual DbSet<Director> Director { get; set; }
        public virtual DbSet<DirectorAddress> DirectorAddress { get; set; }
        public virtual DbSet<DirectorName> DirectorName { get; set; }
        public virtual DbSet<DirectorTelephone> DirectorTelephone { get; set; }
        public virtual DbSet<Endorsement> Endorsement { get; set; }
        public virtual DbSet<HomeAffairs> HomeAffairs { get; set; }
        public virtual DbSet<HomeAffairsConfirmedMarried> HomeAffairMarried { get; set; }
        public virtual DbSet<HomeAffairsSingles> HomeAffairsSingles { get; set; }
        public virtual DbSet<PropertyDeed> PropertyDeed { get; set; }
        public virtual DbSet<Seller> Seller { get; set; }
        public virtual DbSet<Telephone> Telephone { get; set; }
        public virtual DbSet<Auditor> Auditor { get; set; }
        public virtual DbSet<ApplicationSetting> ApplicationSetting { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            modelBuilder.Entity<Telephone>().ToTable("Telephone");
           
            base.OnModelCreating(modelBuilder);
        }
    }
}