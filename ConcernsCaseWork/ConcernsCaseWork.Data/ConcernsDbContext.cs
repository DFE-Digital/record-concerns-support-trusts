using Concerns.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Concerns.Data;

public partial class ConcernsDbContext : DbContext
{
    public ConcernsDbContext()
    {
    }

    public ConcernsDbContext(DbContextOptions<ConcernsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ConcernsCase> ConcernsCase { get; set; }
    public virtual DbSet<ConcernsMeansOfReferral> ConcernsMeansOfReferrals { get; set; }
    public virtual DbSet<ConcernsStatus> ConcernsStatus { get; set; }
    public virtual DbSet<ConcernsRecord> ConcernsRecord { get; set; }
    public virtual DbSet<ConcernsType> ConcernsTypes { get; set; }
    public virtual DbSet<ConcernsRating> ConcernsRatings { get; set; }
    public virtual DbSet<SRMAStatus> SRMAStatuses { get; set; }
    public virtual DbSet<SRMAReason> SRMAReasons { get; set; }
    public virtual DbSet<SRMACase> SRMACases { get; set; }
    public virtual DbSet<FinancialPlanStatus> FinancialPlanStatuses { get; set; }
    public virtual DbSet<FinancialPlanCase> FinancialPlanCases { get; set; }
    public virtual DbSet<NTIUnderConsiderationStatus> NTIUnderConsiderationStatuses { get; set; }
    public virtual DbSet<NTIUnderConsiderationReason> NTIUnderConsiderationReasons { get; set; }
    public virtual DbSet<NTIUnderConsideration> NTIUnderConsiderations { get; set; }
    public virtual DbSet<NTIUnderConsiderationReasonMapping> NTIUnderConsiderationReasonMappings { get; set; }
    public virtual DbSet<NTIWarningLetter> NTIWarningLetters { get; set; }
    public virtual DbSet<NTIWarningLetterConditionType> NTIWarningLetterConditionTypes { get; set; }
    public virtual DbSet<NTIWarningLetterCondition> NTIWarningLetterConditions { get; set; }
    public virtual DbSet<NTIWarningLetterReason> NTIWarningLetterReasons { get; set; }
    public virtual DbSet<NTIWarningLetterStatus> NTIWarningLetterStatuses { get; set; }
    public virtual DbSet<NTIWarningLetterReasonMapping> NTIWarningLetterReasonsMapping { get; set; }
    public virtual DbSet<NTIWarningLetterConditionMapping> NTIWarningLetterConditionsMapping { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
	        optionsBuilder.UseConcernsSqlServer("Data Source=127.0.0.1;Initial Catalog=local_trams_test_db;persist security info=True;User id=sa; Password=StrongPassword905");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
	    modelBuilder.HasDefaultSchema("concerns");
	    
		modelBuilder.HasSequence<int>("GlobalSequence").HasMin(1).StartsAt(1);

        modelBuilder.Entity<ConcernsCase>(entity =>
        {
            entity.ToTable("ConcernsCase");

            entity.HasKey(e => e.Id)
                .HasName("PK__CCase__C5B214360AF620234");

            entity.Property(e => e.Urn)
                .HasDefaultValueSql("NEXT VALUE FOR Concerns.GlobalSequence");

        });

        modelBuilder.Entity<ConcernsStatus>(entity =>
        {
            entity.ToTable("ConcernsStatus");

            entity.HasKey(e => e.Id)
                .HasName("PK__CStatus__C5B214360AF620234");

            entity.Property(e => e.Urn)
                .HasDefaultValueSql("NEXT VALUE FOR Concerns.GlobalSequence");

            entity.HasData(
                new ConcernsStatus
                {
                    Id = 1,
                    Name = "Live",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                },
                new ConcernsStatus
                {
                    Id = 2,
                    Name = "Monitoring",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                },
                new ConcernsStatus
                {
                    Id = 3,
                    Name = "Close",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                }
                );
        });

        modelBuilder.Entity<ConcernsRecord>(entity =>
        {
            entity.ToTable("ConcernsRecord");

            entity.HasKey(e => e.Id)
                .HasName("PK__CRecord");

            entity.Property(e => e.Urn)
                .HasDefaultValueSql("NEXT VALUE FOR Concerns.GlobalSequence");

            entity.HasOne(r => r.ConcernsCase)
                .WithMany(c => c.ConcernsRecords)
                .HasForeignKey(r => r.CaseId)
                .HasConstraintName("FK__ConcernsCase_ConcernsRecord");

            entity.HasOne(r => r.ConcernsType)
                .WithMany(c => c.FkConcernsRecord)
                .HasForeignKey(r => r.TypeId)
                .HasConstraintName("FK__ConcernsRecord_ConcernsType");

            entity.HasOne(r => r.ConcernsRating)
                .WithMany(c => c.FkConcernsRecord)
                .HasForeignKey(r => r.RatingId)
                .HasConstraintName("FK__ConcernsRecord_ConcernsRating");
            
            entity.HasOne(e => e.ConcernsMeansOfReferral)
                .WithMany(e => e.FkConcernsRecord)
                .HasForeignKey(e => e.MeansOfReferralId)
                .HasConstraintName("FK__ConcernsRecord_ConcernsMeansOfReferral");
        });

        modelBuilder.Entity<ConcernsType>(entity =>
        {
            entity.ToTable("ConcernsType");

            entity.HasKey(e => e.Id)
                .HasName("PK__CType");

            entity.Property(e => e.Urn)
                .HasDefaultValueSql("NEXT VALUE FOR Concerns.GlobalSequence");

            entity.HasData(
                new ConcernsType
                {
                    Id = 1,
                    Name = "Compliance",
                    Description = "Financial reporting",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                },
                new ConcernsType
                {
                    Id = 2,
                    Name = "Compliance",
                    Description = "Financial returns",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                },
                new ConcernsType
                {
                    Id = 3,
                    Name = "Financial",
                    Description = "Deficit",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                },
                new ConcernsType
                {
                    Id = 4,
                    Name = "Financial",
                    Description = "Projected deficit / Low future surplus",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                },
                new ConcernsType
                {
                    Id = 5,
                    Name = "Financial",
                    Description = "Cash flow shortfall",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                },
                new ConcernsType
                {
                    Id = 6,
                    Name = "Financial",
                    Description = "Clawback",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                },
                new ConcernsType
                {
                    Id = 7,
                    Name = "Force majeure",
                    Description = "",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                },
                new ConcernsType
                {
                    Id = 8,
                    Name = "Governance",
                    Description = "Governance",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                },
                new ConcernsType
                {
                    Id = 9,
                    Name = "Governance",
                    Description = "Closure",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                },
                new ConcernsType
                {
                    Id = 10,
                    Name = "Governance",
                    Description = "Executive Pay",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                },
                new ConcernsType
                {
                    Id = 11,
                    Name = "Governance",
                    Description = "Safeguarding",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                },
                new ConcernsType
                {
                    Id = 12,
                    Name = "Irregularity",
                    Description = "Allegations and self reported concerns",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                },
                new ConcernsType
                {
                    Id = 13,
                    Name = "Irregularity",
                    Description = "Related party transactions - in year",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                }
            );
        });
        modelBuilder.Entity<ConcernsMeansOfReferral>(entity =>
        {
            entity.ToTable("ConcernsMeansOfReferral");

            entity.HasKey(e => e.Id)
                .HasName("PK__CMeansOfReferral");
            
            entity.Property(e => e.Urn)
                .HasDefaultValueSql("NEXT VALUE FOR Concerns.GlobalSequence");

            entity.HasData(
                new ConcernsMeansOfReferral()
                {
                    Id = 1,
                    Name = "Internal",
                    Description = "ESFA activity, TFFT or other departmental activity",
                    CreatedAt = new DateTime(2022, 7, 28),
                    UpdatedAt = new DateTime(2022, 7, 28)
                },
                new ConcernsMeansOfReferral()
                {
                    Id = 2,
                    Name = "External",
                    Description = "CIU casework, whistleblowing, self reported, RSCs or other government bodies",
                    CreatedAt = new DateTime(2022, 7, 28),
                    UpdatedAt = new DateTime(2022, 7, 28)
                });
        });

        modelBuilder.Entity<ConcernsRating>(entity =>
        {
            entity.ToTable("ConcernsRating");

            entity.HasKey(e => e.Id)
                .HasName("PK__CRating");

            entity.Property(e => e.Urn)
                .HasDefaultValueSql("NEXT VALUE FOR Concerns.GlobalSequence");

            entity.HasData(
                new ConcernsRating
                {
                    Id = 1,
                    Name = "Red-Plus",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                },
                new ConcernsRating
                {
                    Id = 2,
                    Name = "Red",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                },
                new ConcernsRating
                {
                    Id = 3,
                    Name = "Red-Amber",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                },
                new ConcernsRating
                {
                    Id = 4,
                    Name = "Amber-Green",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                },
                new ConcernsRating
                {
                    Id = 5,
                    Name = "n/a",
                    CreatedAt = new DateTime(2021, 11, 17),
                    UpdatedAt = new DateTime(2021, 11, 17)
                });
        });

        modelBuilder.Entity<SRMAStatus>(entity =>
        {
            entity.ToTable("SRMAStatus");

            entity.HasData(
                Enum.GetValues(typeof(Enums.SRMAStatusEnum)).Cast<Enums.SRMAStatusEnum>()
                .Where(enm => enm != Enums.SRMAStatusEnum.Unknown)
                .Select(enm => new SRMAStatus
                {
                    Id = (int)enm,
                    Name = enm.ToString(),
                    CreatedAt = new DateTime(2022, 05, 06),
                    UpdatedAt = new DateTime(2022, 05, 06)
                }));
        });

        modelBuilder.Entity<SRMAReason>(entity =>
        {
            entity.ToTable("SRMAReason");

            entity.HasData(
                Enum.GetValues(typeof(Enums.SRMAReasonOfferedEnum)).Cast<Enums.SRMAReasonOfferedEnum>()
                .Where(enm => enm != Enums.SRMAReasonOfferedEnum.Unknown)
                .Select(enm => new SRMAStatus
                {
                    Id = (int)enm,
                    Name = enm.ToString(),
                    CreatedAt = new DateTime(2022, 05, 06),
                    UpdatedAt = new DateTime(2022, 05, 06)
                }));
        });


        modelBuilder.Entity<FinancialPlanStatus>(entity =>
        {
            var createdAt = new DateTime(2022, 06, 15);
            entity.ToTable("FinancialPlanStatus");

            entity.HasData(
                 new FinancialPlanStatus[]
                {
                    new() { Id = 1, Name = "AwaitingPlan", Description = "Awaiting Plan", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosedStatus = false },
                    new() { Id = 2, Name = "ReturnToTrust", Description = "Return To Trust", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosedStatus = false },
                    new() { Id = 3, Name = "ViablePlanReceived", Description = "Viable Plan Received", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosedStatus = true },
                    new() { Id = 4, Name = "Abandoned", Description = "Abandoned", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosedStatus = true }
                });
        });

        modelBuilder.Entity<NTIUnderConsiderationStatus>(entity =>
        {
            entity.ToTable("NTIUnderConsiderationStatus");

            var createdAt = new DateTime(2022, 07, 12);

            entity.HasData(
                 new NTIUnderConsiderationStatus[]
                {
                    new() { Id = 1, Name = "No further action being taken", Description = "", CreatedAt = createdAt, UpdatedAt = createdAt },
                    new() { Id = 2, Name = "To be escalated", Description="Warning letter or NTI can be set up using \"Add to case\".", CreatedAt = createdAt, UpdatedAt = createdAt }
                });

        });

        modelBuilder.Entity<NTIUnderConsiderationReason>(entity =>
        {
            entity.ToTable("NTIUnderConsiderationReason");

            var createdAt = new DateTime(2022, 07, 12);

            entity.HasData(
                 new NTIUnderConsiderationReason[]
                {
                    new() { Id = 1, Name = "Cash flow problems", CreatedAt = createdAt, UpdatedAt = createdAt },
                    new() { Id = 2, Name = "Cumulative deficit (actual)", CreatedAt = createdAt, UpdatedAt = createdAt },
                    new() { Id = 3, Name = "Cumulative deficit (projected)", CreatedAt = createdAt, UpdatedAt = createdAt },
                    new() { Id = 4, Name = "Governance concerns", CreatedAt = createdAt, UpdatedAt = createdAt },
                    new() { Id = 5, Name = "Non-Compliance with Academies Financial/Trust Handbook", CreatedAt = createdAt, UpdatedAt = createdAt },
                    new() { Id = 6, Name = "Non-Compliance with financial returns", CreatedAt = createdAt, UpdatedAt = createdAt },
                    new() { Id = 7, Name = "Risk of insolvency", CreatedAt = createdAt, UpdatedAt = createdAt },
                    new() { Id = 8, Name = "Safeguarding", CreatedAt = createdAt, UpdatedAt = createdAt }
                });
        });

        modelBuilder.Entity<NTIWarningLetterConditionType>(entity =>
        {
	        entity.ToTable("NTIWarningLetterConditionType");
	        
            var createdAt = new DateTime(2022, 07, 12);

            entity.HasData(
                 new NTIWarningLetterConditionType[]
                {
                    new() { Id = 1, Name = "Financial management conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 1 },
                    new() { Id = 2, Name = "Governance conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 2 },
                    new() { Id = 3, Name = "Compliance conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 3 },
                    new() { Id = 4, Name = "Standard conditions (mandatory)", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 4 }
                });
        });

        modelBuilder.Entity<NTIWarningLetterCondition>(entity =>
        {
	        entity.ToTable("NTIWarningLetterCondition");
	        
            var createdAt = new DateTime(2022, 07, 12);

            entity.HasData(
                 new NTIWarningLetterCondition[]
                {
                    new() { Id = 1, Name = "Trust financial plan", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 1, DisplayOrder = 1 },
                    new() { Id = 2, Name = "Action plan", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 2, DisplayOrder = 1  },
                    new() { Id = 3, Name = "Lines of accountability", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 2, DisplayOrder = 2  },
                    new() { Id = 4, Name = "Providing sufficient challenge", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 2, DisplayOrder = 3  },
                    new() { Id = 5, Name = "Scheme of delegation", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 2, DisplayOrder = 4  },
                    new() { Id = 6, Name = "Publishing requirements (compliance with)", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 3, DisplayOrder = 1  },
                    new() { Id = 7, Name = "Financial returns", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 4, DisplayOrder = 1  }
                });
        });

        modelBuilder.Entity<NTIWarningLetterReason>(entity =>
        {
	        entity.ToTable("NTIWarningLetterReason");
	        
            var createdAt = new DateTime(2022, 07, 12);

            entity.HasData(
                 new NTIWarningLetterReason[]
                {
                    new() { Id = 1, Name = "Cash flow problems", CreatedAt = createdAt, UpdatedAt = createdAt },
                    new() { Id = 2, Name = "Cumulative deficit (actual)", CreatedAt = createdAt, UpdatedAt = createdAt },
                    new() { Id = 3, Name = "Cumulative deficit (projected)", CreatedAt = createdAt, UpdatedAt = createdAt },
                    new() { Id = 4, Name = "Governance concerns", CreatedAt = createdAt, UpdatedAt = createdAt },
                    new() { Id = 5, Name = "Non-compliance with Academies Financial/Trust Handbook", CreatedAt = createdAt, UpdatedAt = createdAt },
                    new() { Id = 6, Name = "Non-compliance with financial returns", CreatedAt = createdAt, UpdatedAt = createdAt },
                    new() { Id = 7, Name = "Risk of insolvency", CreatedAt = createdAt, UpdatedAt = createdAt },
                    new() { Id = 8, Name = "Safeguarding", CreatedAt = createdAt, UpdatedAt = createdAt }
                });
        });

        modelBuilder.Entity<NTIWarningLetterStatus>(entity =>
        {
	        entity.ToTable("NTIWarningLetterStatus");
	        
            var createdAt = new DateTime(2022, 07, 12);

            entity.HasData(
                 new NTIWarningLetterStatus[]
                {
                    new() { Id = 1, Name = "Preparing warning letter", Description = "", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false },
                    new() { Id = 2, Name = "Sent to trust", Description = "", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false },
                    new() { Id = 3, Name = "Cancel warning letter", Description="The warning letter is no longer needed.", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = true },
                    new() { Id = 4, Name = "Conditions met", Description="You are satisfied that all the conditions have been, or will be, met as outlined in the letter", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = true },
                    new() { Id = 5, Name = "Escalate to Notice To Improve", Description="Conditions have not been met. Close NTI: Warning letter and begin NTI on case page using \"Add to case\".", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = true }
                });
        });

        modelBuilder.Entity<NTIUnderConsiderationReasonMapping>()
            .HasOne(n => n.NTIUnderConsideration)
            .WithMany(n => n.UnderConsiderationReasonsMapping)
            .HasForeignKey(n => n.NTIUnderConsiderationId);

        modelBuilder.Entity<NTIUnderConsiderationReasonMapping>()
            .HasOne(n => n.NTIUnderConsiderationReason)
            .WithMany(n => n.UnderConsiderationReasonsMapping)
            .HasForeignKey(n => n.NTIUnderConsiderationReasonId);

        modelBuilder.Entity<NTIWarningLetterReasonMapping>()
            .HasOne(n => n.NTIWarningLetter)
            .WithMany(n => n.WarningLetterReasonsMapping)
            .HasForeignKey(n => n.NTIWarningLetterId);

        modelBuilder.Entity<NTIWarningLetterReasonMapping>()
            .HasOne(n => n.NTIWarningLetterReason)
            .WithMany(n => n.WarningLetterReasonsMapping)
            .HasForeignKey(n => n.NTIWarningLetterReasonId);

        modelBuilder.Entity<NTIWarningLetterConditionMapping>()
            .HasOne(n => n.NTIWarningLetter)
            .WithMany(n => n.WarningLetterConditionsMapping)
            .HasForeignKey(n => n.NTIWarningLetterId);

        modelBuilder.Entity<NTIWarningLetterConditionMapping>()
            .HasOne(n => n.NTIWarningLetterCondition)
            .WithMany(n => n.WarningLetterConditionsMapping)
            .HasForeignKey(n => n.NTIWarningLetterConditionId);
    }

}