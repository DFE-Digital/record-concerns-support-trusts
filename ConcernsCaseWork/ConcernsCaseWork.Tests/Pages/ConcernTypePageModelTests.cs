using NUnit.Framework;

namespace ConcernsCaseWork.Tests.Pages
{
	[Parallelizable(ParallelScope.All)]
	public class ConcernTypePageModelTests
	{
		[Test]
		public async Task WhenOnGetAsync_ReturnsModel()
		{
			// arrange
			var mockCaseModelService = new Mock<ICaseModelService>();
			var mockLogger = new Mock<ILogger<DetailsPageModel>>();
			var mockTrustModelService = new Mock<ITrustModelService>();
			var mockCasesCachedService = new Mock<ICachedService>();
			var mockTypeModelService = new Mock<ITypeModelService>();
			var expected = TrustFactory.BuildTrustDetailsModel();

			mockTypeModelService.Setup(t => t.GetTypes()).ReturnsAsync(new Dictionary<string, IList<string>>());
			mockCasesCachedService.Setup(c => c.GetData<UserState>(It.IsAny<string>())).ReturnsAsync(new UserState { TrustUkPrn = "trust-ukprn" });
			mockTrustModelService.Setup(s => s.GetTrustByUkPrn(It.IsAny<string>())).ReturnsAsync(expected);
			
			var pageModel = SetupDetailsModel(mockCaseModelService.Object, mockCasesCachedService.Object, mockLogger.Object, true);
			
			// act
			await pageModel.OnGetAsync();
			var createCaseModel = pageModel.CreateCaseModel;

			// assert
			Assert.That(pageModel.TempData["Error.Message"], Is.Null);
			Assert.IsAssignableFrom<CreateCaseModel>(createCaseModel);

			Assert.That(typesDictionary, Is.Not.Null);
			Assert.That(createCaseModel, Is.Not.Null);
			Assert.That(createCaseModel.GiasData, Is.Not.Null);
			Assert.That(createCaseModel.GiasData.GroupId, Is.EqualTo(expected.GiasData.GroupId));
			Assert.That(createCaseModel.GiasData.GroupName, Is.EqualTo(expected.GiasData.GroupName));
			Assert.That(createCaseModel.GiasData.UkPrn, Is.EqualTo(expected.GiasData.UkPrn));
			Assert.That(createCaseModel.GiasData.CompaniesHouseNumber, Is.EqualTo(expected.GiasData.CompaniesHouseNumber));
			Assert.That(createCaseModel.GiasData.GroupContactAddress, Is.Not.Null);
			Assert.That(createCaseModel.GiasData.GroupContactAddress.County, Is.EqualTo(expected.GiasData.GroupContactAddress.County));
			Assert.That(createCaseModel.GiasData.GroupContactAddress.Locality, Is.EqualTo(expected.GiasData.GroupContactAddress.Locality));
			Assert.That(createCaseModel.GiasData.GroupContactAddress.Postcode, Is.EqualTo(expected.GiasData.GroupContactAddress.Postcode));
			Assert.That(createCaseModel.GiasData.GroupContactAddress.Street, Is.EqualTo(expected.GiasData.GroupContactAddress.Street));
			Assert.That(createCaseModel.GiasData.GroupContactAddress.Town, Is.EqualTo(expected.GiasData.GroupContactAddress.Town));
			Assert.That(createCaseModel.GiasData.GroupContactAddress.AdditionalLine, Is.EqualTo(expected.GiasData.GroupContactAddress.AdditionalLine));
			Assert.That(createCaseModel.GiasData.GroupContactAddress.DisplayAddress, Is.EqualTo(SharedBuilder.BuildDisplayAddress(expected.GiasData.GroupContactAddress)));
			
			// Verify ILogger
			mockLogger.Verify(
				m => m.Log(
					LogLevel.Information,
					It.IsAny<EventId>(),
					It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("DetailsPageModel")),
					null,
					It.IsAny<Func<It.IsAnyType, Exception, string>>()),
				Times.Once);
		}
		
		private static DetailsPageModel SetupDetailsModel(
			ICaseModelService mockCaseModelService, ICachedService mockCachedService, 
			ILogger<DetailsPageModel> mockLogger, bool isAuthenticated = false)
		{
			(PageContext pageContext, TempDataDictionary tempData, ActionContext actionContext) = PageContextFactory.PageContextBuilder(isAuthenticated);
			
			return new DetailsPageModel(mockCaseModelService, mockCachedService, mockLogger)
			{
				PageContext = pageContext,
				TempData = tempData,
				Url = new UrlHelper(actionContext),
				MetadataProvider = pageContext.ViewData.ModelMetadata
			};
		}
	}
}