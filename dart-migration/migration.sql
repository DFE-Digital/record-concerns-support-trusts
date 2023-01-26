USE [DaRT_Sandbox]
GO
/****** Object:  Schema [dartmigration]    Script Date: 25/01/2023 11:17:25 ******/
CREATE SCHEMA [dartmigration]
GO

/****** Object:  UserDefinedTableType [dartmigration].[CaseUrnTableType]    Script Date: 25/01/2023 11:17:25 ******/
CREATE TYPE [dartmigration].[CaseUrnTableType] AS TABLE(
	[CaseUrn] [int] NOT NULL,
	PRIMARY KEY CLUSTERED 
(
	[CaseUrn] ASC
)WITH (IGNORE_DUP_KEY = OFF)
)
GO

/****** Object:  UserDefinedFunction [dartmigration].[AddLabelAndValueLineIfNotNull]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 -- =============================================
-- Author:		Emma Whitcroft
-- Create date: 24/08/2022
-- Description:	If the value is not null, 
--				concatenate the label and value to the original, appending new line.
--				Otherwise, return the original string
-- =============================================
CREATE FUNCTION [dartmigration].[AddLabelAndValueLineIfNotNull]
(
	@OriginalString nvarchar(MAX) NULL,
	@Label nvarchar(MAX) NULL,
	@Value nvarchar(MAX) NULL
)
RETURNS nvarchar(MAX)
AS
BEGIN
	IF (@Value IS NULL) RETURN @OriginalString
	RETURN CONCAT(ISNULL(@OriginalString,''), @Label, @Value, Char(10), Char(13))
END
GO

/****** Object:  UserDefinedFunction [dartmigration].[GetCaseHistory]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






 -- =============================================
-- Author:		Michael Stock
-- Create date: 20/01/2023
-- Description:	Create the case history including the academy namem URN and CRM enquiry number if a concern has been raised against an academy in DaRT
-- Returns: case history with optional academy information
-- =============================================
CREATE FUNCTION [dartmigration].[GetCaseHistory]
(
	@CaseHistory NVARCHAR(max),
	@Urn NVARCHAR(200),
	@AcademyName NVARCHAR(200),
	@CrmEnquiryNumber NVARCHAR(200)
)
RETURNS VARCHAR(4300)
AS
BEGIN
	DECLARE @Result VARCHAR(4300) = '';
	DECLARE @LineBreak VARCHAR(10) = CHAR(10) + CHAR(13) + CHAR(10) + CHAR(13)
	
	IF @AcademyName IS NOT NULL
	BEGIN
		SET @Result += 'ACADEMY: ' + @AcademyName + @LineBreak
	END

	If @Urn IS NOT NULL
	BEGIN
		SET @Result += 'URN: ' + @Urn + @LineBreak
	END

	IF @CrmEnquiryNumber IS NOT NULL
	BEGIN
		SET @Result += 'CRM REF: ' + @CrmEnquiryNumber + @LineBreak
	END

	SET @Result += @CaseHistory

	RETURN @Result
END
GO
/****** Object:  UserDefinedFunction [dartmigration].[GetConcernCaseStatusId]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Michael Stock
-- Create date: 05/01/2023
-- Description:	Map the string value supplied to a Concern Level for a case (such as Live, Closed etc). 
-- Returns the Id as set in ConcernsStatus table.
-- Note that the Ids are explicitly set in the EF migrations so they are hardcoded here for performance reasons.
-- Defaults to 3 (closed) if a match can't be made.
-- =============================================
CREATE FUNCTION [dartmigration].[GetConcernCaseStatusId]
(
	@DaRTConcernStatusName nvarchar(400)
)
RETURNS int
AS
BEGIN

	IF @DaRTConcernStatusName = 'Live'			 RETURN 1;
	ELSE IF @DaRTConcernStatusName = 'Monitoring ongoing action' RETURN 1; -- Monitored cases are Live in the new system
	ELSE IF @DaRTConcernStatusName = 'Closed'	 RETURN 3;

	RETURN 3;
END
GO
/****** Object:  UserDefinedFunction [dartmigration].[GetConcernRatingId]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

 -- =============================================
-- Author:		Emma Whitcroft
-- Create date: 24/08/2022
-- Description:	Map the string value supplied to a Concern Level (such as Amber, Green etc). 
-- Returns the Id as set in ConcernsLevel table.
-- Note that the Ids are explicitly set in the EF migrations so they are hardcoded here for performance reasons.
-- Defaults to 5 (N/A) if a match can't be made.
-- =============================================
CREATE FUNCTION [dartmigration].[GetConcernRatingId]
(
	@DaRTConcernLevelName nvarchar(400)
)
RETURNS int
AS
BEGIN
	DECLARE @ConcernsLevelId int;

	IF @DaRTConcernLevelName = 'Red Plus'			RETURN 1; --SELECT @ConcernsLevelId = [Id] FROM concerns.concernrating WHERE [Name] = 'Red-Plus'
	ELSE IF @DaRTConcernLevelName = 'Red'			RETURN 2; --SELECT @ConcernsLevelId = [Id] FROM concerns.concernrating WHERE [Name] = 'Red'
	ELSE IF @DaRTConcernLevelName = 'Red\Amber'		RETURN 3; --SELECT @ConcernsLevelId = [Id] FROM concerns.concernrating WHERE [Name] = 'Red-Amber'
	ELSE IF  @DaRTConcernLevelName = 'Amber\Green'  RETURN 4; --SELECT @ConcernsLevelId = [Id] FROM concerns.concernrating WHERE [Name] = 'Amber-Green'
	ELSE IF @DaRTConcernLevelName IS NULL			RETURN 5; --SELECT @ConcernsLevelId = [Id] FROM concerns.concernrating WHERE [Name] = 'N/A'
	
	RETURN 5;
END
GO
/****** Object:  UserDefinedFunction [dartmigration].[GetConcernRecordStatusId]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Michael Stock
-- Create date: 05/01/2023
-- Description:	Map the string value supplied to a Concern Level for a concern record (such as Live, Closed etc). 
-- Returns the Id as set in ConcernsStatus table.
-- Note that the Ids are explicitly set in the EF migrations so they are hardcoded here for performance reasons.
-- Defaults to 3 (closed) if a match can't be made.
-- =============================================
CREATE FUNCTION [dartmigration].[GetConcernRecordStatusId]
(
	@DaRTConcernStatusName nvarchar(400)
)
RETURNS int
AS
BEGIN

	IF @DaRTConcernStatusName = 'Live'			 RETURN 1;
	ELSE IF @DaRTConcernStatusName = 'Monitoring ongoing action' RETURN 3; -- Monitored cases are Live in the new system
	ELSE IF @DaRTConcernStatusName = 'Closed'	 RETURN 3;

	RETURN 3;
END
GO
/****** Object:  UserDefinedFunction [dartmigration].[GetConcernsCaseClosedDate]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


 -- =============================================
-- Author:		Michael Stock
-- Create date: 06/01/2023
-- Description:	Get the closed date for a concerns case based on its current status
-- Returns the date.
-- Note: handles monitored status that have a closed date but will be live in the new system
-- =============================================
CREATE FUNCTION [dartmigration].[GetConcernsCaseClosedDate](
	@DateTime as DateTime2,
	@Status as int
)
RETURNS DateTime2
AS
BEGIN
	IF @Status = 1 RETURN NULL
	RETURN @DateTime
END
GO
/****** Object:  UserDefinedFunction [dartmigration].[GetConcernTypeId]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



-- =============================================
-- Author:		Emma Whitcroft
-- Create date: 24/08/2022
-- Description:	Map the string value supplied to a Concerns Type. Returns the Urn as set in ConcernsType table.
-- Returns the Id as set in ConcernsType table.
-- Note that the Ids are explicitly set in the EF migrations so they are hardcoded here for performance reasons.
-- Defaults to 3 (closed) if a match can't be made.

--Id	Name	Description
--1	Compliance	Financial reporting
--2	Compliance	Financial returns
--3	Financial	Deficit
--4	Financial	Projected deficit / Low future surplus
--5	Financial	Cash flow shortfall
--6	Financial	Clawback
--7	Force majeure	NULL
--8	Governance	Governance
--9	Governance	Closure
--10	Governance	Executive Pay
--11	Governance	Safeguarding
--12	Irregularity	Allegations and self reported concerns
--13	Irregularity	Related party transactions - in year
-- Update 16th December 2022 - remove default value, return null if no mapping can be found
-- Update 20th December 2022 - change values to match new values
-- =============================================
CREATE FUNCTION [dartmigration].[GetConcernTypeId]
(
	@DaRTConcernTypeName nvarchar(400)
)
RETURNS int
AS
BEGIN
	DECLARE @ConcernsTypeUrn int;

	IF @DaRTConcernTypeName = 'Financial - Deficit'							RETURN 3; --SELECT @ConcernsTypeUrn = [Urn] FROM dartmigration.[ConcernType] WHERE [Name] = 'Financial' AND [Description] = 'Deficit'
	ELSE IF @DaRTConcernTypeName = 'Financial - Projected Deficit'			RETURN 4; --SELECT @ConcernsTypeUrn = [Urn] FROM dartmigration.[ConcernType] WHERE [Name] = 'Financial' AND [Description] = 'Projected deficit'
	ELSE IF @DaRTConcernTypeName = 'Financial - Viability'					RETURN 20; --SELECT @ConcernsTypeUrn = [Urn] FROM dartmigration.[ConcernType] WHERE [Name] = 'Financial' AND [Description] = 'Viability'
	ELSE IF @DaRTConcernTypeName = 'Governance + Compliance - Compliance'	RETURN 23; -- SELECT @ConcernsTypeUrn = [Urn] FROM dartmigration.[ConcernType] WHERE [Name] = 'Governance and compliance' AND [Description] = 'Compliance'
	ELSE IF @DaRTConcernTypeName = 'Governance + Compliance - Governance'	RETURN 8; --SELECT @ConcernsTypeUrn = [Urn] FROM dartmigration.[ConcernType] WHERE [Name] = 'Governance and compliance' AND [Description] = 'Governance'
	ELSE IF @DaRTConcernTypeName = 'Irregularity - Irregularity'			RETURN 21; --SELECT @ConcernsTypeUrn = [Urn] FROM dartmigration.[ConcernType] WHERE [Name] = 'Irregularity' AND [Description] = 'Irregularity'
	ELSE IF @DaRTConcernTypeName = 'Irregularity - Suspected Fraud'			RETURN 22; --SELECT @ConcernsTypeUrn = [Urn] FROM dartmigration.[ConcernType] WHERE [Name] = 'Irregularity' AND [Description] = 'Suspected fraud'
	ELSE IF @DaRTConcernTypeName = 'Governance + Compliance - Safeguarding' RETURN 24;
	--ELSE IF @DaRTConcernTypeName = 'Proactive Engagement â€“ SRMA support'	SELECT @ConcernsTypeUrn = [Urn] FROM dartmigration.[ConcernType] WHERE [Name] = '??'

	RETURN NULL; 

END
GO
/****** Object:  UserDefinedFunction [dartmigration].[GetDateOrMinimumIfNull]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Emma Whitcroft
-- Create date: 24/08/2022
-- Description:	Returns the input datetime, or the minimum sql datetime if the input datetime is null 
-- =============================================
CREATE FUNCTION [dartmigration].[GetDateOrMinimumIfNull](
	@DateTime as DateTime2
)
RETURNS DateTime2
AS
BEGIN
	IF @dateTime IS NULL RETURN (SELECT CONVERT (DATETIME, 0))
	RETURN @DateTime
END
GO
/****** Object:  UserDefinedFunction [dartmigration].[GetMeansOfReferralId]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Emma Whitcroft
-- Create date: 24/08/2022
-- Description:	Get Means of Referral Id
--Id	Name
--1	Internal
--2	External
-- Update 16th December - move RSC to 'Internal'
-- =============================================
CREATE FUNCTION [dartmigration].[GetMeansOfReferralId]
(
	@DaRTMeansOfReferralName nvarchar(400)
)
RETURNS int
AS
BEGIN
	IF     @DaRTMeansOfReferralName = 'Auditors'
		OR @DaRTMeansOfReferralName = 'CIU casework'
		OR @DaRTMeansOfReferralName = 'Complainant (not whistleblower)'
		OR @DaRTMeansOfReferralName = 'Education advisers'
		OR @DaRTMeansOfReferralName = 'Media activity'
		OR @DaRTMeansOfReferralName = 'Other government body'
		OR @DaRTMeansOfReferralName = 'Self reported by provider or supplier'
		OR @DaRTMeansOfReferralName = 'Whistleblower- anonymous'
		OR @DaRTMeansOfReferralName = 'Whistleblower- governor'
		OR @DaRTMeansOfReferralName = 'Whistleblower- member of staff'
		OR @DaRTMeansOfReferralName = 'Whistleblower- other'
		OR @DaRTMeansOfReferralName = 'RSCs'
	BEGIN
		RETURN 2; -- External
	END

	ELSE IF @DaRTMeansOfReferralName = 'ESFA activity'
		 OR @DaRTMeansOfReferralName = 'PRA Process'
		 OR @DaRTMeansOfReferralName = 'Trust Financial Forecasting Tool'
	BEGIN
		RETURN 1; -- Internal
	END

	RETURN NULL;
END
GO
/****** Object:  UserDefinedFunction [dartmigration].[GetTerritory]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


 -- =============================================
-- Author:		Emma Whitcroft
-- Create date: 13/09/2022
-- Description:	Gets the system value for the specified territory
-- =============================================
CREATE FUNCTION [dartmigration].[GetTerritory]
(
	@RSC nvarchar(200)
)
RETURNS nvarchar(200)
AS
BEGIN

	IF @RSC = 'North East'			RETURN 'North_And_Utc__North_East'; 
	ELSE IF @RSC = 'North West'		RETURN 'North_And_Utc__North_West'; 
	ELSE IF @RSC = 'Yorkshire and The Humber'  RETURN 'North_And_Utc__Yorkshire_And_Humber'; 
	ELSE IF @RSC = 'UTC'			RETURN 'North_And_Utc__Utc'; 
	ELSE IF @RSC = 'South East'		RETURN 'South_And_South_East__South_East'; 
	ELSE IF @RSC = 'London'			RETURN 'South_And_South_East__London'; 
	ELSE IF @RSC = 'East of England'RETURN 'South_And_South_East__East_Of_England'; 
	ELSE IF @RSC = 'South West'		RETURN 'Midlands_And_West__SouthWest'; 
	ELSE IF @RSC = 'West Midlands'	RETURN 'Midlands_And_West__West_Midlands';
	ELSE IF @RSC = 'East Midlands'	RETURN 'Midlands_And_West__East_Midlands';  
	RETURN NULL
END
GO
/****** Object:  UserDefinedFunction [dartmigration].[GetTrustUkPrn]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 -- =============================================
-- Author:		Emma Whitcroft
-- Create date: 08/09/2022
-- Description:	Get the TrustUkPrn for the specified entity.
-- =============================================
CREATE FUNCTION [dartmigration].[GetTrustUkPrn]
(
	@CompaniesHouseId nvarchar(100) NULL, @ParentCompaniesHouseId nvarchar(100) NULL
)
RETURNS int
AS
BEGIN
	DECLARE @UkPrn int;

	IF @CompaniesHouseId IS NOT NULL	SELECT TOP 1 @UkPrn = [UkPrn] FROM dartmigration.giasgroup WHERE [Companies House Number] = @CompaniesHouseId
	ELSE								SELECT TOP 1 @UkPrn = [UkPrn] FROM dartmigration.giasgroup WHERE [Companies House Number] = @ParentCompaniesHouseId
	RETURN @UkPrn
END
GO
/****** Object:  Table [dartmigration].[dart extract]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dartmigration].[dart extract](
	[Case_Type] [nvarchar](max) NULL,
	[Case_Owner] [nvarchar](max) NULL,
	[Case_Id] [int] NULL,
	[Provider_Type] [nvarchar](max) NULL,
	[Provider_Name] [nvarchar](max) NULL,
	[Provider_Id] [nvarchar](max) NULL,
	[RSC] [nvarchar](max) NULL,
	[LAESTAB] [nvarchar](max) NULL,
	[URN] [nvarchar](max) NULL,
	[Lead_Officer] [nvarchar](max) NULL,
	[Academy_Name] [nvarchar](max) NULL,
	[Parent_company_number] [nvarchar](max) NULL,
	[Parent_trust_name] [nvarchar](max) NULL,
	[Trust_Name] [nvarchar](max) NULL,
	[Company_Number] [varchar](100) NULL,
	[Case_create_date] [datetime2](7) NULL,
	[Status] [nvarchar](max) NULL,
	[Date_Closed] [datetime2](7) NULL,
	[Concern_Level] [nvarchar](max) NULL,
	[Main_Reason_for_ESFA_Concern] [nvarchar](max) NULL,
	[Issue] [nvarchar](max) NULL,
	[Concern_Details_Current_Status] [nvarchar](max) NULL,
	[Next_Steps] [nvarchar](max) NULL,
	[Direction_of_Travel] [nvarchar](max) NULL,
	[Ladders_of_Intervention_Phase] [nvarchar](max) NULL,
	[Means_of_Referral] [nvarchar](max) NULL,
	[Whistleblower_other_text] [nvarchar](max) NULL,
	[Forecasting_tool_run] [nvarchar](max) NULL,
	[Forecasting_tool_categorisation] [nvarchar](max) NULL,
	[Date_of_ESFA_initial_review] [datetime2](7) NULL,
	[Initial_ESFA_review_category] [nvarchar](max) NULL,
	[Justification_of_categorisation] [nvarchar](max) NULL,
	[TFFT_letter_issued] [nvarchar](max) NULL,
	[Date_TFFT_letter_issued] [datetime2](7) NULL,
	[Response_received_trust] [datetime2](7) NULL,
	[Trust_response] [nvarchar](max) NULL,
	[PRA_validation] [nvarchar](max) NULL,
	[AMSG_review] [nvarchar](max) NULL,
	[Date_moved_formal_intervention] [datetime2](7) NULL,
	[Addtional_notes_TFFT] [nvarchar](max) NULL,
	[Efficiency_advisor_deployed] [nvarchar](max) NULL,
	[Date_efficiency_advisor_deployed] [datetime2](7) NULL,
	[Expected_completion_date] [datetime2](7) NULL,
	[Advisors_report_completed] [datetime2](7) NULL,
	[School_improvement_plan] [nvarchar](max) NULL,
	[Reason_for_deploying_advisor] [nvarchar](max) NULL,
	[Date_Last_Updated] [datetime2](7) NULL,
	[DD_Considering_FNtI] [nvarchar](max) NULL,
	[Date_FNtI_warning_letter_issued] [nvarchar](max) NULL,
	[Date_Trust_Issued_with_FNtI] [nvarchar](max) NULL,
	[Proposed_Date_for_FNtI] [nvarchar](max) NULL,
	[PRA_Investigation] [nvarchar](max) NULL,
	[Recovery_plan_requested] [nvarchar](max) NULL,
	[Charities_commission_informed] [nvarchar](max) NULL,
	[date_recovery_plan_requested] [datetime2](7) NULL,
	[expected_date_for_return_recovery_plan] [datetime2](7) NULL,
	[Recovery_plan_status] [nvarchar](max) NULL,
	[times_plan_returned] [tinyint] NULL,
	[times_plan_returned_sea] [nvarchar](max) NULL,
	[date_viable_recovery_plan] [datetime2](7) NULL,
	[date_progressed_next_action] [nvarchar](max) NULL,
	[Deficit] [nvarchar](max) NULL,
	[Projected_deficit] [nvarchar](max) NULL,
	[Cash_flow_problems] [nvarchar](max) NULL,
	[Risk_of_insolvency] [nvarchar](max) NULL,
	[Other_financial_concerns] [nvarchar](max) NULL,
	[Inadequate_financial_governance_management] [nvarchar](max) NULL,
	[Financial_deficit] [nvarchar](max) NULL,
	[Financial_projected_defit] [nvarchar](max) NULL,
	[Financial_viability] [nvarchar](max) NULL,
	[Governance_compliance_compliance] [nvarchar](max) NULL,
	[Governance_compliance_governance] [nvarchar](max) NULL,
	[Irregularity_suspected_fraud] [nvarchar](max) NULL,
	[Irregularity_irregularity] [nvarchar](max) NULL,
	[Resolution_strategy_under_discussion] [nvarchar](max) NULL,
	[Resolution_strategy_other_details] [nvarchar](max) NULL,
	[School_efficiency_promoted] [nvarchar](max) NULL,
	[School_efficiency_used] [nvarchar](max) NULL,
	[Concern_PNA_related] [nvarchar](max) NULL,
	[Family_relationships_formed] [nvarchar](max) NULL,
	[Summary_of_family_relationships] [nvarchar](max) NULL,
	[Benchmarking_promoted] [nvarchar](max) NULL,
	[Benchmarking_used] [nvarchar](max) NULL,
	[Metric_promoted] [nvarchar](max) NULL,
	[Metric_used] [nvarchar](max) NULL,
	[Workforce_planning_promoted] [nvarchar](max) NULL,
	[Workforce_planning_used] [nvarchar](max) NULL,
	[Planning_checks_promoted] [nvarchar](max) NULL,
	[Planning_checks_used] [nvarchar](max) NULL,
	[National_deals_promoted] [nvarchar](max) NULL,
	[national_deals_used] [nvarchar](max) NULL,
	[financial_health_promoted] [nvarchar](max) NULL,
	[financial_health_used] [nvarchar](max) NULL,
	[Summary_of_other_DfE_tools_used] [nvarchar](max) NULL,
	[Summary_of_any_third_party_developed_tools_guidance_used] [nvarchar](max) NULL,
	[Compliance_financial_returns] [nvarchar](max) NULL,
	[Repayment_funding] [nvarchar](max) NULL,
	[Maintaining_recovery_plan] [nvarchar](max) NULL,
	[Maintaining_action_plan] [nvarchar](max) NULL,
	[Commisioned_review] [nvarchar](max) NULL,
	[Terms_of_reference_for_review_agreed_with_ESFA] [nvarchar](max) NULL,
	[Approvals_delegated_freedoms] [nvarchar](max) NULL,
	[Other_1] [nvarchar](max) NULL,
	[Other_1_details] [nvarchar](max) NULL,
	[Other_2] [nvarchar](max) NULL,
	[Other_2_details] [nvarchar](max) NULL,
	[Other_3] [nvarchar](max) NULL,
	[Other_3_details] [nvarchar](max) NULL,
	[FNtI_Status] [nvarchar](100) NULL,
	[Proposed_Date_for_Lifting_FNtI] [nvarchar](max) NULL,
	[Earliest_date_for_lifting_FNtI] [nvarchar](max) NULL,
	[Date_FNTI_Lifted] [nvarchar](max) NULL,
	[Progress_of_FNtI] [nvarchar](max) NULL,
	[Status_of_FNtI_conditions] [nvarchar](max) NULL,
	[Evidence_non_compliance_with_FNtI] [nvarchar](max) NULL,
	[Type_of_investigation] [nvarchar](max) NULL,
	[Reason_for_Investigation] [nvarchar](max) NULL,
	[RAD_Investigation_Lead] [nvarchar](max) NULL,
	[Investigation_status] [nvarchar](max) NULL,
	[Date_institution_informed_of_investigation] [nvarchar](max) NULL,
	[Date_report_agreed_with_institution] [nvarchar](max) NULL,
	[Proposed_date_for_report_publication] [nvarchar](max) NULL,
	[Date_report_published] [nvarchar](max) NULL,
	[Type_of_financial_support_offered] [nvarchar](max) NULL,
	[Main_reason_for_financial_support] [nvarchar](max) NULL,
	[Approved_by] [nvarchar](max) NULL,
	[Amount_of_non_repayable_funding_agreed_by_ESFA] [nvarchar](max) NULL,
	[Amount_of_repayable_funding_agreed_by_ESFA] [nvarchar](max) NULL,
	[Date_ESFA_recovery_is_due_to_start] [nvarchar](max) NULL,
	[Date_ESFA_recovery_is_due_to_end] [nvarchar](max) NULL,
	[Date_recovery_completed] [nvarchar](max) NULL,
	[Financial_support_decision_case_ID] [nvarchar](max) NULL,
	[Type_of_FA_termination] [nvarchar](max) NULL,
	[Reason_for_termination] [nvarchar](max) NULL,
	[How_the_forced_termination_will_proceed] [nvarchar](max) NULL,
	[Date_approved] [nvarchar](max) NULL,
	[Termination_approved_by] [nvarchar](max) NULL,
	[Date_termination_will_be_completed] [nvarchar](max) NULL,
	[Forced_Termination_of_FA_Current_status] [nvarchar](max) NULL,
	[Debt_at_closure] [nvarchar](max) NULL,
	[How_debt_is_to_be_resolved] [nvarchar](max) NULL,
	[Case_History] [nvarchar](max) NULL,
	[Enquiry_Number] [nvarchar](max) NULL,
	[Concern_Description] [nvarchar](max) NULL,
	[Team_Lead_where_Not_ESFA] [nvarchar](max) NULL,
	[Closed] [nvarchar](max) NULL,
	[Academy_crm_guid] [nvarchar](max) NULL,
	[Academy_crm_status] [nvarchar](max) NULL,
	[upin] [nvarchar](max) NULL,
	[Date_opened] [nvarchar](max) NULL,
	[Territory] [nvarchar](max) NULL,
	[Academy_LA] [nvarchar](max) NULL,
	[EFA_Group] [nvarchar](max) NULL,
	[Org_Type] [nvarchar](max) NULL,
	[Academy_RSC] [nvarchar](max) NULL,
	[Latest_Ofsted_Rating] [nvarchar](max) NULL,
	[Type_of_Provision] [nvarchar](max) NULL,
	[Phase_of_Education] [nvarchar](max) NULL,
	[Sixth_Form] [nvarchar](max) NULL,
	[Academy_Type] [nvarchar](max) NULL,
	[Division] [nvarchar](max) NULL,
	[EFA_Lead] [nvarchar](max) NULL,
	[rat_risk_indicator] [nvarchar](max) NULL,
	[Sponsor_Name] [nvarchar](max) NULL,
	[Age_range_of_academy_as_on_CRM_Summariser_1] [nvarchar](max) NULL,
	[Age_range_of_academy_as_on_CRM_Summariser_2] [nvarchar](max) NULL,
	[LACode] [nvarchar](max) NULL,
	[active] [nvarchar](max) NULL,
	[trust] [nvarchar](max) NULL,
	[academy] [nvarchar](max) NULL,
	[school] [nvarchar](max) NULL,
	[DD] [nvarchar](max) NULL,
	[Senior_Advisor_G6] [nvarchar](max) NULL,
	[Lead_Officer_G7] [nvarchar](max) NULL,
	[Commissioner] [nvarchar](max) NULL,
	[Trust_crm_guid] [nvarchar](max) NULL,
	[Trust_crm_status] [nvarchar](max) NULL,
	[crm_id] [nvarchar](max) NULL,
	[Trust_Number] [int] NULL,
	[Trust_Type] [nvarchar](max) NULL,
	[Trust_RSC] [nvarchar](max) NULL,
	[Date_became_red_or_black] [nvarchar](max) NULL,
	[Whistleblower_case_ID] [nvarchar](max) NULL,
	[Date_FNTI_published] [nvarchar](max) NULL,
	[FNTI_warning_letter_submission_decision_case_ID] [nvarchar](max) NULL,
	[Submission_to_issue_FNTI_decision_case_ID] [nvarchar](max) NULL,
	[Submission_to_lift_FNTI_decision_case_ID] [nvarchar](max) NULL,
	[Date_FNTI_lifting_letter_published] [nvarchar](max) NULL,
	[Missing] [tinyint] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dartmigration].[vwConcernCaseRaw]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dartmigration].[vwConcernCaseRaw]
AS
SELECT        Case_Id, Concern_Level, Main_Reason_for_ESFA_Concern, Issue, Concern_Details_Current_Status, Next_Steps, Status, Direction_of_Travel, Case_create_date, Date_Last_Updated, Date_Closed, Company_Number, 
                         Trust_Name, Case_Owner, Means_of_Referral, Parent_company_number, Case_History, RSC, URN, Academy_Name, Enquiry_Number
FROM            dartmigration.[dart extract]
WHERE        (Case_Type = 'Concern')
GO
/****** Object:  Table [dartmigration].[DartUsers]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dartmigration].[DartUsers](
	[EmailAddress] [nvarchar](300) NULL,
	[DaRTName] [nvarchar](300) NULL
) ON [PRIMARY]
GO
/****** Object:  View [dartmigration].[vwConcernsCaseTransformed]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO











CREATE VIEW [dartmigration].[vwConcernsCaseTransformed]
AS
SELECT        
	Case_Id AS Urn,
	CASE ISNULL(du.EmailAddress, '') WHEN '' THEN Case_Owner ELSE du.EmailAddress END as CreatedBy,
	dartmigration.GetConcernRatingId(Concern_Level) AS RatingId,
	dartmigration.GetConcernTypeId(Main_Reason_for_ESFA_Concern) AS ConcernsTypeId,
	Issue,
    Concern_Details_Current_Status AS CurrentStatus,
	Next_Steps AS NextSteps,
	dartmigration.GetConcernCaseStatusId([Status]) AS StatusId,
	Direction_of_Travel AS DirectionOfTravel,
	cast(Case_create_date as date)  AS CreatedAt,
    cast(ISNULL(Date_Last_Updated,ISNULL(Date_Closed, Case_create_date)) as date) AS UpdatedAt,
	cast(dartmigration.GetConcernsCaseClosedDate(Date_Closed, dartmigration.GetConcernCaseStatusId([Status])) as date) AS ClosedAt,
	Trust_Name,
	dartmigration.GetDateOrMinimumIfNull(NULL) AS ReviewAt,
    dartmigration.GetDateOrMinimumIfNull(NULL) AS DeEscalation,
	dartmigration.GetMeansOfReferralId(Means_of_Referral) AS MeansOfReferralId,
	dartmigration.GetTrustUkPrn(Company_Number, Parent_company_number) AS UkPrn,
	dartmigration.GetCaseHistory(Case_History, URN, Academy_Name, Enquiry_Number) AS CaseHistory,
	dartmigration.GetTerritory(RSC) as Territory
FROM    dartmigration.vwConcernCaseRaw
LEFT OUTER JOIN dartmigration.DartUsers du on Case_Owner = du.DartName
GO
/****** Object:  View [dartmigration].[vwConcernsRecordRaw]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dartmigration].[vwConcernsRecordRaw]
AS
SELECT        Case_Id, Concern_Level, Status, Case_create_date, Date_Last_Updated, Date_Closed, Means_of_Referral, Main_Reason_for_ESFA_Concern, Case_Owner
FROM            dartmigration.[dart extract]
WHERE        (Case_Type = 'Concern')
GO
/****** Object:  View [dartmigration].[vwConcernsRecordTransformed]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dartmigration].[vwConcernsRecordTransformed]
AS
SELECT        
	Case_Owner AS CreatedBy,
	Case_Id AS Urn,
	dartmigration.GetConcernRatingId(Concern_Level) AS RatingId,
	dartmigration.GetConcernRecordStatusId(Status) AS StatusId,
	cast(Case_create_date as date) AS CreatedAt,
	 cast(ISNULL(Date_Last_Updated,ISNULL(Date_Closed, Case_create_date)) as date) AS UpdatedAt,
	cast(Date_Closed as date) AS ClosedAt,
	cast(dartmigration.GetDateOrMinimumIfNull(NULL) as date) AS ReviewAt,
	dartmigration.GetMeansOfReferralId(Means_of_Referral) AS MeansOfReferralId,
	Means_of_Referral,
	[Status],
	dartmigration.GetConcernTypeId(Main_Reason_for_ESFA_Concern) AS ConcernTypeId
FROM            
	dartmigration.vwConcernsRecordRaw
GO
/****** Object:  Table [dartmigration].[CaseIdsToBeImported]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dartmigration].[CaseIdsToBeImported](
	[CaseID] [int] NOT NULL,
	[IsImported] [bit] NULL,
 CONSTRAINT [PK_CaseIdsToBeImported] PRIMARY KEY CLUSTERED 
(
	[CaseID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dartmigration].[giasgroup]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dartmigration].[giasgroup](
	[UkPrn] [nvarchar](100) NOT NULL,
	[Companies House Number] [nvarchar](100) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dartmigration].[uspExportDartExtractToConcerns_TestData_Step1-Cases]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




-- =============================================
-- Author:		Emma Whitcroft
-- Create date: 7th September 2022
-- Description:	Populate Concerns data from the 
--              Dart Extract table into the Concerns tables
--				9th December 2022 - add case history
--				13th December 2022 - add territory
-- =============================================
CREATE PROCEDURE [dartmigration].[uspExportDartExtractToConcerns_TestData_Step1-Cases]
(
	 @CaseUrns [dartmigration].[CaseUrnTableType] READONLY
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	-- Get list of Case Urns to use
	DECLARE @CaseUrnTable [dartmigration].[CaseUrnTableType]

	IF (SELECT COUNT(*) FROM @CaseUrns) = 0
		BEGIN
			PRINT 'no case urns submitted, using all urns'
			INSERT INTO @CaseUrnTable SELECT Urn FROM [dartmigration].[vwConcernsCaseTransformed]
		END
	ELSE 
		BEGIN
			PRINT 'case urns submitted, using specified list'
			INSERT INTO @CaseUrnTable SELECT CaseUrn FROM @CaseUrns
		END

		SET IDENTITY_INSERT [sip].[concerns].[ConcernsCase] ON

	INSERT INTO [sip].[concerns].[ConcernsCase]
		(ID,
		[CreatedAt]
		,[UpdatedAt]
		,[ClosedAt]
		,[CreatedBy]
		,[Description]
		,[CrmEnquiry]
		,[TrustUkprn]
		,[DeEscalation]
		,[Issue]
		,[CurrentStatus]
		,[CaseAim]
		,[DeEscalationPoint]
		,[NextSteps]
		,[DirectionOfTravel]
		,[CaseHistory]
		,[ReasonAtReview]
		,[ReviewAt]
		,[StatusId]
		,[RatingId]
		,[Territory])

	SELECT
			Urn,
		   [CreatedAt]
		  ,[UpdatedAt]
		  ,[ClosedAt]
		  ,[CreatedBy]
		  ,'Imported from DaRT ' + FORMAT(GETDATE(), 'dd-MM-yyyy') -- description --' -- not used
		  ,'' -- crm enquiry --' -- not used
		  ,[UkPrn]
		  ,[DeEscalation]
		  ,[Issue]
		  ,[CurrentStatus]
		  ,'' -- case aim --' -- not used
		  ,'' -- deescalation point --' -- not used
		  ,[NextSteps]
		  ,[DirectionOfTravel]
		  ,[CaseHistory]
		  ,'' --reason at review --' -- not used
		  ,[ReviewAt]
		  ,[StatusId]
		  ,[RatingId]
		  ,[Territory]
	  FROM [dartmigration].[vwConcernsCaseTransformed] WHERE Urn IN (SELECT CaseUrn FROM @CaseUrnTable)

	  
		SET IDENTITY_INSERT [sip].[concerns].[ConcernsCase] OFF
 END
GO
/****** Object:  StoredProcedure [dartmigration].[uspExportDartExtractToConcerns_TestData_Step2-Records]    Script Date: 25/01/2023 11:17:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Emma Whitcroft
-- Create date: 7th September 2022
-- Description:	Populate Concerns data from the 
--              Dart Extract table into the Concerns Record table
--              Note that it uses a cursor and hence is veeeery slow
-- =============================================
CREATE PROCEDURE [dartmigration].[uspExportDartExtractToConcerns_TestData_Step2-Records]
(
	 @CaseUrns [dartmigration].[CaseUrnTableType] READONLY
)
AS
BEGIN

	-- Get list of Case Urns to use
	DECLARE @CaseUrnTable [dartmigration].[CaseUrnTableType]

	IF (SELECT COUNT(*) FROM @CaseUrns) = 0
		BEGIN
			PRINT 'no case urns submitted, using all urns'
			INSERT INTO @CaseUrnTable SELECT Urn FROM [dartmigration].[vwConcernsCaseTransformed]
		END
	ELSE 
		BEGIN
			PRINT 'case urns submitted, using specified list'
			INSERT INTO @CaseUrnTable SELECT CaseUrn FROM @CaseUrns
		END


	DECLARE  @CaseId int
	DECLARE  @CaseUrn int
	DECLARE  @ConcernTypeUrn int

	DECLARE db_cursor CURSOR LOCAL FOR 
	SELECT Id, Urn FROM [sip].[concerns].[ConcernsCase] WHERE Urn IN (SELECT CaseUrn FROM @CaseUrnTable)

	OPEN db_cursor  
	FETCH NEXT FROM db_cursor INTO @CaseId, @CaseUrn

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		PRINT 'Start import of ' + CAST(@CaseUrn as varchar(20))
		
		INSERT INTO [sip].[concerns].[ConcernsRecord]
			([CreatedAt]
			,[UpdatedAt]
			,[ReviewAt]
			,[ClosedAt]
			,[Name]
			,[Description]
			,[Reason]
			,[CaseId]
			,[TypeId]
			,[RatingId]
			--,[Urn] -- allow to auto-generate? Think this isn't used any more
			,[StatusId]
			,[MeansOfReferralId])
		SELECT
			dart.[CreatedAt]
			,dart.[UpdatedAt]
			,dart.[ReviewAt]
			,dart.[ClosedAt]
			,t.[Name]
			,t.[Description]
			,t.[Name] + ': ' + t.[Description]
			,@CaseId
			,t.Id
			,dart.[RatingId]
			--,[Urn]
			,dart.[StatusId]
			,dart.[MeansOfReferralId]
		  FROM [dartmigration].[vwConcernsRecordTransformed] dart
		  INNER JOIN [sip].[concerns].[ConcernsType] t ON t.Id = [ConcernTypeId]
		  WHERE dart.Urn = @CaseUrn

		  FETCH NEXT FROM db_cursor INTO @CaseId, @CaseUrn 
	END 

	CLOSE db_cursor  
	DEALLOCATE db_cursor 

END
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[39] 4[38] 2[13] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "dart extract (dartmigration)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 326
               Right = 455
            End
            DisplayFlags = 280
            TopColumn = 9
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 11
         Width = 284
         Width = 1500
         Width = 1500
         Width = 4875
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 3900
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dartmigration', @level1type=N'VIEW',@level1name=N'vwConcernCaseRaw'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dartmigration', @level1type=N'VIEW',@level1name=N'vwConcernCaseRaw'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[23] 4[36] 2[15] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "vwConcernCaseRaw (dartmigration)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 176
               Right = 298
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 17
         Width = 284
         Width = 1500
         Width = 1500
         Width = 2055
         Width = 2820
         Width = 3375
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 2700
         Width = 2340
         Width = 2340
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 4680
         Alias = 7485
         Table = 3240
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dartmigration', @level1type=N'VIEW',@level1name=N'vwConcernsCaseTransformed'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dartmigration', @level1type=N'VIEW',@level1name=N'vwConcernsCaseTransformed'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "dart extract (dartmigration)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 455
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dartmigration', @level1type=N'VIEW',@level1name=N'vwConcernsRecordRaw'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dartmigration', @level1type=N'VIEW',@level1name=N'vwConcernsRecordRaw'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[26] 4[35] 2[15] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "vwConcernsRecordRaw (dartmigration)"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 298
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 10
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 5520
         Alias = 3555
         Table = 3945
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dartmigration', @level1type=N'VIEW',@level1name=N'vwConcernsRecordTransformed'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dartmigration', @level1type=N'VIEW',@level1name=N'vwConcernsRecordTransformed'
GO
