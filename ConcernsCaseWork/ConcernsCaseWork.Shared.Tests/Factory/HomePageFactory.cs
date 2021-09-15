using ConcernsCaseWork.Models;
using System;
using System.Collections.Generic;

namespace ConcernsCaseWork.Shared.Tests.Factory
{
	public static class HomePageFactory
	{
		public static List<HomeModel> BuildHomeModels()
		{
			var dateTimeNow = DateTimeOffset.Now;
			return new List<HomeModel>
			{
				new HomeModel("case-urn", 
					dateTimeNow.ToString("dd-MM-yyyy"),
					dateTimeNow.ToString("dd-MM-yyyy"),
					dateTimeNow.ToString("dd-MM-yyyy"),
					"trust-name",
					"academy-names",
					"case-type",
					"case-subtype",
					"rag-rating",
					"rag-rating-css")
			};
		}
	}
}