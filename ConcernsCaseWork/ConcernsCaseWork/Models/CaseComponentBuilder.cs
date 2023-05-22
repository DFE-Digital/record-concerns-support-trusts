using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Models
{
	public class CaseComponentBuilder
	{
		public static RadioButtonsUiComponent BuildTerritory(string name, int? selectedId = null)
		{
			var radioItems = Enum.GetValues(typeof(Territory))
				.Cast<Territory>()
				.Select(v =>
				{
					return new SimpleRadioItem(v.Description(), (int)v) { TestId = v.Description() };
				}).ToArray();

			return new(ElementRootId: "territory", name, "Which SFSO territory is managing this case?")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				Required = true,
				DisplayName = "SFSO territory"
			};
		}

		public static RadioButtonsUiComponent BuildRiskToTrustComponent(string name, IList<RatingModel> ratingsModel, int? selectedId = null)
		{
			var radioItems = ratingsModel.Select(r =>
			{
				var label = "";

				for (var ragIdx = 0; ragIdx < r.RagRating.Item2.Count; ragIdx++)
				{
					label += $"<span class=\"govuk-tag ragtag {r.RagRatingCss.ElementAt(ragIdx)}\">{r.RagRating.Item2.ElementAt(ragIdx)}</span>";
				}

				return new SimpleRadioItem(label, (int)r.Id) { IsHtmlLabel = true, TestId = r.Name, };
			});

			return new(ElementRootId: "rag-rating", name, "What is the risk to the trust?")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				Required = true,
				DisplayName = "risk to trust rating",
				HintFromPartialView = "_RiskToTrustDetails"
			};
		}
	}
}
