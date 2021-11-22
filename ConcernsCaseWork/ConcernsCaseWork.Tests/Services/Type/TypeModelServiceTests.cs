using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Type;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Service.Redis.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Services.Type
{
	[Parallelizable(ParallelScope.All)]
	public class TypeModelServiceTests
	{
		[Test]
		public async Task WhenGetTypes_ReturnsStructuredDictionary()
		{
			// arrange
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockLogger = new Mock<ILogger<TypeModelService>>();
			
			var typesDto = TypeFactory.BuildListTypeDto();

			mockTypeCachedService.Setup(t => t.GetTypes()).ReturnsAsync(typesDto);
			
			var typeModelService = new TypeModelService(mockTypeCachedService.Object, mockLogger.Object);

			// act
			var structuredTypes = await typeModelService.GetTypeModel();
			
			// assert
			Assert.That(structuredTypes, Is.Not.Null);
			Assert.That(structuredTypes.TypesDictionary, Is.Not.Null);
			Assert.That(structuredTypes.TypesDictionary.Count, Is.EqualTo(5));

			foreach ((string key, IList<TypeModel.TypeValueModel> value) in structuredTypes.TypesDictionary)
			{
				var types = typesDto.Where(t => t.Name.Equals(key, StringComparison.OrdinalIgnoreCase)).ToList();
			
				Assert.That(types.First().Name, Is.EqualTo(key));
				Assert.That(types.Count == 1 ? 0 : types.Count, Is.EqualTo(value.Count));
			}
		}
	}
}