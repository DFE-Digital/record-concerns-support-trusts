using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class UserContextOperationFilter : IOperationFilter
{
	public void Apply(OpenApiOperation operation, OperationFilterContext context)
	{
		if (operation.Parameters is null)
		{
			operation.Parameters = new List<OpenApiParameter>();
		}

		operation.Parameters.Add(new OpenApiParameter
		{
			Name = "x-user-context-name",
			In = ParameterLocation.Header,
			Description = "user name",
			Required = true
		});

		operation.Parameters.Add(new OpenApiParameter
		{
			Name = "x-user-context-role-0",
			In = ParameterLocation.Header,
			Description = "user-role. API calls support 1..* users roles in the format x-user-context-role-* where the index starts from zero",
			Examples = new Dictionary<string, OpenApiExample>()
			{
				{ "caseworker", new OpenApiExample(){ Value = new OpenApiString("concerns-casework.caseworker") } },
				{ "team-leader", new OpenApiExample(){ Value = new OpenApiString("concerns-casework.teamleader") } },
				{ "admin", new OpenApiExample(){ Value = new OpenApiString("concerns-casework.admin") } }
			},
			Required = true,
		});
	}
}