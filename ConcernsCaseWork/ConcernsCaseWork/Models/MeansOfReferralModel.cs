using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class MeansOfReferralModel
	{
		public string Name { get; set; }
		
		public string Description { get; set; }
		
		public long Urn { get; set; }
	}
}