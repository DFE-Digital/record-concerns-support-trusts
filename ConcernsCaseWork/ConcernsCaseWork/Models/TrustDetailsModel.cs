using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Models
{
	/// <summary>
	/// Frontend model classes used only for UI rendering
	/// </summary>
	public sealed class TrustDetailsModel
	{
		public GiasDataModel GiasData { get; }
		
		public List<EstablishmentModel> Establishments { get; }


		public double TotalPupilCapacity { get { return CalculateTotalPupilCapacity(); } }

		public double TotalPupils { get { return CalculateTotalPupils(); } }

		public double PupilCapacityPercentage { get { return CalculatePupilCapacityPercentage(); } }


		private int CalculateTotalPupilCapacity()
		{
			int totalCapacity = 0;

			this.Establishments.ForEach(establishment =>
			{
				totalCapacity += Int32.Parse(establishment.SchoolCapacity);

			});

			return totalCapacity;
		}

		private int CalculateTotalPupils()
		{
			int totalPupils = 0;

			this.Establishments.ForEach(establishment =>
			{
				totalPupils += Int32.Parse(establishment.Census.NumberOfPupils);

			});

			return totalPupils;
		}

		private double CalculatePupilCapacityPercentage()
		{
			return Math.Round((this.TotalPupils / this.TotalPupilCapacity), 2) * 100;
		}


		public TrustDetailsModel(GiasDataModel giasData, List<EstablishmentModel> establishments) => 
			(GiasData, Establishments) = (giasData, establishments);
	}
}