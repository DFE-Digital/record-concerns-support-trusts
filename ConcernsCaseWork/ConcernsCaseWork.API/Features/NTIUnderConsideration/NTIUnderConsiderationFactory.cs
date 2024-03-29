﻿using ConcernsCaseWork.API.Contracts.NtiUnderConsideration;
using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.API.Features.NTIUnderConsideration
{
	public static class NTIUnderConsiderationFactory
	{
		public static Data.Models.NTIUnderConsideration CreateDBModel(CreateNTIUnderConsiderationRequest createNTIUnderConsiderationRequest)
		{
			return new Data.Models.NTIUnderConsideration
			{
				CaseUrn = createNTIUnderConsiderationRequest.CaseUrn,
				UnderConsiderationReasonsMapping = createNTIUnderConsiderationRequest.UnderConsiderationReasonsMapping.Select(r =>
				{
					return new NTIUnderConsiderationReasonMapping()
					{
						NTIUnderConsiderationReasonId = r
					};
				}).ToList(),
				Notes = createNTIUnderConsiderationRequest.Notes,
				CreatedAt = createNTIUnderConsiderationRequest.CreatedAt,
				CreatedBy = createNTIUnderConsiderationRequest.CreatedBy,
				UpdatedAt = createNTIUnderConsiderationRequest.UpdatedAt,
				ClosedAt = createNTIUnderConsiderationRequest.ClosedAt,
				ClosedStatusId = createNTIUnderConsiderationRequest.ClosedStatusId
			};
		}

		public static Data.Models.NTIUnderConsideration CreateDBModel(PatchNTIUnderConsiderationRequest patchNTIUnderConsiderationRequest)
		{
			return new Data.Models.NTIUnderConsideration
			{
				Id = patchNTIUnderConsiderationRequest.Id,
				CaseUrn = patchNTIUnderConsiderationRequest.CaseUrn,
				UnderConsiderationReasonsMapping = patchNTIUnderConsiderationRequest.UnderConsiderationReasonsMapping.Select(r =>
				{
					return new NTIUnderConsiderationReasonMapping()
					{
						NTIUnderConsiderationReasonId = r
					};
				}).ToList(),
				Notes = patchNTIUnderConsiderationRequest.Notes,
				CreatedAt = patchNTIUnderConsiderationRequest.CreatedAt,
				CreatedBy = patchNTIUnderConsiderationRequest.CreatedBy,
				UpdatedAt = patchNTIUnderConsiderationRequest.UpdatedAt,
				ClosedAt = patchNTIUnderConsiderationRequest.ClosedAt,
				ClosedStatusId = patchNTIUnderConsiderationRequest.ClosedStatusId
			};
		}

		public static NTIUnderConsiderationResponse CreateResponse(Data.Models.NTIUnderConsideration model)
		{
			return new NTIUnderConsiderationResponse
			{
				Id = model.Id,
				CaseUrn = model.CaseUrn,
				Notes = model.Notes,
				UnderConsiderationReasonsMapping = model.UnderConsiderationReasonsMapping.Select(r => { return r.NTIUnderConsiderationReasonId; }).ToArray(),
				CreatedAt = model.CreatedAt,
				CreatedBy = model.CreatedBy,
				UpdatedAt = model.UpdatedAt,
				ClosedAt = model.ClosedAt,
				ClosedStatusId = model.ClosedStatusId
			};
		}
	}
}
