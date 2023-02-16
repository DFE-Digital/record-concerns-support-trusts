using Ardalis.GuardClauses;
using ConcernsCaseWork.Data.Auditing;
using ConcernsCaseWork.Data.Conventions;
using ConcernsCaseWork.Data.Models;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions;
using ConcernsCaseWork.Data.Models.Concerns.Case.Management.Actions.Decisions.Outcome;
using ConcernsCaseWork.Data.Models.Concerns.TeamCasework;
using ConcernsCaseWork.UserContext;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Transactions;

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

		public virtual DbSet<Audit> Audits { get; set; }
		public virtual DbSet<ConcernsCase> ConcernsCase { get; set; }
		public virtual DbSet<ConcernsMeansOfReferral> ConcernsMeansOfReferrals { get; set; }
		public virtual DbSet<ConcernsRating> ConcernsRatings { get; set; }
		public virtual DbSet<ConcernsRecord> ConcernsRecord { get; set; }
		public virtual DbSet<ConcernsStatus> ConcernsStatus { get; set; }
		public virtual DbSet<ConcernsCaseworkTeam> ConcernsTeamCaseworkTeam { get; set; }
		public virtual DbSet<ConcernsCaseworkTeamMember> ConcernsTeamCaseworkTeamMember { get; set; }
		public virtual DbSet<ConcernsType> ConcernsTypes { get; set; }
		public virtual DbSet<DecisionOutcome> DecisionOutcomes { get; set; }
		public virtual DbSet<Decision> Decisions { get; set; }
		public virtual DbSet<FinancialPlanCase> FinancialPlanCases { get; set; }
		public virtual DbSet<FinancialPlanStatus> FinancialPlanStatuses { get; set; }
		public virtual DbSet<NoticeToImprove> NoticesToImprove { get; set; }
		public virtual DbSet<NoticeToImproveCondition> NoticeToImproveConditions { get; set; }
		public virtual DbSet<NoticeToImproveConditionMapping> NoticeToImproveConditionsMappings { get; set; }
		public virtual DbSet<NoticeToImproveConditionType> NoticeToImproveConditionTypes { get; set; }
		public virtual DbSet<NoticeToImproveReason> NoticeToImproveReasons { get; set; }
		public virtual DbSet<NoticeToImproveReasonMapping> NoticeToImproveReasonsMappings { get; set; }
		public virtual DbSet<NoticeToImproveStatus> NoticeToImproveStatuses { get; set; }
		public virtual DbSet<NTIUnderConsiderationReasonMapping> NTIUnderConsiderationReasonMappings { get; set; }
		public virtual DbSet<NTIUnderConsiderationReason> NTIUnderConsiderationReasons { get; set; }
		public virtual DbSet<NTIUnderConsideration> NTIUnderConsiderations { get; set; }
		public virtual DbSet<NTIUnderConsiderationStatus> NTIUnderConsiderationStatuses { get; set; }
		public virtual DbSet<NTIWarningLetterCondition> NTIWarningLetterConditions { get; set; }
		public virtual DbSet<NTIWarningLetterConditionMapping> NTIWarningLetterConditionsMapping { get; set; }
		public virtual DbSet<NTIWarningLetterConditionType> NTIWarningLetterConditionTypes { get; set; }
		public virtual DbSet<NTIWarningLetterReason> NTIWarningLetterReasons { get; set; }
		public virtual DbSet<NTIWarningLetterReasonMapping> NTIWarningLetterReasonsMapping { get; set; }
		public virtual DbSet<NTIWarningLetter> NTIWarningLetters { get; set; }
		public virtual DbSet<NTIWarningLetterStatus> NTIWarningLetterStatuses { get; set; }
		public virtual DbSet<SRMACase> SRMACases { get; set; }
		public virtual DbSet<SRMAReason> SRMAReasons { get; set; }
		public virtual DbSet<SRMAStatus> SRMAStatuses { get; set; }
		public virtual DbSet<TrustFinancialForecast> TrustFinancialForecasts { get; set; }

		public override int SaveChanges(bool acceptAllChangesOnSuccess)
		{
			var userName = GetCurrentUsername();
			var auditedEntities = GetAuditsNeeded();
			int auditsWritten = 0;

			using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
			{
				// commit changes to get IDs for new records. Then commit audits as they refer to those IDs. Use transaction to make the two commits atomic.
				var entriesWritten = base.SaveChanges(acceptAllChangesOnSuccess);
				InsertAuditsNeeded(userName, auditedEntities);

				if (auditedEntities.Count > 0)
				{
					auditsWritten = base.SaveChanges(acceptAllChangesOnSuccess);
				}
				scope.Complete();

				return entriesWritten + auditsWritten;
			}
		}

		public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
		{
			var userName = GetCurrentUsername();
			var auditedEntities = GetAuditsNeeded();
			int auditsWritten = 0;

			using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
			{
				// commit changes to get IDs for new records. Then commit audits as they refer to those IDs. Use transaction to make the two commits atomic.
				var entriesWritten = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken).ConfigureAwait(true);

				if (auditedEntities.Count > 0)
				{
					await InsertAuditsNeededAsync(userName, auditedEntities, cancellationToken);
					auditsWritten = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken).ConfigureAwait(true);
				}

				scope.Complete();

				return entriesWritten + auditsWritten;
			}
		}

		protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
		{
			configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseConcernsSqlServer("Data Source=127.0.0.1;Initial Catalog=local_trams_test_db;persist security info=True;User id=sa; Password=StrongPassword905");
			}
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

			base.OnModelCreating(modelBuilder);
		}


		/// <summary>
		/// Takes an IAuditable and coverts it into an Audit entity that can be added to the db context.
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="userName"></param>
		/// <param name="changeType"></param>
		/// <returns></returns>
		private Audit BuildAudit(IAuditable entity, string userName, AuditChangeType changeType) => new Audit(entity.GetType().Name, userName, DateTimeOffset.Now, changeType, Serialise(entity));


		/// <summary>
		/// Returns the audits needed - audit type and the entity being audited
		/// </summary>
		/// <returns></returns>
		private List<(AuditChangeType AuditType, object Entity)> GetAuditsNeeded()
		{
			this.ChangeTracker.DetectChanges();

			var auditsNeeded = this.ChangeTracker.Entries()
					.Where(t => EntityStateToAuditChangeTypeMap.IsSupported(t.State) && t.Entity is IAuditable)
					.Select(t => (EntityStateToAuditChangeTypeMap.Map(t.State), t.Entity))
					.ToList();
			return auditsNeeded;
		}

		private string GetCurrentUsername()
		{
			if (string.IsNullOrWhiteSpace(_userInfoService.UserInfo?.Name) && Debugger.IsAttached)
			{
				Debugger.Break();
			}
			return _userInfoService.UserInfo?.Name ?? "Unknown";
		}

		/// <summary>
		/// Takes the list of audit entries needed and writes them to the dbset, ready to be saved.
		/// </summary>
		/// <param name="userName"></param>
		/// <param name="auditedEntities"></param>
		private void InsertAuditsNeeded(string userName, List<(AuditChangeType AuditType, object Entity)> auditedEntities)
		{
			var audits = auditedEntities.Select(x => BuildAudit((IAuditable)x.Entity, userName, x.AuditType)).ToArray();
			this.Audits.AddRange(audits);
		}

		private async Task InsertAuditsNeededAsync(string userName, List<(AuditChangeType AuditType, object Entity)> auditedEntities, CancellationToken cancellationToken)
		{
			var audits = auditedEntities.Select(x => BuildAudit((IAuditable)x.Entity, userName, x.AuditType)).ToArray();
			await this.Audits.AddRangeAsync(audits, cancellationToken);
		}

		private string Serialise(object obj) => JsonSerializer.Serialize(obj, new JsonSerializerOptions()
		{
			ReferenceHandler = ReferenceHandler.IgnoreCycles,
			WriteIndented = false,
			AllowTrailingCommas = true,
		});
	}
}