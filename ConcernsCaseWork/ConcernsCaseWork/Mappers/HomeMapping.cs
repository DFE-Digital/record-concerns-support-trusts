using ConcernsCaseWork.Models;
using Service.TRAMS.Cases;
using Service.TRAMS.Rating;
using Service.TRAMS.Records;
using Service.TRAMS.Status;
using Service.TRAMS.Trusts;
using Service.TRAMS.Type;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ConcernsCaseWork.Mappers
{
	public static class HomeMapping
	{
		private const string DateFormat = "dd-MM-yyyy";
		
		public static IList<HomeModel> Map(IEnumerable<CaseDto> casesDto,
			IList<TrustDetailsDto> trustsDetailsDto, IList<RecordDto> recordsDto,
			IList<RatingDto> ratingsDto, IList<TypeDto> typesDto, StatusDto statusDto)
		{
			var cases = new List<HomeModel>();
			
			if (statusDto.Name.Equals(StatusEnum.Live.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				foreach (var caseDto in casesDto.OrderByDescending(c => c.UpdatedAt))
				{
					MapCases(cases, caseDto, trustsDetailsDto, recordsDto, ratingsDto, typesDto);
				}
			} 
			else if (statusDto.Name.Equals(StatusEnum.Monitoring.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				foreach (var caseDto in casesDto.OrderBy(c => c.ReviewAt))
				{
					MapCases(cases, caseDto, trustsDetailsDto, recordsDto, ratingsDto, typesDto);
				}
			}
			else if (statusDto.Name.Equals(StatusEnum.Close.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				foreach (var caseDto in casesDto.OrderBy(c => c.ClosedAt))
				{
					MapCases(cases, caseDto, trustsDetailsDto, recordsDto, ratingsDto, typesDto);
				}
			}
			
			return cases;
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