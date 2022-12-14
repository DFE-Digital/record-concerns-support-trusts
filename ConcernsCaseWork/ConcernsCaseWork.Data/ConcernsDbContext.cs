using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome;
using ConcernsCaseWork.Data.Models.Concerns.TeamCasework;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using DecisionOutcomeStatus = ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome.DecisionOutcomeStatus;

namespace ConcernsCaseWork.Data
{
    public partial class ConcernsDbContext : DbContext
    {
		const string TablePrefix = "concerns";
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
        public virtual DbSet<NoticeToImprove> NoticesToImprove { get; set; }
        public virtual DbSet<NoticeToImproveConditionType> NoticeToImproveConditionTypes { get; set; }
        public virtual DbSet<NoticeToImproveCondition> NoticeToImproveConditions { get; set; }
        public virtual DbSet<NoticeToImproveReason> NoticeToImproveReasons { get; set; }
        public virtual DbSet<NoticeToImproveStatus> NoticeToImproveStatuses { get; set; }
        public virtual DbSet<NoticeToImproveReasonMapping> NoticeToImproveReasonsMappings { get; set; }
        public virtual DbSet<NoticeToImproveConditionMapping> NoticeToImproveConditionsMappings { get; set; }
        public virtual DbSet<ConcernsCaseworkTeam> ConcernsTeamCaseworkTeam { get; set; }
        public virtual DbSet<ConcernsCaseworkTeamMember> ConcernsTeamCaseworkTeamMember { get; set; }
		public virtual DbSet<DecisionOutcome> DecisionOutcomes { get; set; }
		public virtual DbSet<Decision> Decisions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {    
	        if (!optionsBuilder.IsConfigured)
	        {
		        optionsBuilder.UseConcernsSqlServer("Data Source=127.0.0.1;Initial Catalog=local_trams_test_db;persist security info=True;User id=sa; Password=StrongPassword905");
	        }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConcernsCase>(entity =>
            {
                entity.ToTable("ConcernsCase", "concerns");

                entity.HasKey(e => e.Id)
                    .HasName("PK__CCase__C5B214360AF620234");

                entity.Property(e => e.Urn)
	                .HasComputedColumnSql("[Id]");

                entity.HasMany(x => x.Decisions)
                    .WithOne();

                entity.Property(e => e.Territory)
	                .HasConversion<string>();
            });

            modelBuilder.Entity<ConcernsStatus>(entity =>
            {
                entity.ToTable("ConcernsStatus", "concerns");

                entity.HasKey(e => e.Id)
                    .HasName("PK__CStatus__C5B214360AF620234");

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
                entity.ToTable("ConcernsRecord", "concerns");

                entity.HasKey(e => e.Id)
                    .HasName("PK__CRecord");

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
                entity.ToTable("ConcernsType", "concerns");

                entity.HasKey(e => e.Id)
                    .HasName("PK__CType");

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
                        Description = null,
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
                entity.ToTable("ConcernsMeansOfReferral", "concerns");

                entity.HasKey(e => e.Id)
                    .HasName("PK__CMeansOfReferral");

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
                        Description = "CIU casework, whistleblowing, self reported, regional director (RD) or other government bodies",
                        CreatedAt = new DateTime(2022, 7, 28),
                        UpdatedAt = new DateTime(2022, 11, 09)
                    });
            });

            modelBuilder.Entity<ConcernsRating>(entity =>
            {
                entity.ToTable("ConcernsRating", "concerns");

                entity.HasKey(e => e.Id)
                    .HasName("PK__CRating");

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
                entity.ToTable("SRMAStatus", "concerns");

                entity.HasData(
                    Enum.GetValues(typeof(Enums.SRMAStatus)).Cast<Enums.SRMAStatus>()
                    .Where(enm => enm != Enums.SRMAStatus.Unknown)
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
                entity.ToTable("SRMAReason", "concerns");

                entity.HasData(
                    Enum.GetValues(typeof(Enums.SRMAReasonOffered)).Cast<Enums.SRMAReasonOffered>()
                    .Where(enm => enm != Enums.SRMAReasonOffered.Unknown)
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
                entity.ToTable("FinancialPlanStatus", "concerns");

                entity.HasData(
                     new FinancialPlanStatus[]
                    {
                        new FinancialPlanStatus{ Id = 1, Name = "AwaitingPlan", Description = "Awaiting Plan", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosedStatus = false },
                        new FinancialPlanStatus{ Id = 2, Name = "ReturnToTrust", Description = "Return To Trust", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosedStatus = false },
                        new FinancialPlanStatus{ Id = 3, Name = "ViablePlanReceived", Description = "Viable Plan Received", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosedStatus = true },
                        new FinancialPlanStatus{ Id = 4, Name = "Abandoned", Description = "Abandoned", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosedStatus = true }
                    });
            });

            modelBuilder.Entity<NTIUnderConsiderationStatus>(entity =>
            {
                entity.ToTable("NTIUnderConsiderationStatus", "concerns");

                var createdAt = new DateTime(2022, 07, 12);

                entity.HasData(
                     new NTIUnderConsiderationStatus[]
                    {
                        new NTIUnderConsiderationStatus{ Id = 1, Name = "No further action being taken", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NTIUnderConsiderationStatus{ Id = 2, Name = "To be escalated", Description="Warning letter or NTI can be set up using \"Add to case\".", CreatedAt = createdAt, UpdatedAt = createdAt }
                    });
            });

            modelBuilder.Entity<NTIUnderConsiderationReason>(entity =>
            {
                entity.ToTable("NTIUnderConsiderationReason", "concerns");

                var createdAt = new DateTime(2022, 07, 12);

                entity.HasData(
                     new NTIUnderConsiderationReason[]
                    {
                        new NTIUnderConsiderationReason{ Id = 1, Name = "Cash flow problems", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NTIUnderConsiderationReason{ Id = 2, Name = "Cumulative deficit (actual)", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NTIUnderConsiderationReason{ Id = 3, Name = "Cumulative deficit (projected)", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NTIUnderConsiderationReason{ Id = 4, Name = "Governance concerns", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NTIUnderConsiderationReason{ Id = 5, Name = "Non-Compliance with Academies Financial/Trust Handbook", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NTIUnderConsiderationReason{ Id = 6, Name = "Non-Compliance with financial returns", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NTIUnderConsiderationReason{ Id = 7, Name = "Risk of insolvency", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NTIUnderConsiderationReason{ Id = 8, Name = "Safeguarding", CreatedAt = createdAt, UpdatedAt = createdAt }
                    });
            });

            modelBuilder.Entity<NTIWarningLetterConditionType>(entity =>
            {
                var createdAt = new DateTime(2022, 07, 12);

                entity.HasData(
                     new NTIWarningLetterConditionType[]
                    {
                        new NTIWarningLetterConditionType{ Id = 1, Name = "Financial management conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 1 },
                        new NTIWarningLetterConditionType{ Id = 2, Name = "Governance conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 2 },
                        new NTIWarningLetterConditionType{ Id = 3, Name = "Compliance conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 3 },
                        new NTIWarningLetterConditionType{ Id = 4, Name = "Standard conditions (mandatory)", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 4 }
                    });
            });

            modelBuilder.Entity<NTIWarningLetterCondition>(entity =>
            {
                var createdAt = new DateTime(2022, 07, 12);

                entity.HasData(
                     new NTIWarningLetterCondition[]
                    {
                        new NTIWarningLetterCondition{ Id = 1, Name = "Trust financial plan", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 1, DisplayOrder = 1 },
                        new NTIWarningLetterCondition{ Id = 2, Name = "Action plan", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 2, DisplayOrder = 1  },
                        new NTIWarningLetterCondition{ Id = 3, Name = "Lines of accountability", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 2, DisplayOrder = 2  },
                        new NTIWarningLetterCondition{ Id = 4, Name = "Providing sufficient challenge", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 2, DisplayOrder = 3  },
                        new NTIWarningLetterCondition{ Id = 5, Name = "Scheme of delegation", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 2, DisplayOrder = 4  },
                        new NTIWarningLetterCondition{ Id = 6, Name = "Publishing requirements (compliance with)", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 3, DisplayOrder = 1  },
                        new NTIWarningLetterCondition{ Id = 7, Name = "Financial returns", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = 4, DisplayOrder = 1  }
                    });
            });

            modelBuilder.Entity<NTIWarningLetterReason>(entity =>
            {
                var createdAt = new DateTime(2022, 07, 12);

                entity.HasData(
                     new NTIWarningLetterReason[]
                    {
                        new NTIWarningLetterReason{ Id = 1, Name = "Cash flow problems", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NTIWarningLetterReason{ Id = 2, Name = "Cumulative deficit (actual)", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NTIWarningLetterReason{ Id = 3, Name = "Cumulative deficit (projected)", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NTIWarningLetterReason{ Id = 4, Name = "Governance concerns", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NTIWarningLetterReason{ Id = 5, Name = "Non-compliance with Academies Financial/Trust Handbook", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NTIWarningLetterReason{ Id = 6, Name = "Non-compliance with financial returns", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NTIWarningLetterReason{ Id = 7, Name = "Risk of insolvency", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NTIWarningLetterReason{ Id = 8, Name = "Safeguarding", CreatedAt = createdAt, UpdatedAt = createdAt }
                    });
            });

            modelBuilder.Entity<NTIWarningLetterStatus>(entity =>
            {
                var createdAt = new DateTime(2022, 07, 12);

                entity.HasData(
                     new NTIWarningLetterStatus[]
                    {
                        new NTIWarningLetterStatus{ Id = 1, Name = "Preparing warning letter", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false, PastTenseName="" },
                        new NTIWarningLetterStatus{ Id = 2, Name = "Sent to trust", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false, PastTenseName="" },
                        new NTIWarningLetterStatus{ Id = 3, Name = "Cancel warning letter", Description="The warning letter is no longer needed.", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = true, PastTenseName="Cancelled" },
                        new NTIWarningLetterStatus{ Id = 4, Name = "Conditions met", Description="You are satisfied that all the conditions have been, or will be, met as outlined in the letter", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = true, PastTenseName="Conditions met" },
                        new NTIWarningLetterStatus{ Id = 5, Name = "Escalate to Notice To Improve", Description="Conditions have not been met. Close NTI: Warning letter and begin NTI on case page using \"Add to case\".", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = true, PastTenseName="Escalated to Notice To Improve" }
                    });
            });

            modelBuilder.Entity<NoticeToImproveStatus>(entity =>
            {
                var createdAt = new DateTime(2022, 08, 17);

                entity.HasData(
                     new NoticeToImproveStatus[]
                    {
                        new NoticeToImproveStatus{ Id = 1, Name = "Preparing NTI", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false },
                        new NoticeToImproveStatus{ Id = 2, Name = "Issued NTI", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false },
                        new NoticeToImproveStatus{ Id = 3, Name = "Progress on track", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false},
                        new NoticeToImproveStatus{ Id = 4, Name = "Evidence of NTI non-compliance", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false },
                        new NoticeToImproveStatus{ Id = 5, Name = "Serious NTI breaches - considering escalation to TWN", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false },
                        new NoticeToImproveStatus{ Id = 6, Name = "Submission to lift NTI in progress", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false },
                        new NoticeToImproveStatus{ Id = 7, Name = "Submission to close NTI in progress", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = false },
                        new NoticeToImproveStatus{ Id = 8, Name = "Lifted", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = true },
                        new NoticeToImproveStatus{ Id = 9, Name = "Closed", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = true },
                        new NoticeToImproveStatus{ Id = 10, Name = "Cancelled", CreatedAt = createdAt, UpdatedAt = createdAt, IsClosingState = true }
                    });
            });

            modelBuilder.Entity<NoticeToImproveReason>(entity =>
            {
                var createdAt = new DateTime(2022, 08, 17);

                entity.HasData(
                     new NoticeToImproveReason[]
                    {
                        new NoticeToImproveReason{ Id = 1, Name = "Cash flow problems", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NoticeToImproveReason{ Id = 2, Name = "Cumulative deficit (actual)", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NoticeToImproveReason{ Id = 3, Name = "Cumulative deficit (projected)", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NoticeToImproveReason{ Id = 4, Name = "Governance concerns", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NoticeToImproveReason{ Id = 5, Name = "Non-compliance with Academies Financial/Trust Handbook", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NoticeToImproveReason{ Id = 6, Name = "Non-compliance with financial returns", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NoticeToImproveReason{ Id = 7, Name = "Risk of insolvency", CreatedAt = createdAt, UpdatedAt = createdAt },
                        new NoticeToImproveReason{ Id = 8, Name = "Safeguarding", CreatedAt = createdAt, UpdatedAt = createdAt }
                    });
            });

            modelBuilder.Entity<NoticeToImproveConditionType>(entity =>
            {
                var createdAt = new DateTime(2022, 07, 12);

                entity.HasData(
                     new NoticeToImproveConditionType[]
                    {
                        new NoticeToImproveConditionType{ Id = 1, Name = "Financial management conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 1 },
                        new NoticeToImproveConditionType{ Id = 2, Name = "Governance conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 2 },
                        new NoticeToImproveConditionType{ Id = 3, Name = "Compliance conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 3 },
                        new NoticeToImproveConditionType{ Id = 4, Name = "Safeguarding conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 4 },
                        new NoticeToImproveConditionType{ Id = 5, Name = "Fraud and irregularity", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 5 },
                        new NoticeToImproveConditionType{ Id = 6, Name = "Standard conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 6 },
                        new NoticeToImproveConditionType{ Id = 7, Name = "Additional Financial Support conditions", CreatedAt = createdAt, UpdatedAt = createdAt, DisplayOrder = 7 }
                    });
            });

            modelBuilder.Entity<NoticeToImproveCondition>(entity =>
            {
                var createdAt = new DateTime(2022, 08, 17);

                var financialManagementConditionTypeId = 1;
                var governanceConditionTypeId = 2;
                var complianceConditionTypeId = 3;
                var safeguardingConditionTypeId = 4;
                var fraudAndIrregularityConditionTypeId = 5;
                var standardConditionTypeId = 6;
                var additionalFinancialSupportConditionTypeId = 7;

                entity.HasData(
                     new NoticeToImproveCondition[]
                    {
                        new NoticeToImproveCondition{ Id = 1, Name = "Audit and risk committee", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = financialManagementConditionTypeId, DisplayOrder = 1 },
                        new NoticeToImproveCondition{ Id = 2, Name = "Internal audit findings", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = financialManagementConditionTypeId, DisplayOrder = 2 },
                        new NoticeToImproveCondition{ Id = 3, Name = "Trust financial plan", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = financialManagementConditionTypeId, DisplayOrder = 3  },
                        new NoticeToImproveCondition{ Id = 4, Name = "Financial management and governance review", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = financialManagementConditionTypeId, DisplayOrder = 4 },
                        new NoticeToImproveCondition{ Id = 5, Name = "Financial systems & controls and internal scrutiny", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = financialManagementConditionTypeId, DisplayOrder = 5 },
                        new NoticeToImproveCondition{ Id = 6, Name = "Integrated curriculum and financial planning", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = financialManagementConditionTypeId, DisplayOrder = 6 },
                        new NoticeToImproveCondition{ Id = 7, Name = "Monthly management accounts", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = financialManagementConditionTypeId, DisplayOrder = 7 },
                        new NoticeToImproveCondition{ Id = 8, Name = "National deals for schools", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = financialManagementConditionTypeId, DisplayOrder = 8 },
                        new NoticeToImproveCondition{ Id = 9, Name = "School resource management", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = financialManagementConditionTypeId, DisplayOrder = 9 },

                        new NoticeToImproveCondition{ Id = 10, Name = "Academy ambassadors", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 1 },
                        new NoticeToImproveCondition{ Id = 11, Name = "Academy transfer", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 2 },
                        new NoticeToImproveCondition{ Id = 12, Name = "Action plan", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 3 },
                        new NoticeToImproveCondition{ Id = 13, Name = "AGM of members", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 4 },
                        new NoticeToImproveCondition{ Id = 14, Name = "Board meetings", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 5 },
                        new NoticeToImproveCondition{ Id = 15, Name = "Independant review of governance", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 6 },
                        new NoticeToImproveCondition{ Id = 16, Name = "Lines of accountability", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 7 },
                        new NoticeToImproveCondition{ Id = 17, Name = "Providing sufficient challenge", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 8 },
                        new NoticeToImproveCondition{ Id = 18, Name = "Scheme of delegation", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 9 },
                        new NoticeToImproveCondition{ Id = 19, Name = "School improvement", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 10 },
                        new NoticeToImproveCondition{ Id = 20, Name = "Strengthen governance", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = governanceConditionTypeId, DisplayOrder = 11 },

                        new NoticeToImproveCondition{ Id = 21, Name = "Admissions", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = complianceConditionTypeId, DisplayOrder = 1 },
                        new NoticeToImproveCondition{ Id = 22, Name = "Excessive executive payments (high pay)", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = complianceConditionTypeId, DisplayOrder = 2 },
                        new NoticeToImproveCondition{ Id = 23, Name = "Publishing requirements (compliance with)", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = complianceConditionTypeId, DisplayOrder = 3 },
                        new NoticeToImproveCondition{ Id = 24, Name = "Related party Transactions (RPTs)", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = complianceConditionTypeId, DisplayOrder = 4 },

                        new NoticeToImproveCondition{ Id = 25, Name = "Review and update safeguarding policies", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = safeguardingConditionTypeId, DisplayOrder = 1 },
                        new NoticeToImproveCondition{ Id = 26, Name = "Commission external review of safeguarding", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = safeguardingConditionTypeId, DisplayOrder = 2 },
                        new NoticeToImproveCondition{ Id = 27, Name = "Appoint trustee with leadership responsibility for safeguarding", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = safeguardingConditionTypeId, DisplayOrder = 3 },
                        new NoticeToImproveCondition{ Id = 28, Name = "Safeguarding recruitment process", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = safeguardingConditionTypeId, DisplayOrder = 4 },

                        new NoticeToImproveCondition{ Id = 29, Name = "Novel, contentious, and/or repercussive transactions", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = fraudAndIrregularityConditionTypeId, DisplayOrder = 1 },
                        new NoticeToImproveCondition{ Id = 30, Name = "Off-payroll payments", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = fraudAndIrregularityConditionTypeId, DisplayOrder = 2 },
                        new NoticeToImproveCondition{ Id = 31, Name = "Procurement policy", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = fraudAndIrregularityConditionTypeId, DisplayOrder = 3 },
                        new NoticeToImproveCondition{ Id = 32, Name = "Register of interests", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = fraudAndIrregularityConditionTypeId, DisplayOrder = 4 },

                        new NoticeToImproveCondition{ Id = 33, Name = "Financial returns", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = standardConditionTypeId, DisplayOrder = 1 },
                        new NoticeToImproveCondition{ Id = 34, Name = "Delegated freedoms", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = standardConditionTypeId, DisplayOrder = 2 },
                        new NoticeToImproveCondition{ Id = 35, Name = "Trustee contact details", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = standardConditionTypeId, DisplayOrder = 3 },

                        new NoticeToImproveCondition{ Id = 36, Name = "Review of board and executive team capability", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = additionalFinancialSupportConditionTypeId, DisplayOrder = 1 },
                        new NoticeToImproveCondition{ Id = 37, Name = "Academy transfer (lower risk)", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = additionalFinancialSupportConditionTypeId, DisplayOrder = 2 },
                        new NoticeToImproveCondition{ Id = 38, Name = "Move to latest model funding agreement", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = additionalFinancialSupportConditionTypeId, DisplayOrder = 3 },
                        new NoticeToImproveCondition{ Id = 39, Name = "Qualified Floating Charge (QFC)", CreatedAt = createdAt, UpdatedAt = createdAt, ConditionTypeId = additionalFinancialSupportConditionTypeId, DisplayOrder = 4 }
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

            modelBuilder.Entity<NoticeToImproveConditionMapping>()
                .HasOne(n => n.NoticeToImprove)
                .WithMany(n => n.NoticeToImproveConditionsMapping)
                .HasForeignKey(n => n.NoticeToImproveId);

            modelBuilder.Entity<NoticeToImproveConditionMapping>()
                .HasOne(n => n.NoticeToImproveCondition)
                .WithMany(n => n.NoticeToImproveConditionsMapping)
                .HasForeignKey(n => n.NoticeToImproveConditionId);

            modelBuilder.Entity<NoticeToImproveReasonMapping>()
                .HasOne(n => n.NoticeToImprove)
                .WithMany(n => n.NoticeToImproveReasonsMapping)
                .HasForeignKey(n => n.NoticeToImproveId);

            modelBuilder.Entity<NoticeToImproveReasonMapping>()
                .HasOne(n => n.NoticeToImproveReason)
                .WithMany(n => n.NoticeToImproveReasonsMapping)
                .HasForeignKey(n => n.NoticeToImproveReasonId);

            modelBuilder.Entity<ConcernsCaseworkTeam>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasMany(n => n.TeamMembers)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Decision>(e =>
            {
	            e.ToTable("ConcernsDecision", "concerns");
	            e.HasKey(x => x.DecisionId);
	            e.Property(x => x.TotalAmountRequested).HasColumnType("money");
	            e.HasMany(x => x.DecisionTypes).WithOne();
				e.HasOne(x => x.Outcome);
            });

            modelBuilder.Entity<DecisionType>(e =>
            {
	            e.ToTable("ConcernsDecisionType", "concerns");
	            e.HasKey(x => new { x.DecisionId, x.DecisionTypeId });
            });

            modelBuilder.Entity<DecisionTypeId>(e =>
            {
	            e.ToTable("ConcernsDecisionTypeId", "concerns");
	            e.HasKey(x => x.Id);
	            e.HasData(
		            Enum.GetValues(typeof(Enums.Concerns.DecisionType)).Cast<Enums.Concerns.DecisionType>()
			            .Select(enm => new DecisionTypeId(enm, enm.ToString())));
            });

            modelBuilder.Entity<DecisionStatus>(e =>
            {
	            e.ToTable("ConcernsDecisionStates", "concerns");
	            e.HasKey(x => x.Id);
	            e.HasData(
		            Enum.GetValues(typeof(Enums.Concerns.DecisionStatus)).Cast<Enums.Concerns.DecisionStatus>()
			            .Select(enm => new DecisionStatus(enm) { Name = enm.ToString() }));
            });

			BuildDecisionOutcomeTable(modelBuilder);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

		private void BuildDecisionOutcomeTable(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<DecisionOutcome>(e =>
			{
				e.ToTable("DecisionOutcome", TablePrefix);
				e.HasKey(x => x.DecisionOutcomeId);
				e.Property(x => x.TotalAmount).HasColumnType("money");
				e.HasMany(x => x.BusinessAreasConsulted).WithOne();
			});

			modelBuilder.Entity<DecisionOutcomeBusinessAreaMapping>(e =>
			{
				e.ToTable("DecisionOutcomeBusinessAreaMapping", TablePrefix);
				e.HasKey(x => new { x.DecisionOutcomeId, x.DecisionOutcomeBusinessId });
			});

			modelBuilder.Entity<DecisionOutcomeStatus>(e =>
			{
				e.ToTable("DecisionOutcomeStatus", TablePrefix);
				e.HasKey(x => x.Id);
				e.HasData(
					Enum
						.GetValues(typeof(API.Contracts.Decisions.Outcomes.DecisionOutcomeStatus))
						.Cast<API.Contracts.Decisions.Outcomes.DecisionOutcomeStatus>()
						.Select(enm => new DecisionOutcomeStatus() { Id = enm, Name = enm.ToString() }));
			});

			modelBuilder.Entity<DecisionOutcomeAuthorizer>(e =>
			{
				e.ToTable("DecisionOutcomeAuthorizer", TablePrefix);
				e.HasKey(x => x.Id);
				e.HasData(
					Enum
						.GetValues(typeof(API.Contracts.Decisions.Outcomes.DecisionOutcomeAuthorizer))
						.Cast<API.Contracts.Decisions.Outcomes.DecisionOutcomeAuthorizer>()
						.Select(enm => new DecisionOutcomeAuthorizer() { Id = enm, Name = enm.ToString() }));
			});

			modelBuilder.Entity<DecisionOutcomeBusinessArea>(e =>
			{
				e.ToTable("DecisionOutcomeBusinessArea", TablePrefix);
				e.HasKey(x => x.Id);
				e.HasData(
				Enum
					.GetValues(typeof(API.Contracts.Decisions.Outcomes.DecisionOutcomeBusinessArea))
					.Cast<API.Contracts.Decisions.Outcomes.DecisionOutcomeBusinessArea>()
					.Select(enm => new DecisionOutcomeBusinessArea() { Id = enm, Name = enm.ToString() }));
			});
		}
    }
}