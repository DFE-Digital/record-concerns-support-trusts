using System.Collections.Generic;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class TypeModel
	{
		public IDictionary<string, IList<TypeValueModel>> TypesDictionary { get; set; }
		
		public string CheckedType { get; set; } = string.Empty;

		public string CheckedSubType { get; set; } = string.Empty;
		
		public string TypeDisplay
		{
			get
			{
				var separator = string.IsNullOrEmpty(CheckedSubType) ? string.Empty : ":";
				return $"{CheckedType}{separator} {CheckedSubType ?? string.Empty}";
			}
		}

		public sealed class TypeValueModel
		{
			public long Urn { get; set; }
			public string SubType { get; set; }
		}
	}
}