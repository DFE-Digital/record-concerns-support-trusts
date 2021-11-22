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
		public static IList<HomeModel> Map(IEnumerable<CaseDto> casesDto,
			IList<TrustDetailsDto> trustsDetailsDto, IList<RecordDto> recordsDto,
			IList<RatingDto> ratingsDto, IList<TypeDto> typesDto, StatusDto statusDto)
		{
			var cases = new List<HomeModel>();
			
			if (statusDto.Name.Equals(StatusEnum.Live.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				cases.AddRange(casesDto.OrderByDescending(c => c.UpdatedAt).Select(caseDto => MapCases(caseDto, trustsDetailsDto, recordsDto, ratingsDto, typesDto)).Where(homeModel => homeModel != null));
			} 
			else if (statusDto.Name.Equals(StatusEnum.Monitoring.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				cases.AddRange(casesDto.OrderBy(c => c.ReviewAt).Select(caseDto => MapCases(caseDto, trustsDetailsDto, recordsDto, ratingsDto, typesDto)).Where(homeModel => homeModel != null));
			}
			else if (statusDto.Name.Equals(StatusEnum.Close.ToString(), StringComparison.OrdinalIgnoreCase))
			{
				cases.AddRange(casesDto.OrderByDescending(c => c.ClosedAt).Select(caseDto => MapCases(caseDto, trustsDetailsDto, recordsDto, ratingsDto, typesDto)).Where(homeModel => homeModel != null));
			}
			
			return cases;
		}

		private static HomeModel MapCases(CaseDto caseDto, IList<TrustDetailsDto> trustsDetailsDto, 
			IEnumerable<RecordDto> recordsDto, IEnumerable<RatingDto> ratingsDto, IEnumerable<TypeDto> typesDto)
		{
			// Find trust / academies
			var trustName = FetchTrustName(trustsDetailsDto, caseDto);
			var academies = FetchAcademies(trustsDetailsDto, caseDto);

			// Find primary case record
			var primaryRecordModel = recordsDto.FirstOrDefault(r => r.CaseUrn.CompareTo(caseDto.Urn) == 0 && r.Primary);
			if (primaryRecordModel is null) return null;
				
			// Find primary type
			var primaryCaseType = typesDto.FirstOrDefault(t => t.Urn.CompareTo(primaryRecordModel.TypeUrn) == 0);
			if (primaryCaseType is null) return null;
			
			// Rag rating
			var ratingName = ratingsDto.Where(r => r.Urn.CompareTo(primaryRecordModel.RatingUrn) == 0)
				.Select(r => r.Name)
				.First();

			var rag = RatingMapping.FetchRag(ratingName);
			var ragCss = RatingMapping.FetchRagCss(ratingName);
				
			return new HomeModel(
				caseDto.Urn.ToString(), 
				caseDto.CreatedAt,
				caseDto.UpdatedAt,
				caseDto.ClosedAt,
				caseDto.ReviewAt,
				trustName,
				academies,
				primaryCaseType.Name,
				primaryCaseType.Description,
				rag,
				ragCss
			);
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