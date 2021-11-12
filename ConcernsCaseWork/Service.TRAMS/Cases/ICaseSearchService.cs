﻿using Service.TRAMS.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.TRAMS.Cases
{
	public interface ICaseSearchService
	{
		Task<IList<CaseDto>> GetCasesByCaseTrustSearch(CaseTrustSearch caseTrustSearch);
		Task<IList<CaseDto>> GetCasesByCaseSearch(PageSearch pageSearch);
	}
}