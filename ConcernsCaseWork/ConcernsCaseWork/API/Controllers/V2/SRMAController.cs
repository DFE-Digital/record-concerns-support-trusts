using Concerns.Data.Enums;
using Concerns.Data.RequestModels.CaseActions.SRMA;
using Concerns.Data.ResponseModels;
using Concerns.Data.ResponseModels.CaseActions.SRMA;
using Concerns.Data.UseCases;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace Concerns.Data.Controllers.V2
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
        public ActionResult<ApiSingleResponseV2<SRMAResponse>> Create(CreateSRMARequest request)
        {
            var createdSRMA = _createSRMAUseCase.Execute(request);
            var response = new ApiSingleResponseV2<SRMAResponse>(createdSRMA);

            return new ObjectResult(response) { StatusCode = StatusCodes.Status201Created };
        }

        [HttpGet]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<SRMAResponse>> GetSRMAById(int srmaId)
        {
            var srma = _getSRMAByIdUseCase.Execute(srmaId);
            var response = new ApiSingleResponseV2<SRMAResponse>(srma);

            return Ok(response);
        }

        [HttpGet]
        [Route("case/{caseUrn}")]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<ICollection<SRMAResponse>>> GetSRMAsByCase(int caseUrn)
        {
            var srmas = _getSRMAsByCaseIdUseCase.Execute(caseUrn);
            var response = new ApiSingleResponseV2<ICollection<SRMAResponse>>(srmas);

            return Ok(response);
        }

        [HttpPatch]
        [Route("{srmaId}/update-status")]
        [MapToApiVersion("2.0")]
        public ActionResult<ApiSingleResponseV2<SRMAResponse>> UpdateStatus(int srmaId, SRMAStatus status)
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
        public ActionResult<ApiSingleResponseV2<SRMAResponse>> UpdateReason(int srmaId, SRMAReasonOffered reason)
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
        public ActionResult<ApiSingleResponseV2<SRMAResponse>> UpdateOfferedDate(int srmaId, string offeredDate)
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
        public ActionResult<ApiSingleResponseV2<SRMAResponse>> UpdateNotes(int srmaId, string notes)
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
        public ActionResult<ApiSingleResponseV2<SRMAResponse>> UpdateVisitDates(int srmaId, string startDate, string endDate)
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
        public ActionResult<ApiSingleResponseV2<SRMAResponse>> UpdateDateAccepted(int srmaId, string acceptedDate)
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
        public ActionResult<ApiSingleResponseV2<SRMAResponse>> UpdateDateReportSent(int srmaId, string dateReportSent)
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
        public ActionResult<ApiSingleResponseV2<SRMAResponse>> UpdateDateClosed(int srmaId, string dateClosed)
        {
            try
            {
                var patched = _patchSRMAUseCase.Execute(new PatchSRMARequest
                {
                    SRMAId = srmaId,
                    Delegate = (srma) =>
                    {
                        srma.ClosedAt = DeserialiseDateTime(dateClosed);
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