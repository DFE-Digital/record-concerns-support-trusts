using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Ratings;
using ConcernsCaseWork.Services.Types;
using Microsoft.Extensions.Logging;
using Service.Redis.Models;
using Service.Redis.Records;
using Service.Redis.Status;
using Service.TRAMS.Records;
using Service.TRAMS.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Records
{
	public sealed class RecordModelService : IRecordModelService
	{
		private readonly IStatusCachedService _statusCachedService;
		private readonly IRecordCachedService _recordCachedService;
		private readonly IRatingModelService _ratingModelService;
		private readonly ITypeModelService _typeModelService;
		private readonly ILogger<RecordModelService> _logger;
		
		public RecordModelService(IRecordCachedService recordCachedService, 
			IStatusCachedService statusCachedService,
			IRatingModelService ratingModelService,
			ITypeModelService typeModelService,
			ILogger<RecordModelService> logger)
		{
			_recordCachedService = recordCachedService;
			_statusCachedService = statusCachedService;
			_ratingModelService = ratingModelService;
			_typeModelService = typeModelService;
			_logger = logger;
		}

		public Task<IList<RecordModel>> GetRecordsModelByCaseUrn(string caseworker, long caseUrn)
		{
			_logger.LogInformation("RecordModelService::GetRecordsModelByCaseUrn");

			var recordsDtoTask = _recordCachedService.GetRecordsByCaseUrn(caseworker, caseUrn);
			var typesDtoTask = _typeModelService.GetTypes();
			var ratingsDtoTask = _ratingModelService.GetRatings();
			var statusesDtoTask = _statusCachedService.GetStatuses();
			
			Task.WaitAll(recordsDtoTask, typesDtoTask, ratingsDtoTask, statusesDtoTask);
				
			var recordsDto = recordsDtoTask.Result;
			var typesDto = typesDtoTask.Result;
			var ratingsDto = ratingsDtoTask.Result;
			var statusesDto = statusesDtoTask.Result;
				
			// Map the records dto to model
			var recordsModel = RecordMapping.MapDtoToModel(recordsDto, typesDto, ratingsDto, statusesDto);

			return Task.FromResult(recordsModel);
		}

		public async Task<RecordModel> GetRecordModelById(string caseworker, long caseUrn, long id)
		{
			_logger.LogInformation("RecordModelService::GetRecordModelByUrn");

			var records = await GetRecordsModelByCaseUrn(caseworker, caseUrn);

			if (!records.Any()) throw new Exception($"Case {caseUrn} does not contain any records");
			var recordModel = records.FirstOrDefault(r => r.Id == id) ?? records.First();

			return recordModel;
		}

		public async Task PatchRecordStatus(PatchRecordModel patchRecordModel)
		{
			try
			{
				// Fetch Records & statuses
				var recordsDto = await _recordCachedService.GetRecordsByCaseUrn(patchRecordModel.CreatedBy, patchRecordModel.CaseUrn);
				var statusesDto = await _statusCachedService.GetStatuses();

				var recordDto = recordsDto.FirstOrDefault(r => r.Id.CompareTo(patchRecordModel.Id) == 0);
				var statusDto = statusesDto.FirstOrDefault(s => s.Id.CompareTo(patchRecordModel.StatusId) == 0);

				recordDto = RecordMapping.MapClosure(patchRecordModel, recordDto, statusDto);

				await _recordCachedService.PatchRecordById(recordDto, patchRecordModel.CreatedBy);
			}
			catch (Exception ex)
			{
				_logger.LogError("RecordModelService::PatchRecordStatus exception {Message}", ex.Message);

				throw;
			}
		}
		
		public async Task<IList<CreateRecordModel>> GetCreateRecordsModelByCaseUrn(string caseworker, long caseUrn)
		{
			_logger.LogInformation("RecordModelService::GetCreateRecordsModelByCaseUrn");

			var recordsDto = await _recordCachedService.GetRecordsByCaseUrn(caseworker, caseUrn);
			var typesDto = await _typeModelService.GetTypes();
			var ratingsDto = await _ratingModelService.GetRatings();
			
			// Map the records dto to model
			var createRecordsModel = RecordMapping.MapDtoToCreateRecordModel(recordsDto, typesDto, ratingsDto);

			return createRecordsModel;
		}

		public async Task<RecordDto> PostRecordByCaseUrn(CreateRecordModel createRecordModel, string caseworker)
		{
			_logger.LogInformation("RecordModelService::PostRecordByCaseUrn");
			
			// Fetch Status
			var statusDto = await _statusCachedService.GetStatusByName(StatusEnum.Live.ToString());
			
			var currentDate = DateTimeOffset.Now;
			var createRecordDto = new CreateRecordDto(
				currentDate, 
				currentDate, 
				currentDate, 
				currentDate, 
				createRecordModel.Type, 
				createRecordModel.SubType, 
				createRecordModel.TypeDisplay, 
				createRecordModel.CaseUrn, 
				createRecordModel.TypeId,
				createRecordModel.RatingId, 
				statusDto.Id,
				createRecordModel.MeansOfReferralId);
			
			var recordDto = await _recordCachedService.PostRecordByCaseUrn(createRecordDto, caseworker);
			
			return recordDto;
		}
	}
}
