﻿using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Service.Records;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Mappers
{
	public static class RecordMapping
	{
		public static RecordDto MapRating(PatchRecordModel patchRecordModel, RecordDto recordDto)
		{
			return new RecordDto(recordDto.CreatedAt, 
				patchRecordModel.UpdatedAt, 
				recordDto.ReviewAt,
				recordDto.ClosedAt, 
				recordDto.Name, 
				recordDto.Description, 
				recordDto.Reason, 
				recordDto.CaseUrn, 
				recordDto.TypeId, 
				patchRecordModel.RatingId, 
				recordDto.Id, 
				recordDto.StatusId);
		}
		
		public static RecordDto MapClosure(PatchRecordModel patchRecordModel, RecordDto recordDto)
		{
			return new RecordDto(recordDto.CreatedAt,
				patchRecordModel.UpdatedAt,
				recordDto.ReviewAt,
				patchRecordModel.ClosedAt ?? recordDto.ClosedAt, 
				recordDto.Name, recordDto.Description, 
				recordDto.Reason, recordDto.CaseUrn, 
				recordDto.TypeId, recordDto.RatingId,
				recordDto.Id, (int)ConcernStatus.Close);
		}

		public static IList<RecordModel> MapDtoToModel(IList<RecordDto> recordsDto)
		{
			var recordsModel = new List<RecordModel>();
			if (recordsDto is null || !recordsDto.Any()) return recordsModel;
			
			recordsModel.AddRange(recordsDto.Select(recordDto =>
			{
				var recordModel = new RecordModel(
					recordDto.CaseUrn,
					recordDto.TypeId,
					recordDto.RatingId,
					recordDto.Id,
					recordDto.StatusId);

				return recordModel;
			}));

			return recordsModel;
		}
		
		public static IList<CreateRecordModel> MapDtoToCreateRecordModel(IList<RecordDto> recordsDto)
		{
			var createRecordsModel = new List<CreateRecordModel>();
			if (recordsDto is null || !recordsDto.Any()) return createRecordsModel;
		
			createRecordsModel.AddRange(recordsDto.Select(recordDto =>
			{				
				var createRecordModel = new CreateRecordModel
				{
					CaseUrn = recordDto.CaseUrn,
					TypeId = recordDto.TypeId,
					RatingId = recordDto.RatingId,
					StatusId = recordDto.StatusId
				};

				return createRecordModel;
			}));
			
			return createRecordsModel;
		}
	}
}