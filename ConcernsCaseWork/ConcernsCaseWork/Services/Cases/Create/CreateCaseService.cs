using Ardalis.GuardClauses;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Redis.Models;
using ConcernsCaseWork.Redis.Ratings;
using ConcernsCaseWork.Redis.Status;
using ConcernsCaseWork.Redis.Users;
using ConcernsCaseWork.Service.Cases;
using ConcernsCaseWork.Service.Status;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases.Create;

public class CreateCaseService : ICreateCaseService
{
	private readonly ILogger<CreateCaseService> _logger;
	private readonly IStatusCachedService _statusCachedService;
	private readonly ICaseService _caseService;
	private readonly IRatingCachedService _ratingCachedService;
	private readonly ISRMAService _srmaService;

	public CreateCaseService(
		ILogger<CreateCaseService> logger,
		IStatusCachedService statusCachedService,
		ICaseService caseService,
		IRatingCachedService ratingCachedService,
		ISRMAService srmaService)
	{
		_srmaService = Guard.Against.Null(srmaService);
		_ratingCachedService = Guard.Against.Null(ratingCachedService);
		_statusCachedService = Guard.Against.Null(statusCachedService);
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
			var statusDto = await _statusCachedService.GetStatusByName(StatusEnum.Live.ToString());
			var ratingDto = await _ratingCachedService.GetDefaultRating();

			var createdAndUpdatedDate = DateTime.Now;

			var dto = new CreateCaseDto(
				createdAndUpdatedDate,
				createdAndUpdatedDate,
				DateTime.MinValue,
				userName,
				null,
				trustUkPrn,
				null,
				DateTimeOffset.MinValue,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				statusDto.Id,
				ratingDto.Id,
				null,
				trustCompaniesHouseNumber,
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