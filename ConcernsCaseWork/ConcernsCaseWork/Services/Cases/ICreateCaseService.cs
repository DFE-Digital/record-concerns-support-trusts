using ConcernsCaseWork.Models;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases
{
	public interface ICreateCaseService
	{
		Task SetTrustInCreateCaseWizard(string userName, string trustUkPrn);
		Task SetCaseTypeInNewCaseWizard(string userName, int caseType);
		Task<string> GetSelectedTrustUkPrn(string userName);
		Task<TrustAddressModel> GetSelectedTrustAddress(string userName);
		Task StartCreateNewCaseWizard(string userName);
	}
}