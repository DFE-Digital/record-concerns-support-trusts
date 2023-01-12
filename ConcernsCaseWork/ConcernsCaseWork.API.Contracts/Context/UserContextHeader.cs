using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConcernsCaseWork.API.Contracts.Context
{
	public class UserContextHeader
	{
		public string Name { get; set; }
		public string[] Roles { get; set; }

		public IEnumerable<KeyValuePair<string,string>> ToHeadersKVP()
		{
			yield return new KeyValuePair<string, string>("x-userContext-name", this.Name);

			for (int i = 0; i < this.Roles.Length; i++)
			{
				yield return new KeyValuePair<string, string>($"x-userContext-role-{i}", this.Roles[i]);
			}
		}
	}
}
