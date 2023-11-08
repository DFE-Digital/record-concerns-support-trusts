using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Service.Records;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Records
{
	public sealed class RecordModelService : IRecordModelService
	{
		private readonly IRecordService _recordCachedService;
		private readonly ILogger<RecordModelService> _logger;
		
		public RecordModelService(IRecordService recordCachedService, 
			ILogger<RecordModelService> logger)
		{
			_recordCachedService = recordCachedService;
			_logger = logger;
		}

		public Task<IList<RecordModel>> GetRecordsModelByCaseUrn(long caseUrn)
		{
			_logger.LogInformation("RecordModelService::GetRecordsModelByCaseUrn");

			var recordsDtoTask = _recordCachedService.GetRecordsByCaseUrn(caseUrn);
			
			Task.WaitAll(recordsDtoTask);
				
			var recordsDto = recordsDtoTask.Result;
				
			// Map the records dto to model
			var recordsModel = RecordMapping.MapDtoToModel(recordsDto);

			return Task.FromResult(recordsModel);
		}

		public async Task<RecordModel> GetRecordModelById(long caseUrn, long id)
		{
			_logger.LogInformation("RecordModelService::GetRecordModelByUrn");

			var records = await GetRecordsModelByCaseUrn(caseUrn);

			if (!records.Any()) throw new Exception($"Case {caseUrn} does not contain any records");
			var recordModel = records.FirstOrDefault(r => r.Id == id) ?? records.First();

			return recordModel;
		}

		public async Task PatchRecordStatus(PatchRecordModel patchRecordModel)
		{
			try
			{
				// Fetch Records & statuses
				var recordsDto = await _recordCachedService.GetRecordsByCaseUrn(patchRecordModel.CaseUrn);

				var recordDto = recordsDto.FirstOrDefault(r => r.Id.CompareTo(patchRecordModel.Id) == 0);

				recordDto = RecordMapping.MapClosure(patchRecordModel, recordDto);

				await _recordCachedService.PatchRecordById(recordDto);
			}
			catch (Exception ex)
			{
				_logger.LogError("RecordModelService::PatchRecordStatus exception {Message}", ex.Message);

				throw;
			}
		}
		
		public async Task<IList<CreateRecordModel>> GetCreateRecordsModelByCaseUrn(long caseUrn)
		{
			_logger.LogInformation("RecordModelService::GetCreateRecordsModelByCaseUrn");

			var recordsDto = await _recordCachedService.GetRecordsByCaseUrn(caseUrn);
			
			// Map the records dto to model
			var createRecordsModel = RecordMapping.MapDtoToCreateRecordModel(recordsDto);

			return createRecordsModel;
		}

		public async Task<RecordDto> PostRecordByCaseUrn(CreateRecordModel createRecordModel)
		{
			_logger.LogInformation("RecordModelService::PostRecordByCaseUrn");
			
			// Fetch Status			
			var currentDate = DateTimeOffset.Now;
			var createRecordDto = new CreateRecordDto(
				currentDate, 
				currentDate, 
				currentDate, 
				null, 
				null, 
				null, 
				createRecordModel.CaseUrn, 
				createRecordModel.TypeId,
				createRecordModel.RatingId,
				(int)ConcernStatus.Live,
				createRecordModel.MeansOfReferralId);
			
			var recordDto = await _recordCachedService.PostRecordByCaseUrn(createRecordDto);
			
			return recordDto;
		}
	}
}
