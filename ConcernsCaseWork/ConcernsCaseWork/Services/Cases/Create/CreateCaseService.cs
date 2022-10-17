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
		try
		{
			var model = await GetCachedNonConcernsCaseModel(userName);
			
			if (model.TrustUkPrn is null)
            {
            	throw new Exception();
            }
			
			var statusDto = await _statusCachedService.GetStatusByName(StatusEnum.Live.ToString());
			var ratingDto = await _ratingCachedService.GetDefaultRating();

			var now = DateTime.Now;
			model.CreatedAt = now;
			model.UpdatedAt = now;
			model.ClosedAt = DateTime.MinValue;
			model.StatusUrn = statusDto.Urn; 

			var dto = new CreateCaseDto(model.CreatedAt, 
				model.UpdatedAt, 
				DateTime.MinValue, 
				model.ClosedAt, 
				model.CreatedBy, 
				null, 
				model.TrustUkPrn, 
				null, 
				DateTimeOffset.MinValue, 
				null, 
				null, 
				null, 
				null, 
				null, 
				null, 
				model.StatusUrn,
				ratingDto.Urn);
			
			var newCase = await _caseCachedService.PostCase(dto);

			await FinaliseCreatingCase(userName);
		
			return newCase.Urn;
		}
		catch (Exception ex)
		{
			_logger.LogError("CaseModelService::PostCase exception {Message}", ex.Message);

			throw;
		}
	}
	
	private async Task<CreateNonConcernsCaseModel> GetCachedNonConcernsCaseModel(string userName)
	{
		_logger.LogMethodEntered();
		
		try
		{
			var userState = await _userStateCachedService.GetData(userName) ?? new UserState(userName);
			userState.CreateNonConcernsCaseModel.TrustUkPrn = userState.TrustUkPrn;
			userState.CreateNonConcernsCaseModel.CreatedBy = userName;
			await _userStateCachedService.StoreData(userName, userState);
			return userState.CreateNonConcernsCaseModel;
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);
			
			throw;
		}
	}

	private async Task FinaliseCreatingCase(string userName)
	{
		var userState = await _userStateCachedService.GetData(userName) ?? new UserState(userName);
		userState.CreateNonConcernsCaseModel = new CreateNonConcernsCaseModel();
		userState.CreateCaseModel = new CreateCaseModel();
		await _userStateCachedService.StoreData(userName, userState);
	}
}