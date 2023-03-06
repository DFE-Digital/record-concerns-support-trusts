using Ardalis.GuardClauses;
using ConcernsCaseWork.API.RequestModels.Concerns.TeamCasework;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.Concerns.TeamCasework;
using ConcernsCaseWork.API.UseCases;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ConcernsCaseWork.API.Controllers
{
    [ApiVersion("2.0")]
    [ApiController]
    [Route("v{version:apiVersion}/concerns-team-casework")]

    public class ConcernsTeamCaseworkController : ControllerBase
    {
        private readonly ILogger<ConcernsTeamCaseworkController> _logger;
        private readonly IGetConcernsCaseworkTeam _getCommand;
        private readonly IGetConcernsCaseworkTeamOwners _getTeamOwnersCommand;
        private readonly IUpdateConcernsCaseworkTeam _updateCommand;
        private readonly IGetOwnersOfOpenCases _getOwnersOfOpenCases;

        public ConcernsTeamCaseworkController(ILogger<ConcernsTeamCaseworkController> logger, 
            IGetConcernsCaseworkTeam getTeamCommand,
            IGetConcernsCaseworkTeamOwners getTeamOwnersCommand,
            IUpdateConcernsCaseworkTeam updateCommand,
            IGetOwnersOfOpenCases getOwnersOfOpenCases)
        {
            _logger=logger ?? throw new ArgumentNullException(nameof(logger));
            _getCommand = getTeamCommand ?? throw new ArgumentNullException(nameof(getTeamCommand));
            _getTeamOwnersCommand = getTeamOwnersCommand ?? throw  new ArgumentNullException(nameof(getTeamOwnersCommand));
            _updateCommand = updateCommand ?? throw new ArgumentNullException(nameof(updateCommand));
            _getOwnersOfOpenCases = Guard.Against.Null(getOwnersOfOpenCases);
        }

        [HttpGet("owners/{ownerId}")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<ConcernsCaseworkTeamResponse>>> GetTeam(string ownerId, CancellationToken cancellationToken)
        {
            return await LogAndInvoke(async () =>
            {
                var result = await _getCommand.Execute(ownerId, cancellationToken);
                if (result is null)
                {
                    // successful, but nothing to return as no team created yet.
                    return NoContent();
                }

                var responseData = new ApiSingleResponseV2<ConcernsCaseworkTeamResponse>(result);
                return Ok(responseData);
            });
        }

        [HttpGet("owners")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<string[]>>> GetTeamOwners(CancellationToken cancellationToken)
        {
            return await LogAndInvoke(async () =>
            {
                var result = await _getTeamOwnersCommand.Execute(cancellationToken);
                if (result is null)
                {
                    return Ok(new ApiSingleResponseV2<string[]>(Array.Empty<string>()));
                }

                var responseData = new ApiSingleResponseV2<string[]>(result);
                return Ok(responseData);
            });
        }

        [HttpGet("owners/open-cases")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<string[]>>> GetOwnersOfOpenCases(CancellationToken cancellationToken)
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
	        [StringLength(300)]string ownerId,
            [FromBody] ConcernsCaseworkTeamUpdateRequest updateModel,
            CancellationToken cancellationToken)
        {
            return await LogAndInvoke(async () =>
            {
                if (updateModel == null || updateModel.OwnerId != ownerId)
                {
                    return BadRequest(new { Error = "update model does not match ownerId" });
                }

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
