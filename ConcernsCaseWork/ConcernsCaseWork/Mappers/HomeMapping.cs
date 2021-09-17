using ConcernsCaseWork.Models;
using Service.TRAMS.Cases;
using Service.TRAMS.Rating;
using Service.TRAMS.Records;
using Service.TRAMS.Status;
using Service.TRAMS.Trusts;
using Service.TRAMS.Type;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Mappers
{
	public static class HomeMapping
	{
		private static readonly Dictionary<string, IList<string>> Rags = new Dictionary<string, IList<string>>(7)
		{
			{"n/a", new List<string> { "-" }}, 
			{"Red-Plus", new List<string> { "Red Plus" }}, 
			{"Red", new List<string> { "Red" }}, 
			{"Red-Amber", new List<string> { "Red", "Amber" }}, 
			{"Amber-Green", new List<string> { "Amber", "Green" }},
			{"Amber", new List<string> { "Amber" }},
			{"Green", new List<string> { "Green" }}
		};
		private static readonly Dictionary<string, IList<string>> RagsCss = new Dictionary<string, IList<string>>(7)
		{
			{"n/a", new List<string> { "" }}, 
			{"Red-Plus", new List<string> { "ragtag__redplus" }}, 
			{"Red", new List<string> { "ragtag__red" }}, 
			{"Red-Amber", new List<string> { "ragtag__red", "ragtag__amber" }}, 
			{"Amber-Green", new List<string> { "ragtag__amber", "ragtag__green" }},
			{"Amber", new List<string> { "ragtag__amber" }},
			{"Green", new List<string> { "ragtag__green" }}
		};
		
		private const string DateFormat = "dd-MM-yyyy";
		
		public static (IList<HomeModel>, IList<HomeModel>) Map(IEnumerable<CaseDto> casesDto,
			IList<TrustDetailsDto> trustsDetailsDto, IList<RecordDto> recordsDto,
			IList<RatingDto> ratingsDto, IList<TypeDto> typesDto, StatusDto statusLiveDto,
			StatusDto statusMonitoringDto)
		{
			var activeCases = new List<HomeModel>();
			var monitoringCases = new List<HomeModel>();

			foreach (var caseDto in casesDto.OrderByDescending(c => c.UpdatedAt))
			{
				// Find trust / academies
				var trustName = FetchTrustName(trustsDetailsDto, caseDto);
				var academies = FetchAcademies(trustsDetailsDto, caseDto);

				// Find primary case record
				var primaryRecordModel = recordsDto.FirstOrDefault(r => r.CaseUrn.CompareTo(caseDto.Urn) == 0 && r.Primary);
				if (primaryRecordModel is null) continue;
				
				// Find primary type
				var primaryCaseType = typesDto.FirstOrDefault(t => t.Urn.CompareTo(primaryRecordModel.TypeUrn) == 0);
				if (primaryCaseType is null) continue;
				
				// Find primary case type urn
				var recordDto = recordsDto.FirstOrDefault(r => r.CaseUrn.CompareTo(caseDto.Urn) == 0);
				if (recordDto is null) continue;
				
				// Rag rating
				var rating = ratingsDto.Where(r => r.Urn.CompareTo(recordDto.RatingUrn) == 0)
					.Select(r => r.Name)
					.First();

				var rag = FetchRag(rating);
				var ragCss = FetchRagCss(rating);
				
				// Filter per status
				if (caseDto.Status.CompareTo(statusLiveDto.Urn) == 0)
				{
					activeCases.Add(new HomeModel(
						caseDto.Urn.ToString(), 
						caseDto.CreatedAt.ToString(DateFormat),
						caseDto.UpdatedAt.ToString(DateFormat),
						caseDto.ClosedAt.ToString(DateFormat),
						trustName,
						academies,
						primaryCaseType.Name,
						primaryCaseType.Description,
						rag,
						ragCss
					));
				}
				else if (caseDto.Status.CompareTo(statusMonitoringDto.Urn) == 0)
				{
					monitoringCases.Add(new HomeModel(
						caseDto.Urn.ToString(), 
						caseDto.CreatedAt.ToString(DateFormat),
						caseDto.UpdatedAt.ToString(DateFormat),
						caseDto.ClosedAt.ToString(DateFormat),
						trustName,
						academies,
						primaryCaseType.Name,
						primaryCaseType.Description,
						rag,
						ragCss
					));
				}
			}
			
			return (activeCases, monitoringCases);
		}

		public static IList<string> FetchRag(string rating)
		{
			var defaultRating = new List<string> { "n/a" };
			return Rags.TryGetValue(rating ?? "n/a", out var rag) ? rag : defaultRating;
		}
		
		public static IList<string> FetchRagCss(string rating)
		{
			var defaultRating = new List<string> { "n/a" };
			return RagsCss.TryGetValue(rating ?? "n/a", out var ragCss) ? ragCss : defaultRating;
		}
		
		public static string FetchTrustName(IEnumerable<TrustDetailsDto> trustsDetailsDto, CaseDto caseDto)
		{
			 return trustsDetailsDto.Where(t => t.GiasData.UkPrn == caseDto.TrustUkPrn)
				 .Select(t => t.GiasData.GroupName)
				 .FirstOrDefault();
		}

		private static string FetchAcademies(IEnumerable<TrustDetailsDto> trustsDetailsDto, CaseDto caseDto)
		{
			return trustsDetailsDto.Where(t => t.GiasData.UkPrn == caseDto.TrustUkPrn && t.Establishments != null)
				.Select(t => string.Join(",", t.Establishments.Select(e => e.EstablishmentName)))
				.FirstOrDefault();
		}
	}
}