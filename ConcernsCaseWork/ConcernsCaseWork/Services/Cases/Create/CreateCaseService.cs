using Ardalis.GuardClauses;
using ConcernsCaseWork.Extensions;
using Microsoft.Extensions.Logging;
using Service.Redis.Cases;
using Service.Redis.Models;
using Service.Redis.Ratings;
using Service.Redis.Status;
using Service.Redis.Users;
using Service.TRAMS.Cases;
using Service.TRAMS.Status;
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

	public CreateCaseService(
		ILogger<CreateCaseService> logger, 
		IUserStateCachedService userStateCachedService, 
		IStatusCachedService statusCachedService, 
		ICaseCachedService caseCachedService, 
		IRatingCachedService ratingCachedService)
	{
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
				statusDto.Urn,
				ratingDto.Urn);
			
			var newCase = await _caseCachedService.PostCase(dto);
			return newCase.Urn;
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