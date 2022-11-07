using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.Ratings;
using ConcernsCaseWork.Service.Records;
using ConcernsCaseWork.Service.Status;
using ConcernsCaseWork.Service.Types;
using System;
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
				createCaseModel.StatusId,
				createCaseModel.RatingId);
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
				StatusId = caseDto.StatusId,
				StatusName = status,
				RatingId = caseDto.RatingId
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
				statusDto.Id,
				caseDto.RatingId);
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
				caseDto.StatusId,
				caseDto.RatingId);
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
				caseDto.StatusId,
				caseDto.RatingId);
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
				caseDto.StatusId,
				caseDto.RatingId);
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
				caseDto.StatusId,
				caseDto.RatingId);
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
				caseDto.StatusId,
				caseDto.RatingId);
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
				caseDto.StatusId,
				caseDto.RatingId);
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
				caseDto.StatusId,
				patchCaseModel.RatingId);
		}

		public static List<TrustCasesModel> MapTrustCases(IList<RecordDto> recordsDto, IList<RatingDto> ratingsDto, IList<TypeDto> typesDto,
			IEnumerable<CaseDto> casesDto, IList<StatusDto> statusesDto)
		{
			var trustCases = new List<TrustCasesModel>();

			if (!recordsDto.Any() | !ratingsDto.Any() | !typesDto.Any() | !statusesDto.Any()) return trustCases;

			var ratingsModel = RatingMapping.MapDtoToModelList(ratingsDto);
			var recordsModel = RecordMapping.MapDtoToModel(recordsDto.ToList(), typesDto, ratingsDto, statusesDto);

			trustCases.AddRange(
				casesDto.Select(caseDto =>
			{
				var ratingModel = ratingsModel.FirstOrDefault(r => r.Id.CompareTo(caseDto.RatingId) == 0);
				if (ratingModel is null) return null;

				var caseRecordsModel = recordsModel.Where(r => r.CaseUrn.CompareTo(caseDto.Urn) == 0).ToList();
				if (!caseRecordsModel.Any()) return null;
				
				var trustCase = new TrustCasesModel(caseDto.Urn,
					caseRecordsModel,
					ratingModel,
					caseDto.CreatedAt, caseDto.ClosedAt,
					ConvertStatusToEnum(caseDto.StatusId, statusesDto));

				return trustCase;
			}).Where(trustCasesModel => trustCasesModel != null)
				);

			return trustCases;
		}

		private static StatusEnum ConvertStatusToEnum(long caseStatusId, IList<StatusDto> statusesDto)
		{
			var caseStatusDto = statusesDto?.SingleOrDefault(s => s.Id == caseStatusId);

			StatusEnum returnValue = StatusEnum.Unknown;

			if (caseStatusDto != null && Enum.TryParse(typeof(StatusEnum), caseStatusDto.Name, ignoreCase: true, out object status))
			{
				returnValue = (StatusEnum)(status ?? StatusEnum.Unknown);
			}

			return returnValue;
		}
	}
}