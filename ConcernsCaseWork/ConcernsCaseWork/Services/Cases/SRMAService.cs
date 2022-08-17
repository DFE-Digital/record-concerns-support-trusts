using ConcernsCaseWork.Enums;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using Service.Redis.CaseActions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Trams = ConcernsCasework.Service.CaseActions;

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

		public async Task SetDateClosed(long srmaId, DateTime? ClosedDate)
		{
			await _cachedSrmaProvider.SetDateClosed(srmaId, ClosedDate);	
		}

		public async Task SetDateReportSent(long srmaId, DateTime? reportSentDate)
		{
			await _cachedSrmaProvider.SetDateReportSent(srmaId, reportSentDate);
		}

		public async Task SetNotes(long srmaId, string notes)
		{
			await _cachedSrmaProvider.SetNotes(srmaId, notes);
		}

		public async Task SetOfferedDate(long srmaId, DateTime offeredDate)
		{
			await _cachedSrmaProvider.SetOfferedDate(srmaId, offeredDate);	
		}

		public async Task SetReason(long srmaId, SRMAReasonOffered reason)
		{
			await _cachedSrmaProvider.SetReason(srmaId, (Trams.SRMAReasonOffered)reason);	
		}

		public async Task SetStatus(long srmaId, SRMAStatus status)
		{
			await _cachedSrmaProvider.SetStatus(srmaId, (Trams.SRMAStatus)status);
		}

		public async Task SetVisitDates(long srmaId, DateTime startDate, DateTime? endDate)
		{
			await _cachedSrmaProvider.SetVisitDates(srmaId, startDate, endDate);
		}
	}
}
