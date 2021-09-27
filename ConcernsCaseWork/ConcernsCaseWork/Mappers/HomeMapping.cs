using ConcernsCaseWork.Models;
using Service.TRAMS.Cases;
using Service.TRAMS.Rating;
using Service.TRAMS.Records;
using Service.TRAMS.Status;
using Service.TRAMS.Trusts;
using Service.TRAMS.Type;
using System.Collections.Generic;
using System.Linq;
using ConcernsCaseWork.Extensions;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Mappers
{
	public static class HomeMapping
	{
		private const string DateFormat = "dd-MM-yyyy";
		
		public static (IList<HomeModel>, IList<HomeModel>) Map(IEnumerable<CaseDto> casesDto,
			IList<TrustDetailsDto> trustsDetailsDto, IList<RecordDto> recordsDto,
			IList<RatingDto> ratingsDto, IList<TypeDto> typesDto, StatusDto statusLiveDto,
			StatusDto statusMonitoringDto)
		{
			var activeCases = new List<HomeModel>();
			var monitoringCases = new List<HomeModel>();

			// Active and Monitoring cases split
			(IList<CaseDto> activeCasesDto, IList<CaseDto> monitoringCasesDto) = casesDto.Split(
				caseDto => caseDto.Status.CompareTo(statusLiveDto.Urn) == 0, 
				caseDto => caseDto.Status.CompareTo(statusMonitoringDto.Urn) == 0);

			var tasks = new List<Task>
			{
				Task.Run(() => {
					foreach (var caseDto in activeCasesDto.OrderByDescending(c => c.UpdatedAt))
					{
						MapCases(activeCases, caseDto, trustsDetailsDto, recordsDto, ratingsDto, typesDto);
					}
				}),
				Task.Run(() => {
					foreach (var caseDto in monitoringCasesDto.OrderBy(c => c.ReviewAt))
					{
						MapCases(monitoringCases, caseDto, trustsDetailsDto, recordsDto, ratingsDto, typesDto);
					}
				})
			};

			Task.WaitAll(tasks.ToArray());
			
			return (activeCases, monitoringCases);
		}

		private static void MapCases(ICollection<HomeModel> cases, CaseDto caseDto, IList<TrustDetailsDto> trustsDetailsDto, 
			IList<RecordDto> recordsDto, IEnumerable<RatingDto> ratingsDto, IEnumerable<TypeDto> typesDto)
		{
			// Find trust / academies
			var trustName = FetchTrustName(trustsDetailsDto, caseDto);
			var academies = FetchAcademies(trustsDetailsDto, caseDto);

			// Find primary case record
			var primaryRecordModel = recordsDto.FirstOrDefault(r => r.CaseUrn.CompareTo(caseDto.Urn) == 0 && r.Primary);
			if (primaryRecordModel is null) return;
				
			// Find primary type
			var primaryCaseType = typesDto.FirstOrDefault(t => t.Urn.CompareTo(primaryRecordModel.TypeUrn) == 0);
			if (primaryCaseType is null) return;
				
			// Find primary case type urn
			var recordDto = recordsDto.FirstOrDefault(r => r.CaseUrn.CompareTo(caseDto.Urn) == 0);
			if (recordDto is null) return;
				
			// Rag rating
			var rating = ratingsDto.Where(r => r.Urn.CompareTo(recordDto.RatingUrn) == 0)
				.Select(r => r.Name)
				.First();

			var rag = RagMapping.FetchRag(rating);
			var ragCss = RagMapping.FetchRagCss(rating);
				
			cases.Add(new HomeModel(
				caseDto.Urn.ToString(), 
				caseDto.CreatedAt.ToString(DateFormat),
				caseDto.UpdatedAt.ToString(DateFormat),
				caseDto.ClosedAt.ToString(DateFormat),
				caseDto.ReviewAt.ToString(DateFormat),
				trustName,
				academies,
				primaryCaseType.Name,
				primaryCaseType.Description,
				rag,
				ragCss
			));
		}
		
		public static string FetchTrustName(IEnumerable<TrustDetailsDto> trustsDetailsDto, CaseDto caseDto)
		{
			 return trustsDetailsDto.Where(t => t is { GiasData: { } } && t.GiasData.UkPrn == caseDto.TrustUkPrn)
				 .Select(t => t.GiasData.GroupName)
				 .FirstOrDefault();
		}

		private static string FetchAcademies(IEnumerable<TrustDetailsDto> trustsDetailsDto, CaseDto caseDto)
		{
			return trustsDetailsDto.Where(t => t is { GiasData: { } } && t.GiasData.UkPrn == caseDto.TrustUkPrn && t.Establishments != null)
				.Select(t => string.Join(",", t.Establishments.Select(e => e.EstablishmentName)))
				.FirstOrDefault();
		}
	}
}