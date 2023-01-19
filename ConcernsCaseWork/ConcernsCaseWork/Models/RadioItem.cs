using System;

namespace ConcernsCaseWork.Models
{
	public class RadioItem
	{
		private string _id;
		
		public string Description { get; set; }

		public string Text { get; set; }

		public bool IsChecked { get; set; }

		public string Id
		{
			get { return _id ?? Text?.Trim()?.ToLower()?.Replace(" ", "") ?? throw new ArgumentNullException("Text not set"); }
			set { _id = value; }
		}
	}
}
