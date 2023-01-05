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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.Sql(@"
				DROP TABLE [concerns].[kpi-CaseAction]
				DROP TABLE [concerns].[kpi-Case]
				DROP TABLE [concerns].[kpi-Concern]
			");
        }
    }
}
