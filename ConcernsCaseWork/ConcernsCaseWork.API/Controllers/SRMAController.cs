using ConcernsCaseWork.API.RequestModels.CaseActions.SRMA;
using ConcernsCaseWork.API.ResponseModels;
using ConcernsCaseWork.API.ResponseModels.CaseActions.SRMA;
using ConcernsCaseWork.API.UseCases;
using ConcernsCaseWork.Data.Enums;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace ConcernsCaseWork.API.Controllers
{
    [ApiVersion("2.0")]
    [ApiController]
    [Route("v{version:apiVersion}/case-actions/srma")]
    public class SRMAController : Controller
    {
        private readonly ILogger<SRMAController> _logger;
        private readonly IUseCase<CreateSRMARequest, SRMAResponse> _createSRMAUseCase;
        private readonly IUseCase<int, ICollection<SRMAResponse>> _getSRMAsByCaseIdUseCase;
        private readonly IUseCase<int, SRMAResponse> _getSRMAByIdUseCase;
        private readonly IUseCase<PatchSRMARequest, SRMAResponse> _patchSRMAUseCase;

        public SRMAController(
            ILogger<SRMAController> logger,
            IUseCase<CreateSRMARequest, SRMAResponse> createSRMAUseCase,
            IUseCase<int, ICollection<SRMAResponse>> getSRMAsByCaseIdUseCase,
            IUseCase<int, SRMAResponse> getSRMAByIdUseCase,
            IUseCase<PatchSRMARequest, SRMAResponse> patchSRMAUseCase)
        {
            _logger = logger;
            _createSRMAUseCase = createSRMAUseCase;
            _getSRMAsByCaseIdUseCase = getSRMAsByCaseIdUseCase;
            _getSRMAByIdUseCase = getSRMAByIdUseCase;
            _patchSRMAUseCase = patchSRMAUseCase;
        }

        [HttpPost]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<SRMAResponse>>> Create(CreateSRMARequest request, CancellationToken cancellationToken = default)
        {
            var createdSRMA = _createSRMAUseCase.Execute(request);
            var response = new ApiSingleResponseV2<SRMAResponse>(createdSRMA);

            return new ObjectResult(response) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<SRMAResponse>>> GetSRMAById(int srmaId, CancellationToken cancellationToken = default)
        {
            var srma = _getSRMAByIdUseCase.Execute(srmaId);
            var response = new ApiSingleResponseV2<SRMAResponse>(srma);

            return Ok(response);
        }

        [HttpGet]
        [Route("case/{caseUrn}")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<ICollection<SRMAResponse>>>> GetSRMAsByCase(int caseUrn, CancellationToken cancellationToken = default)
        {
            var srmas = _getSRMAsByCaseIdUseCase.Execute(caseUrn);
            var response = new ApiSingleResponseV2<ICollection<SRMAResponse>>(srmas);

            return Ok(response);
        }

        [HttpPatch]
        [Route("{srmaId}/update-status")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<SRMAResponse>>> UpdateStatus(int srmaId, SRMAStatus status, CancellationToken cancellationToken = default)
        {
            var patched = _patchSRMAUseCase.Execute(new PatchSRMARequest
            {
                SRMAId = srmaId,
                Delegate = (srma) =>
                {
                    srma.StatusId = (int)status;
                    return srma;
                }
            });

            var response = new ApiSingleResponseV2<SRMAResponse>(patched);

            return Ok(response);
        }

        [HttpPatch]
        [Route("{srmaId}/update-reason")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<SRMAResponse>>> UpdateReason(int srmaId, SRMAReasonOffered reason, CancellationToken cancellationToken = default)
        {
            var patched = _patchSRMAUseCase.Execute(new PatchSRMARequest
            {
                SRMAId = srmaId,
                Delegate = (srma) =>
                {
                    srma.ReasonId = (int)reason;
                    return srma;
                }
            });

            var response = new ApiSingleResponseV2<SRMAResponse>(patched);

            return Ok(response);
        }

        [HttpPatch]
        [Route("{srmaId}/update-offered-date")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<SRMAResponse>>> UpdateOfferedDate(int srmaId, string offeredDate, CancellationToken cancellationToken = default)
        {
            try
            {
                DateTime? dateOffered = DeserialiseDateTime(offeredDate);

                if (dateOffered == null)
                {
                    return BadRequest("Offered Date Cannot Be Null");
                }
                else
                {
                    var patched = _patchSRMAUseCase.Execute(new PatchSRMARequest
                    {
                        SRMAId = srmaId,
                        Delegate = (srma) =>
                        {
                            srma.DateOffered = dateOffered.Value;
                            return srma;
                        }
                    });

                    var response = new ApiSingleResponseV2<SRMAResponse>(patched);

                    return Ok(response);
                }
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "DateTime received doesn't conform to format");
                throw;
            }
        }

        [HttpPatch]
        [Route("{srmaId}/update-notes")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<SRMAResponse>>> UpdateNotes(int srmaId, [StringLength(2000)] string notes, CancellationToken cancellationToken = default)
        {
            var patched = _patchSRMAUseCase.Execute(new PatchSRMARequest
            {
                SRMAId = srmaId,
                Delegate = (srma) =>
                {
                    srma.Notes = notes;
                    return srma;
                }
            });

            var response = new ApiSingleResponseV2<SRMAResponse>(patched);

            return Ok(response);
        }

        [HttpPatch]
        [Route("{srmaId}/update-visit-dates")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<SRMAResponse>>> UpdateVisitDates(int srmaId, string startDate, string endDate, CancellationToken cancellationToken = default)
        {
            try
            {
                var patched = _patchSRMAUseCase.Execute(new PatchSRMARequest
                {
                    SRMAId = srmaId,
                    Delegate = (srma) =>
                    {
                        srma.StartDateOfVisit = DeserialiseDateTime(startDate);
                        srma.EndDateOfVisit = DeserialiseDateTime(endDate);
                        return srma;
                    }
                });

                var response = new ApiSingleResponseV2<SRMAResponse>(patched);

                return Ok(response);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "DateTime received doesn't conform to format");
                throw;
            }
        }

        [HttpPatch]
        [Route("{srmaId}/update-date-accepted")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<SRMAResponse>>> UpdateDateAccepted(int srmaId, string acceptedDate, CancellationToken cancellationToken = default)
        {
            try
            {
                var patched = _patchSRMAUseCase.Execute(new PatchSRMARequest
                {
                    SRMAId = srmaId,
                    Delegate = (srma) =>
                    {
                        srma.DateAccepted = DeserialiseDateTime(acceptedDate);
                        return srma;
                    }
                });

                var response = new ApiSingleResponseV2<SRMAResponse>(patched);

                return Ok(response);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "DateTime received doesn't conform to format");
                throw;
            }
        }

        [HttpPatch]
        [Route("{srmaId}/update-date-report-sent")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<SRMAResponse>>> UpdateDateReportSent(int srmaId, string dateReportSent, CancellationToken cancellationToken = default)
        {
            try
            {
                var patched = _patchSRMAUseCase.Execute(new PatchSRMARequest
                {
                    SRMAId = srmaId,
                    Delegate = (srma) =>
                    {
                        srma.DateReportSentToTrust = DeserialiseDateTime(dateReportSent);
                        return srma;
                    }
                });

                var response = new ApiSingleResponseV2<SRMAResponse>(patched);

                return Ok(response);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "DateTime received doesn't conform to format");
                throw;
            }
        }

        [HttpPatch]
        [Route("{srmaId}/update-closed-date")]
        [MapToApiVersion("2.0")]
        public async Task<ActionResult<ApiSingleResponseV2<SRMAResponse>>> UpdateDateClosed(int srmaId, CancellationToken cancellationToken = default)
        {
            try
            {
                var patched = _patchSRMAUseCase.Execute(new PatchSRMARequest
                {
                    SRMAId = srmaId,
                    Delegate = (srma) =>
                    {
						srma.ClosedAt = DateTime.Now;
                        return srma;
                    }
                });

                var response = new ApiSingleResponseV2<SRMAResponse>(patched);

                return Ok(response);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "DateTime received doesn't conform to format");
                throw;
            }
        }

        private DateTime? DeserialiseDateTime(string value)
        {
            var dateTimeFormatInfo = CultureInfo.InvariantCulture.DateTimeFormat;
            return string.IsNullOrWhiteSpace(value) ? null : (DateTime?)DateTime.ParseExact(value, "dd-MM-yyyy", dateTimeFormatInfo, DateTimeStyles.None);
        }
    }
}