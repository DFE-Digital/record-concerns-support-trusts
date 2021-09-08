using ConcernsCaseWork.Models;
using System;
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

		private const string StatusLive = "Live";
		private const string StatusMonitoring = "Monitoring";
		private const string DateFormat = "dd-MM-yyyy";
		
		public static (IList<HomeModel>, IList<HomeModel>) Map(IList<CaseModel> casesModel,
			IList<TrustDetailsModel> trustsDetailsModel, IList<RecordModel> recordsModel,
			IList<RatingModel> ragsRatingModel, IList<TypeModel> typesModel)
		{
			var activeCases = new List<HomeModel>();
			var monitoringCases = new List<HomeModel>();

			foreach (var caseModel in casesModel)
			{
				// Find trust / academies
				var trustName = trustsDetailsModel.Where(t => t.GiasData.UkPrn == caseModel.TrustUkPrn)
					.Select(t => t.GiasData.GroupName)
					.FirstOrDefault();
				var academies = trustsDetailsModel.Where(t => t.GiasData.UkPrn == caseModel.TrustUkPrn && t.Establishments != null)
					.Select(t => string.Join(",", t.Establishments.Select(e => e.EstablishmentName)))
					.FirstOrDefault();

				// Find primary case type urn
				var recordModel = recordsModel.FirstOrDefault(r => r.CaseUrn == caseModel.Urn && r.Primary);

				// Find primary type
				var primaryCaseType = typesModel.FirstOrDefault(t => t.Urn == recordModel?.TypeUrn);

				// Rag rating
				var rating = ragsRatingModel.Where(r => r.Urn == recordModel?.RatingUrn)
					.Select(r => r.Name)
					.FirstOrDefault();

				Rags.TryGetValue(rating ?? "n/a", out var rag);
				RagsCss.TryGetValue(rating ?? "n/a", out var ragCss);

				// Filter per status
				if (string.Equals(caseModel.Status, StatusLive, StringComparison.CurrentCultureIgnoreCase))
				{
					activeCases.Add(new HomeModel(
						caseModel.Urn.ToString(), 
						caseModel.CreatedAt.ToString(DateFormat),
						caseModel.UpdateAt.ToString(DateFormat),
						caseModel.ClosedAt.ToString(DateFormat),
						trustName,
						academies,
						primaryCaseType.Name,
						primaryCaseType.Description,
						rag,
						ragCss
					));
				}
				else if (string.Equals(caseModel.Status, StatusMonitoring, StringComparison.CurrentCultureIgnoreCase))
				{
					monitoringCases.Add(new HomeModel(
						caseModel.Urn.ToString(), 
						caseModel.CreatedAt.ToString(DateFormat),
						caseModel.UpdateAt.ToString(DateFormat),
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