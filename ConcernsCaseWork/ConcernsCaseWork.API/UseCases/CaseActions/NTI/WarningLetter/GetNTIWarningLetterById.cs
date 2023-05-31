using Azure.Core;
using ConcernsCaseWork.API.Contracts.ResponseModels.Concerns.Decisions;
using ConcernsCaseWork.API.Factories;
using ConcernsCaseWork.API.Factories.CaseActionFactories;
using ConcernsCaseWork.API.ResponseModels.CaseActions.NTI.WarningLetter;
using ConcernsCaseWork.Data.Gateways;
using System;

namespace ConcernsCaseWork.API.UseCases.CaseActions.NTI.WarningLetter
{
	public class GetNTIWarningLetterById : IUseCase<long, NTIWarningLetterResponse>
	{
		private readonly INTIWarningLetterGateway _gateway;

		public GetNTIWarningLetterById(INTIWarningLetterGateway gateway)
		{
			_gateway = gateway;
		}

		public NTIWarningLetterResponse Execute(long warningLetterId)
		{
			return ExecuteAsync(warningLetterId).Result;
		}

		public async Task<NTIWarningLetterResponse> ExecuteAsync(long warningLetterId)
		{
			var warningLetter = await _gateway.GetNTIWarningLetterById(warningLetterId);
			if (warningLetter ==null)
			{
				return null;
				//return Task.FromResult<NTIWarningLetterResponse>(null).Result;
			}
			return NTIWarningLetterFactory.CreateResponse(warningLetter);
		}

		//public async NTIWarningLetterResponse Execute(long warningLetterId)
		//{
		//	var result = await _gateway.GetNTIWarningLetterById(warningLetterId);

		//	return result == null ? null : NTIWarningLetterFactory.CreateResponse(result);


		//}

		//public async Task<NTIWarningLetterResponse> ExecuteAsync(long warningLetterId)
		//{
		//	var result = await _gateway.GetNTIWarningLetterById(warningLetterId);

		//	return result == null ? null : NTIWarningLetterFactory.CreateResponse(result);
		//}

		//     public async Task<NTIWarningLetterResponse> ExecuteAsync(long warningLetterId)
		//     {
		//var warningLetter = await _gateway.GetNTIWarningLetterById(warningLetterId);
		//if (warningLetter == null)
		//{
		//	return Task.FromResult<NTIWarningLetterResponse>(null);
		//}
		//return NTIWarningLetterFactory.CreateResponse(warningLetter);


		////async Task<NTIWarningLetterResponse> DoWork()
		////{
		////	var warningLetter = await _gateway.GetNTIWarningLetterById(warningLetterId);

		////	return warningLetter != null ? NTIWarningLetterFactory.CreateResponse(warningLetter): null;
		////}

		////return await DoWork();
		//}



		//public NTIWarningLetterResponse Execute(long warningLetterId)
		//{
		//	return ExecuteAsync(warningLetterId).Result;
		//}

		//public async Task<NTIWarningLetterResponse> ExecuteAsync(long warningLetterId)
		//{
		//	var warningLetter = await _gateway.GetNTIWarningLetterById(warningLetterId);
		//	return NTIWarningLetterFactory.CreateResponse(warningLetter);
		//}
	}
}
