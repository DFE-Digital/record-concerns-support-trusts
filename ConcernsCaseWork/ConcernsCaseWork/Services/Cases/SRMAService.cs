using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using Service.Redis.CaseActions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace ConcernsCaseWork.Services.Cases
{
	public class SRMAService : ISRMAService
	{
		private readonly CachedSRMAProvider _cachedSrmaProvider;

		public SRMAService(CachedSRMAProvider cachedSrmaProvider)
		{
			_cachedSrmaProvider = cachedSrmaProvider;
		}

		public async Task<SRMAModel> GetSRMAById(long srmaId)
		{
			var srmaDto = await _cachedSrmaProvider.GetSRMAById(srmaId);
			return CaseActionsMapping.Map(srmaDto);
		}

		public async Task SaveSRMA(SRMAModel srma)
		{
			var savedSRMA = await _cachedSrmaProvider.SaveSRMA(CaseActionsMapping.Map(srma));
		}

		public async Task<IEnumerable<SRMAModel>> GetSRMAsForCase(long caseUrn)
		{
			var srmas = await _cachedSrmaProvider.GetSRMAsForCase(caseUrn);
			return srmas?.Select(dto => CaseActionsMapping.Map(dto));
		}

		public async Task SetDateAccepted(long srmaId, DateTime? acceptedDate)
		{
			await _cachedSrmaProvider.SetDateAccepted(srmaId, acceptedDate);	
		}

		public Task SetDateClosed(long srmaId, DateTime? ClosedDate)
		{
			throw new NotImplementedException();
		}

		public Task SetDateReportSent(long srmaId, DateTime? reportSentDate)
		{
			throw new NotImplementedException();
		}

		public Task SetNotes(long srmaId, string notes)
		{
			throw new NotImplementedException();
		}

		public Task SetOfferedDate(long srmaId, DateTime offeredDate)
		{
			throw new NotImplementedException();
		}

		public Task SetReason(long srmaId, SRMAReasonOffered reason)
		{
			throw new NotImplementedException();
		}

		public Task SetStatus(long srmaId, SRMAStatus status)
		{
			throw new NotImplementedException();
		}

		public async Task SetVisitDates(long srmaId, DateTime startDate, DateTime? endDate)
		{
			await _cachedSrmaProvider.SetVisitDates(srmaId, startDate, endDate);
		}
	}
}
