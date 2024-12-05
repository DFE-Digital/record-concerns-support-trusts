﻿using ConcernsCaseWork.Pages;
using ConcernsCaseWork.Pages.Base;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace ConcernsCaseWork.Tests.Security
{
	public class AuthorizeAttributeTests
	{
		public AuthorizeAttributeTests()
		{
			this.UnauthorizedPages = new Type[]
			{
				typeof(ErrorPageModel),
			};
		}

		public Type[] UnauthorizedPages { get; }

		[Test]
		public void All_Pages_Include_Authorize_Attribute()
		{
			var pages = this.GetAllPagesExceptUnauthorizedPages();

			pages.Length.Should().BeGreaterThan(0);

			using (var scope = new AssertionScope())
			{
				foreach (Type page in pages)
				{
					var authAttributes = page.GetCustomAttributes<AuthorizeAttribute>();
					if (authAttributes == null || !authAttributes.Any())
					{
						scope.AddPreFormattedFailure($"Could not find [Authorize] attribute and no exemption for the following page type: {page.Name} ({page.FullName})");
					}
				}
			}
		}

		private Type[] GetAllPagesExceptUnauthorizedPages()
		{
			return Assembly
				.GetAssembly(typeof(AbstractPageModel))
				.GetTypes()
				.Where(x => x.IsAssignableTo(typeof(PageModel)) && !this.UnauthorizedPages.Contains(x))
				.ToArray();
		}
	}
}
