﻿namespace ConcernsCasework.Service.Nti
{
	public class NtiService : INtiService
	{
		public async Task<ICollection<NtiDto>> GetNtisForCaseAsync(long caseUrn)
		{
			var tempData = Enumerable.Range(1, 3).Select(i => new NtiDto
			{
				Id = i,
				CaseUrn = caseUrn
			});

			tempData = Enumerable.Empty<NtiDto>();

			return await Task.FromResult(tempData.ToArray());
		}
	}
}
