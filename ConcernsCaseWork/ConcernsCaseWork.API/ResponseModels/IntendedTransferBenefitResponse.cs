using System.Collections.Generic;

namespace ConcernsCaseWork.API.ResponseModels
{
    public class IntendedTransferBenefitResponse
    {
        public List<string> SelectedBenefits { get; set; }
        public string OtherBenefitValue { get; set; }
    }
}