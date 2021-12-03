using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Redis.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Pages.Admin
{
	[Authorize]
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public class EditRolePageModel : AbstractPageModel
	{
		private readonly IRbacManager _rbacManager;
		private readonly ILogger<EditRolePageModel> _logger;

		public IList<RoleEnum> Roles { get; private set; }
		public IList<RoleEnum> UserRoles { get; private set; }
		
		public EditRolePageModel(IRbacManager rbacManager, ILogger<EditRolePageModel> logger)
		{
			_rbacManager = rbacManager;
			_logger = logger;
		}
		
		public async Task OnGetAsync()
		{
			try
			{
				_logger.LogInformation("EditRolePageModel::OnGetAsync");
				
				var userName = RouteData.Values["username"].ToString();
				if (string.IsNullOrEmpty(userName)) 
					throw new Exception("EditRolePageModel::OnGetAsync::Missing route data");
				
				TempData["Data.UserName"] = userName;

				Roles = new List<RoleEnum> { RoleEnum.Admin, RoleEnum.Leader, RoleEnum.User };
				UserRoles = await _rbacManager.GetUserRoles(User.Identity.Name);
			}
			catch (Exception ex)
			{
				_logger.LogError("EditRolePageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}
		}

		public async Task<ActionResult> OnPostEditRole()
		{
			try
			{
				var rolesSelected = Request.Form["role"].ToString();
				if (string.IsNullOrEmpty(rolesSelected)) 
					throw new Exception("EditRolePageModel::OnPostEditRole::Missing request form data");
				
				
				

			}
			catch (Exception ex)
			{
				_logger.LogError("EditRolePageModel::OnPostEditRole::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}
			
			return Redirect("/admin");
		}
	}
}