CREATE VIEW [concerns].[vw_GetCasesJointRisk] AS
SELECT
	cc.Id AS CaseId,
	cc.CreatedAt AS CaseCreated,
	trust_risk.Name AS TrustRisk,
	cc.TrustCompaniesHouseNumber AS CompaniesHouseNumber,
	cc.UpdatedAt AS CaseLastUpdated,
	cc.NextSteps AS NextSteps,
	cc.Issue AS Issue,
	STRING_AGG(ct.Name + COALESCE(' ' + ct.Description, '') + ': ' + concerns_risk.Name, ', ') AS ConcernRisks,
	COALESCE(nti_status.Name, nti_wl_status.Name) AS NtiStatus,
	COALESCE(nti.DateStarted, nti_uc.CreatedAt, nti_wl.DateLetterSent) AS DateIssued
FROM
	concerns.ConcernsCase cc WITH(NOLOCK)
LEFT OUTER JOIN
	concerns.ConcernsRating AS trust_risk WITH(NOLOCK)
ON
	cc.RatingId = trust_risk.Id
LEFT OUTER JOIN
	concerns.ConcernsRecord concerns_record WITH(NOLOCK)
ON
	cc.Id = concerns_record.CaseId
	AND concerns_record.ClosedAt IS NULL
LEFT OUTER JOIN
	concerns.ConcernsRating AS concerns_risk WITH(NOLOCK)
ON
	concerns_record.RatingId = concerns_risk.Id
LEFT OUTER JOIN
	concerns.ConcernsType ct WITH(NOLOCK)
ON
	concerns_record.TypeId = ct.Id
LEFT OUTER JOIN 
	concerns.NoticeToImproveCase nti WITH(NOLOCK) 
ON 
	cc.Id = nti.CaseUrn
	AND nti.ClosedAt IS NULL
LEFT OUTER JOIN 
	concerns.NoticeToImproveStatus nti_status WITH(NOLOCK) 
ON 
	nti.StatusId = nti_status.Id
LEFT OUTER JOIN 
	concerns.NTIUnderConsiderationCase nti_uc WITH(NOLOCK) 
ON 
	cc.Id = nti_uc.CaseUrn
	AND nti_uc.ClosedAt IS NULL
LEFT OUTER JOIN 
	concerns.NTIWarningLetterCase nti_wl WITH(NOLOCK) 
ON 
	cc.Id = nti_wl.CaseUrn
	AND nti_wl.ClosedAt IS NULL
LEFT OUTER JOIN 
	concerns.NTIWarningLetterStatus nti_wl_status WITH(NOLOCK) 
ON 
	nti_wl.StatusId = nti_wl_status.Id
GROUP BY
	cc.Id,
	cc.CreatedAt,
	trust_risk.Name,
	cc.TrustCompaniesHouseNumber,
	cc.UpdatedAt,
	cc.NextSteps,
	cc.Issue,
	nti_status.Name,
	nti_wl_status.Name,
	nti.DateStarted,
	nti_uc.CreatedAt,
	nti_wl.DateLetterSent