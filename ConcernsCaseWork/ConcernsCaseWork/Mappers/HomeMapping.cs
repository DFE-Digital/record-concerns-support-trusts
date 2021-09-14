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
		private static readonly Dictionary<string, string> Rags = new Dictionary<string, string>(5)
		{
			{"n/a", "-"}, {"Red-Plus", "Red Plus"}, 
			{"Red", "Red"}, {"Red-Amber", "Red Amber"}, 
			{"Amber-Green", "Amber Green"},
			{"Green", "Green"}
		};
		private static readonly Dictionary<string, string> RagsCss = new Dictionary<string, string>(5)
		{
			{"n/a", ""}, {"Red-Plus", "ragtag__redplus"}, 
			{"Red", "ragtag__red"}, {"Red-Amber", "ragtag__redamber"}, 
			{"Amber-Green", "ragtag__ambergreen"},
			{"Green", "ragtag__green"}
		};
		
		private const string DateFormat = "dd-MM-yyyy";
		
		public static (IList<HomeModel>, IList<HomeModel>) Map(IEnumerable<CaseDto> casesDto,
			IList<TrustDetailsDto> trustsDetailsDto, IList<RecordDto> recordsDto,
			IList<RatingDto> ratingsDto, IList<TypeDto> typesDto, StatusDto statusLiveDto,
			StatusDto statusMonitoringDto)
		{
			var activeCases = new List<HomeModel>();
			var monitoringCases = new List<HomeModel>();

			foreach (var caseDto in casesDto)
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

		public static string FetchRag(string rating)
		{
			var defaultRating = "n/a";
			return Rags.TryGetValue(rating ?? defaultRating, out var rag) ? rag : defaultRating;
		}
		
		public static string FetchRagCss(string rating)
		{
			var defaultRating = "n/a";
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