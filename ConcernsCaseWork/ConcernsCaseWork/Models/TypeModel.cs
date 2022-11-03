using System.Collections.Generic;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class TypeModel
	{
		public IDictionary<string, IList<TypeValueModel>> TypesDictionary { get; set; }
		
		public string Type { get; set; } = string.Empty;

		public string SubType { get; set; } = string.Empty;
		
		public string TypeDisplay
		{
			get
			{
				var separator = string.IsNullOrEmpty(SubType) ? string.Empty : ":";
				return $"{Type}{separator} {SubType ?? string.Empty}";
			}
		}

		public sealed class TypeValueModel
		{
			public long Id { get; set; }
			public string SubType { get; set; }
		}
	}
}