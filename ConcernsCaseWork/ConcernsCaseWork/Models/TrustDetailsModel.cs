using ConcernsCaseWork.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class TrustDetailsModel
	{
		public GiasDataModel GiasData { get; }
		
		public List<EstablishmentModel> Establishments { get; }

		public double TotalPupilCapacity { get { return Establishments.Sum(establishment => Utilities.ParseInt(establishment.SchoolCapacity)); } }

		public double TotalPupils { get { return Establishments.Sum(establishment => Utilities.ParseInt(establishment.Census.NumberOfPupils)); } }

		public double PupilCapacityPercentage { get { return Math.Round((TotalPupils / TotalPupilCapacity), 2) * 100; } }

		public TrustDetailsModel(GiasDataModel giasData, List<EstablishmentModel> establishments) => 
			(GiasData, Establishments) = (giasData, establishments);
	}
}