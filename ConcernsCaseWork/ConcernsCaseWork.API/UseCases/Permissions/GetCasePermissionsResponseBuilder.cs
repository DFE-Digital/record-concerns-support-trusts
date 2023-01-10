using ConcernsCaseWork.API.Contracts.Permissions;

namespace ConcernsCaseWork.API.UseCases.Permissions
{
	public class GetCasePermissionsResponseBuilder
	{
		public GetCasePermissionsResponse Build(GetCasePermissionsResponseBuilderParams parameters)
		{
			var result = new GetCasePermissionsResponse();

			if (IsCaseOwner(parameters) || parameters.IsAdmin)
			{
				result.Permissions.Add(CasePermission.Edit);
			}

			return result;
		}

		private bool IsCaseOwner(GetCasePermissionsResponseBuilderParams parameters)
		{
			return parameters.Username == parameters.CaseOwner;
		}
	}

	public class GetCasePermissionsResponseBuilderParams
	{
		public string CaseOwner { get; set; }
		public string Username { get; set; }
		public bool IsAdmin { get; set; }
	}
}
