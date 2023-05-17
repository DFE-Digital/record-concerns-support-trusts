-- Delete all data from concerns by username

BEGIN TRANSACTION

-- Add the name of the user you want to delete data for
DECLARE @username VARCHAR(100) = 'insert-name'

SELECT 
	Id
INTO
	#cases_to_delete
FROM
	concerns.ConcernsCase
WHERE
	CreatedBy = @username

-- NTI
SELECT
	nti.Id
INTO #ntis_to_delete
FROM
	concerns.NoticeToImproveCase nti
INNER JOIN
	concerns.ConcernsCase c
ON
	nti.CaseUrn = c.Id

DELETE nti_reason
FROM 
	concerns.NoticeToImproveReasonMapping nti_reason 
WHERE 
	nti_reason.NoticeToImproveId IN (SELECT Id FROM #ntis_to_delete)

DELETE nti_condition
FROM 
	concerns.NoticeToImproveConditionMapping nti_condition 
WHERE 
	nti_condition.NoticeToImproveId IN (SELECT Id FROM #ntis_to_delete)

DELETE nti
FROM
	concerns.NoticeToImproveCase nti
WHERE
	nti.Id IN (SELECT Id FROM #ntis_to_delete)

-- NTI Under consideration
SELECT
	nti_uc.Id
INTO #nti_ucs_to_delete
FROM
	concerns.NTIUnderConsiderationCase nti_uc
INNER JOIN
	concerns.ConcernsCase c
ON
	nti_uc.CaseUrn = c.Id

DELETE nti_uc_reason
FROM 
	concerns.NTIUnderConsiderationReasonMapping nti_uc_reason 
WHERE 
	nti_uc_reason.NTIUnderConsiderationId IN (SELECT Id FROM #ntis_to_delete)

DELETE nti_uc
FROM
	concerns.NTIUnderConsiderationCase nti_uc
WHERE
	nti_uc.Id IN (SELECT Id FROM #nti_ucs_to_delete)

-- NTI Warning Letter
SELECT
	nti_wl.Id
INTO #nti_wls_to_delete
FROM
	concerns.NTIWarningLetterCase nti_wl
INNER JOIN
	concerns.ConcernsCase c
ON
	nti_wl.CaseUrn = c.Id

DELETE nti_wl_reason
FROM 
	concerns.NTIWarningLetterReasonMapping nti_wl_reason 
WHERE 
	nti_wl_reason.NTIWarningLetterId IN (SELECT Id FROM #nti_wls_to_delete)

DELETE nti_wl_condition
FROM 
	concerns.NTIWarningLetterConditionMapping nti_wl_condition 
WHERE 
	nti_wl_condition.NTIWarningLetterId IN (SELECT Id FROM #nti_wls_to_delete)

DELETE nti_wl
FROM
	concerns.NTIWarningLetterCase nti_wl
WHERE
	nti_wl.Id IN (SELECT Id FROM #nti_wls_to_delete)

-- SRMA
DELETE srma
FROM
	concerns.SRMACase srma
WHERE
	srma.CaseUrn IN (SELECT Id FROM #cases_to_delete)

-- Trust financial forecast
DELETE tff
FROM
	concerns.TrustFinancialForecast tff
WHERE
	tff.CaseUrn IN (SELECT Id FROM #cases_to_delete)

-- Decision
SELECT
	d.DecisionId
INTO #decisions_to_delete
FROM
	concerns.ConcernsDecision d
INNER JOIN
	concerns.ConcernsCase c
ON
	d.ConcernsCaseId = c.Id

SELECT
	do.DecisionOutcomeId
INTO #decision_outcomes_to_delete
FROM
	concerns.DecisionOutcome do
INNER JOIN
	concerns.ConcernsDecision d
ON
	do.DecisionId = d.DecisionId

DELETE do_ba
FROM
	concerns.DecisionOutcomeBusinessAreaMapping do_ba
WHERE
	do_ba.DecisionOutcomeId IN (SELECT DecisionOutcomeId FROM #decision_outcomes_to_delete)

DELETE do
FROM 
	concerns.DecisionOutcome do
WHERE 
	do.DecisionId IN (SELECT DecisionId FROM #decisions_to_delete)

DELETE d
FROM 
	concerns.ConcernsDecision d
WHERE 
	d.DecisionId IN (SELECT DecisionId FROM #decisions_to_delete)

-- Cases
DELETE cr
FROM
	concerns.ConcernsRecord cr
WHERE
	cr.CaseId IN (SELECT Id FROM #cases_to_delete)

DELETE c
FROM 
	concerns.ConcernsCase c 
WHERE
	c.Id IN (SELECT Id FROM #cases_to_delete)

DROP TABLE #cases_to_delete
DROP TABLE #ntis_to_delete
DROP TABLE #nti_ucs_to_delete
DROP TABLE #nti_wls_to_delete
DROP TABLE #decisions_to_delete
DROP TABLE #decision_outcomes_to_delete

-- Inspect the result and commit/rollback as appriopriate
-- ROLLBACK;
-- COMMIT;