using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.API.Contracts.Enums;
using ConcernsCaseWork.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Models
{
	public class CaseComponentBuilder
	{
		private const string NarritiveHintText = "This information can be used in Stage 1 and Stage 2 meetings.";

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

		public static RadioButtonsUiComponent BuildRiskToTrust(string name, IList<RatingModel> ratingsModel, int? selectedId = null)
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

		public static RadioButtonsUiComponent BuildConcernRiskRating(string name, IList<RatingModel> ratingsModel, int? selectedId = null)
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

			return new(ElementRootId: "rag-rating", name, "Select concern risk rating")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				Required = true,
				DisplayName = "concern risk rating",			
			};
		}

		public static RadioButtonsUiComponent BuildMeansOfReferral(string name, int? selectedId = null)
		{
			var meansOfReferralValues = new[]
			{
				new {
					enumValue = MeansOfReferral.Internal,
					HintText = "ESFA activity, TFF or other departmental activity"
				},
				new {
					enumValue = MeansOfReferral.External,
					HintText = "CIU casework, whistleblowing, self reported, regional director (RD) or other government bodies"
				}
			};

			var radioItems = meansOfReferralValues
				.Select(v =>
				{
					return new SimpleRadioItem(v.enumValue.Description(), (int)v.enumValue) { TestId = v.enumValue.Description(), HintText = v.HintText };
				}).ToArray();

			return new(ElementRootId: "means-of-referral", name, "Means of referral")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				Required = true,
				DisplayName = "means of referral"
			};
		}

		public static RadioButtonsUiComponent BuildConcernType(string name, int? selectedId = null, int? selectedSubId = null)
		{
			// The id's of financial, irregularity and governance are completely made up
			// This is so we can validate the selection, but do not clash with existing values for real selections
			// Only the sub options for all but force majeure are the real values
			var radioItems = new List<SimpleRadioItem>()
			{
				new SimpleRadioItem("Financial", 101)
				{
					SubRadioItems = new List<SubRadioItem>()
					{
						BuildSubRadioItem(ConcernType.FinancialDeficit.Description(), ConcernType.FinancialDeficit),
						BuildSubRadioItem(ConcernType.FinancialProjectedDeficit.Description(), ConcernType.FinancialProjectedDeficit),
						BuildSubRadioItem(ConcernType.FinancialViability.Description(), ConcernType.FinancialViability)
					}
				},
				new SimpleRadioItem("Force majeure", (int)ConcernType.ForceMajeure),
				new SimpleRadioItem("Governance and compliance", 102)
				{
					SubRadioItems = new List<SubRadioItem>()
					{
						BuildSubRadioItem("Governance", ConcernType.Governance),
						BuildSubRadioItem("Compliance", ConcernType.Compliance)
					}
				},
				new SimpleRadioItem("Irregularity", 103)
				{
					SubRadioItems = new List<SubRadioItem>()
					{
						BuildSubRadioItem("Irregularity", ConcernType.Irregularity),
						BuildSubRadioItem(ConcernType.IrregularitySuspectedFraud.Description(), ConcernType.IrregularitySuspectedFraud)
					}
				}
			};

			return new(ElementRootId: "concern-type", name, "Select concern type")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				SelectedSubId = selectedSubId,
				Required = true,
				DisplayName = "concern type",
				SubItemDisplayName = "concern subtype",
				HintFromPartialView = "_RiskManagementFramework"
			};
		}

		public static TextAreaUiComponent BuildIssue(string name, string? value = null)
		=> new("issue", name, "Issue")
		{
			HintText = NarritiveHintText,
			Text = new ValidateableString()
			{
				MaxLength = 2000,
				StringContents = value,
				DisplayName = "Issue",
				Required = true
			}
		};

		public static TextAreaUiComponent BuildCurrentStatus(string name, string? value = null)
		=> new("current-status", name, "Current status")
		{
			HintText = NarritiveHintText,
			Text = new ValidateableString()
			{
				MaxLength = 4000,
				StringContents = value,
				DisplayName = "Current status",
			}
		};

		public static TextAreaUiComponent BuildCaseAim(string name, string? value = null)
		=> new("case-aim", name, "Case aim")
		{
			HintText = NarritiveHintText,
			Text = new ValidateableString()
			{
				MaxLength = 1000,
				StringContents = value,
				DisplayName = "Case aim"
			}
		};

		public static TextAreaUiComponent BuildDeEscalationPoint(string name, string? value = null)
		=> new("de-escalation-point", name, "De-escalation point")
		{
			HintText = NarritiveHintText,
			Text = new ValidateableString()
			{
				MaxLength = 1000,
				StringContents = value,
				DisplayName = "De-escalation point"
			}
		};

		public static TextAreaUiComponent BuildNextSteps(string name, string? value = null)
		=> new("next-steps", name, "Next steps")
		{
			HintText = NarritiveHintText,
			Text = new ValidateableString()
			{
				MaxLength = 4000,
				StringContents = value,
				DisplayName = "Next steps"
			}
		};

		public static TextAreaUiComponent BuildCaseHistory(string name, string? value = null)
		=> new("case-history", name, "Case history")
		{
			HintText = NarritiveHintText,
			Text = new ValidateableString()
			{
				MaxLength = 4300,
				StringContents = value,
				DisplayName = "Case history"
			}
		};

		private static SubRadioItem BuildSubRadioItem(string label, ConcernType concernType)
		{
			return new SubRadioItem(label, (int)concernType) { TestId = concernType.Description() };
		}
	}
}
