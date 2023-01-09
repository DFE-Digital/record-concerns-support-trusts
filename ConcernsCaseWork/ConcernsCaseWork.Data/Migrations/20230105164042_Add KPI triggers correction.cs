using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddKPItriggerscorrection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
		   migrationBuilder.Sql(@"
				IF NOT EXISTS (select * from sys.objects where schema_id=SCHEMA_ID('concerns') AND type='TR' and name='tgrConcernsCase_Insert_Update')
				BEGIN

					EXEC ('CREATE TRIGGER [concerns].[tgrConcernsCase_Insert_Update] ON [concerns].[ConcernsCase]
						AFTER INSERT, UPDATE 
					AS 
					BEGIN
						SET NOCOUNT ON;

						-- Case Created
						INSERT INTO [concerns].[kpi-Case]
								   ([CaseId]
								   ,[DateTimeOfChange]
								   ,[DataItemChanged]
								   ,[Operation]
								   ,[OldValue]
								   ,[NewValue])
						SELECT
								currentRecord.Id,
								currentRecord.UpdatedAt,
								''CreatedAt'',
								''Create'',
								NULL,
								FORMAT(currentRecord.CreatedAt, ''dd-MM-yyyy'')
							FROM
								inserted AS currentRecord
							LEFT OUTER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							WHERE originalRecord.Id IS NULL

						-- Modified Risk to Trust
						INSERT INTO [concerns].[kpi-Case]
								   ([CaseId]
								   ,[DateTimeOfChange]
								   ,[DataItemChanged]
								   ,[Operation]
								   ,[OldValue]
								   ,[NewValue])
						SELECT
								currentRecord.Id,
								currentRecord.UpdatedAt,
								''Risk to Trust'',
								''Update'',
								originalRating.[Name],
								updatedRating.[Name]
							FROM
								inserted AS currentRecord
							LEFT OUTER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							LEFT OUTER JOIN 
								[concerns].[ConcernsRating] [originalRating] ON [originalRating].Id = [originalRecord].RatingId
							INNER JOIN 
								[concerns].[ConcernsRating] [updatedRating]  ON [updatedRating].Id = [currentRecord].RatingId
							WHERE ISNULL([originalRecord].RatingId,0) <> [currentRecord].RatingId

						-- Modified Direction of Travel
						INSERT INTO [concerns].[kpi-Case]
								   ([CaseId]
								   ,[DateTimeOfChange]
								   ,[DataItemChanged]
								   ,[Operation]
								   ,[OldValue]
								   ,[NewValue])
						SELECT
								currentRecord.Id,
								currentRecord.UpdatedAt,
								''Direction of Travel'',
								''Update'',
								originalRecord.[DirectionOfTravel],
								currentRecord.[DirectionOfTravel]
							FROM
								inserted AS currentRecord
							LEFT OUTER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							WHERE ISNULL([originalRecord].[DirectionOfTravel], 0) <> [currentRecord].[DirectionOfTravel]


						-- Case Closed
						INSERT INTO [concerns].[kpi-Case]
								   ([CaseId]
								   ,[DateTimeOfChange]
								   ,[DataItemChanged]
								   ,[Operation]
								   ,[OldValue]
								   ,[NewValue])
						SELECT
								currentRecord.Id,
								currentRecord.UpdatedAt,
								''ClosedAt'',
								''Close'',
								NULL,
								FORMAT(currentRecord.ClosedAt, ''dd-MM-yyyy'')
							FROM
								inserted AS currentRecord
							LEFT OUTER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							WHERE currentRecord.ClosedAt IS NOT NULL
						END')
					END
				");
	        
	        migrationBuilder.Sql(@"
				IF NOT EXISTS (select * from sys.objects where schema_id=SCHEMA_ID('concerns') AND type='TR' and name='tgrConcern_Insert_Update')
				BEGIN

					EXEC ('CREATE TRIGGER [concerns].[tgrConcern_Insert_Update] ON [concerns].[ConcernsRecord]
						AFTER INSERT, UPDATE 
					AS 
					BEGIN
						SET NOCOUNT ON;

						-- Concern Created
						INSERT INTO [concerns].[kpi-Concern]
								   ([RecordId]
								   ,[CaseId]
								   ,[DateTimeOfChange]
								   ,[DataItemChanged]
								   ,[Operation]
								   ,[OldValue]
								   ,[NewValue])
						SELECT
								currentRecord.Id,
								currentRecord.CaseId,
								currentRecord.UpdatedAt,
								''CreatedAt'',
								''Create'',
								NULL,
								FORMAT(currentRecord.CreatedAt, ''dd-MM-yyyy'')
							FROM
								inserted AS currentRecord
							LEFT OUTER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							WHERE originalRecord.Id IS NULL

						-- Modified Risk to Trust
						INSERT INTO [concerns].[kpi-Concern]
								   ([RecordId]
								   ,[CaseId]
								   ,[DateTimeOfChange]
								   ,[DataItemChanged]
								   ,[Operation]
								   ,[OldValue]
								   ,[NewValue])
						SELECT
								currentRecord.Id,
								currentRecord.CaseId,
								currentRecord.UpdatedAt,
								''Risk'',
								''Update'',
								originalRating.[Name],
								updatedRating.[Name]
							FROM
								inserted AS currentRecord
							LEFT OUTER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							LEFT OUTER JOIN 
								[concerns].[ConcernsRating] [originalRating] ON [originalRating].[Id] = [originalRecord].[RatingId]
							INNER JOIN 
								[concerns].[ConcernsRating] [updatedRating]  ON [updatedRating].[Id] = [currentRecord].[RatingId]
							WHERE ISNULL([originalRecord].[RatingId], 0) <> [currentRecord].[RatingId]

						-- Concern Closed
						INSERT INTO [concerns].[kpi-Concern]
								   ([RecordId]
								   ,[CaseId]
								   ,[DateTimeOfChange]
								   ,[DataItemChanged]
								   ,[Operation]
								   ,[OldValue]
								   ,[NewValue])
						SELECT
								currentRecord.Id,
								currentRecord.CaseId,
								currentRecord.UpdatedAt,
								''ClosedAt'',
								''Close'',
								NULL,
								FORMAT(currentRecord.ClosedAt, ''dd-MM-yyyy'')
							FROM
								inserted AS currentRecord
							INNER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							WHERE currentRecord.ClosedAt IS NOT NULL
					END')
				END
			");
	        
	        migrationBuilder.Sql(@"
				IF NOT EXISTS (select * from sys.objects where schema_id=SCHEMA_ID('concerns') AND type='TR' and name='tgrFinancialPlanCaseAction_Insert_Update')
				BEGIN

					EXEC ('CREATE TRIGGER [concerns].[tgrFinancialPlanCaseAction_Insert_Update] ON [concerns].[FinancialPlanCase]
						AFTER INSERT, UPDATE 
					AS 
					BEGIN
						SET NOCOUNT ON;

						-- Action opened
						INSERT INTO [concerns].[kpi-CaseAction]
								   ([ActionType],
									[ActionId],
									[CaseUrn],
									[DateTimeOfChange],
									[DataItemChanged],
									[Operation],
									[OldValue],
									[NewValue])
						SELECT
								''FinancialPlanCase'',
								currentRecord.Id,
								currentRecord.CaseUrn,
								currentRecord.UpdatedAt,
								''CreatedAt'',
								''Create'',
								NULL,
								FORMAT(currentRecord.CreatedAt, ''dd-MM-yyyy'')
							FROM
								inserted AS currentRecord
							LEFT OUTER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							WHERE originalRecord.Id IS NULL

						-- Modified Status
						INSERT INTO [concerns].[kpi-CaseAction]
								   ([ActionType],
									[ActionId],
									[CaseUrn],
									[DateTimeOfChange],
									[DataItemChanged],
									[Operation],
									[OldValue],
									[NewValue])
						SELECT
								''FinancialPlanCase'',
								currentRecord.Id,
								currentRecord.CaseUrn,
								currentRecord.UpdatedAt,
								''Status'',
								''Update'',
								[originalStatus].[Name],
								[updatedStatus].[Name]
							FROM
								inserted AS currentRecord
							LEFT OUTER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							LEFT OUTER JOIN 
								[concerns].[FinancialPlanStatus] [originalStatus] ON [originalStatus].Id = [originalRecord].StatusId
							INNER JOIN 
								[concerns].[FinancialPlanStatus] [updatedStatus]  ON [updatedStatus].Id = [currentRecord].StatusId
							WHERE ISNULL([originalRecord].StatusId, 0) <> [currentRecord].StatusId


						-- Action Closed
						INSERT INTO [concerns].[kpi-CaseAction]
								   ([ActionType],
									[ActionId],
									[CaseUrn],
									[DateTimeOfChange],
									[DataItemChanged],
									[Operation],
									[OldValue],
									[NewValue])
						SELECT
								''FinancialPlanCase'',
								currentRecord.Id,
								currentRecord.CaseUrn,
								currentRecord.UpdatedAt,
								''ClosedAt'',
								''Close'',
								NULL,
								FORMAT(currentRecord.ClosedAt, ''dd-MM-yyyy'')
							FROM
								inserted AS currentRecord
							INNER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							WHERE currentRecord.ClosedAt IS NOT NULL
					END')
				END
			");
	        
	        migrationBuilder.Sql(@"
				IF NOT EXISTS (select * from sys.objects where schema_id=SCHEMA_ID('concerns') AND type='TR' and name='tgrNoticeToImproveCaseAction_Insert_Update')
				BEGIN

					EXEC ('CREATE TRIGGER [concerns].[tgrNoticeToImproveCaseAction_Insert_Update] ON [concerns].[NoticeToImproveCase]
						AFTER INSERT, UPDATE 
					AS 
					BEGIN
						SET NOCOUNT ON;

						-- Action opened
						INSERT INTO [concerns].[kpi-CaseAction]
								   ([ActionType],
									[ActionId],
									[CaseUrn],
									[DateTimeOfChange],
									[DataItemChanged],
									[Operation],
									[OldValue],
									[NewValue])
						SELECT
								''NoticeToImproveCase'',
								currentRecord.Id,
								currentRecord.CaseUrn,
								currentRecord.UpdatedAt,
								''CreatedAt'',
								''Create'',
								NULL,
								FORMAT(currentRecord.CreatedAt, ''dd-MM-yyyy'')
							FROM
								inserted AS currentRecord
							LEFT OUTER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							WHERE originalRecord.Id IS NULL

						-- Modified Status
						INSERT INTO [concerns].[kpi-CaseAction]
								   ([ActionType],
									[ActionId],
									[CaseUrn],
									[DateTimeOfChange],
									[DataItemChanged],
									[Operation],
									[OldValue],
									[NewValue])
						SELECT
								''NoticeToImproveCase'',
								currentRecord.Id,
								currentRecord.CaseUrn,
								currentRecord.UpdatedAt,
								''Status'',
								''Update'',
								[originalStatus].[Name],
								[updatedStatus].[Name]
							FROM
								inserted AS currentRecord
							LEFT OUTER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							LEFT OUTER JOIN 
								[concerns].[NoticeToImproveStatus] [originalStatus] ON [originalStatus].Id = [originalRecord].StatusId
							INNER JOIN 
								[concerns].[NoticeToImproveStatus] [updatedStatus]  ON [updatedStatus].Id = [currentRecord].StatusId
							WHERE ISNULL([originalRecord].StatusId,0) <> [currentRecord].StatusId

						-- Modified Closed Status
						INSERT INTO [concerns].[kpi-CaseAction]
								   ([ActionType],
									[ActionId],
									[CaseUrn],
									[DateTimeOfChange],
									[DataItemChanged],
									[Operation],
									[OldValue],
									[NewValue])
						SELECT
								''NoticeToImproveCase'',
								currentRecord.Id,
								currentRecord.CaseUrn,
								currentRecord.UpdatedAt,
								''Status'',
								''Close'',
								NULL,
								[updatedStatus].[Name]
							FROM
								inserted AS currentRecord
							INNER JOIN 
								[concerns].[NoticeToImproveStatus] [updatedStatus]  ON [updatedStatus].Id = [currentRecord].ClosedStatusId
							WHERE currentRecord.ClosedAt IS NOT NULL

						-- Action Closed
						INSERT INTO [concerns].[kpi-CaseAction]
								   ([ActionType],
									[ActionId],
									[CaseUrn],
									[DateTimeOfChange],
									[DataItemChanged],
									[Operation],
									[OldValue],
									[NewValue])
						SELECT
								''NoticeToImproveCase'',
								currentRecord.Id,
								currentRecord.CaseUrn,
								currentRecord.UpdatedAt,
								''ClosedAt'',
								''Close'',
								NULL,
								FORMAT(currentRecord.ClosedAt, ''dd-MM-yyyy'')
							FROM
								inserted AS currentRecord
							INNER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							WHERE currentRecord.ClosedAt IS NOT NULL
					END')
				END
			");
	        
	        migrationBuilder.Sql(@"
				IF NOT EXISTS (select * from sys.objects where schema_id=SCHEMA_ID('concerns') AND type='TR' and name='tgrNTIUnderConsiderationCaseAction_Insert_Update')
				BEGIN

					EXEC ('CREATE TRIGGER [concerns].[tgrNTIUnderConsiderationCaseAction_Insert_Update] ON [concerns].[NTIUnderConsiderationCase]
						AFTER INSERT, UPDATE 
					AS 
					BEGIN
						SET NOCOUNT ON;

						DECLARE @SourceTablename varchar(50) = ''NTIUnderConsiderationCase'';

						-- Action opened
						INSERT INTO [concerns].[kpi-CaseAction]
								   ([ActionType],
									[ActionId],
									[CaseUrn],
									[DateTimeOfChange],
									[DataItemChanged],
									[Operation],
									[OldValue],
									[NewValue])
						SELECT
								@SourceTablename,
								currentRecord.Id,
								currentRecord.CaseUrn,
								currentRecord.UpdatedAt,
								''CreatedAt'',
								''Create'',
								NULL,
								FORMAT(currentRecord.CreatedAt, ''dd-MM-yyyy'')
							FROM
								inserted AS currentRecord
							LEFT OUTER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							WHERE originalRecord.Id IS NULL

						-- Modified Status
						INSERT INTO [concerns].[kpi-CaseAction]
								   ([ActionType],
									[ActionId],
									[CaseUrn],
									[DateTimeOfChange],
									[DataItemChanged],
									[Operation],
									[OldValue],
									[NewValue])
						SELECT
								@SourceTablename,
								currentRecord.Id,
								currentRecord.CaseUrn,
								currentRecord.UpdatedAt,
								''Status'',
								''Update'',
								null,
								[updatedStatus].Name
							FROM
								inserted AS currentRecord
							INNER JOIN 
								[concerns].[NTIUnderConsiderationStatus] [updatedStatus]  ON [updatedStatus].Id = [currentRecord].ClosedStatusId
							WHERE currentRecord.ClosedStatusId IS NOT NULL AND currentRecord.ClosedStatusId in (SELECT Id FROM [concerns].[NTIUnderConsiderationStatus])

						-- Action Closed
						INSERT INTO [concerns].[kpi-CaseAction]
								   ([ActionType],
									[ActionId],
									[CaseUrn],
									[DateTimeOfChange],
									[DataItemChanged],
									[Operation],
									[OldValue],
									[NewValue])
						SELECT
								@SourceTablename,
								currentRecord.Id,
								currentRecord.CaseUrn,
								currentRecord.UpdatedAt,
								''ClosedAt'',
								''Close'',
								NULL,
								FORMAT(currentRecord.ClosedAt, ''dd-MM-yyyy'')
							FROM
								inserted AS currentRecord
							INNER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							WHERE currentRecord.ClosedStatusId in (SELECT Id FROM [concerns].[NTIUnderConsiderationStatus]) -- NTIUnderConsiderationCase table only has ClosedStatusId column
			
					END')
				END
			");
	        
	        migrationBuilder.Sql(@"
				IF NOT EXISTS (select * from sys.objects where schema_id=SCHEMA_ID('concerns') AND type='TR' and name='tgrNTIWarningLetterCaseAction_Insert_Update')
				BEGIN

					EXEC ('CREATE TRIGGER [concerns].[tgrNTIWarningLetterCaseAction_Insert_Update] ON [concerns].[NTIWarningLetterCase]
						AFTER INSERT, UPDATE 
					AS 
					BEGIN
						SET NOCOUNT ON;

						-- Action opened
						INSERT INTO [concerns].[kpi-CaseAction]
								   ([ActionType],
									[ActionId],
									[CaseUrn],
									[DateTimeOfChange],
									[DataItemChanged],
									[Operation],
									[OldValue],
									[NewValue])
						SELECT
								''NTIWarningLetterCase'',
								currentRecord.Id,
								currentRecord.CaseUrn,
								currentRecord.UpdatedAt,
								''CreatedAt'',
								''Create'',
								NULL,
								FORMAT(currentRecord.CreatedAt, ''dd-MM-yyyy'')
							FROM
								inserted AS currentRecord
							LEFT OUTER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							WHERE originalRecord.Id IS NULL

						-- Modified Status
						INSERT INTO [concerns].[kpi-CaseAction]
								   ([ActionType],
									[ActionId],
									[CaseUrn],
									[DateTimeOfChange],
									[DataItemChanged],
									[Operation],
									[OldValue],
									[NewValue])
						SELECT
								''NTIWarningLetterCase'',
								currentRecord.Id,
								currentRecord.CaseUrn,
								currentRecord.UpdatedAt,
								''Status'',
								''Update'',
								[originalStatus].Name,
								[updatedStatus].Name
							FROM
								inserted AS currentRecord
							LEFT OUTER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							LEFT OUTER JOIN 
								[concerns].[NTIWarningLetterStatus] [originalStatus] ON [originalStatus].Id = [originalRecord].StatusId
							INNER JOIN 
								[concerns].[NTIWarningLetterStatus] [updatedStatus]  ON [updatedStatus].Id = [currentRecord].StatusId
							WHERE  ISNULL([originalRecord].StatusId, 0) <> [currentRecord].StatusId
							
						-- Modified Closed Status
						INSERT INTO [concerns].[kpi-CaseAction]
								   ([ActionType],
									[ActionId],
									[CaseUrn],
									[DateTimeOfChange],
									[DataItemChanged],
									[Operation],
									[OldValue],
									[NewValue])
						SELECT
								''NTIWarningLetterCase'',
								currentRecord.Id,
								currentRecord.CaseUrn,
								currentRecord.UpdatedAt,
								''ClosedStatus'',
								''Update'',
								null,
								[updatedStatus].Name
							FROM
								inserted AS currentRecord
							INNER JOIN 
								[concerns].[NTIWarningLetterStatus] [updatedStatus]  ON [updatedStatus].Id = [currentRecord].ClosedStatusId

						-- Action Closed
						INSERT INTO [concerns].[kpi-CaseAction]
								   ([ActionType],
									[ActionId],
									[CaseUrn],
									[DateTimeOfChange],
									[DataItemChanged],
									[Operation],
									[OldValue],
									[NewValue])
						SELECT
								''NTIWarningLetterCase'',
								currentRecord.Id,
								currentRecord.CaseUrn,
								currentRecord.UpdatedAt,
								''ClosedAt'',
								''Close'',
								NULL,
								FORMAT(currentRecord.ClosedAt, ''dd-MM-yyyy'')
							FROM
								inserted AS currentRecord
							INNER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							WHERE currentRecord.ClosedAt IS NOT NULL
					END')
				END
			");
	        
	        migrationBuilder.Sql(@"
				IF NOT EXISTS (select * from sys.objects where schema_id=SCHEMA_ID('concerns') AND type='TR' and name='tgrSRMACaseAction_Insert_Update')
				BEGIN

					EXEC ('CREATE TRIGGER [concerns].[tgrSRMACaseAction_Insert_Update] ON [concerns].[SRMACase]
						AFTER INSERT, UPDATE 
					AS 
					BEGIN
						SET NOCOUNT ON;

						-- Action opened
						INSERT INTO [concerns].[kpi-CaseAction]
								   ([ActionType],
									[ActionId],
									[CaseUrn],
									[DateTimeOfChange],
									[DataItemChanged],
									[Operation],
									[OldValue],
									[NewValue])
						SELECT
								''SRMACase'',
								currentRecord.Id,
								currentRecord.CaseUrn,
								currentRecord.UpdatedAt,
								''CreatedAt'',
								''Create'',
								NULL,
								FORMAT(currentRecord.CreatedAt, ''dd-MM-yyyy'')
							FROM
								inserted AS currentRecord
							LEFT OUTER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							WHERE originalRecord.Id IS NULL

						-- Modified Status
						INSERT INTO [concerns].[kpi-CaseAction]
								   ([ActionType],
									[ActionId],
									[CaseUrn],
									[DateTimeOfChange],
									[DataItemChanged],
									[Operation],
									[OldValue],
									[NewValue])
						SELECT
								''SRMACase'',
								currentRecord.Id,
								currentRecord.CaseUrn,
								currentRecord.UpdatedAt,
								''Status'',
								''Update'',
								[originalStatus].[Name],
								[updatedStatus].[Name]
							FROM
								inserted AS currentRecord
							LEFT OUTER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							LEFT OUTER JOIN 
								[concerns].[SRMAStatus] [originalStatus] ON [originalStatus].Id = [originalRecord].StatusId
							INNER JOIN 
								[concerns].[SRMAStatus] [updatedStatus]  ON [updatedStatus].Id = [currentRecord].StatusId
							WHERE ISNULL([originalRecord].StatusId, 0) <> [currentRecord].StatusId

						-- Action Closed
						INSERT INTO [concerns].[kpi-CaseAction]
								   ([ActionType],
									[ActionId],
									[CaseUrn],
									[DateTimeOfChange],
									[DataItemChanged],
									[Operation],
									[OldValue],
									[NewValue])
						SELECT
								''SRMACase'',
								currentRecord.Id,
								currentRecord.CaseUrn,
								currentRecord.UpdatedAt,
								''ClosedAt'',
								''Close'',
								NULL,
								FORMAT(currentRecord.ClosedAt, ''dd-MM-yyyy'')
							FROM
								inserted AS currentRecord
							INNER JOIN
								deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
							WHERE currentRecord.StatusId in (SELECT Id FROM [concerns].[SRMAStatus]) AND currentRecord.ClosedAt is not null
			
					END')
				END
			");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(@"
				DROP TRIGGER [concerns].[tgrConcernsCase_Insert_Update] 
				DROP TRIGGER [concerns].[tgrConcern_Insert_Update]
				DROP TRIGGER [concerns].[tgrFinancialPlanCaseAction_Insert_Update]
				DROP TRIGGER [concerns].[tgrNoticeToImproveCaseAction_Insert_Update]
				DROP TRIGGER [concerns].[tgrNTIUnderConsiderationCaseAction_Insert_Update]
				DROP TRIGGER [concerns].[tgrNTIWarningLetterCaseAction_Insert_Update]
				DROP TRIGGER [concerns].[tgrSRMACaseAction_Insert_Update]
			");
        }
    }
}
