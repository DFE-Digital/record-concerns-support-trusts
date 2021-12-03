using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Pages.Base;
using ConcernsCaseWork.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service.Redis.Security;
using System;
using System.Collections.Generic;
using System.Linq;
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
		public string UserName { get; private set; }
		public string PreviousUrl { get; private set; }
		
		public EditRolePageModel(IRbacManager rbacManager, ILogger<EditRolePageModel> logger)
		{
			_rbacManager = rbacManager;
			_logger = logger;
		}
		
		public async Task<ActionResult> OnGetAsync()
		{
			var userName = string.Empty;
			
			try
			{
				_logger.LogInformation("EditRolePageModel::OnGetAsync");
				
				userName = RouteData.Values["username"].ToString();
				if (string.IsNullOrEmpty(userName)) 
					throw new Exception("EditRolePageModel::OnGetAsync::Missing route data");
			}
			catch (Exception ex)
			{
				_logger.LogError("EditRolePageModel::OnGetAsync::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnGetPage;
			}
			
			return await LoadPage(Request.Headers["Referer"].ToString(), userName);
		}

		public async Task<ActionResult> OnPostEditRole(string url)
		{
			var userName = string.Empty;
			
			try
			{
				var rolesSelected = Request.Form["role"].ToString();
				userName = Request.Form["username"].ToString();
				if (string.IsNullOrEmpty(rolesSelected) || string.IsNullOrEmpty(userName)) 
					throw new Exception("EditRolePageModel::OnPostEditRole::Missing request form data");

				var rolesSplit = rolesSelected.Split(",");
				var rolesEnum = rolesSplit.Select(role => role.ToEnum<RoleEnum>()).ToList();
				
				await _rbacManager.UpdateUserRoles(userName, rolesEnum);

				return Redirect("/admin");
			}
			catch (Exception ex)
			{
				_logger.LogError("EditRolePageModel::OnPostEditRole::Exception - {Message}", ex.Message);

				TempData["Error.Message"] = ErrorOnPostPage;
			}
			
			return await LoadPage(url, userName);
		}
		
		private async Task<ActionResult> LoadPage(string url, string userName)
		{
			if (string.IsNullOrEmpty(userName)) return Page();
			
			UserName = userName;
			Roles = new List<RoleEnum> { RoleEnum.Admin, RoleEnum.Leader, RoleEnum.User };
			UserRoles = await _rbacManager.GetUserRoles(UserName);
			PreviousUrl = url;

			return Page();
		}
	}
}