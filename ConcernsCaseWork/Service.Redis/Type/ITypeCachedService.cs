﻿using Service.TRAMS.RecordSrma;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Redis.Type
{
	public interface ITypeCachedService
	{
		Task<IList<TypeDto>> GetTypes();
	}
}