using Microsoft.AspNetCore.Http;

namespace ConcernsCaseWork.Pages.Validators
{
	public static class ConcernTypeValidator
	{
		public static bool IsValid(IFormCollection formCollection)
		{
			var type = formCollection["type"].ToString();
			var subType = formCollection["sub-type"].ToString();
			var ragRating = formCollection["rating"].ToString();
			var meansOfReferral = formCollection["meansOfReferralUrn"].ToString();
			
			// Force majeure
			var splitType = type.Split(":");
			string typeUrn;
			
			if (splitType.Length > 1)
			{
				// Get type urn from type
				typeUrn = splitType[0];
			}
			else
			{
				// Get type urn from sub type
				var splitSubtype = subType.Split(":");
				typeUrn = splitSubtype[0];
			}
			
			return !string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(ragRating) && !string.IsNullOrEmpty(typeUrn) && !string.IsNullOrEmpty(meansOfReferral);
		}

		public static bool IsEditValid(IFormCollection formCollection)
		{
			var type = formCollection["type"].ToString();
			var subType = formCollection["sub-type"].ToString();
			
			// Force majeure
			var splitType = type.Split(":");
			string typeUrn;
			
			if (splitType.Length > 1)
			{
				// Get type urn from type
				typeUrn = splitType[0];
			}
			else
			{
				// Get type urn from sub type
				var splitSubtype = subType.Split(":");
				typeUrn = splitSubtype[0];
			}
			
			return !string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(typeUrn);
		}
	}
}