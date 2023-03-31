SELECT
	concerns.ConcernsCase.Id AS CaseId,
	concerns.ConcernsCase.CreatedAt AS CaseCreated,
	TrustRisk.Name AS TrustRisk,
	concerns.ConcernsCase.TrustCompaniesHouseNumber AS CompaniesHouseNumber,
	concerns.ConcernsCase.UpdatedAt AS CaseLastUpdated,
	concerns.ConcernsCase.NextSteps AS NextSteps,
	concerns.ConcernsCase.Issue AS Issue,
	STRING_AGG(ConcernRisk.Name, ', ') AS ConcernRisks
FROM
	concerns.ConcernsCase WITH(NOLOCK)
INNER JOIN
	concerns.ConcernsRating AS TrustRisk WITH(NOLOCK)
ON
	TrustRisk.Id = concerns.ConcernsCase.RatingId
INNER JOIN
	concerns.ConcernsRecord WITH(NOLOCK)
ON
	concerns.ConcernsRecord.CaseId = concerns.ConcernsCase.Id
INNER JOIN
	concerns.ConcernsRating AS ConcernRisk WITH(NOLOCK)
ON
	ConcernRisk.Id = concerns.ConcernsRecord.RatingId
GROUP BY
	concerns.ConcernsCase.Id,
	concerns.ConcernsCase.CreatedAt,
	TrustRisk.Name,
	concerns.ConcernsCase.TrustCompaniesHouseNumber,
	concerns.ConcernsCase.UpdatedAt,
	concerns.ConcernsCase.NextSteps,
	concerns.ConcernsCase.Issue