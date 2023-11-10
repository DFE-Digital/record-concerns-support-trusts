﻿using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Concerns;
using System;
using System.Collections.Generic;
using System.Linq;
using ConcernsCaseWork.Utils.Extensions;

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

		public static RadioButtonsUiComponent BuildRegion(string name, int? selectedId = null)
		{
			var radioItems = Enum.GetValues(typeof(Region))
				.Cast<Region>()
				.Select(v =>
				{
					return new SimpleRadioItem(v.Description(), (int)v) { TestId = v.Description() };
				}).ToArray();

			return new(ElementRootId: "region", name, "Which region is managing this case?")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				Required = true,
				DisplayName = "region"
			};
		}

		public static RadioButtonsUiComponent BuildRiskToTrust(string name, int? selectedId = null)
		{
			var radioItems = BuildRatingRadioItems();

			return new(ElementRootId: "rag-rating", name, "What is the overall risk to the trust?")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				Required = true,
				DisplayName = "risk to trust rating",
				HintFromPartialView = "_RiskToTrustDetails"
			};
		}

		public static RadioButtonsUiComponent BuildConcernRiskRating(string name, int? selectedId = null)
		{
			var radioItems = BuildRatingRadioItems();

			return new(ElementRootId: "rag-rating", name, "Select concern risk rating")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				Required = true,
				DisplayName = "concern risk rating",
				HintFromPartialView = "_RiskManagementFramework"
			};
		}

		public static RadioButtonsUiComponent BuildMeansOfReferral(Division? division, string name, int? selectedId = null)
		{
			var regionsGroup = Division.RegionsGroup;

			var meansOfReferralValues = new[]
			{
				new {
					enumValue = MeansOfReferral.Internal,
					HintText = division == regionsGroup ? "Regions Group activity including SCCU, or other departmental activity" : "For example, management letter, external review of governance, ESFA activity or other departmental activity."
				},
				new {
					enumValue = MeansOfReferral.External,
					HintText = division == regionsGroup ? "Whistleblowing, self-reported by trust, SFSO, Ofsted or other government bodies" : "For example, whistleblowing, self-reported, SCCU, CIU casework, regional director (RD), Ofsted or other government bodies."
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

		public static RadioButtonsUiComponent BuildConcernType(Division? division, string name, int? selectedId = null)
		{
			if (division == Division.RegionsGroup)
			{
				return BuildConcernTypeRegionsGroup(name, selectedId);
			}

			return BuildConcernTypeSfso(name, selectedId);
		}

		public static TextAreaUiComponent BuildIssue(string name, string? value = null)
		=> new("issue", name, "Issue (required)")
		{
			HintFromPartialView = "_IssueHint",
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
			HintFromPartialView = "_CurrentStatusHint",
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
			HintFromPartialView = "_CaseAimHint",
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
			HintFromPartialView = "_DeescalationPointHint",
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
			HintFromPartialView = "_NextStepsHint",
			Text = new ValidateableString()
			{
				MaxLength = 4000,
				StringContents = value,
				DisplayName = "Next steps"
			}
		};

		public static TextAreaUiComponent BuildCaseHistory(string name, string? value = null)
		=> new("case-history", name, "Case notes")
		{
			HintFromPartialView = "_CaseHistoryHint",
			Text = new ValidateableString()
			{
				MaxLength = 4300,
				StringContents = value,
				DisplayName = "Case notes"
			}
		};

		public static List<RatingLabelModel> GetRatings()
		{
			var result = new List<RatingLabelModel>()
			{
				new RatingLabelModel()
				{
					Id = (int)ConcernRating.AmberGreen,
					Label = $"<span class=\"govuk-tag ragtag ragtag__amber\">Amber</span><span class=\"govuk-tag ragtag ragtag__green\">Green</span>",
					Names = new List<string>() { "Amber", "Green" }
				},
				new RatingLabelModel()
				{
					Id = (int)ConcernRating.RedAmber,
					Label = $"<span class=\"govuk-tag ragtag ragtag__red\">Red</span><span class=\"govuk-tag ragtag ragtag__amber\">Amber</span>",
					Names = new List<string>() { "Red", "Amber" }
				},
				new RatingLabelModel()
				{
					Id = (int)ConcernRating.Red,
					Label = $"<span class=\"govuk-tag ragtag ragtag__red\">Red</span>",
					Names = new List<string>() { "Red" }
				},
				new RatingLabelModel()
				{
					Id = (int)ConcernRating.RedPlus,
					Label = $"<span class=\"govuk-tag ragtag ragtag__redplus\">Red Plus</span>",
					Names = new List<string>() { "Red Plus" }
				}
			};

			return result;
		}

		private static RadioButtonsUiComponent BuildConcernTypeSfso(string name, int? selectedId = null)
		{
			var radioItems = new List<SimpleRadioItem>()
			{
				new SimpleRadioItem(ConcernType.FinancialDeficit.Description(), (int)ConcernType.FinancialDeficit) { TestId = ConcernType.FinancialDeficit.Description() },
				new SimpleRadioItem(ConcernType.FinancialProjectedDeficit.Description(), (int)ConcernType.FinancialProjectedDeficit) { TestId = ConcernType.FinancialProjectedDeficit.Description() },
				new SimpleRadioItem(ConcernType.FinancialViability.Description(), (int)ConcernType.FinancialViability) { TestId = ConcernType.FinancialViability.Description() },
				new SimpleRadioItem(ConcernType.Compliance.Description(), (int)ConcernType.Compliance) { TestId = ConcernType.Compliance.Description() },
				new SimpleRadioItem(ConcernType.FinancialGovernance.Description(), (int)ConcernType.FinancialGovernance) { TestId = ConcernType.FinancialGovernance.Description() },
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

		private static RadioButtonsUiComponent BuildConcernTypeRegionsGroup(string name, int? selectedId = null)
		{
			var radioItems = new List<SimpleRadioItem>()
			{
				new SimpleRadioItem(ConcernType.Governance.Description(), (int)ConcernType.Governance) { TestId = ConcernType.Governance.Description() },
				new SimpleRadioItem(ConcernType.NonCompliance.Description(), (int)ConcernType.NonCompliance) { TestId = ConcernType.NonCompliance.Description() },
				new SimpleRadioItem(ConcernType.Safeguarding.Description(), (int)ConcernType.Safeguarding) { TestId = ConcernType.Safeguarding.Description() }
			};

			return new(ElementRootId: "concern-type", name, "Select concern type")
			{
				RadioItems = radioItems,
				SelectedId = selectedId,
				Required = true,
				DisplayName = "concern type",
				HintFromPartialView = "_ConcernTypeHint"
			};
		}

		private static IEnumerable<SimpleRadioItem> BuildRatingRadioItems()
		{
			var ratings = GetRatings();

			var result = ratings.Select(r =>
			{
				var label = r.Label;

				return new SimpleRadioItem(label, (int)r.Id) { IsHtmlLabel = true, TestId = string.Join("-", r.Names), };
			});

			return result;
		}
	}
}
