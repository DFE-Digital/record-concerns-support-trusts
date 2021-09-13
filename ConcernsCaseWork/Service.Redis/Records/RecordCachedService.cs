using AutoMapper;
using Microsoft.Extensions.Logging;
using Service.Redis.Base;
using Service.Redis.Models;
using Service.TRAMS.Records;
using Service.TRAMS.Sequence;
using System.Threading.Tasks;

namespace Service.Redis.Records
{
	public sealed class RecordCachedService : CachedService, IRecordCachedService
	{
		private readonly ILogger<RecordCachedService> _logger;
		private readonly IRecordService _recordService;
		private readonly IMapper _mapper;
		
		/// <summary>
		/// TODO Remove IMapper and project references when TRAMS API is live
		/// </summary>
		public RecordCachedService(IMapper mapper, ICacheProvider cacheProvider, IRecordService recordService, ILogger<RecordCachedService> logger) 
			: base(cacheProvider)
		{
			_recordService = recordService;
			_mapper = mapper;
			_logger = logger;
		}
		
		public async Task<RecordDto> PostRecordByCaseUrn(CreateRecordDto createRecordDto, string caseworker)
		{
			_logger.LogInformation("RecordCachedService::PostRecordByCaseUrn");

			// TODO Enable only when TRAMS API is live
			// Create record on TRAMS API
			//var newRecord = await _recordService.PostRecordByCaseUrn(createRecordDto);
			//if (newRecord is null) throw new ApplicationException("Error::RecordCachedService::PostRecordByCaseUrn");

			// TODO Remove when TRAMS API is live
			var newRecord = _mapper.Map<RecordDto>(createRecordDto);
			newRecord.Urn = LongSequence.Generator();
			
			// Store in cache for 24 hours (default)
			var caseState = await GetData<UserState>(caseworker);
			if (caseState is null)
			{
				caseState = new UserState
				{
					CasesDetails = { { newRecord.CaseUrn, 
						new CaseWrapper { 
							Records = { { newRecord.Urn, 
								new RecordWrapper { RecordDto = newRecord } }} 
						} 
					} }
				};
			}
			else
			{
				if (caseState.CasesDetails.ContainsKey(newRecord.CaseUrn) 
				    && caseState.CasesDetails.TryGetValue(newRecord.CaseUrn, out var caseWrapper))
				{
					caseWrapper.Records.Add(newRecord.Urn, new RecordWrapper {  RecordDto = newRecord });
				}
				else
				{
					caseWrapper = new CaseWrapper();
					caseWrapper.Records.Add(newRecord.Urn, new RecordWrapper {  RecordDto = newRecord });
					
					caseState.CasesDetails.Add(newRecord.CaseUrn, caseWrapper);
				}
			}
			await StoreData(caseworker, caseState);
			
			return newRecord;
		}
	}
}