using ConcernsCaseWork.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases;

public interface ICaseSummaryService
{
	Task<List<ActiveCaseSummaryModel>> GetActiveCaseSummariesByCaseworker(string caseworker);
}