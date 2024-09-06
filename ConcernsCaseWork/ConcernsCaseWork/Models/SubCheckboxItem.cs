using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ConcernsCaseWork.Models
{
	public record SubCheckboxItem
	{
		public SubCheckboxItem(int elementRootId, string name, string heading)
		{
			ElementRootId = elementRootId;
			Name = name;
			Heading = heading;
		}

		public int ElementRootId { get; set; }
		public string Name { get; set; }
		public string Heading { get; set; }

		public bool Checked { get; set; }
		public string Text { get; set; }
		public string HintText { get; set; }
	}
}
