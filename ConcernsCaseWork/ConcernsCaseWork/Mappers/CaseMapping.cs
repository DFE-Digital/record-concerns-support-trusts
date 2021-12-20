﻿using ConcernsCaseWork.Models;
using Service.Redis.Models;
using Service.TRAMS.Cases;
using Service.TRAMS.Ratings;
using Service.TRAMS.Records;
using Service.TRAMS.Status;
using Service.TRAMS.Types;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Mappers
{
	public static class CaseMapping
	{
		public static CreateCaseDto Map(CreateCaseModel createCaseModel)
		{
			return new CreateCaseDto(
				createCaseModel.CreatedAt, 
				createCaseModel.UpdatedAt, 
				createCaseModel.ReviewAt, 
				createCaseModel.ClosedAt, 
				createCaseModel.CreatedBy, 
				createCaseModel.CrmEnquiry, 
				createCaseModel.TrustUkPrn, 
				createCaseModel.ReasonAtReview,
				createCaseModel.DeEscalation, 
				createCaseModel.Issue, 
				createCaseModel.CurrentStatus, 
				createCaseModel.NextSteps, 
				createCaseModel.CaseAim,
				createCaseModel.DeEscalationPoint, 
				createCaseModel.DirectionOfTravel, 
				createCaseModel.StatusUrn, 
				createCaseModel.RatingUrn);
		}
		
		public static CaseModel Map(CaseDto caseDto, string status = null)
		{
			return new CaseModel
			{
				CreatedAt = caseDto.CreatedAt, 
				UpdatedAt = caseDto.UpdatedAt, 
				ReviewAt = caseDto.ReviewAt, 
				ClosedAt = caseDto.ClosedAt,
				CreatedBy = caseDto.CreatedBy, 
				Description = caseDto.Description, 
				CrmEnquiry = caseDto.CrmEnquiry, 
				TrustUkPrn = caseDto.TrustUkPrn, 
				ReasonAtReview = caseDto.ReasonAtReview,
				DeEscalation = caseDto.DeEscalation, 
				Issue = caseDto.Issue, 
				CurrentStatus = caseDto.CurrentStatus, 
				NextSteps = caseDto.NextSteps, 
				CaseAim = caseDto.CaseAim,
				DeEscalationPoint = caseDto.DeEscalationPoint,
				DirectionOfTravel = caseDto.DirectionOfTravel, 
				Urn = caseDto.Urn,
				StatusUrn = caseDto.StatusUrn,
				StatusName = status,
				RatingUrn = caseDto.RatingUrn
			};
		}
		
		public static CaseDto MapClosure(PatchCaseModel patchCaseModel, CaseDto caseDto, StatusDto statusDto)
		{
			return new CaseDto(
				caseDto.CreatedAt, 
				patchCaseModel.UpdatedAt,
				patchCaseModel.ReviewAt ?? caseDto.ReviewAt, 
				patchCaseModel.ClosedAt ?? caseDto.ClosedAt, 
				caseDto.CreatedBy, 
				caseDto.Description,
				caseDto.CrmEnquiry, 
				caseDto.TrustUkPrn, 
				patchCaseModel.ReasonAtReview,
				caseDto.DeEscalation, 
				caseDto.Issue, 
				caseDto.CurrentStatus,
				caseDto.NextSteps, 
				caseDto.CaseAim, 
				caseDto.DeEscalationPoint, 
				caseDto.DirectionOfTravel,
				caseDto.Urn, 
				statusDto.Urn, 
				caseDto.RatingUrn);
		}
		
		public static CaseDto MapDirectionOfTravel(PatchCaseModel patchCaseModel, CaseDto caseDto)
		{
			return new CaseDto(
				caseDto.CreatedAt, 
				patchCaseModel.UpdatedAt,
				caseDto.ReviewAt, 
				caseDto.ClosedAt, 
				caseDto.CreatedBy, 
				caseDto.Description,
				caseDto.CrmEnquiry, 
				caseDto.TrustUkPrn, 
				caseDto.ReasonAtReview,
				caseDto.DeEscalation, 
				caseDto.Issue, 
				caseDto.CurrentStatus,
				caseDto.NextSteps, 
				caseDto.CaseAim, 
				caseDto.DeEscalationPoint, 
				patchCaseModel.DirectionOfTravel,
				caseDto.Urn, 
				caseDto.StatusUrn, 
				caseDto.RatingUrn);
		}
		
		public static CaseDto MapIssue(PatchCaseModel patchCaseModel, CaseDto caseDto)
		{
			return new CaseDto(
				caseDto.CreatedAt, 
				patchCaseModel.UpdatedAt,
				caseDto.ReviewAt, 
				caseDto.ClosedAt, 
				caseDto.CreatedBy, 
				caseDto.Description,
				caseDto.CrmEnquiry, 
				caseDto.TrustUkPrn, 
				caseDto.ReasonAtReview,
				caseDto.DeEscalation, 
				patchCaseModel.Issue, 
				caseDto.CurrentStatus,
				caseDto.NextSteps, 
				caseDto.CaseAim, 
				caseDto.DeEscalationPoint, 
				caseDto.DirectionOfTravel,
				caseDto.Urn, 
				caseDto.StatusUrn, 
				caseDto.RatingUrn);
		}
		
		public static CaseDto MapCurrentStatus(PatchCaseModel patchCaseModel, CaseDto caseDto)
		{
			return new CaseDto(
				caseDto.CreatedAt, 
				patchCaseModel.UpdatedAt,
				caseDto.ReviewAt, 
				caseDto.ClosedAt, 
				caseDto.CreatedBy, 
				caseDto.Description,
				caseDto.CrmEnquiry, 
				caseDto.TrustUkPrn, 
				caseDto.ReasonAtReview,
				caseDto.DeEscalation, 
				caseDto.Issue, 
				patchCaseModel.CurrentStatus,
				caseDto.NextSteps, 
				caseDto.CaseAim, 
				caseDto.DeEscalationPoint, 
				caseDto.DirectionOfTravel,
				caseDto.Urn, 
				caseDto.StatusUrn, 
				caseDto.RatingUrn);
		}
		
		public static CaseDto MapCaseAim(PatchCaseModel patchCaseModel, CaseDto caseDto)
		{
			return new CaseDto(
				caseDto.CreatedAt, 
				patchCaseModel.UpdatedAt,
				caseDto.ReviewAt, 
				caseDto.ClosedAt, 
				caseDto.CreatedBy, 
				caseDto.Description,
				caseDto.CrmEnquiry, 
				caseDto.TrustUkPrn, 
				caseDto.ReasonAtReview,
				caseDto.DeEscalation, 
				caseDto.Issue, 
				caseDto.CurrentStatus,
				caseDto.NextSteps, 
				patchCaseModel.CaseAim, 
				caseDto.DeEscalationPoint, 
				caseDto.DirectionOfTravel,
				caseDto.Urn, 
				caseDto.StatusUrn, 
				caseDto.RatingUrn);
		}
		
		public static CaseDto MapDeEscalationPoint(PatchCaseModel patchCaseModel, CaseDto caseDto)
		{
			return new CaseDto(
				caseDto.CreatedAt,
				patchCaseModel.UpdatedAt,
				caseDto.ReviewAt,
				caseDto.ClosedAt, 
				caseDto.CreatedBy, 
				caseDto.Description,
				caseDto.CrmEnquiry, 
				caseDto.TrustUkPrn, 
				caseDto.ReasonAtReview,
				caseDto.DeEscalation, 
				caseDto.Issue, 
				caseDto.CurrentStatus,
				caseDto.NextSteps, 
				caseDto.CaseAim, 
				patchCaseModel.DeEscalationPoint, 
				caseDto.DirectionOfTravel,
				caseDto.Urn, 
				caseDto.StatusUrn, 
				caseDto.RatingUrn);
		}

		public static CaseDto MapNextSteps(PatchCaseModel patchCaseModel, CaseDto caseDto)
		{
			return new CaseDto(
				caseDto.CreatedAt, 
				patchCaseModel.UpdatedAt,
				caseDto.ReviewAt, 
				caseDto.ClosedAt, 
				caseDto.CreatedBy, 
				caseDto.Description,
				caseDto.CrmEnquiry, 
				caseDto.TrustUkPrn, 
				caseDto.ReasonAtReview,
				caseDto.DeEscalation, 
				caseDto.Issue, 
				caseDto.CurrentStatus,
				patchCaseModel.NextSteps, 
				caseDto.CaseAim, 
				caseDto.DeEscalationPoint, 
				caseDto.DirectionOfTravel,
				caseDto.Urn, 
				caseDto.StatusUrn, 
				caseDto.RatingUrn);
		}

		public static CaseDto MapRating(PatchCaseModel patchCaseModel, CaseDto caseDto)
		{
			return new CaseDto(
				caseDto.CreatedAt,
				patchCaseModel.UpdatedAt,
				caseDto.ReviewAt,
				caseDto.ClosedAt,
				caseDto.CreatedBy,
				caseDto.Description,
				caseDto.CrmEnquiry,
				caseDto.TrustUkPrn,
				caseDto.ReasonAtReview,
				caseDto.DeEscalation,
				caseDto.Issue,
				caseDto.CurrentStatus,
				caseDto.NextSteps,
				caseDto.CaseAim,
				caseDto.DeEscalationPoint,
				caseDto.DirectionOfTravel,
				caseDto.Urn,
				caseDto.StatusUrn,
				patchCaseModel.RatingUrn);
		}


		public static List<TrustCasesModel> MapTrustCases(IEnumerable<RecordDto> recordsDto, IList<RatingDto> ragsRatingDto, IList<TypeDto> typesDto,
			IList<CaseDto> casesDto, StatusDto liveStatus, StatusDto closeStatus)
		{
			var trustCases = new List<TrustCasesModel>();
				
			trustCases.AddRange(
				recordsDto.Select(recordDto =>
				{
					var ragRatingDto = ragsRatingDto.FirstOrDefault(r => r.Urn.CompareTo(recordDto.RatingUrn) == 0);
					if (ragRatingDto is null) return null;

					var caseType = typesDto.FirstOrDefault(t => t.Urn.CompareTo(recordDto.TypeUrn) == 0);
					if (caseType is null) return null;

					var caseDto = casesDto.FirstOrDefault(c => c.Urn.CompareTo(recordDto.CaseUrn) == 0);
					if (caseDto is null) return null;
					
					var trustCase = new TrustCasesModel(
						recordDto.CaseUrn,
						caseType.Name,
						caseType.Description,
						RatingMapping.FetchRag(ragRatingDto.Name),
						RatingMapping.FetchRagCss(ragRatingDto.Name),
						caseDto.CreatedAt,
						caseDto.ClosedAt,
						caseDto.StatusUrn.CompareTo(liveStatus.Urn) == 0 ? liveStatus.Name : closeStatus.Name);

					return trustCase;
				}).Where(trustCasesModel => trustCasesModel != null)
			);

			return trustCases;
		}
	}
}