using ConcernsCaseWork.Data.Models;

namespace ConcernsCaseWork.Data.Gateways
{
    public interface INTIWarningLetterGateway
    {
        Task<NTIWarningLetter> CreateNTIWarningLetter(NTIWarningLetter request);
        Task<NTIWarningLetter> GetNTIWarningLetterById(long ntiWarningLetterId);
        Task<List<NTIWarningLetterStatus>> GetAllStatuses();
        Task<List<NTIWarningLetterReason>> GetAllReasons();
        Task<List<NTIWarningLetterCondition>> GetAllConditions();
        Task<List<NTIWarningLetterConditionType>> GetAllConditionTypes();
        Task<ICollection<NTIWarningLetter>> GetNTIWarningLetterByCaseUrn(int caseUrn);
        Task<NTIWarningLetter> PatchNTIWarningLetter(NTIWarningLetter patchNTIWarningLetter);
    }
}
