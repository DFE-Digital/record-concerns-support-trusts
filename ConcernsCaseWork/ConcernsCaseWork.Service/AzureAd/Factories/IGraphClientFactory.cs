using Microsoft.Graph;

namespace ConcernsCaseWork.Service.AzureAd.Factories;

public interface IGraphClientFactory
{
	public GraphServiceClient Create();
}
