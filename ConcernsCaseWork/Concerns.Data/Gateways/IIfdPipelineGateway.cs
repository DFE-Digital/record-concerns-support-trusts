using Concerns.Data.Models;

namespace Concerns.Data.Gateways
{
    public interface IIfdPipelineGateway
    {
        IEnumerable<IfdPipeline> GetPipelineProjectsByStatus(int page, int take, List<string> statues);
        IEnumerable<IfdPipeline> GetPipelineProjects(int page, int take);
    }
}