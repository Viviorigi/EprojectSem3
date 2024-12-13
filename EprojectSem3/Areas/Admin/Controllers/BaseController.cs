﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EprojectSem3.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(AuthenticationSchemes = "MyAppAuthenticationAdmin")]
	public class BaseController : Controller
	{
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			String? email = User.Identity?.Name;
			String? role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
			if (role == "2")
			{
				base.OnActionExecuting(context);
			}
			else
			{
				context.Result = new RedirectToRouteResult(new RouteValueDictionary {
					{ "controller", "LoginAdmin" },
					{ "action", "AccessDenied" } });
			}
		}
	}
}