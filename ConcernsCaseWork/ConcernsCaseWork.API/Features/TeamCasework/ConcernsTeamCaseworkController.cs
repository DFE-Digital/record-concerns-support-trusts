﻿using Ardalis.GuardClauses;
using ConcernsCaseWork.API.Contracts.Common;
using ConcernsCaseWork.API.Contracts.PolicyType;
using ConcernsCaseWork.API.Contracts.TeamCasework;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ConcernsCaseWork.API.Features.TeamCasework
{
	[ApiVersion("2.0")]
	[ApiController]
	[Authorize(Policy = Policy.Default)]
	[Route("v{version:apiVersion}/concerns-team-casework")]

	public class ConcernsTeamCaseworkController(ILogger<ConcernsTeamCaseworkController> logger,
		IGetConcernsCaseworkTeam getTeamCommand,
		IGetConcernsCaseworkTeamOwners getTeamOwnersCommand,
		IUpdateConcernsCaseworkTeam updateCommand,
		IGetOwnersOfOpenCases getOwnersOfOpenCases) : ControllerBase
	{
		private readonly ILogger<ConcernsTeamCaseworkController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
		private readonly IGetConcernsCaseworkTeam _getCommand = getTeamCommand ?? throw new ArgumentNullException(nameof(getTeamCommand));
		private readonly IGetConcernsCaseworkTeamOwners _getTeamOwnersCommand = getTeamOwnersCommand ?? throw new ArgumentNullException(nameof(getTeamOwnersCommand));
		private readonly IUpdateConcernsCaseworkTeam _updateCommand = updateCommand ?? throw new ArgumentNullException(nameof(updateCommand));
		private readonly IGetOwnersOfOpenCases _getOwnersOfOpenCases = Guard.Against.Null(getOwnersOfOpenCases);

		[HttpGet("owners/{ownerId}")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<ConcernsCaseworkTeamResponse>>> GetTeam(string ownerId, CancellationToken cancellationToken = default)
		{
			return await LogAndInvoke(async () =>
			{
				var result = await _getCommand.Execute(ownerId, cancellationToken);
				if (result is null)
					// successful, but nothing to return as no team created yet.
					return NoContent();

				var responseData = new ApiSingleResponseV2<ConcernsCaseworkTeamResponse>(result);
				return Ok(responseData);
			});
		}

		[HttpGet("owners")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<string[]>>> GetTeamOwners(CancellationToken cancellationToken = default)
		{
			return await LogAndInvoke(async () =>
			{
				var result = await _getTeamOwnersCommand.Execute(cancellationToken);
				if (result is null)
					return Ok(new ApiSingleResponseV2<string[]>(Array.Empty<string>()));

				var responseData = new ApiSingleResponseV2<string[]>(result);
				return Ok(responseData);
			});
		}

		[HttpGet("owners/open-cases")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<string[]>>> GetOwnersOfOpenCases(CancellationToken cancellationToken = default)
		{
			return await LogAndInvoke(async () =>
			{
				var results = await _getOwnersOfOpenCases.Execute(cancellationToken);
				return Ok(new ApiSingleResponseV2<string[]>(results));
			});
		}

		[HttpPut("owners/{ownerId}")]
		[MapToApiVersion("2.0")]
		public async Task<ActionResult<ApiSingleResponseV2<ConcernsCaseworkTeamResponse>>> Put(
			[StringLength(300)] string ownerId,
			[FromBody] ConcernsCaseworkTeamUpdateRequest updateModel,
			CancellationToken cancellationToken = default)
		{
			return await LogAndInvoke(async () =>
			{
				if (updateModel == null || updateModel.OwnerId != ownerId)
					return BadRequest(new { Error = "update model does not match ownerId" });

				var result = await _updateCommand.Execute(updateModel, cancellationToken);
				var responseData = new ApiSingleResponseV2<ConcernsCaseworkTeamResponse>(result);
				return Ok(responseData);
			});
		}

		private async Task<ActionResult> LogAndInvoke(Func<Task<ActionResult>> method, [CallerMemberName] string caller = "")
		{
			_logger.LogInformation($"Invoking {caller}");
			var result = await method();
			_logger.LogInformation($"Returning from {caller}");
			return result;
		}
	}
}
