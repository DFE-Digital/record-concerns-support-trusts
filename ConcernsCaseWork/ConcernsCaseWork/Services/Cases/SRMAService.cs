using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models.CaseActions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ConcernsCaseWork.Service.Permissions;
using ConcernsCaseWork.Service.CaseActions;

namespace ConcernsCaseWork.Services.Cases
{
	public class SRMAService : ISRMAService
	{
		private readonly SRMAProvider _srmaProvider;
		private readonly ICasePermissionsService _casePermissionsService;

		public SRMAService(
			SRMAProvider srmaProvider,
			ICasePermissionsService casePermissionsService)
		{
			_srmaProvider = srmaProvider;
			_casePermissionsService = casePermissionsService;
		}

		public async Task<SRMAModel> GetSRMAById(long srmaId)
		{
			var srmaDto = await _srmaProvider.GetSRMAById(srmaId);
			return CaseActionsMapping.Map(srmaDto);
		}

		public async Task<SRMAModel> GetSRMAViewModel(long caseId, long srmaId)
		{
			var srmaDto = await _srmaProvider.GetSRMAById(srmaId);
			var permissionsResponse = await _casePermissionsService.GetCasePermissions(caseId);

			return CaseActionsMapping.Map(srmaDto, permissionsResponse);
		}

		public async Task SaveSRMA(SRMAModel srma)
		{
			await _srmaProvider.SaveSRMA(CaseActionsMapping.Map(srma));
		}

		public async Task<IEnumerable<SRMAModel>> GetSRMAsForCase(long caseUrn)
		{
			var srmas = await _srmaProvider.GetSRMAsForCase(caseUrn);
			return srmas?.Select(dto => CaseActionsMapping.Map(dto));
		}

		public async Task SetDateAccepted(long srmaId, DateTime? acceptedDate)
		{
			await _srmaProvider.SetDateAccepted(srmaId, acceptedDate);
		}

		public async Task SetDateClosed(long srmaId)
		{
			await _srmaProvider.SetDateClosed(srmaId);
		}

		public async Task SetDateReportSent(long srmaId, DateTime? reportSentDate)
		{
			await _srmaProvider.SetDateReportSent(srmaId, reportSentDate);
		}

		public async Task SetNotes(long srmaId, string notes)
		{
			await _srmaProvider.SetNotes(srmaId, notes);
		}

		public async Task SetOfferedDate(long srmaId, DateTime offeredDate)
		{
			await _srmaProvider.SetOfferedDate(srmaId, offeredDate);
		}

		public async Task SetReason(long srmaId, Enums.SRMAReasonOffered reason)
		{
			await _srmaProvider.SetReason(srmaId, (SRMAReasonOffered)reason);
		}

		public async Task SetStatus(long srmaId, Enums.SRMAStatus status)
		{
			await _srmaProvider.SetStatus(srmaId, (SRMAStatus)status);
		}

		public async Task SetVisitDates(long srmaId, DateTime startDate, DateTime? endDate)
		{
			await _srmaProvider.SetVisitDates(srmaId, startDate, endDate);
		}
	}
}
