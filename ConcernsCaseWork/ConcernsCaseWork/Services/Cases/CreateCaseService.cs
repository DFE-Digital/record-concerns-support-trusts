using Ardalis.GuardClauses;
using AutoMapper;
using ConcernsCaseWork.Extensions;
using ConcernsCaseWork.Models;
using ConcernsCaseWork.Services.Trusts;
using Microsoft.Extensions.Logging;
using Service.Redis.Models;
using Service.Redis.Users;
using System;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.Cases;

public class CreateCaseService : ICreateCaseService
{
	private readonly ILogger<CreateCaseService> _logger;
	private readonly IUserStateCachedService _userStateCachedService;
	private readonly ITrustModelService _trustModelService;
	private readonly IMapper _mapper;

	public CreateCaseService(ILogger<CreateCaseService> logger, IUserStateCachedService userStateCachedService, IMapper mapper, ITrustModelService trustModelService)
	{
		_trustModelService = Guard.Against.Null(trustModelService);
		_mapper = Guard.Against.Null(mapper);
		_logger = Guard.Against.Null(logger);
		_userStateCachedService = Guard.Against.Null(userStateCachedService);
	}
	
	public async Task<TrustAddressModel> GetSelectedTrustAddress(string userName)
	{
		_logger.LogMethodEntered();
		
		try
		{
			var trustUkPrn = await GetSelectedTrustUkPrn(userName);
			return await _trustModelService.GetTrustAddressByUkPrn(trustUkPrn);
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);
			
			throw;
		}
	}
	
	public async Task<string> GetSelectedTrustUkPrn(string userName)
	{
		_logger.LogMethodEntered();
		
		try
		{
			var model = await GetCurrentModel(userName);
			return model.TrustUkPrn;
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);
			
			throw;
		}
	}

	public async Task<CreateCaseWizardModel> GetCreateCaseWizard(string userName)
	{
		_logger.LogMethodEntered();
		
		try
		{
			return await GetCurrentModel(userName);
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);
			
			throw;
		}
	}

	public async Task StartCreateNewCaseWizard(string userName)
	{
		_logger.LogMethodEntered();
		
		try
		{
			var wizardModel = CreateCaseWizardModel.CreateInstance();

			await StoreModel(userName, wizardModel);
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);

			throw;
		}
	}

	public async Task SetTrustInCreateCaseWizard(string userName, string trustUkPrn)
	{
		_logger.LogMethodEntered();
		
		try
		{
			var wizardModel = await GetCurrentModel(userName);
			ValidateCanUpdateModel(wizardModel);
			
			wizardModel.SetTrustUkPrn(trustUkPrn);

			await StoreModel(userName, wizardModel);
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);

			throw;
		}
	}
	
	public async Task SetCaseTypeInNewCaseWizard(string userName, int caseType)
	{
		_logger.LogMethodEntered();
		
		try
		{
			var wizardModel = await GetCurrentModel(userName);
			ValidateCanUpdateModel(wizardModel);
			
			wizardModel.SetCaseType(caseType);

			await StoreModel(userName, wizardModel);
		}
		catch (Exception ex)
		{
			_logger.LogErrorMsg(ex);

			throw;
		}
	}

	private static void ValidateCanUpdateModel(CreateCaseWizardModel wizardModel)
	{
		if (wizardModel is null) { throw new Exception();}

		if (wizardModel.IsComplete()) { throw new Exception(); }
	}

	private async Task<CreateCaseWizardModel> GetCurrentModel(string userName)
	{
		var userState = await _userStateCachedService.GetData(userName) ?? new UserState(userName);
			
		return _mapper.Map<CreateCaseWizardModel>(userState.CreateCaseWizardModel);
	}
	
	private async Task StoreModel(string userName, CreateCaseWizardModel wizardModel)
	{
		var userState = await _userStateCachedService.GetData(userName) ?? new UserState(userName);
		
		userState.CreateCaseWizardModel = _mapper.Map<CreateCaseWizardModelDto>(wizardModel);

		await _userStateCachedService.StoreData(userName, userState);
	}
}
