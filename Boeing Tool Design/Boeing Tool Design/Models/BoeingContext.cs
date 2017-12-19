namespace Boeing_Tool_Design.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class BoeingContext : DbContext
    {
        public BoeingContext() : base("name=BoeingContext") {}

        public virtual DbSet<AccessLevel> AccessLevels { get; set; }
        public virtual DbSet<AdjustmentFactor> AdjustmentFactors { get; set; }
        public virtual DbSet<AppUser> AppUsers { get; set; }
        public virtual DbSet<ComplexityLevel> ComplexityLevels { get; set; }
        public virtual DbSet<DesignOrder> DesignOrders { get; set; }
        public virtual DbSet<Estimate> Estimates { get; set; }
        public virtual DbSet<FamilyClass> FamilyClasses { get; set; }
        public virtual DbSet<Statistic> Statistics { get; set; }
        public virtual DbSet<StressWorkType> StressWorkTypes { get; set; }
        public virtual DbSet<ToolType> ToolTypes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AccessLevel>()
                .Property(e => e.CreatedDate)
                .HasPrecision(2);

            modelBuilder.Entity<AccessLevel>()
                .Property(e => e.UpdatedDate)
                .HasPrecision(2);

            modelBuilder.Entity<AdjustmentFactor>()
                .Property(e => e.CreatedDate)
                .HasPrecision(2);

            modelBuilder.Entity<AdjustmentFactor>()
                .Property(e => e.UpdatedDate)
                .HasPrecision(2);

            modelBuilder.Entity<AppUser>()
                .Property(e => e.CreatedDate)
                .HasPrecision(2);

            modelBuilder.Entity<AppUser>()
                .Property(e => e.UpdatedDate)
                .HasPrecision(2);

            modelBuilder.Entity<ComplexityLevel>()
                .Property(e => e.CreatedDate)
                .HasPrecision(2);

            modelBuilder.Entity<ComplexityLevel>()
                .Property(e => e.UpdatedDate)
                .HasPrecision(2);

            modelBuilder.Entity<DesignOrder>()
                .Property(e => e.CreatedDate)
                .HasPrecision(2);

            modelBuilder.Entity<DesignOrder>()
                .Property(e => e.UpdatedDate)
                .HasPrecision(2);

            modelBuilder.Entity<Estimate>()
                .Property(e => e.CreatedDate)
                .HasPrecision(2);

            modelBuilder.Entity<Estimate>()
                .Property(e => e.UpdatedDate)
                .HasPrecision(2);

            modelBuilder.Entity<FamilyClass>()
                .Property(e => e.CreatedDate)
                .HasPrecision(2);

            modelBuilder.Entity<FamilyClass>()
                .Property(e => e.UpdatedDate)
                .HasPrecision(2);

            modelBuilder.Entity<Statistic>()
                .Property(e => e.EffectiveStartDate)
                .HasPrecision(2);

            modelBuilder.Entity<Statistic>()
                .Property(e => e.EffectiveEndDate)
                .HasPrecision(2);

            modelBuilder.Entity<Statistic>()
                .Property(e => e.CreatedDate)
                .HasPrecision(2);

            modelBuilder.Entity<Statistic>()
                .Property(e => e.UpdatedDate)
                .HasPrecision(2);

            modelBuilder.Entity<StressWorkType>()
                .Property(e => e.CreatedDate)
                .HasPrecision(2);

            modelBuilder.Entity<StressWorkType>()
                .Property(e => e.UpdatedDate)
                .HasPrecision(2);

            modelBuilder.Entity<ToolType>()
                .Property(e => e.CreatedDate)
                .HasPrecision(2);

            modelBuilder.Entity<ToolType>()
                .Property(e => e.UpdatedDate)
                .HasPrecision(2);
        }
    }
}
