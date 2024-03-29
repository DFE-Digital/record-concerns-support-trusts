﻿namespace ConcernsCaseWork.API.Contracts.Trusts
{
	public class TrustResponse
	{
		public IFDDataResponse IfdData { get; set; }
		public GIASDataResponse GiasData { get; set; }
		public List<EstablishmentResponse> Establishments { get; set; }
	}
}
