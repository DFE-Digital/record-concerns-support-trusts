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
			{"Red", "Red"}, {"Red-Amber", "Red | Amber"}, 
			{"Amber-Green", "Amber | Green"}
		};
		private static readonly Dictionary<string, string> RagsCss = new Dictionary<string, string>(5)
		{
			{"n/a", ""}, {"Red-Plus", "ragtag__redplus"}, 
			{"Red", "ragtag__red"}, {"Red-Amber", "ragtag__redamber"}, 
			{"Amber-Green", "ragtag__ambergreen"}
		};
		
		private const string DateFormat = "dd-MM-yyyy";
		
		public static (IList<HomeModel>, IList<HomeModel>) Map(IList<CaseDto> casesDto,
			IList<TrustDetailsDto> trustsDetailsDto, IList<RecordDto> recordsDto,
			IList<RatingDto> ratingsDto, IList<TypeDto> typesDto, StatusDto statusLiveDto,
			StatusDto statusMonitoringDto)
		{
			var activeCases = new List<HomeModel>();
			var monitoringCases = new List<HomeModel>();

			foreach (var caseModel in casesDto)
			{
				// Find trust / academies
				var trustName = trustsDetailsDto.Where(t => t.GiasData.UkPrn == caseModel.TrustUkPrn)
					.Select(t => t.GiasData.GroupName)
					.FirstOrDefault();
				var academies = trustsDetailsDto.Where(t => t.GiasData.UkPrn == caseModel.TrustUkPrn && t.Establishments != null)
					.Select(t => string.Join(",", t.Establishments.Select(e => e.EstablishmentName)))
					.FirstOrDefault();

				// Find primary case type urn
				var recordModel = recordsDto.FirstOrDefault(r => r.CaseUrn == caseModel.Urn && r.Primary);

				// Find primary type
				var primaryCaseType = typesDto.FirstOrDefault(t => t.Urn == recordModel?.TypeUrn);

				// Rag rating
				var rating = ratingsDto.Where(r => r.Urn == recordModel?.RatingUrn)
					.Select(r => r.Name)
					.FirstOrDefault();

				Rags.TryGetValue(rating ?? "n/a", out var rag);
				RagsCss.TryGetValue(rating ?? "n/a", out var ragCss);

				// Filter per status
				if (caseModel.Status.CompareTo(statusLiveDto.Urn) == 0)
				{
					activeCases.Add(new HomeModel(
						caseModel.Urn.ToString(), 
						caseModel.CreatedAt.ToString(DateFormat),
						caseModel.UpdatedAt.ToString(DateFormat),
						caseModel.ClosedAt.ToString(DateFormat),
						trustName,
						academies,
						primaryCaseType.Name,
						primaryCaseType.Description,
						rag,
						ragCss
					));
				}
				else if (caseModel.Status.CompareTo(statusMonitoringDto.Urn) == 0)
				{
					monitoringCases.Add(new HomeModel(
						caseModel.Urn.ToString(), 
						caseModel.CreatedAt.ToString(DateFormat),
						caseModel.UpdatedAt.ToString(DateFormat),
						caseModel.ClosedAt.ToString(DateFormat),
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
	}
}