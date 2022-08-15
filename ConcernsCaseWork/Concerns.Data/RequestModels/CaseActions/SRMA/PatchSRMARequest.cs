using Concerns.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace Concerns.Data.RequestModels.CaseActions.SRMA
{
    public class PatchSRMARequest
    {
        [Required]
        public int SRMAId { get; set; }

        [Required]
        public Func<SRMACase, SRMACase> Delegate { get; set; } 
    }
}
