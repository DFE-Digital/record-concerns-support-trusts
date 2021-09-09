﻿using Service.TRAMS.Cases;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Status
{
	public interface IStatusCachedService
	{
		Task<IList<StatusDto>> GetStatuses();
	}
}