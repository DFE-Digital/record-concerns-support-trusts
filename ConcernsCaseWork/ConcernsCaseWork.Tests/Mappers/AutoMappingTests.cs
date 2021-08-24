using AutoMapper;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class AutoMappingTests
	{
		[Test]
		public void ConfigurationIsValid()
		{
			// arrange
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			
			// assert
			config.AssertConfigurationIsValid();
		}

		[Test]
		public void ConvertFromCasesDtoToCasesModelIsValid()
		{
			// arrange
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();
			
			var casesDto = CaseDtoFactory.CreateListCaseDto();
			
			// act
			var casesModel = mapper.Map<IList<CaseModel>>(casesDto);
			
			// assert
			Assert.IsAssignableFrom<List<CaseModel>>(casesModel);
			Assert.That(casesModel.Count, Is.EqualTo(casesDto.Count));
			foreach (var expected in casesModel)
			{
				foreach (var actual in casesDto.Where(actual => expected.Id.Equals(actual.Id)))
				{
					Assert.That(expected.TrustName, Is.EqualTo(actual.TrustName));
					Assert.That(expected.Rag, Is.EqualTo(actual.Rag));
					Assert.That(expected.Type, Is.EqualTo(actual.Type));
					Assert.That(expected.DaysOpen, Is.EqualTo(actual.DaysOpen));
				}
			}
		}
	}
}