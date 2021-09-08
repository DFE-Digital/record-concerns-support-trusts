using ConcernsCaseWork.Models;
using System.Collections.Generic;

namespace ConcernsCaseWork.Mappers
{
	public static class HomeMapping
	{
		public static (IList<HomeUiModel>, IList<HomeUiModel>) Map(IList<CaseModel> casesModel,
			IList<TrustDetailsModel> trustsDetailsModel, IList<RecordModel> recordsModel,
			IList<RatingModel> ragsRatingModel)
		{
			var activeCases = new List<HomeUiModel>();
			var monitoringCases = new List<HomeUiModel>();

			
			
			
			
			
			return (activeCases, monitoringCases);
		}
	}
}