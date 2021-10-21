using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Helpers
{
	public class Utilities
	{
		public static int ParseInt(string input)
		{
			bool result = int.TryParse(input, out int value);
			return result ? value : 0;
		}
	}
}
