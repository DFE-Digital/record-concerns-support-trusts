using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Security
{
	public class RenderManager
	{
		public static Boolean IsCaseOwner(string username, string caseCreatedByUsername)
		{
			//return username == caseCreatedByUsername;
			return false;
		}
	}
}
