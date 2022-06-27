using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.Nti
{
	public class NtiService : INtiService
	{
		public Task<NtiDto> CreateNti(NtiDto ntiDto)
		{
			// todo: talk to api when api is ready
			ntiDto.Id = DateTime.Now.Ticks;
			return Task.FromResult(ntiDto);
		}

		public async Task<ICollection<NtiDto>> GetNtisForCase(long caseUrn)
		{
			// todo: talk to api when api is ready
			return await Task.FromResult(Enumerable.Empty<NtiDto>().ToArray());
		}
	}
}
