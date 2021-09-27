using ConcernsCaseWork.Pages;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NUnit.Framework;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class FileResourcePageModelTests
	{
		[Test]
		public void WhenOnGetDownloadRiskManagementPdf_ReturnsPdfDocument()
		{
			// arrange
			var pageModel = SetupFileResourceModel(true);
			
			// act
			var response = pageModel.OnGetDownloadRiskManagementPdf();

			// assert
			Assert.IsInstanceOf(typeof(VirtualFileResult), response);
			var virtualFileResult = response as VirtualFileResult;

			Assert.That(virtualFileResult, Is.Not.Null);
			Assert.That(virtualFileResult.FileName, Is.EqualTo("/files/Risk_Management_Framework.pdf"));
			Assert.That(virtualFileResult.FileDownloadName, Is.Empty);
			Assert.That(virtualFileResult.ContentType, Is.EqualTo("application/pdf"));
		}
		
		private static FileResourcePageModel SetupFileResourceModel(bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new FileResourcePageModel()
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}