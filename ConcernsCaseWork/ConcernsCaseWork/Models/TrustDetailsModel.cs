using ConcernsCaseWork.Extensions;
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

		public IfdDataModel IfdData { get; }

		public List<EstablishmentModel> Establishments { get; }

		public string TrustNameCounty
		{
			get
			{
				var county = IfdData.GroupContactAddress.County;
				var trustName = GiasData.GroupName;
				return string.IsNullOrEmpty(county) ? trustName : $"{trustName} ({county})";
			}
		}

		public string DisplayAddress
		{
			get
			{
				return GiasData.GroupContactAddress.DisplayAddress;
			}
		}

		public double TotalPupilCapacity { get { return Establishments.Sum(establishment => establishment.SchoolCapacity.ParseToInt()); } }

		public double TotalPupils { get { return Establishments.Sum(establishment => establishment.Census.NumberOfPupils.ParseToInt()); } }

		public double PupilCapacityPercentage
		{
			get
			{
				return TotalPupils != 0 && TotalPupilCapacity != 0 ? Math.Round((Math.Round((TotalPupils / TotalPupilCapacity), 2) * 100), 3) : 0;
			}
		}

		public TrustDetailsModel(GiasDataModel giasData, IfdDataModel ifdData, List<EstablishmentModel> establishments) =>
			(GiasData, IfdData, Establishments) = (giasData, ifdData, establishments);
	}
}