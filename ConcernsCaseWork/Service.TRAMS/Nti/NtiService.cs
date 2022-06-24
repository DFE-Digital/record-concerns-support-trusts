using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.Nti
{
	class NtiService : INtiService
	{
		public Task<NtiDto> CreateNti(NtiDto ntiDto)
		{
			// todo: talk to api when api is ready
			ntiDto.Id = DateTime.Now.Ticks;
			return Task.FromResult(ntiDto);
		}
	}
}
