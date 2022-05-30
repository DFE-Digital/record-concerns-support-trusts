using System;
using System.Collections.Generic;
using System.Text;

namespace Service.TRAMS.CaseActions
{
	public enum SRMAStatus
	{
		Unknown = 0,
		TrustConsidering = 1,
		PreparingForDeployment = 2,
		Deployed = 3,
		Declined = 4,
		Canceled = 5,
		Complete = 6
	}
}
