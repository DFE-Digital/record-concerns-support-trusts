using Service.Redis.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Redis.CaseActions
{
	internal class CachedSRMAProvider : CachedService
	{
		public CachedSRMAProvider(ICacheProvider cacheProvider) : base(cacheProvider)
		{
			
		}
	}
}
