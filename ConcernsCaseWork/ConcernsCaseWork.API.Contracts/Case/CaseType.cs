﻿using System.ComponentModel;

namespace ConcernsCaseWork.API.Contracts.Case
{
	public enum CaseType
	{
		[Description("A concern or multiple concerns")]
		Concerns = 1,
		[Description("Actions or decisions not related to a concern")]
		NonConcerns = 2,
		ExConcerns = 3
	}
}
