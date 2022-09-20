using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Concerns.Data.Migrations
{
    public partial class AddNTI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropSequence(
            //    name: "GlobalSequence",
            //    schema: "concerns");

            //migrationBuilder.CreateSequence<int>(
            //    name: "ConcernsGlobalSequence",
            //    minValue: 1L);

            migrationBuilder.AddColumn<string>(
                name: "PastTenseName",
                schema: "concerns",
                table: "NTIWarningLetterStatus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Urn",
                schema: "concerns",
                table: "ConcernsType",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR ConcernsGlobalSequence",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR Concerns.GlobalSequence");

            migrationBuilder.AlterColumn<int>(
                name: "Urn",
                schema: "concerns",
                table: "ConcernsStatus",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR ConcernsGlobalSequence",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR Concerns.GlobalSequence");

            migrationBuilder.AlterColumn<int>(
                name: "Urn",
                schema: "concerns",
                table: "ConcernsRecord",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR ConcernsGlobalSequence",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR Concerns.GlobalSequence");

            migrationBuilder.AlterColumn<int>(
                name: "Urn",
                schema: "concerns",
                table: "ConcernsRating",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR ConcernsGlobalSequence",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR Concerns.GlobalSequence");

            migrationBuilder.AlterColumn<int>(
                name: "Urn",
                schema: "concerns",
                table: "ConcernsMeansOfReferral",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR ConcernsGlobalSequence",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR Concerns.GlobalSequence");

            migrationBuilder.AlterColumn<int>(
                name: "Urn",
                schema: "concerns",
                table: "ConcernsCase",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR ConcernsGlobalSequence",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR Concerns.GlobalSequence");

            migrationBuilder.CreateTable(
                name: "ConcernsCaseworkTeam",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConcernsCaseworkTeam", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NoticeToImproveConditionType",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoticeToImproveConditionType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NoticeToImproveReason",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoticeToImproveReason", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NoticeToImproveStatus",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsClosingState = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoticeToImproveStatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConcernsCaseworkTeamMember",
                schema: "concerns",
                columns: table => new
                {
                    TeamMemberId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamMember = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcernsCaseworkTeamId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConcernsCaseworkTeamMember", x => x.TeamMemberId);
                    table.ForeignKey(
                        name: "FK_ConcernsCaseworkTeamMember_ConcernsCaseworkTeam_ConcernsCaseworkTeamId",
                        column: x => x.ConcernsCaseworkTeamId,
                        principalSchema: "concerns",
                        principalTable: "ConcernsCaseworkTeam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NoticeToImproveCondition",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConditionTypeId = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoticeToImproveCondition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NoticeToImproveCondition_NoticeToImproveConditionType_ConditionTypeId",
                        column: x => x.ConditionTypeId,
                        principalSchema: "concerns",
                        principalTable: "NoticeToImproveConditionType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NoticeToImproveCase",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseUrn = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    DateStarted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedStatusId = table.Column<int>(type: "int", nullable: true),
                    SumissionDecisionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateNTILifted = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateNTIClosed = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoticeToImproveCase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NoticeToImproveCase_NoticeToImproveStatus_ClosedStatusId",
                        column: x => x.ClosedStatusId,
                        principalSchema: "concerns",
                        principalTable: "NoticeToImproveStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NoticeToImproveCase_NoticeToImproveStatus_StatusId",
                        column: x => x.StatusId,
                        principalSchema: "concerns",
                        principalTable: "NoticeToImproveStatus",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NoticeToImproveConditionMapping",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoticeToImproveId = table.Column<long>(type: "bigint", nullable: false),
                    NoticeToImproveConditionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoticeToImproveConditionMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NoticeToImproveConditionMapping_NoticeToImproveCase_NoticeToImproveId",
                        column: x => x.NoticeToImproveId,
                        principalSchema: "concerns",
                        principalTable: "NoticeToImproveCase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NoticeToImproveConditionMapping_NoticeToImproveCondition_NoticeToImproveConditionId",
                        column: x => x.NoticeToImproveConditionId,
                        principalSchema: "concerns",
                        principalTable: "NoticeToImproveCondition",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NoticeToImproveReasonMapping",
                schema: "concerns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NoticeToImproveId = table.Column<long>(type: "bigint", nullable: false),
                    NoticeToImproveReasonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoticeToImproveReasonMapping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NoticeToImproveReasonMapping_NoticeToImproveCase_NoticeToImproveId",
                        column: x => x.NoticeToImproveId,
                        principalSchema: "concerns",
                        principalTable: "NoticeToImproveCase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NoticeToImproveReasonMapping_NoticeToImproveReason_NoticeToImproveReasonId",
                        column: x => x.NoticeToImproveReasonId,
                        principalSchema: "concerns",
                        principalTable: "NoticeToImproveReason",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 7,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "NTIUnderConsiderationStatus",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "NTIWarningLetterStatus",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "PastTenseName" },
                values: new object[] { null, "" });

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "NTIWarningLetterStatus",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "PastTenseName" },
                values: new object[] { null, "" });

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "NTIWarningLetterStatus",
                keyColumn: "Id",
                keyValue: 3,
                column: "PastTenseName",
                value: "Conditions met");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "NTIWarningLetterStatus",
                keyColumn: "Id",
                keyValue: 4,
                column: "PastTenseName",
                value: "Conditions met");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "NTIWarningLetterStatus",
                keyColumn: "Id",
                keyValue: 5,
                column: "PastTenseName",
                value: "Conditions met");

            migrationBuilder.InsertData(
                schema: "concerns",
                table: "NoticeToImproveConditionType",
                columns: new[] { "Id", "CreatedAt", "DisplayOrder", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2022, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Financial management conditions", new DateTime(2022, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2022, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Governance conditions", new DateTime(2022, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2022, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Compliance conditions", new DateTime(2022, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2022, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "Safeguarding conditions", new DateTime(2022, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2022, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "Fraud and irregularity", new DateTime(2022, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, new DateTime(2022, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, "Standard conditions", new DateTime(2022, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, new DateTime(2022, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, "Additional Financial Support conditions", new DateTime(2022, 7, 12, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                schema: "concerns",
                table: "NoticeToImproveReason",
                columns: new[] { "Id", "CreatedAt", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cash flow problems", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cumulative deficit (actual)", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cumulative deficit (projected)", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Governance concerns", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Non-compliance with Academies Financial/Trust Handbook", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Non-compliance with financial returns", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Risk of insolvency", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Safeguarding", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                schema: "concerns",
                table: "NoticeToImproveStatus",
                columns: new[] { "Id", "CreatedAt", "IsClosingState", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Preparing NTI", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Issued NTI", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Progress on track", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Evidence of NTI non-compliance", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Serious NTI breaches - considering escalation to TWN", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Submission to lift NTI in progress", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Submission to close NTI in progress", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Lifted", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Closed", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Cancelled", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "SRMAReason",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "SchoolsFinancialSupportAndOversight");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "SRMAReason",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "RegionsGroupIntervention");

            migrationBuilder.InsertData(
                schema: "concerns",
                table: "NoticeToImproveCondition",
                columns: new[] { "Id", "ConditionTypeId", "CreatedAt", "DisplayOrder", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Audit and risk committee", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 1, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Internal audit findings", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 1, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Trust financial plan", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 1, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "Financial management and governance review", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 1, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "Financial systems & controls and internal scrutiny", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, 1, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, "Integrated curriculum and financial planning", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, 1, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, "Monthly management accounts", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, 1, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, "National deals for schools", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, 1, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, "School resource management", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, 2, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Academy ambassadors", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 11, 2, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Academy transfer", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 12, 2, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Action plan", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 13, 2, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "AGM of members", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 14, 2, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 5, "Board meetings", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 15, 2, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, "Independant review of governance", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 16, 2, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 7, "Lines of accountability", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 17, 2, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 8, "Providing sufficient challenge", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 18, 2, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 9, "Scheme of delegation", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 19, 2, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 10, "School improvement", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 20, 2, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 11, "Strengthen governance", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 21, 3, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Admissions", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 22, 3, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Excessive executive payments (high pay)", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 23, 3, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Publishing requirements (compliance with)", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 24, 3, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "Related party Transactions (RPTs)", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 25, 4, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Review and update safeguarding policies", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 26, 4, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Commission external review of safeguarding", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 27, 4, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Appoint trustee with leadership responsibility for safeguarding", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 28, 4, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "Safeguarding recruitment process", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 29, 5, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Novel, contentious, and/or repercussive transactions", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 30, 5, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Off-payroll payments", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 31, 5, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Procurement policy", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 32, 5, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "Register of interests", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 33, 6, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Financial returns", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 34, 6, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Delegated freedoms", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 35, 6, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Trustee contact details", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 36, 7, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Review of board and executive team capability", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 37, 7, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Academy transfer (lower risk)", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 38, 7, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "Move to latest model funding agreement", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 39, 7, new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 4, "Qualified Floating Charge (QFC)", new DateTime(2022, 8, 17, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConcernsCaseworkTeamMember_ConcernsCaseworkTeamId",
                schema: "concerns",
                table: "ConcernsCaseworkTeamMember",
                column: "ConcernsCaseworkTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeToImproveCase_ClosedStatusId",
                schema: "concerns",
                table: "NoticeToImproveCase",
                column: "ClosedStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeToImproveCase_StatusId",
                schema: "concerns",
                table: "NoticeToImproveCase",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeToImproveCondition_ConditionTypeId",
                schema: "concerns",
                table: "NoticeToImproveCondition",
                column: "ConditionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeToImproveConditionMapping_NoticeToImproveConditionId",
                schema: "concerns",
                table: "NoticeToImproveConditionMapping",
                column: "NoticeToImproveConditionId");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeToImproveConditionMapping_NoticeToImproveId",
                schema: "concerns",
                table: "NoticeToImproveConditionMapping",
                column: "NoticeToImproveId");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeToImproveReasonMapping_NoticeToImproveId",
                schema: "concerns",
                table: "NoticeToImproveReasonMapping",
                column: "NoticeToImproveId");

            migrationBuilder.CreateIndex(
                name: "IX_NoticeToImproveReasonMapping_NoticeToImproveReasonId",
                schema: "concerns",
                table: "NoticeToImproveReasonMapping",
                column: "NoticeToImproveReasonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConcernsCaseworkTeamMember",
                schema: "concerns");

            migrationBuilder.DropTable(
                name: "NoticeToImproveConditionMapping",
                schema: "concerns");

            migrationBuilder.DropTable(
                name: "NoticeToImproveReasonMapping",
                schema: "concerns");

            migrationBuilder.DropTable(
                name: "ConcernsCaseworkTeam",
                schema: "concerns");

            migrationBuilder.DropTable(
                name: "NoticeToImproveCondition",
                schema: "concerns");

            migrationBuilder.DropTable(
                name: "NoticeToImproveCase",
                schema: "concerns");

            migrationBuilder.DropTable(
                name: "NoticeToImproveReason",
                schema: "concerns");

            migrationBuilder.DropTable(
                name: "NoticeToImproveConditionType",
                schema: "concerns");

            migrationBuilder.DropTable(
                name: "NoticeToImproveStatus",
                schema: "concerns");

            migrationBuilder.DropSequence(
                name: "ConcernsGlobalSequence");

            migrationBuilder.DropColumn(
                name: "PastTenseName",
                schema: "concerns",
                table: "NTIWarningLetterStatus");

            migrationBuilder.CreateSequence<int>(
                name: "GlobalSequence",
                schema: "concerns",
                minValue: 1L);

            migrationBuilder.AlterColumn<int>(
                name: "Urn",
                schema: "concerns",
                table: "ConcernsType",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Concerns.GlobalSequence",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR ConcernsGlobalSequence");

            migrationBuilder.AlterColumn<int>(
                name: "Urn",
                schema: "concerns",
                table: "ConcernsStatus",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Concerns.GlobalSequence",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR ConcernsGlobalSequence");

            migrationBuilder.AlterColumn<int>(
                name: "Urn",
                schema: "concerns",
                table: "ConcernsRecord",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Concerns.GlobalSequence",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR ConcernsGlobalSequence");

            migrationBuilder.AlterColumn<int>(
                name: "Urn",
                schema: "concerns",
                table: "ConcernsRating",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Concerns.GlobalSequence",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR ConcernsGlobalSequence");

            migrationBuilder.AlterColumn<int>(
                name: "Urn",
                schema: "concerns",
                table: "ConcernsMeansOfReferral",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Concerns.GlobalSequence",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR ConcernsGlobalSequence");

            migrationBuilder.AlterColumn<int>(
                name: "Urn",
                schema: "concerns",
                table: "ConcernsCase",
                type: "int",
                nullable: false,
                defaultValueSql: "NEXT VALUE FOR Concerns.GlobalSequence",
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValueSql: "NEXT VALUE FOR ConcernsGlobalSequence");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "ConcernsType",
                keyColumn: "Id",
                keyValue: 7,
                column: "Description",
                value: "");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "NTIUnderConsiderationStatus",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "NTIWarningLetterStatus",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "NTIWarningLetterStatus",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "SRMAReason",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "AMSDIntervention");

            migrationBuilder.UpdateData(
                schema: "concerns",
                table: "SRMAReason",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "RDDIntervention");
        }
    }
}
