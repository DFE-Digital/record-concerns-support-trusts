using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		public async Task<NtiDto> GetNti(long ntiId)
		{
			return await Task.FromResult(ntiDtos.SingleOrDefault(nti => nti.Id == ntiId));
		}

		public async Task<ICollection<NtiDto>> GetNtisForCase(long caseUrn)
		{
			return await Task.FromResult(ntiDtos.Where(nti => nti.CaseUrn == caseUrn).ToArray());
		}

		public async Task<NtiDto> PatchNti(NtiDto ntiDto)
		{
			ntiDtos = ntiDtos.Where(nti => nti.Id != ntiDto.Id).ToList();
			ntiDtos.Add(ntiDto);
			return await Task.FromResult(ntiDto);
		}
	}
}
