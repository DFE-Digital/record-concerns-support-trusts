using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.TRAMS.Nti
{
	public class NtiTempService : INtiService
	{
		private static List<NtiDto> ntiDtos = new List<NtiDto>();

		public Task<NtiDto> CreateNti(NtiDto ntiDto)
		{
			ntiDto.Id = DateTime.Now.Ticks;
			ntiDtos.Add(ntiDto);
			return Task.FromResult(ntiDto);
		}

		public async Task<ICollection<NtiDto>> GetNtisForCase(long caseUrn)
		{
			return await Task.FromResult(ntiDtos.Where(nti => nti.CaseUrn == caseUrn).ToArray());
		}

		public async Task<NtiDto> GetNTIUnderConsiderationById(long underConsiderationId)
		{
			return await Task.FromResult(ntiDtos.FirstOrDefault(nti => nti.Id == underConsiderationId));
		}
	}
}
