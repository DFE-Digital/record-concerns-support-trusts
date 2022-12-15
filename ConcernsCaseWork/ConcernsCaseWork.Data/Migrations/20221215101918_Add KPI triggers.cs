using Microsoft.EntityFrameworkCore.Migrations;

namespace ConcernsCaseWork.Data.Migrations
{
	public partial class AddKPItriggers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
			migrationBuilder.Sql(@"
				CREATE TABLE [concerns].[kpi-Case](
					[Id] [int] IDENTITY(1,1) NOT NULL,
					[Timestamp] [datetime2](7) NOT NULL,
					[CaseId] [int] NOT NULL,
					[DateTimeOfChange] [datetime2](7) NOT NULL,
					[DataItemChanged] [varchar](100) NULL,
					[Operation] [varchar](20) NOT NULL,
					[OldValue] [nvarchar](4000) NULL,
					[NewValue] [nvarchar](4000) NULL,
				 CONSTRAINT [PK_kpi.Case] PRIMARY KEY CLUSTERED 
				(
					[Id] ASC
				)
				) ON [PRIMARY]
				GO

				ALTER TABLE [concerns].[kpi-Case] ADD  CONSTRAINT [DF_kpi.Case_Timestamp]  DEFAULT (getdate()) FOR [Timestamp]
				GO

				CREATE TABLE [concerns].[kpi-Concern](
					[Id] [int] IDENTITY(1,1) NOT NULL,
					[Timestamp] [datetime2](7) NOT NULL,
					[RecordId] [int] NOT NULL,
					[CaseId] [int] NOT NULL,
					[DateTimeOfChange] [datetime2](7) NOT NULL,
					[DataItemChanged] [varchar](100) NULL,
					[Operation] [varchar](20) NOT NULL,
					[OldValue] [nvarchar](4000) NULL,
					[NewValue] [nvarchar](4000) NULL,
				 CONSTRAINT [PK_kpi.Concern] PRIMARY KEY CLUSTERED 
				(
					[Id] ASC
				)
				) ON [PRIMARY]
				GO

				ALTER TABLE [concerns].[kpi-Concern] ADD  CONSTRAINT [DF_kpi.Concern_Timestamp]  DEFAULT (getdate()) FOR [Timestamp]
				GO

				CREATE TABLE [concerns].[kpi-CaseAction](
					[Id] [int] IDENTITY(1,1) NOT NULL,
					[ActionType] [varchar](50) NULL,
					[Timestamp] [datetime2](7) NOT NULL,
					[ActionId] [bigint] NOT NULL,
					[CaseUrn] [int] NOT NULL,
					[DateTimeOfChange] [datetime2](7) NOT NULL,
					[DataItemChanged] [varchar](100) NULL,
					[Operation] [varchar](20) NOT NULL,
					[OldValue] [nvarchar](4000) NULL,
					[NewValue] [nvarchar](4000) NULL,
				 CONSTRAINT [PK_kpi.CaseAction] PRIMARY KEY CLUSTERED 
				(
					[Id] ASC
				)
				) ON [PRIMARY]
				GO

				ALTER TABLE [concerns].[kpi-CaseAction] ADD  CONSTRAINT [DF_kpi.CaseAction_Timestamp]  DEFAULT (getdate()) FOR [Timestamp]
				GO
			");
			
	        migrationBuilder.Sql(@"
				CREATE TRIGGER [concerns].[tgrConcernsCase_Insert_Update] ON [concerns].[ConcernsCase]
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
							'CreatedAt',
							'Create',
							NULL,
							FORMAT(currentRecord.CreatedAt, 'dd-MM-yyyy')
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
							'Risk to Trust',
							'Update',
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
							'Direction of Travel',
							'Update',
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
							'ClosedAt',
							'Close',
							NULL,
							FORMAT(currentRecord.ClosedAt, 'dd-MM-yyyy')
						FROM
							inserted AS currentRecord
						LEFT OUTER JOIN
							deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
						WHERE currentRecord.ClosedAt IS NOT NULL
				END
				");
	        
	        migrationBuilder.Sql(@"
				CREATE TRIGGER [concerns].[tgrConcern_Insert_Update] ON [concerns].[ConcernsRecord]
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
							'CreatedAt',
							'Create',
							NULL,
							FORMAT(currentRecord.CreatedAt, 'dd-MM-yyyy')
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
							'Risk',
							'Update',
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
							'ClosedAt',
							'Close',
							NULL,
							FORMAT(currentRecord.ClosedAt, 'dd-MM-yyyy')
						FROM
							inserted AS currentRecord
						INNER JOIN
							deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
						WHERE currentRecord.ClosedAt IS NOT NULL
				END
			");
	        
	        migrationBuilder.Sql(@"
				CREATE TRIGGER [concerns].[tgrFinancialPlanCaseAction_Insert_Update] ON [concerns].[FinancialPlanCase]
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
							'FinancialPlanCase',
							currentRecord.Id,
							currentRecord.CaseUrn,
							currentRecord.UpdatedAt,
							'CreatedAt',
							'Create',
							NULL,
							FORMAT(currentRecord.CreatedAt, 'dd-MM-yyyy')
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
							'FinancialPlanCase',
							currentRecord.Id,
							currentRecord.CaseUrn,
							currentRecord.UpdatedAt,
							'Status',
							'Update',
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
							'FinancialPlanCase',
							currentRecord.Id,
							currentRecord.CaseUrn,
							currentRecord.UpdatedAt,
							'ClosedAt',
							'Close',
							NULL,
							FORMAT(currentRecord.ClosedAt, 'dd-MM-yyyy')
						FROM
							inserted AS currentRecord
						INNER JOIN
							deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
						WHERE currentRecord.ClosedAt IS NOT NULL
				END
			");
	        
	        migrationBuilder.Sql(@"
				CREATE TRIGGER [concerns].[tgrNoticeToImproveCaseAction_Insert_Update] ON [concerns].[NoticeToImproveCase]
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
							'NoticeToImproveCase',
							currentRecord.Id,
							currentRecord.CaseUrn,
							currentRecord.UpdatedAt,
							'CreatedAt',
							'Create',
							NULL,
							FORMAT(currentRecord.CreatedAt, 'dd-MM-yyyy')
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
							'NoticeToImproveCase',
							currentRecord.Id,
							currentRecord.CaseUrn,
							currentRecord.UpdatedAt,
							'Status',
							'Update',
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
							'NoticeToImproveCase',
							currentRecord.Id,
							currentRecord.CaseUrn,
							currentRecord.UpdatedAt,
							'Status',
							'Close',
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
							'NoticeToImproveCase',
							currentRecord.Id,
							currentRecord.CaseUrn,
							currentRecord.UpdatedAt,
							'ClosedAt',
							'Close',
							NULL,
							FORMAT(currentRecord.ClosedAt, 'dd-MM-yyyy')
						FROM
							inserted AS currentRecord
						INNER JOIN
							deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
						WHERE currentRecord.ClosedAt IS NOT NULL
				END
			");
	        
	        migrationBuilder.Sql(@"
				CREATE TRIGGER [concerns].[tgrNTIUnderConsiderationCaseAction_Insert_Update] ON [concerns].[NTIUnderConsiderationCase]
					AFTER INSERT, UPDATE 
				AS 
				BEGIN
					SET NOCOUNT ON;

					DECLARE @SourceTablename varchar(50) = 'NTIUnderConsiderationCase';

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
							'CreatedAt',
							'Create',
							NULL,
							FORMAT(currentRecord.CreatedAt, 'dd-MM-yyyy')
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
							'Status',
							'Update',
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
							'ClosedAt',
							'Close',
							NULL,
							FORMAT(currentRecord.ClosedAt, 'dd-MM-yyyy')
						FROM
							inserted AS currentRecord
						INNER JOIN
							deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
						WHERE currentRecord.ClosedStatusId in (SELECT Id FROM [concerns].[NTIUnderConsiderationStatus]) -- NTIUnderConsiderationCase table only has ClosedStatusId column
				END
			");
	        
	        migrationBuilder.Sql(@"
				CREATE TRIGGER [concerns].[tgrNTIWarningLetterCaseAction_Insert_Update] ON [concerns].[NTIWarningLetterCase]
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
							'NTIWarningLetterCase',
							currentRecord.Id,
							currentRecord.CaseUrn,
							currentRecord.UpdatedAt,
							'CreatedAt',
							'Create',
							NULL,
							FORMAT(currentRecord.CreatedAt, 'dd-MM-yyyy')
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
							'NTIWarningLetterCase',
							currentRecord.Id,
							currentRecord.CaseUrn,
							currentRecord.UpdatedAt,
							'Status',
							'Update',
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
							'NTIWarningLetterCase',
							currentRecord.Id,
							currentRecord.CaseUrn,
							currentRecord.UpdatedAt,
							'ClosedStatus',
							'Update',
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
							'NTIWarningLetterCase',
							currentRecord.Id,
							currentRecord.CaseUrn,
							currentRecord.UpdatedAt,
							'ClosedAt',
							'Close',
							NULL,
							FORMAT(currentRecord.ClosedAt, 'dd-MM-yyyy')
						FROM
							inserted AS currentRecord
						INNER JOIN
							deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
						WHERE currentRecord.ClosedAt IS NOT NULL
				END
			");
	        
	        migrationBuilder.Sql(@"
				CREATE TRIGGER [concerns].[tgrSRMACaseAction_Insert_Update] ON [concerns].[SRMACase]
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
							'SRMACase',
							currentRecord.Id,
							currentRecord.CaseUrn,
							currentRecord.UpdatedAt,
							'CreatedAt',
							'Create',
							NULL,
							FORMAT(currentRecord.CreatedAt, 'dd-MM-yyyy')
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
							'SRMACase',
							currentRecord.Id,
							currentRecord.CaseUrn,
							currentRecord.UpdatedAt,
							'Status',
							'Update',
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
							'SRMACase',
							currentRecord.Id,
							currentRecord.CaseUrn,
							currentRecord.UpdatedAt,
							'ClosedAt',
							'Close',
							NULL,
							FORMAT(currentRecord.ClosedAt, 'dd-MM-yyyy')
						FROM
							inserted AS currentRecord
						INNER JOIN
							deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
						WHERE currentRecord.StatusId in (SELECT Id FROM [concerns].[SRMAStatus]) AND currentRecord.ClosedAt is not null
				END
			");
        }

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

				DROP TABLE [concerns].[kpi-CaseAction]
				DROP TABLE [concerns].[kpi-Case]
				DROP TABLE [concerns].[kpi-Concern]
			");
        }
    }
}
