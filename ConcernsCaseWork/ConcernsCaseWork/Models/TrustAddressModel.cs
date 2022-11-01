using System;

namespace ConcernsCaseWork.Models;

[Serializable]
public record TrustAddressModel(string TrustName, string County, string DisplayAddress);