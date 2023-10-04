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
				HintFromPartialView = "_RiskManagementFramework"
			};
		}

		public static RadioButtonsUiComponent BuildMeansOfReferral(string name, int? selectedId = null)
		{
			var meansOfReferralValues = new[]
			{
				new {
					enumValue = MeansOfReferral.Internal,
					HintText = "For example, management letter, external review of governance, ESFA activity or other departmental activity."
				},
				new {
					enumValue = MeansOfReferral.External,
					HintText = "For example, whistleblowing, self-reported, SCCU, CIU casework, regional director (RD), Ofsted or other government bodies."
				}
			};

			var radioItems = meansOfReferralValues
				.Select(v =>
				{
					return new SimpleRadioItem(v.enumValue.Description(), (int)v.enumValue) { TestId = v.enumValue.Description(), HintText = v.HintText };
				}).ToArray();

			return new(ElementRootId: "means-of-referral", name, "What was the source of the concern?")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				Required = true,
				DisplayName = "means of referral"
			};
		}

		public static RadioButtonsUiComponent BuildConcernType(string name, int? selectedId = null)
		{
			var radioItems = new List<SimpleRadioItem>()
			{
				new SimpleRadioItem(ConcernType.FinancialDeficit.Description(), (int)ConcernType.FinancialDeficit) { TestId = ConcernType.FinancialDeficit.Description() },
				new SimpleRadioItem(ConcernType.FinancialProjectedDeficit.Description(), (int)ConcernType.FinancialProjectedDeficit) { TestId = ConcernType.FinancialProjectedDeficit.Description() },
				new SimpleRadioItem(ConcernType.FinancialViability.Description(), (int)ConcernType.FinancialViability) { TestId = ConcernType.FinancialViability.Description() },
				new SimpleRadioItem(ConcernType.Compliance.Description(), (int)ConcernType.Compliance) { TestId = ConcernType.Compliance.Description() },
				new SimpleRadioItem(ConcernType.Governance.Description(), (int)ConcernType.Governance) { TestId = ConcernType.Governance.Description() },
				new SimpleRadioItem(ConcernType.ForceMajeure.Description(), (int)ConcernType.ForceMajeure) { TestId = ConcernType.ForceMajeure.Description() },
				new SimpleRadioItem(ConcernType.Irregularity.Description(), (int)ConcernType.Irregularity) { TestId = ConcernType.Irregularity.Description() },
				new SimpleRadioItem(ConcernType.IrregularitySuspectedFraud.Description(), (int)ConcernType.IrregularitySuspectedFraud) { TestId = ConcernType.IrregularitySuspectedFraud.Description() }
			};

			return new(ElementRootId: "concern-type", name, "Select concern type")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				Required = true,
				DisplayName = "concern type",
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
