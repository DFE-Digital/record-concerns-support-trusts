using ConcernsCaseWork.Data.Configurations;
using ConcernsCaseWork.Data.Configurations.Decisions;
using ConcernsCaseWork.Data.Configurations.Decisions.DecisionOutcome;
using ConcernsCaseWork.Data.Conventions;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome;
using ConcernsCaseWork.Data.Models.Concerns.TeamCasework;
using Microsoft.EntityFrameworkCore;

namespace ConcernsCaseWork.Data
{
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
		
		public virtual DbSet<TrustFinancialForecast> TrustFinancialForecasts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {    
	        if (!optionsBuilder.IsConfigured)
	        {
		        optionsBuilder.UseConcernsSqlServer("Data Source=127.0.0.1;Initial Catalog=local_trams_test_db;persist security info=True;User id=sa; Password=StrongPassword905");
	        }
        }
        
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
	        configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
	        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

	        base.OnModelCreating(modelBuilder);
        }
    }
}