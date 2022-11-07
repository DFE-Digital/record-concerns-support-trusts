using System;

namespace ConcernsCaseWork.Models
{
	public class RadioItem
	{
		public string Description { get; set; }

		public string Text { get; set; }

		private string id;

		public bool IsChecked { get; set; }

		public string Id
		{
			get { return id ?? Text?.Trim()?.ToLower()?.Replace(" ", "") ?? throw new ArgumentNullException("Text not set"); }
			set { id = value; }
		}
	}
}
