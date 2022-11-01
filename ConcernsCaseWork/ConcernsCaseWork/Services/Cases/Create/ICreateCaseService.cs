using Service.Redis.Models;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases.Create;

public interface ICreateCaseService
{
	Task<long> CreateNonConcernsCase(string userName);
}