using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using Service.Redis.Nti;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public class NtiModelService : INtiModelService
	{
		private readonly INtiCachedService _ntiCachedService;

		public NtiModelService(INtiCachedService ntiCachedService)
		{
			_ntiCachedService = ntiCachedService;
		}

		public async Task<NtiModel> CreateNti(NtiModel nti)
		{
			nti.CreatedAt = DateTime.Now;
			var dto = await _ntiCachedService.CreateNti(NtiMappers.ToDBModel(nti));
			return NtiMappers.ToServiceModel(dto);
		}

		public async Task<NtiModel> GetNtiUnderConsideration(long ntiUcId)
		{
			return await Task.FromResult(new NtiModel
			{
				Id = ntiUcId,
				Notes = "test notes",
				NtiReasonsForConsidering = new NtiReasonForConsideringModel[]
				{
					new NtiReasonForConsideringModel{ Id = 1, Name = "test"},
					new NtiReasonForConsideringModel{ Id = 3, Name = "test"}
				}
			});
		}

		public async Task<IEnumerable<NtiModel>> GetNtiUnderConsiderationsForCase(long caseUrn)
		{
			var ntis = await _ntiCachedService.GetNtisForCase(caseUrn);
			return ntis.Select(nti => NtiMappers.ToServiceModel(nti));
		}
	}
}
