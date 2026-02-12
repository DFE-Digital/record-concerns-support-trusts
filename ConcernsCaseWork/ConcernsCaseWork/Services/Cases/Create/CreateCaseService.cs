using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Case;
using ConcernsCaseWork.API.Contracts.Concerns;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Service.Cases;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases.Create;

public class CreateCaseService : ICreateCaseService
{
	private readonly ILogger<CreateCaseService> _logger;
	private readonly ICaseService _caseService;
	private readonly ISRMAService _srmaService;

	public CreateCaseService(
		ILogger<CreateCaseService> logger,
		ICaseService caseService,
		ISRMAService srmaService)
	{
		_srmaService = Guard.Against.Null(srmaService);
		_caseService = Guard.Against.Null(caseService);
		_logger = Guard.Against.Null(logger);
	}

	public async Task<long> CreateNonConcernsCase(string userName, string trustUkPrn, string trustCompaniesHouseNumber)
	{
		_logger.LogMethodEntered();

		Guard.Against.NullOrEmpty(userName);
		Guard.Against.NullOrEmpty(trustUkPrn);
		Guard.Against.NullOrEmpty(trustCompaniesHouseNumber);
		
		try
		{
			var createdAndUpdatedDate = DateTime.Now;

			var dto = new CreateCaseDto(
				createdAndUpdatedDate,
				createdAndUpdatedDate,
				DateTime.MinValue,
				userName,
				null,
				trustUkPrn,
				null,
				null,
				DateTimeOffset.MinValue,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				(int)CaseStatus.Live,
				(int)ConcernRating.NotApplicable,
				false,
				null,
				null,
				trustCompaniesHouseNumber,
				null,
				null);

			var newCase = await _caseService.PostCase(dto);
			return newCase.Urn;
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);

			throw;
		}
	}

	public async Task<long> CreateNonConcernsCase(string userName, string trustUkPrn, string trustCompaniesHouseNumber, SRMAModel srmaModel)
	{
		_logger.LogMethodEntered();

		try
		{
			var caseUrn = await CreateNonConcernsCase(userName, trustUkPrn, trustCompaniesHouseNumber);

			srmaModel.CaseUrn = caseUrn;

			await _srmaService.SaveSRMA(srmaModel);

			return caseUrn;
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);

			throw;
		}
	}
}