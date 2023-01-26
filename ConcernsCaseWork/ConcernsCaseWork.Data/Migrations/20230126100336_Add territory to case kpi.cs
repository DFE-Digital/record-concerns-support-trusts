using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConcernsCaseWork.Data.Migrations
{
    /// <inheritdoc />
    public partial class Addterritorytocasekpi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
		   migrationBuilder.Sql(@"
				EXEC ('ALTER TRIGGER [concerns].[tgrConcernsCase_Insert_Update] ON [concerns].[ConcernsCase]
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

					-- Modified Territory
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
							''Territory'',
							''Update'',
							originalRecord.[Territory],
							currentRecord.[Territory]
						FROM
							inserted AS currentRecord
						LEFT OUTER JOIN
							deleted AS originalRecord ON originalRecord.Id = currentRecord.Id
						WHERE ISNULL([originalRecord].[Territory], '''') <> [currentRecord].[Territory]

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
				");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
		   migrationBuilder.Sql(@"
				EXEC ('ALTER TRIGGER [concerns].[tgrConcernsCase_Insert_Update] ON [concerns].[ConcernsCase]
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
				");
        }
    }
}
