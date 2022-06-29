using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.TRAMS.Nti
{
	public class NtiReasonsService : INtiReasonsService
	{
		public async Task<ICollection<NtiReasonDto>> GetAllReasons()
		{
			if (DateTime.Now > new DateTime(2022, 07, 10))
			{
				throw new InvalidOperationException("Remove dummy data, get data from the api, delete this check.");
			}

			return await Task.FromResult(Enumerable.Range(1, 5).Select(i => 
			new NtiReasonDto { Id = i, Name = $"Reason {i}" })
			.ToArray());
		}
	}
}
