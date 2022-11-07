using Ardalis.GuardClauses;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Logging;
using ConcernsCaseWork.Models.CaseActions;
using ConcernsCaseWork.Redis.Cases;
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
	private readonly IUserStateCachedService _userStateCachedService;
	private readonly IStatusCachedService _statusCachedService;
	private readonly ICaseCachedService _caseCachedService;
	private readonly IRatingCachedService _ratingCachedService;
	private readonly ISRMAService _srmaService;

	public CreateCaseService(
		ILogger<CreateCaseService> logger, 
		IUserStateCachedService userStateCachedService, 
		IStatusCachedService statusCachedService, 
		ICaseCachedService caseCachedService, 
		IRatingCachedService ratingCachedService,
		ISRMAService srmaService)
	{
		_srmaService = Guard.Against.Null(srmaService);
		_ratingCachedService = Guard.Against.Null(ratingCachedService);
		_statusCachedService = Guard.Against.Null(statusCachedService);
		_userStateCachedService = Guard.Against.Null(userStateCachedService);
		_caseCachedService = Guard.Against.Null(caseCachedService);
		_logger = Guard.Against.Null(logger);
	}

	public async Task<long> CreateNonConcernsCase(string userName)
	{
		_logger.LogMethodEntered();
		
		try
		{
			var trustUkPrn = await GetSelectedTrustUkPrn(userName);
			
			var statusDto = await _statusCachedService.GetStatusByName(StatusEnum.Live.ToString());
			var ratingDto = await _ratingCachedService.GetDefaultRating();

			var createdAndUpdatedDate = DateTime.Now;

			var dto = new CreateCaseDto(
				createdAndUpdatedDate, 
				createdAndUpdatedDate, 
				DateTime.MinValue, 
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
				statusDto.Id,
				ratingDto.Id);
			
			var newCase = await _caseCachedService.PostCase(dto);
			return newCase.Urn;
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);

			throw;
		}
	}
	
	public async Task<long> CreateNonConcernsCase(string userName, SRMAModel srmaModel)
	{
		_logger.LogMethodEntered();
		
		try
		{
			var caseUrn = await CreateNonConcernsCase(userName);
			
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
	
	private async Task<string> GetSelectedTrustUkPrn(string userName)
	{
		var userState = await _userStateCachedService.GetData(userName) ?? new UserState(userName);
			
		if (string.IsNullOrEmpty(userState.TrustUkPrn))
			throw new Exception("Cached TrustUkPrn is not set");
		
		return userState.TrustUkPrn;
	}
}