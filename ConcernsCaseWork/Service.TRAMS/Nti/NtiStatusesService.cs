using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.Nti
{
	public class NtiStatusesService : INtiStatusesService
	{
		private readonly ILogger<object> logger;

		public NtiStatusesService(ILogger<Object> logger)
		{
			this.logger = logger;
		}

		public async Task<ICollection<NtiStatusDto>> GetNtiStatusesAsync()
		{
			var tempData = Enumerable.Range(0, 5).Select(i => new NtiStatusDto
			{
				Id = i,
				CreatedAt = DateTime.Now,
				Name = $"Status {i}"
			});

			return await Task.FromResult(tempData.ToArray());
		}
	}
}