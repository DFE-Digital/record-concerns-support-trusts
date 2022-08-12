using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace Concerns.Data.Models
{
    [Table("IfdPipeline", Schema = "mstr")]
    public class IfdPipeline
    {
        [Column("SK")]
        public long Sk { get; set; }
        [Column("p_rid")]
        [StringLength(11)]
        public string PRid { get; set; }
        [Column("RID")]
        [StringLength(11)]
        public string Rid { get; set; }
        [Column("General Details.URN")]
        [StringLength(100)]
        public string GeneralDetailsUrn { get; set; }
        [Column("General Details.LAESTAB")]
        [StringLength(100)]
        public string GeneralDetailsLaestab { get; set; }
        [Column("General Details.Project Name")]
        [StringLength(100)]
        public string GeneralDetailsProjectName { get; set; }
        [Column("General Details.Academy URN")]
        [StringLength(100)]
        public string GeneralDetailsAcademyUrn { get; set; }
        [Column("General Details.Academy LAESTAB")]
        [StringLength(100)]
        public string GeneralDetailsAcademyLaestab { get; set; }
        [Column("General Details.Academy UKPRN")]
        [StringLength(100)]
        public string GeneralDetailsAcademyUkprn { get; set; }
        [Column("General Details.Academy Name")]
        [StringLength(100)]
        public string GeneralDetailsAcademyName { get; set; }
        [Column("General Details.Interest  Status")]
        [StringLength(100)]
        public string GeneralDetailsInterestStatus { get; set; }
        [Column("General Details.Project status")]
        [StringLength(100)]
        public string GeneralDetailsProjectStatus { get; set; }
        [Column("General Details.Academy Status")]
        [StringLength(100)]
        public string GeneralDetailsAcademyStatus { get; set; }
        [Column("General Details.dAO Progress")]
        [StringLength(100)]
        public string GeneralDetailsDAoProgress { get; set; }
        [Column("General Details.Stage")]
        [StringLength(100)]
        public string GeneralDetailsStage { get; set; }
        [Column("General Details.Route of Project")]
        [StringLength(100)]
        public string GeneralDetailsRouteOfProject { get; set; }
        [Column("General Details.Expected opening date", TypeName = "date")]
        public DateTime? GeneralDetailsExpectedOpeningDate { get; set; }
        [Column("General Details.Actual date opened", TypeName = "date")]
        public DateTime? GeneralDetailsActualDateOpened { get; set; }
        [Column("General Details.Re-brokered date", TypeName = "date")]
        public DateTime? GeneralDetailsReBrokeredDate { get; set; }
        [Column("General Details.Local Authority")]
        [StringLength(100)]
        public string GeneralDetailsLocalAuthority { get; set; }
        [Column("General Details.RSC Region")]
        [StringLength(100)]
        public string GeneralDetailsRscRegion { get; set; }
        [Column("General Details.Phase")]
        [StringLength(100)]
        public string GeneralDetailsPhase { get; set; }
        [Column("General Details.Grade 6")]
        [StringLength(100)]
        public string GeneralDetailsGrade6 { get; set; }
        [Column("General Details.Team leader")]
        [StringLength(100)]
        public string GeneralDetailsTeamLeader { get; set; }
        [Column("General Details.Project lead")]
        [StringLength(100)]
        public string GeneralDetailsProjectLead { get; set; }
        [Column("General Details.Divisional lead")]
        [StringLength(100)]
        public string GeneralDetailsDivisionalLead { get; set; }
        [Column("General Details.Interest project lead")]
        [StringLength(100)]
        public string GeneralDetailsInterestProjectLead { get; set; }
        [Column("General Details.Record Status")]
        [StringLength(100)]
        public string GeneralDetailsRecordStatus { get; set; }
        [Column("Interest.Date of Interest", TypeName = "date")]
        public DateTime? InterestDateOfInterest { get; set; }
        [Column("Interest.Response to interest contact date", TypeName = "date")]
        public DateTime? InterestResponseToInterestContactDate { get; set; }
        [Column("Interest.Comments")]
        public string InterestComments { get; set; }
        [Column("Interest.Contact name")]
        [StringLength(100)]
        public string InterestContactName { get; set; }
        [Column("Interest.Contact phone")]
        [StringLength(100)]
        public string InterestContactPhone { get; set; }
        [Column("Interest.Contact Email")]
        [StringLength(100)]
        public string InterestContactEmail { get; set; }
        [Column("Delivery Process.RAG Rating")]
        [StringLength(100)]
        public string DeliveryProcessRagRating { get; set; }
        [Column("Delivery Process.Main Issue for delay")]
        [StringLength(100)]
        public string DeliveryProcessMainIssueForDelay { get; set; }
        [Column("Delivery Process.Secondary Issue for delay")]
        [StringLength(100)]
        public string DeliveryProcessSecondaryIssueForDelay { get; set; }
        [Column("Delivery Process.PFI")]
        [StringLength(100)]
        public string DeliveryProcessPfi { get; set; }
        [Column("Delivery Process.Date for discussion by RSC/HTB", TypeName = "date")]
        public DateTime? DeliveryProcessDateForDiscussionByRscHtb { get; set; }
        [Column("Delivery Process.General Comments")]
        public string DeliveryProcessGeneralComments { get; set; }
        [Column("Delivery Process.Link to Workplaces")]
        public string DeliveryProcessLinkToWorkplaces { get; set; }
        [Column("Delivery Process.Main contact for conversion")]
        [StringLength(100)]
        public string DeliveryProcessMainContactForConversion { get; set; }
        [Column("Delivery Process.Main contact for conversion name")]
        [StringLength(100)]
        public string DeliveryProcessMainContactForConversionName { get; set; }
        [Column("Delivery Process.Main contact for conversion email")]
        [StringLength(100)]
        public string DeliveryProcessMainContactForConversionEmail { get; set; }
        [Column("Delivery Process.Main contact for conversion phone")]
        [StringLength(100)]
        public string DeliveryProcessMainContactForConversionPhone { get; set; }
        [Column("Delivery Process.Application form reference")]
        [StringLength(100)]
        public string DeliveryProcessApplicationFormReference { get; set; }
        [Column("Delivery Process.Considering SoS IEB stage")]
        [StringLength(100)]
        public string DeliveryProcessConsideringSoSIebStage { get; set; }
        [Column("Delivery Process.Expected Date for GB", TypeName = "date")]
        public DateTime? DeliveryProcessExpectedDateForGb { get; set; }
        [Column("Delivery Process.SoS imposed IEB")]
        [StringLength(100)]
        public string DeliveryProcessSoSImposedIeb { get; set; }
        [Column("Delivery Process.Date SoS IEB issued", TypeName = "date")]
        public DateTime? DeliveryProcessDateSoSIebIssued { get; set; }
        [Column("Delivery Process.Date LA IEB application received", TypeName = "date")]
        public DateTime? DeliveryProcessDateLaIebApplicationReceived { get; set; }
        [Column("Delivery Process.Date LA IEB application approved", TypeName = "date")]
        public DateTime? DeliveryProcessDateLaIebApplicationApproved { get; set; }
        [Column("Delivery Process.Date of Initial Meeting", TypeName = "date")]
        public DateTime? DeliveryProcessDateOfInitialMeeting { get; set; }
        [Column("Delivery Process.Date Sponsor Match agreed", TypeName = "date")]
        public DateTime? DeliveryProcessDateSponsorMatchAgreed { get; set; }
        [Column("Delivery Process.Date dAO Due Diligence Annex received", TypeName = "date")]
        public DateTime? DeliveryProcessDateDAoDueDiligenceAnnexReceived { get; set; }
        [Column("Delivery Process.Date Church/Foundation Consultation received", TypeName = "date")]
        public DateTime? DeliveryProcessDateChurchFoundationConsultationReceived { get; set; }
        [Column("Delivery Process.Viability Concern Effecting Sponsor Match")]
        [StringLength(100)]
        public string DeliveryProcessViabilityConcernEffectingSponsorMatch { get; set; }
        [Column("Delivery Process.Viability Closure Route")]
        [StringLength(100)]
        public string DeliveryProcessViabilityClosureRoute { get; set; }
        [Column("Delivery Process.Date Parent informed by Sponsor", TypeName = "date")]
        public DateTime? DeliveryProcessDateParentInformedBySponsor { get; set; }
        [Column("Delivery Process.Date for Discussion by RSC/ HTB  for agreeing pre-opening grant", TypeName = "date")]
        public DateTime? DeliveryProcessDateForDiscussionByRscHtbForAgreeingPreOpeningGrant { get; set; }
        [Column("Delivery Process.Actual Date of GB Resolution", TypeName = "date")]
        public DateTime? DeliveryProcessActualDateOfGbResolution { get; set; }
        [Column("Delivery Process.Letter sent with DtF Actions (GB & LA)", TypeName = "date")]
        public DateTime? DeliveryProcessLetterSentWithDtFActionsGbLa { get; set; }
        [Column("Delivery Process.Direction to Facilitate Conversion")]
        [StringLength(100)]
        public string DeliveryProcessDirectionToFacilitateConversion { get; set; }
        [Column("Delivery Process.Date Direction to Facilitate Conversion Issued (GB or LA)", TypeName = "date")]
        public DateTime? DeliveryProcessDateDirectionToFacilitateConversionIssuedGbOrLa { get; set; }
        [Column("Delivery Process.Equality Impact Assessments Complete")]
        [StringLength(100)]
        public string DeliveryProcessEqualityImpactAssessmentsComplete { get; set; }
        [Column("Delivery Process.PAN")]
        [StringLength(100)]
        public string DeliveryProcessPan { get; set; }
        [Column("Delivery Process.Acknowledgement and follow-up sent to school", TypeName = "date")]
        public DateTime? DeliveryProcessAcknowledgementAndFollowUpSentToSchool { get; set; }
        [Column("Delivery Process.Grant Payment Type")]
        [StringLength(100)]
        public string DeliveryProcessGrantPaymentType { get; set; }
        [Column("Delivery Process.Grant Payment processed", TypeName = "date")]
        public DateTime? DeliveryProcessGrantPaymentProcessed { get; set; }
        [Column("Delivery Process.Land")]
        [StringLength(1000)]
        public string DeliveryProcessLand { get; set; }
        [Column("Delivery Process.Risks associated to land")]
        public string DeliveryProcessRisksAssociatedToLand { get; set; }
        [Column("Delivery Process.Articles of Associations")]
        [StringLength(100)]
        public string DeliveryProcessArticlesOfAssociations { get; set; }
        [Column("Delivery Process.Articles of Associations received/cleared", TypeName = "date")]
        public DateTime? DeliveryProcessArticlesOfAssociationsReceivedCleared { get; set; }
        [Column("Delivery Process.Articles of Association related comments")]
        public string DeliveryProcessArticlesOfAssociationRelatedComments { get; set; }
        [Column("Delivery Process.Funding Agreement")]
        [StringLength(100)]
        public string DeliveryProcessFundingAgreement { get; set; }
        [Column("Delivery Process.Funding Agreement received/cleared", TypeName = "date")]
        public DateTime? DeliveryProcessFundingAgreementReceivedCleared { get; set; }
        [Column("Delivery Process.Funding Agreement related comments")]
        public string DeliveryProcessFundingAgreementRelatedComments { get; set; }
        [Column("Delivery Process.Commercial Transfer Agreement")]
        [StringLength(100)]
        public string DeliveryProcessCommercialTransferAgreement { get; set; }
        [Column("Delivery Process.Commercial Transfer Agreement received/cleared", TypeName = "date")]
        public DateTime? DeliveryProcessCommercialTransferAgreementReceivedCleared { get; set; }
        [Column("Delivery Process.Commercial Transfer Agreement related comments")]
        public string DeliveryProcessCommercialTransferAgreementRelatedComments { get; set; }
        [Column("Delivery Process.Risk protection arrangements")]
        [StringLength(100)]
        public string DeliveryProcessRiskProtectionArrangements { get; set; }
        [Column("Delivery Process.Reason for no RPA")]
        [StringLength(100)]
        public string DeliveryProcessReasonForNoRpa { get; set; }
        [Column("Delivery Process.Risk protection agreement start date", TypeName = "date")]
        public DateTime? DeliveryProcessRiskProtectionAgreementStartDate { get; set; }
        [Column("Delivery Process.Comments for Ofsted Pre–opening Inspection")]
        public string DeliveryProcessCommentsForOfstedPreOpeningInspection { get; set; }
        [Column("Delivery Process.Funding Agreement Conditions met", TypeName = "date")]
        public DateTime? DeliveryProcessFundingAgreementConditionsMet { get; set; }
        [Column("Delivery Process.Date settlement agreement approved", TypeName = "date")]
        public DateTime? DeliveryProcessDateSettlementAgreementApproved { get; set; }
        [Column("Delivery Process.Number of settlement agreements")]
        [StringLength(100)]
        public string DeliveryProcessNumberOfSettlementAgreements { get; set; }
        [Column("Delivery Process.Total amount paid to school employees")]
        [StringLength(100)]
        public string DeliveryProcessTotalAmountPaidToSchoolEmployees { get; set; }
        [Column("Delivery Process.DfE/EFA contribution")]
        [StringLength(100)]
        public string DeliveryProcessDfEEfaContribution { get; set; }
        [Column("Delivery Process.% of DfE/EFA contribution to total paid to employees")]
        [StringLength(100)]
        public string DeliveryProcessOfDfEEfaContributionToTotalPaidToEmployees { get; set; }
        [Column("Delivery Process.Did settlement exceed contractual terms?")]
        [StringLength(100)]
        public string DeliveryProcessDidSettlementExceedContractualTerms { get; set; }
        [Column("Delivery Process.Who paid the enhancement?")]
        [StringLength(100)]
        public string DeliveryProcessWhoPaidTheEnhancement { get; set; }
        [Column("Delivery Process.Baseline Date", TypeName = "date")]
        public DateTime? DeliveryProcessBaselineDate { get; set; }
        [Column("Delivery Process.Pay Run")]
        [StringLength(100)]
        public string DeliveryProcessPayRun { get; set; }
        [Column("Proposed Academy Details.New Academy Name")]
        [StringLength(100)]
        public string ProposedAcademyDetailsNewAcademyName { get; set; }
        [Column("Proposed Academy Details.New Academy URN")]
        [StringLength(100)]
        public string ProposedAcademyDetailsNewAcademyUrn { get; set; }
        [Column("Proposed Academy Details.Academy Phase Proposed")]
        [StringLength(100)]
        public string ProposedAcademyDetailsAcademyPhaseProposed { get; set; }
        [Column("Proposed Academy Details.Academy main contact name")]
        [StringLength(100)]
        public string ProposedAcademyDetailsAcademyMainContactName { get; set; }
        [Column("Proposed Academy Details.Academy Main Contact Role")]
        [StringLength(100)]
        public string ProposedAcademyDetailsAcademyMainContactRole { get; set; }
        [Column("Proposed Academy Details.Academy Main Contact Email")]
        [StringLength(100)]
        public string ProposedAcademyDetailsAcademyMainContactEmail { get; set; }
        [Column("Proposed Academy Details.Academy Main Contact Phone")]
        [StringLength(100)]
        public string ProposedAcademyDetailsAcademyMainContactPhone { get; set; }
        [Column("Proposed Academy Details.Academy Main Contact Address Line 1")]
        [StringLength(100)]
        public string ProposedAcademyDetailsAcademyMainContactAddressLine1 { get; set; }
        [Column("Proposed Academy Details.Academy Main Contact Address Line 2")]
        [StringLength(100)]
        public string ProposedAcademyDetailsAcademyMainContactAddressLine2 { get; set; }
        [Column("Proposed Academy Details.Academy Main Contact Address Line 3")]
        [StringLength(100)]
        public string ProposedAcademyDetailsAcademyMainContactAddressLine3 { get; set; }
        [Column("Proposed Academy Details.Academy Main Contact Town")]
        [StringLength(100)]
        public string ProposedAcademyDetailsAcademyMainContactTown { get; set; }
        [Column("Proposed Academy Details.Academy Main Contact County")]
        [StringLength(100)]
        public string ProposedAcademyDetailsAcademyMainContactCounty { get; set; }
        [Column("Proposed Academy Details.Academy Main Contact Postcode")]
        [StringLength(100)]
        public string ProposedAcademyDetailsAcademyMainContactPostcode { get; set; }
        [Column("Proposed Academy Details.Academy Lead Finance Name")]
        [StringLength(100)]
        public string ProposedAcademyDetailsAcademyLeadFinanceName { get; set; }
        [Column("Proposed Academy Details.Academy Lead Finance Email")]
        [StringLength(100)]
        public string ProposedAcademyDetailsAcademyLeadFinanceEmail { get; set; }
        [Column("Proposed Academy Details.Academy Lead Finance Phone")]
        [StringLength(100)]
        public string ProposedAcademyDetailsAcademyLeadFinancePhone { get; set; }
        [Column("Proposed Academy Details.Academy Secure Access Contact name")]
        [StringLength(100)]
        public string ProposedAcademyDetailsAcademySecureAccessContactName { get; set; }
        [Column("Proposed Academy Details.Academy Secure Access Contact email")]
        [StringLength(100)]
        public string ProposedAcademyDetailsAcademySecureAccessContactEmail { get; set; }
        [Column("Proposed Academy Details.Academy Proposed Capacity - Primary (R-Yr6)")]
        [StringLength(100)]
        public string ProposedAcademyDetailsAcademyProposedCapacityPrimaryRYr6 { get; set; }
        [Column("Proposed Academy Details.Academy Proposed Capacity - Secondary (Yr7 - Yr11)")]
        [StringLength(100)]
        public string ProposedAcademyDetailsAcademyProposedCapacitySecondaryYr7Yr11 { get; set; }
        [Column("Proposed Academy Details.Academy Proposed Capacity - Post 16")]
        [StringLength(100)]
        public string ProposedAcademyDetailsAcademyProposedCapacityPost16 { get; set; }
        [Column("Proposed Academy Details.Post 16")]
        [StringLength(100)]
        public string ProposedAcademyDetailsPost16 { get; set; }
        [Column("Proposed Academy Details.GAG Funding Pupil Numbers Type")]
        [StringLength(100)]
        public string ProposedAcademyDetailsGagFundingPupilNumbersType { get; set; }
        [Column("Proposed Academy Details.MAT FA Clauses 3.A - 3.F Option 1(Conv & Spons)")]
        [StringLength(100)]
        public string ProposedAcademyDetailsMatFaClauses3A3FOption1ConvSpons { get; set; }
        [Column("Proposed Academy Details.MAT FA Clauses 3.A - 3.F Option 2(FS & New Prov)")]
        [StringLength(100)]
        public string ProposedAcademyDetailsMatFaClauses3A3FOption2FsNewProv { get; set; }
        [Column("Proposed Academy Details.MAT FA Clauses 3.H (if applicable & not Conv)")]
        [StringLength(100)]
        public string ProposedAcademyDetailsMatFaClauses3HIfApplicableNotConv { get; set; }
        [Column("Proposed Academy Details.SAT FA Clauses 3.16-3.21 Option 1 (Conv & Spons)")]
        [StringLength(100)]
        public string ProposedAcademyDetailsSatFaClauses316321Option1ConvSpons { get; set; }
        [Column("Proposed Academy Details.SAT FA Clauses 3.16-3.21 Option 2 (FS & New Prov)")]
        [StringLength(100)]
        public string ProposedAcademyDetailsSatFaClauses316321Option2FsNewProv { get; set; }
        [Column("Proposed Academy Details.SAT FA Clause 3.23 (if applicable & not Conv)")]
        [StringLength(100)]
        public string ProposedAcademyDetailsSatFaClause323IfApplicableNotConv { get; set; }
        [Column("Approval Process.Application Date", TypeName = "date")]
        public DateTime? ApprovalProcessApplicationDate { get; set; }
        [Column("Approval Process.Re-start application date", TypeName = "date")]
        public DateTime? ApprovalProcessReStartApplicationDate { get; set; }
        [Column("Approval Process.Applied or Brokered")]
        [StringLength(100)]
        public string ApprovalProcessAppliedOrBrokered { get; set; }
        [Column("Approval Process.Date Submitted for AO decision", TypeName = "date")]
        public DateTime? ApprovalProcessDateSubmittedForAoDecision { get; set; }
        [Column("Approval Process.AO Decision Method")]
        [StringLength(100)]
        public string ApprovalProcessAoDecisionMethod { get; set; }
        [Column("Approval Process.Date RSC/HTB approval granted", TypeName = "date")]
        public DateTime? ApprovalProcessDateRscHtbApprovalGranted { get; set; }
        [Column("Approval Process.AO Issued Date", TypeName = "date")]
        public DateTime? ApprovalProcessAoIssuedDate { get; set; }
        [Column("Approval Process.Revoked dAO date", TypeName = "date")]
        public DateTime? ApprovalProcessRevokedDAoDate { get; set; }
        [Column("Approval Process.Date RSC funding agreement approved in principle", TypeName = "date")]
        public DateTime? ApprovalProcessDateRscFundingAgreementApprovedInPrinciple { get; set; }
        [Column("Approval Process.Funding Agreement Approved Date", TypeName = "date")]
        public DateTime? ApprovalProcessFundingAgreementApprovedDate { get; set; }
        [Column("EFA Funding.UPIN")]
        [StringLength(6)]
        public string EfaFundingUpin { get; set; }
        [Column("EFA Funding.Bank details received", TypeName = "date")]
        public DateTime? EfaFundingBankDetailsReceived { get; set; }
        [Column("EFA Funding.Draft letter sent date", TypeName = "date")]
        public DateTime? EfaFundingDraftLetterSentDate { get; set; }
        [Column("EFA Funding.Draft letter target date", TypeName = "date")]
        public DateTime? EfaFundingDraftLetterTargetDate { get; set; }
        [Column("EFA Funding.EFA territory")]
        [StringLength(100)]
        public string EfaFundingEfaTerritory { get; set; }
        [Column("EFA Funding.EFA welcome letter and finance letter sent date", TypeName = "date")]
        public DateTime? EfaFundingEfaWelcomeLetterAndFinanceLetterSentDate { get; set; }
        [Column("EFA Funding.Expected payment date", TypeName = "date")]
        public DateTime? EfaFundingExpectedPaymentDate { get; set; }
        [Column("EFA Funding.Final funding letter sent date", TypeName = "date")]
        public DateTime? EfaFundingFinalFundingLetterSentDate { get; set; }
        [Column("EFA Funding.Final funding letter target date", TypeName = "date")]
        public DateTime? EfaFundingFinalFundingLetterTargetDate { get; set; }
        [Column("EFA Funding.SUG available")]
        [StringLength(100)]
        public string EfaFundingSugAvailable { get; set; }
        [Column("EFA Funding.Reminder letter sent date", TypeName = "date")]
        public DateTime? EfaFundingReminderLetterSentDate { get; set; }
        [Column("EFA Funding.NAV Code")]
        [StringLength(100)]
        public string EfaFundingNavCode { get; set; }
        [Column("EFA Handover.PDF FA saved in workplace", TypeName = "date")]
        public DateTime? EfaHandoverPdfFaSavedInWorkplace { get; set; }
        [Column("EFA Handover.Support Grant certificate received", TypeName = "date")]
        public DateTime? EfaHandoverSupportGrantCertificateReceived { get; set; }
        [Column("EFA Handover.Pre Opening certificate received", TypeName = "date")]
        public DateTime? EfaHandoverPreOpeningCertificateReceived { get; set; }
        [Column("EFA Handover.Funding Agreement documents redacted and saved in workplaces", TypeName = "date")]
        public DateTime? EfaHandoverFundingAgreementDocumentsRedactedAndSavedInWorkplaces { get; set; }
        [Column("EFA Handover.Funding Agreement on Gov.UK", TypeName = "date")]
        public DateTime? EfaHandoverFundingAgreementOnGovUk { get; set; }
        [Column("EFA Handover.Funding Agreement to remote storage", TypeName = "date")]
        public DateTime? EfaHandoverFundingAgreementToRemoteStorage { get; set; }
        [Column("EFA Handover.Funding Agreement copy to the trust", TypeName = "date")]
        public DateTime? EfaHandoverFundingAgreementCopyToTheTrust { get; set; }
        [Column("EFA Handover.Issue 1 (requiring EFA action)")]
        [StringLength(100)]
        public string EfaHandoverIssue1RequiringEfaAction { get; set; }
        [Column("EFA Handover.Issue 2 (requiring EFA action)")]
        [StringLength(100)]
        public string EfaHandoverIssue2RequiringEfaAction { get; set; }
        [Column("EFA Handover.Live issue(s) comments")]
        [StringLength(500)]
        public string EfaHandoverLiveIssueSComments { get; set; }
        [Column("EFA Handover.Issue 1 (to be aware of)")]
        [StringLength(100)]
        public string EfaHandoverIssue1ToBeAwareOf { get; set; }
        [Column("EFA Handover.Issue 2 (to be aware of)")]
        [StringLength(100)]
        public string EfaHandoverIssue2ToBeAwareOf { get; set; }
        [Column("EFA Handover.Other issue(s) comments")]
        [StringLength(500)]
        public string EfaHandoverOtherIssueSComments { get; set; }
        [Column("EFA Handover.SACRE exemption given")]
        [StringLength(100)]
        public string EfaHandoverSacreExemptionGiven { get; set; }
        [Column("EFA Handover.SACRE exemption issued on", TypeName = "date")]
        public DateTime? EfaHandoverSacreExemptionIssuedOn { get; set; }
        [Column("EFA Handover.SACRE exemption expiry date", TypeName = "date")]
        public DateTime? EfaHandoverSacreExemptionExpiryDate { get; set; }
        [Column("EFA Handover.SACRE exemption renewal applied for")]
        [StringLength(100)]
        public string EfaHandoverSacreExemptionRenewalAppliedFor { get; set; }
        [Column("EFA Handover.SACRE exemption renewal approved", TypeName = "date")]
        public DateTime? EfaHandoverSacreExemptionRenewalApproved { get; set; }
        [Column("EFA Handover.SACRE exemption renewal rejected", TypeName = "date")]
        public DateTime? EfaHandoverSacreExemptionRenewalRejected { get; set; }
        [Column("EFA Handover.SACRE new exemption expiry date", TypeName = "date")]
        public DateTime? EfaHandoverSacreNewExemptionExpiryDate { get; set; }
        [Column("EFA Handover.Handover complete date", TypeName = "date")]
        public DateTime? EfaHandoverHandoverCompleteDate { get; set; }
        [Column("Project template information.Academic year")]
        [StringLength(4)]
        public string ProjectTemplateInformationAcademicYear { get; set; }
        [Column("Project template information.Financial year")]
        [StringLength(4)]
        public string ProjectTemplateInformationFinancialYear { get; set; }
        [Column("Project template information.Viability issue?")]
        [StringLength(100)]
        public string ProjectTemplateInformationViabilityIssue { get; set; }
        [Column("Project template information.Deficit?")]
        [StringLength(100)]
        public string ProjectTemplateInformationDeficit { get; set; }
        [Column("Project template information.Relevant distance")]
        [StringLength(100)]
        public string ProjectTemplateInformationRelevantDistance { get; set; }
        [Column("Project template information.Project management forecast (£)")]
        [StringLength(100)]
        public string ProjectTemplateInformationProjectManagementForecast { get; set; }
        [Column("Project template information.Education advice & development of educational plan, curriculum, staffing structure and policies")]
        [StringLength(100)]
        public string ProjectTemplateInformationEducationAdviceDevelopmentOfEducationalPlanCurriculumStaffingStructureAndPolicies { get; set; }
        [Column("Project template information.Legal services")]
        [StringLength(100)]
        public string ProjectTemplateInformationLegalServices { get; set; }
        [Column("Project template information.Communications and marketing support")]
        [StringLength(100)]
        public string ProjectTemplateInformationCommunicationsAndMarketingSupport { get; set; }
        [Column("Project template information.Consultation services")]
        [StringLength(100)]
        public string ProjectTemplateInformationConsultationServices { get; set; }
        [Column("Project template information.HR and recruitment services (incl. TUPE)")]
        [StringLength(100)]
        public string ProjectTemplateInformationHrAndRecruitmentServicesInclTupe { get; set; }
        [Column("Project template information.Financial management and advice")]
        [StringLength(100)]
        public string ProjectTemplateInformationFinancialManagementAndAdvice { get; set; }
        [Column("Project template information.Appointment of key staff, including Principle Designate")]
        [StringLength(100)]
        public string ProjectTemplateInformationAppointmentOfKeyStaffIncludingPrincipleDesignate { get; set; }
        [Column("Project template information.Financial information systems")]
        [StringLength(100)]
        public string ProjectTemplateInformationFinancialInformationSystems { get; set; }
        [Column("Project template information.Other (e.g. ICT systems, project contingency allocation)")]
        [StringLength(100)]
        public string ProjectTemplateInformationOtherEGIctSystemsProjectContingencyAllocation { get; set; }
        [Column("Project template information.EIG rationale")]
        public string ProjectTemplateInformationEigRationale { get; set; }
        [Column("Project template information.Rationale for project")]
        public string ProjectTemplateInformationRationaleForProject { get; set; }
        [Column("Project template information.Rationale for sponsor")]
        public string ProjectTemplateInformationRationaleForSponsor { get; set; }
        [Column("Project template information.Risks and issues")]
        public string ProjectTemplateInformationRisksAndIssues { get; set; }
        [Column("Project template information.Projected revenue balance at year end")]
        [StringLength(100)]
        public string ProjectTemplateInformationProjectedRevenueBalanceAtYearEnd { get; set; }
        [Column("Project template information.Revenue deficit reasons and remedial action")]
        public string ProjectTemplateInformationRevenueDeficitReasonsAndRemedialAction { get; set; }
        [Column("Project template information.Projected capital balance at year end")]
        [StringLength(100)]
        public string ProjectTemplateInformationProjectedCapitalBalanceAtYearEnd { get; set; }
        [Column("Project template information.Capital deficit reasons and remedial action")]
        public string ProjectTemplateInformationCapitalDeficitReasonsAndRemedialAction { get; set; }
        [Column("Project template information.<AY>+1 capacity forecast")]
        [StringLength(100)]
        public string ProjectTemplateInformationAy1CapacityForecast { get; set; }
        [Column("Project template information.<AY>+1 total pupil number forecast")]
        [StringLength(100)]
        public string ProjectTemplateInformationAy1TotalPupilNumberForecast { get; set; }
        [Column("Project template information.<AY>+2 capacity forecast")]
        [StringLength(100)]
        public string ProjectTemplateInformationAy2CapacityForecast { get; set; }
        [Column("Project template information.<AY>+2 total pupil number forecast")]
        [StringLength(100)]
        public string ProjectTemplateInformationAy2TotalPupilNumberForecast { get; set; }
        [Column("Project template information.<AY>+3 capacity forecast")]
        [StringLength(100)]
        public string ProjectTemplateInformationAy3CapacityForecast { get; set; }
        [Column("Project template information.<AY>+3 total pupil number forecast")]
        [StringLength(100)]
        public string ProjectTemplateInformationAy3TotalPupilNumberForecast { get; set; }
        [Column("Project template information.<FY> Total allocation and income")]
        [StringLength(100)]
        public string ProjectTemplateInformationFyTotalAllocationAndIncome { get; set; }
        [Column("Project template information.<FY> Revenue gross expenditure ")]
        [StringLength(100)]
        public string ProjectTemplateInformationFyRevenueGrossExpenditure { get; set; }
        [Column("Project template information.<FY> Revenue balance in year")]
        [StringLength(100)]
        public string ProjectTemplateInformationFyRevenueBalanceInYear { get; set; }
        [Column("Project template information.<FY> Revenue balance brought forward")]
        [StringLength(100)]
        public string ProjectTemplateInformationFyRevenueBalanceBroughtForward { get; set; }
        [Column("Project template information.<FY> Revenue balance carried forward")]
        [StringLength(100)]
        public string ProjectTemplateInformationFyRevenueBalanceCarriedForward { get; set; }
        [Column("Project template information.<FY>+1 Total allocation and income")]
        [StringLength(100)]
        public string ProjectTemplateInformationFy1TotalAllocationAndIncome { get; set; }
        [Column("Project template information.<FY>+1 Revenue gross expenditure ")]
        [StringLength(100)]
        public string ProjectTemplateInformationFy1RevenueGrossExpenditure { get; set; }
        [Column("Project template information.<FY>+1 Revenue balance in year")]
        [StringLength(100)]
        public string ProjectTemplateInformationFy1RevenueBalanceInYear { get; set; }
        [Column("Project template information.<FY>+1 Revenue balance brought forward")]
        [StringLength(100)]
        public string ProjectTemplateInformationFy1RevenueBalanceBroughtForward { get; set; }
        [Column("Project template information.<FY>+1 Revenue balance carried forward")]
        [StringLength(100)]
        public string ProjectTemplateInformationFy1RevenueBalanceCarriedForward { get; set; }
        [Column("Project template information.<FY>+2 Total allocation and income")]
        [StringLength(100)]
        public string ProjectTemplateInformationFy2TotalAllocationAndIncome { get; set; }
        [Column("Project template information.<FY>+2 Revenue gross expenditure ")]
        [StringLength(100)]
        public string ProjectTemplateInformationFy2RevenueGrossExpenditure { get; set; }
        [Column("Project template information.<FY>+2 Revenue balance in year")]
        [StringLength(100)]
        public string ProjectTemplateInformationFy2RevenueBalanceInYear { get; set; }
        [Column("Project template information.<FY>+2 Revenue balance brought forward")]
        [StringLength(100)]
        public string ProjectTemplateInformationFy2RevenueBalanceBroughtForward { get; set; }
        [Column("Project template information.<FY>+2 Revenue balance carried forward")]
        [StringLength(100)]
        public string ProjectTemplateInformationFy2RevenueBalanceCarriedForward { get; set; }
        [Column("Ofsted.Latest Ofsted section 5 Category of Concern")]
        [StringLength(100)]
        public string OfstedLatestOfstedSection5CategoryOfConcern { get; set; }
        [Column("Ofsted.Latest Ofsted section 5 Date in Category 4", TypeName = "date")]
        public DateTime? OfstedLatestOfstedSection5DateInCategory4 { get; set; }
        [Column("Ofsted.Latest Ofsted section 5 inspection date", TypeName = "date")]
        public DateTime? OfstedLatestOfstedSection5InspectionDate { get; set; }
        [Column("Ofsted.Latest Ofsted section 5 Overall Effectiveness")]
        [StringLength(100)]
        public string OfstedLatestOfstedSection5OverallEffectiveness { get; set; }
        [Column("Ofsted.Number of months in category 4")]
        [StringLength(100)]
        public string OfstedNumberOfMonthsInCategory4 { get; set; }
        [Column("Ofsted.Latest Ofsted section 8 inspection date", TypeName = "date")]
        public DateTime? OfstedLatestOfstedSection8InspectionDate { get; set; }
        [Column("Ofsted.Latest Ofsted section 8 judgement")]
        [StringLength(100)]
        public string OfstedLatestOfstedSection8Judgement { get; set; }
        [Column("Trust & Sponsor Management.Sponsor 1 (Provisional)")]
        [StringLength(5)]
        public string TrustSponsorManagementSponsor1Provisional { get; set; }
        [Column("Trust & Sponsor Management.Sponsor 1 Name (Provisional)")]
        [StringLength(100)]
        public string TrustSponsorManagementSponsor1NameProvisional { get; set; }
        [Column("Trust & Sponsor Management.Sponsor 2 (Provisional)")]
        [StringLength(5)]
        public string TrustSponsorManagementSponsor2Provisional { get; set; }
        [Column("Trust & Sponsor Management.Sponsor 2 Name (Provisional)")]
        [StringLength(100)]
        public string TrustSponsorManagementSponsor2NameProvisional { get; set; }
        [Column("Trust & Sponsor Management.Sponsor 3 (Provisional)")]
        [StringLength(5)]
        public string TrustSponsorManagementSponsor3Provisional { get; set; }
        [Column("Trust & Sponsor Management.Sponsor 3 Name (Provisional)")]
        [StringLength(100)]
        public string TrustSponsorManagementSponsor3NameProvisional { get; set; }
        [Column("Trust & Sponsor Management.Trust")]
        [StringLength(100)]
        public string TrustSponsorManagementTrust { get; set; }
        [Column("Trust & Sponsor Management.Co-sponsor 1")]
        [StringLength(5)]
        public string TrustSponsorManagementCoSponsor1 { get; set; }
        [Column("Trust & Sponsor Management.Co-sponsor 1 Sponsor Name")]
        [StringLength(100)]
        public string TrustSponsorManagementCoSponsor1SponsorName { get; set; }
        [Column("Trust & Sponsor Management.Co-sponsor 2")]
        [StringLength(5)]
        public string TrustSponsorManagementCoSponsor2 { get; set; }
        [Column("Trust & Sponsor Management.Co-sponsor 2 Sponsor Name")]
        [StringLength(100)]
        public string TrustSponsorManagementCoSponsor2SponsorName { get; set; }
        [Column("Trust & Sponsor Management.Co-sponsor 3")]
        [StringLength(5)]
        public string TrustSponsorManagementCoSponsor3 { get; set; }
        [Column("Trust & Sponsor Management.Co-sponsor 3 Sponsor Name")]
        [StringLength(100)]
        public string TrustSponsorManagementCoSponsor3SponsorName { get; set; }
        [Column("Trust & Sponsor Management.Previous Trust")]
        [StringLength(5)]
        public string TrustSponsorManagementPreviousTrust { get; set; }
        [Column("Trust & Sponsor Management.Previous Trust name")]
        [StringLength(100)]
        public string TrustSponsorManagementPreviousTrustName { get; set; }
        [Column("Trust & Sponsor Management.Previous sponsor id")]
        [StringLength(5)]
        public string TrustSponsorManagementPreviousSponsorId { get; set; }
        [Column("Trust & Sponsor Management.Previous sponsor name")]
        [StringLength(100)]
        public string TrustSponsorManagementPreviousSponsorName { get; set; }
        [Column("Case Data.Closing")]
        [StringLength(100)]
        public string CaseDataClosing { get; set; }
        [Column("Case Data.Closure status")]
        [StringLength(100)]
        public string CaseDataClosureStatus { get; set; }
        [Column("Case Data.Date closure commenced", TypeName = "date")]
        public DateTime? CaseDataDateClosureCommenced { get; set; }
        [Column("Case Data.Project progress")]
        [StringLength(100)]
        public string CaseDataProjectProgress { get; set; }
        [Column("Case Data.Expected Closure date", TypeName = "date")]
        public DateTime? CaseDataExpectedClosureDate { get; set; }
        [Column("Case Data.Re-brokered date Case Data", TypeName = "date")]
        public DateTime? CaseDataReBrokeredDateCaseData { get; set; }
        [Column("Case Data.Trust notice?")]
        [StringLength(100)]
        public string CaseDataTrustNotice { get; set; }
        [Column("Case Data.Current KS2 RAG")]
        [StringLength(100)]
        public string CaseDataCurrentKs2Rag { get; set; }
        [Column("Case Data.Current KS4 RAG")]
        [StringLength(100)]
        public string CaseDataCurrentKs4Rag { get; set; }
        [Column("Case Data.Current KS5 RAG")]
        [StringLength(100)]
        public string CaseDataCurrentKs5Rag { get; set; }
        [Column("Case Data.Current Confidence Term")]
        [StringLength(100)]
        public string CaseDataCurrentConfidenceTerm { get; set; }
        [Column("Case Data.Current KS2 Confidence Measure")]
        [StringLength(100)]
        public string CaseDataCurrentKs2ConfidenceMeasure { get; set; }
        [Column("Case Data.Current KS4 Confidence Measure")]
        [StringLength(100)]
        public string CaseDataCurrentKs4ConfidenceMeasure { get; set; }
        [Column("Case Data.Status")]
        [StringLength(100)]
        public string CaseDataStatus { get; set; }
        [Column("Case Data.FNtl issued", TypeName = "date")]
        public DateTime? CaseDataFntlIssued { get; set; }
        [Column("Case Data.FNtl removed", TypeName = "date")]
        public DateTime? CaseDataFntlRemoved { get; set; }
        [Column("Case Data.EFA RAG rating")]
        [StringLength(100)]
        public string CaseDataEfaRagRating { get; set; }
        [Column("Case Data.Concern Type")]
        [StringLength(100)]
        public string CaseDataConcernType { get; set; }
        [Column("Case Data.KS2 below the floor?")]
        [StringLength(100)]
        public string CaseDataKs2BelowTheFloor { get; set; }
        [Column("Case Data.KS4 below the floor?")]
        [StringLength(100)]
        public string CaseDataKs4BelowTheFloor { get; set; }
        [Column("Case Data.KS5 below the floor? - Academic Case")]
        [StringLength(100)]
        public string CaseDataKs5BelowTheFloorAcademicCase { get; set; }
        [Column("Case Data.KS5 below the floor? - Applied General Case")]
        [StringLength(100)]
        public string CaseDataKs5BelowTheFloorAppliedGeneralCase { get; set; }
        [Column("Case Data.KS2 Coasting Academy?")]
        [StringLength(100)]
        public string CaseDataKs2CoastingAcademy { get; set; }
        [Column("Case Data.KS4 Coasting Academy?")]
        [StringLength(100)]
        public string CaseDataKs4CoastingAcademy { get; set; }
        [Column("Case Data.Education adviser timing of next visit")]
        [StringLength(100)]
        public string CaseDataEducationAdviserTimingOfNextVisit { get; set; }
        [Column("Case Data.Increased capacity in Trust/GB")]
        [StringLength(100)]
        public string CaseDataIncreasedCapacityInTrustGb { get; set; }
        [Column("Case Data.Change of leadership")]
        [StringLength(100)]
        public string CaseDataChangeOfLeadership { get; set; }
        [Column("Case Data.Academy contact number")]
        [StringLength(100)]
        public string CaseDataAcademyContactNumber { get; set; }
        [Column("Case Data.Academy head/principal")]
        [StringLength(100)]
        public string CaseDataAcademyHeadPrincipal { get; set; }
        [Column("Case Data.Comments/next steps")]
        public string CaseDataCommentsNextSteps { get; set; }
        [Column("Case Data.Planned Action")]
        [StringLength(100)]
        public string CaseDataPlannedAction { get; set; }
        [Column("Case Data.Other action taken")]
        [StringLength(100)]
        public string CaseDataOtherActionTaken { get; set; }
        [Column("Case Data.Date of initial contact", TypeName = "date")]
        public DateTime? CaseDataDateOfInitialContact { get; set; }
        [Column("Case Data.Link to Workplaces")]
        public string CaseDataLinkToWorkplaces { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Modified { get; set; }
        [Column("Modified By")]
        [StringLength(100)]
        public string ModifiedBy { get; set; }
    }
}
