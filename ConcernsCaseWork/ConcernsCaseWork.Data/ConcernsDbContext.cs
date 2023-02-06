using Ardalis.GuardClauses;
using ConcernsCaseWork.Data.Conventions;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome;
using ConcernsCaseWork.Data.Models.Concerns.TeamCasework;
using ConcernsCaseWork.UserContext;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.AccessControl;

namespace ConcernsCaseWork.Data
{
	public partial class ConcernsDbContext : DbContext
	{
		private readonly IServerUserInfoService _userInfoService;

		public ConcernsDbContext()
		{
		}

		public ConcernsDbContext(DbContextOptions<ConcernsDbContext> options, IServerUserInfoService userInfoService)
			: base(options)
		{
			Guard.Against.Null(userInfoService);
			_userInfoService = userInfoService;
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

		public virtual DbSet<Audit> Audits { get; set; }

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

		public override int SaveChanges()
		{
			this.ChangeTracker.DetectChanges();

			if (string.IsNullOrWhiteSpace(_userInfoService.UserInfo?.Name) && Debugger.IsAttached)
			{
				Debugger.Break();
			}

			var userName = _userInfoService.UserInfo?.Name ?? "Unknown";
			List<(AuditChangeType AuditType, object Entity)> auditedEntities = ListAuditsNeeded();

			using (var transaction = Database.BeginTransaction())
			{
				// commit entities to get IDs written
				var entriesWritten = base.SaveChanges();

				// Write any audits needed to the dbset.
				InsertAuditsNeeded(userName, auditedEntities);

				// save them.
				var auditsWritten = base.SaveChanges();

				transaction.Commit();
				return entriesWritten + auditsWritten;
			}
		}

		/// <summary>
		/// Takes the list of audit entries needed and writes them to the dbset, ready to be saved.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="auditedEntities"></param>
		private void InsertAuditsNeeded(string userName, List<(AuditChangeType AuditType, object Entity)> auditedEntities)
		{
			foreach (var auditableEntity in auditedEntities)
			{
				this.Audits.Add(BuildAudit((IAuditable)auditableEntity.Entity, userName, auditableEntity.AuditType));
			}
		}

		/// <summary>
		/// Returns the audits needed - audit type and the entity being audited
		/// </summary>
		/// <returns></returns>
		private List<(AuditChangeType AuditType, object Entity)> ListAuditsNeeded()
		{
			List<(AuditChangeType AuditType, object Entity)> auditedEntities = new List<(AuditChangeType AuditType, object Entity)>();

			// remember what needs to be audited, before we save changes.
			auditedEntities.AddRange(
				this.ChangeTracker.Entries()
					.Where(t => t.State == EntityState.Added && t.Entity is IAuditable)
					.Select(t => (AuditChangeType.INSERT, t.Entity)));

			auditedEntities.AddRange(
				this.ChangeTracker.Entries()
					.Where(t => t.State == EntityState.Modified && t.Entity is IAuditable)
					.Select(t => (AuditChangeType.UPDATE, t.Entity)));
			return auditedEntities;
		}

		private static Audit BuildAudit(IAuditable entity, string userName, AuditChangeType changeType)
			=> new Audit(entity.GetType().Name, userName, DateTimeOffset.Now, changeType, entity.Serialise());
	}
}