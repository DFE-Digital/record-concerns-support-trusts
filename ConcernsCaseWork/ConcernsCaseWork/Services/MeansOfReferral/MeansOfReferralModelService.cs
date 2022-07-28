using AutoMapper;
using ConcernsCaseWork.Models;
using Microsoft.Extensions.Logging;
using Service.Redis.MeansOfReferral;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConcernsCaseWork.Services.MeansOfReferral
{
	public sealed class MeansOfReferralModelService : IMeansOfReferralModelService
	{
		private readonly IMeansOfReferralCachedService _meansOfReferralsCachedService;
		private readonly ILogger<MeansOfReferralModelService> _logger;
		private readonly IMapper _mapper;

		public MeansOfReferralModelService(IMeansOfReferralCachedService meansOfReferralCachedService, 
			IMapper mapper, 
			ILogger<MeansOfReferralModelService> logger)
		{
			_meansOfReferralsCachedService = meansOfReferralCachedService;
			_mapper = mapper;
			_logger = logger;
		}
		
		public async Task<IList<MeansOfReferralModel>> GetMeansOfReferrals()
		{
			_logger.LogInformation("{ClassName}::{MethodName}", nameof(MeansOfReferralModelService), nameof(GetMeansOfReferrals));
			
			var meansOfReferralDtos = await _meansOfReferralsCachedService.GetMeansOfReferralsAsync();

			return meansOfReferralDtos
				.Select(dto => new MeansOfReferralModel
				{
					Name = dto.Name, 
					Urn = dto.Urn, 
					Description = dto.Description
				})
				.ToList();
		}
	}
}