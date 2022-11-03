using ConcernsCaseWork.Models;
using ConcernsCaseWork.Redis.Types;
using ConcernsCaseWork.Services.Types;
using ConcernsCaseWork.Shared.Tests.Factory;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Tests.Services.Types
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
				Assert.That(types.Count, Is.EqualTo(value.Count));
			}
		}

		[Test]
		public async Task WhenGetSelectedTypeModelByUrn_ReturnsTypeModel()
		{
			// arrange
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockLogger = new Mock<ILogger<TypeModelService>>();
			
			var typesDto = TypeFactory.BuildListTypeDto();
			var expectedTypeDto = typesDto.First();
			var expectedTypeModel = TypeFactory.BuildTypeModel();

			mockTypeCachedService.Setup(t => t.GetTypes()).ReturnsAsync(typesDto);

			var typeModelService = new TypeModelService(mockTypeCachedService.Object, mockLogger.Object);
			
			// act
			var actualTypeModel = await typeModelService.GetSelectedTypeModelById(expectedTypeDto.Id);
			
			// assert
			Assert.That(actualTypeModel, Is.Not.Null);
			Assert.That(actualTypeModel.Type, Is.EqualTo(expectedTypeModel.Type));
			Assert.That(actualTypeModel.TypeDisplay, Is.EqualTo(expectedTypeModel.TypeDisplay));
			Assert.That(actualTypeModel.TypesDictionary, Is.Not.Null);
			Assert.That(actualTypeModel.SubType, Is.EqualTo(expectedTypeModel.SubType));
		}
		
		[Test]
		public async Task WhenGetTypeModelByUrn_ReturnsTypeModel()
		{
			// arrange
			var mockTypeCachedService = new Mock<ITypeCachedService>();
			var mockLogger = new Mock<ILogger<TypeModelService>>();
			
			var typesDto = TypeFactory.BuildListTypeDto();
			var expectedTypeDto = typesDto.First();
			var expectedTypeModel = TypeFactory.BuildTypeModel();

			mockTypeCachedService.Setup(t => t.GetTypes()).ReturnsAsync(typesDto);

			var typeModelService = new TypeModelService(mockTypeCachedService.Object, mockLogger.Object);
			
			// act
			var actualTypeModel = await typeModelService.GetTypeModelByUrn(expectedTypeDto.Id);
			
			// assert
			Assert.That(actualTypeModel, Is.Not.Null);
			Assert.That(actualTypeModel.Type, Is.EqualTo(expectedTypeModel.Type));
			Assert.That(actualTypeModel.TypeDisplay, Is.EqualTo(expectedTypeModel.TypeDisplay));
			Assert.That(actualTypeModel.TypesDictionary, Is.Null);
			Assert.That(actualTypeModel.SubType, Is.EqualTo(expectedTypeModel.SubType));
		}
	}
}