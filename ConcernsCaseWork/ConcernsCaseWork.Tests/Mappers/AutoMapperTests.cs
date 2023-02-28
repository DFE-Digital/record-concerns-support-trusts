using AutoMapper;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Mappers;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Shared.Tests.Factory;
using NUnit.Framework;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace ConcernsCaseWork.Tests.Mappers
{
	[Parallelizable(ParallelScope.All)]
	public class AutoMapperTests
	{
		[Test]
		public void ConvertFrom_TrustsSummaryDto_To_TrustsSummaryModel_IsValid()
		{
			// arrange
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();

			var trustsDto = TrustFactory.BuildListTrustSummaryDto();

			// act
			var trustsModel = mapper.Map<IList<TrustSearchModel>>(trustsDto);

			// assert
			Assert.IsAssignableFrom<List<TrustSearchModel>>(trustsModel);
			Assert.That(trustsModel.Count, Is.EqualTo(trustsDto.Count));
			foreach (var expected in trustsModel)
			{
				foreach (var actual in trustsDto.Where(actual => expected.UkPrn.Equals(actual.UkPrn)))
				{
					Assert.That(expected.Urn, Is.EqualTo(actual.Urn));
					Assert.That(expected.UkPrn, Is.EqualTo(actual.UkPrn));
					Assert.That(expected.GroupName, Is.EqualTo(actual.GroupName));
					Assert.That(expected.CompaniesHouseNumber, Is.EqualTo(actual.CompaniesHouseNumber));
				}
			}
		}

		[Test]
		public void ConvertFrom_TrustDetailsDto_To_TrustsDetailsModel_IsValid()
		{
			// arrange
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();

			var trustDetailsDto = TrustFactory.BuildTrustDetailsDto();

			// act
			var trustDetailsModel = mapper.Map<TrustDetailsModel>(trustDetailsDto);

			// assert
			Assert.IsAssignableFrom<TrustDetailsModel>(trustDetailsModel);
			Assert.That(trustDetailsModel.GiasData, Is.Not.Null);
			Assert.That(trustDetailsModel.GiasData.GroupId, Is.EqualTo(trustDetailsDto.GiasData.GroupId));
			Assert.That(trustDetailsModel.GiasData.GroupName, Is.EqualTo(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(trustDetailsDto.GiasData.GroupName)));
			Assert.That(trustDetailsModel.GiasData.UkPrn, Is.EqualTo(trustDetailsDto.GiasData.UkPrn));
			Assert.That(trustDetailsModel.GiasData.CompaniesHouseNumber, Is.EqualTo(trustDetailsDto.GiasData.CompaniesHouseNumber));
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress, Is.Not.Null);
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress.County, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.County));
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress.Locality, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.Locality));
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress.Postcode, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.Postcode));
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress.Street, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.Street));
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress.Town, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.Town));
			Assert.That(trustDetailsModel.GiasData.GroupContactAddress.AdditionalLine, Is.EqualTo(trustDetailsDto.GiasData.GroupContactAddress.AdditionalLine));
		}

		[Test]
		public void ConvertFrom_EstablishmentSummaryDto_To_EstablishmentSummaryModel_IsValid()
		{
			// arrange
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();

			var establishmentSummaryDto = EstablishmentFactory.BuildEstablishmentSummaryDto();

			// act
			var establishmentSummaryModel = mapper.Map<EstablishmentSummaryModel>(establishmentSummaryDto);

			// assert
			Assert.IsAssignableFrom<EstablishmentSummaryModel>(establishmentSummaryModel);
			Assert.That(establishmentSummaryModel.Name, Is.EqualTo(establishmentSummaryDto.Name));
			Assert.That(establishmentSummaryModel.Urn, Is.EqualTo(establishmentSummaryDto.Urn));
			Assert.That(establishmentSummaryModel.UkPrn, Is.EqualTo(establishmentSummaryDto.UkPrn));
		}

		[Test]
		public void ConvertFrom_GiasDataDto_To_GiasDataModel_IsValid()
		{
			// arrange
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();

			var giasDataDto = GiasDataFactory.BuildGiasDataDto();

			// act
			var giasDataModel = mapper.Map<GiasDataModel>(giasDataDto);

			// assert
			Assert.IsAssignableFrom<GiasDataModel>(giasDataModel);
			Assert.That(giasDataModel, Is.Not.Null);
			Assert.That(giasDataModel.GroupId, Is.EqualTo(giasDataDto.GroupId));
			Assert.That(giasDataModel.GroupName, Is.EqualTo(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(giasDataDto.GroupName)));
			Assert.That(giasDataModel.UkPrn, Is.EqualTo(giasDataDto.UkPrn));
			Assert.That(giasDataModel.CompaniesHouseNumber, Is.EqualTo(giasDataDto.CompaniesHouseNumber));
			Assert.That(giasDataModel.GroupContactAddress, Is.Not.Null);
			Assert.That(giasDataModel.GroupContactAddress.County, Is.EqualTo(giasDataDto.GroupContactAddress.County));
			Assert.That(giasDataModel.GroupContactAddress.Locality, Is.EqualTo(giasDataDto.GroupContactAddress.Locality));
			Assert.That(giasDataModel.GroupContactAddress.Postcode, Is.EqualTo(giasDataDto.GroupContactAddress.Postcode));
			Assert.That(giasDataModel.GroupContactAddress.Street, Is.EqualTo(giasDataDto.GroupContactAddress.Street));
			Assert.That(giasDataModel.GroupContactAddress.Town, Is.EqualTo(giasDataDto.GroupContactAddress.Town));
			Assert.That(giasDataModel.GroupContactAddress.AdditionalLine, Is.EqualTo(giasDataDto.GroupContactAddress.AdditionalLine));
			Assert.That(giasDataModel.CompaniesHouseWebsite, Is.EqualTo($"https://find-and-update.company-information.service.gov.uk/company/{giasDataDto.CompaniesHouseNumber}"));
		}

		[Test]
		public void ConvertFrom_EstablishmentDto_To_EstablishmentModel_IsValid()
		{
			// arrange
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = config.CreateMapper();

			var establishmentDto = EstablishmentFactory.BuildEstablishmentDto();

			// act
			var establishmentModel = mapper.Map<EstablishmentModel>(establishmentDto);

			// assert
			Assert.IsAssignableFrom<EstablishmentModel>(establishmentModel);
			Assert.That(establishmentModel.Urn, Is.EqualTo(establishmentDto.Urn));
			Assert.That(establishmentModel.EstablishmentName, Is.EqualTo(establishmentDto.EstablishmentName));
			Assert.That(establishmentModel.EstablishmentNumber, Is.EqualTo(establishmentDto.EstablishmentNumber));
			Assert.That(establishmentModel.LocalAuthorityCode, Is.EqualTo(establishmentDto.LocalAuthorityCode));
			Assert.That(establishmentModel.LocalAuthorityName, Is.EqualTo(establishmentDto.LocalAuthorityName));
			Assert.That(establishmentModel.HeadteacherTitle, Is.EqualTo(establishmentDto.HeadteacherTitle));
			Assert.That(establishmentModel.HeadteacherFirstName, Is.EqualTo(establishmentDto.HeadteacherFirstName));
			Assert.That(establishmentModel.HeadteacherLastName, Is.EqualTo(establishmentDto.HeadteacherLastName));
			Assert.That(establishmentModel.HeadteacherFullName, Is.EqualTo($"{establishmentDto.HeadteacherTitle} {establishmentDto.HeadteacherFirstName} {establishmentDto.HeadteacherLastName}"));
			Assert.That(establishmentModel.EstablishmentType.Code, Is.EqualTo(establishmentDto.EstablishmentType.Code));
			Assert.That(establishmentModel.EstablishmentType.Name, Is.EqualTo(establishmentDto.EstablishmentType.Name));
		}

		[Test]
		public void Assert_MappingConfiguration_Is_Valid()
		{
			// Auto mapper includes functionality to check that config is valid for you.
			var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapping>());
			var mapper = new Mapper(config);
			mapper.CompileAndValidate();
		}
	}
}