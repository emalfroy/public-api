namespace Public.Api.Extract
{
    using System;
    using System.Globalization;
    using System.Net.Mime;
    using System.Threading;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Common.Infrastructure;
    using Infrastructure.Swagger;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Converters;
    using Swashbuckle.AspNetCore.Filters;
    using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Extract")]
    [ApiOrder(Order = ApiOrder.Extract)]
    [Produces(MediaTypeNames.Application.Zip)]
    public class ExtractController : ApiController<ExtractController>
    {
        private readonly ExtractDownloads _extractDownloads;

        public ExtractController(
            ConnectionMultiplexerProvider redis,
            ILogger<ExtractController> logger,
            ExtractDownloads extractDownloads)
            : base(redis, logger)
            => _extractDownloads = extractDownloads;

        /// <summary>Download het meest recente extract.</summary>
        /// <param name="cancellationToken"></param>
        /// <response code="302">Als de extract download gevonden is.</response>
        /// <response code="404">Als er geen extract download gevonden kan worden.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("extract")]
        [ProducesResponseType(typeof(void), StatusCodes.Status302Found)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status302Found, typeof(ExtractRedirectResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ExtractNotFoundResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        public async Task<IActionResult> Extract(CancellationToken cancellationToken = default)
            => await _extractDownloads.RedirectToMostRecent(cancellationToken);

        /// <summary>Download het extract voor de gevraagde datum.</summary>
        /// <param name="extractDate">yyyy-MM-dd</param>
        /// <param name="cancellationToken"></param>
        /// <response code="302">Als de extract download gevonden is.</response>
        /// <response code="400">Als de extract download datum niet het correcte formaat heeft.</response>
        /// <response code="404">Als de extract download niet gevonden kan worden.</response>
        /// <response code="500">Als er een interne fout is opgetreden.</response>
        [HttpGet("extract/{extractDate}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status302Found)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [SwaggerResponseExample(StatusCodes.Status302Found, typeof(ExtractRedirectResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ExtractBadRequestResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(ExtractNotFoundResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples), jsonConverter: typeof(StringEnumConverter))]
        public async Task<IActionResult> Extract(string extractDate, CancellationToken cancellationToken = default)
            => DateTime.TryParseExact(extractDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
                ? await _extractDownloads.RedirectTo(date, cancellationToken)
                : throw new ApiException("Ongeldige datum.", StatusCodes.Status400BadRequest);
    }

    public class ExtractRedirectResponseExamples : IExamplesProvider<object>
    {
        public object GetExamples() => new object();
    }

    public class ExtractBadRequestResponseExamples : IExamplesProvider<ProblemDetails>
    {
        public ProblemDetails GetExamples()
            => new ProblemDetails
            {
                HttpStatus = StatusCodes.Status400BadRequest,
                Title = ProblemDetails.DefaultTitle,
                Detail = "Ongeldige datum.",
                ProblemInstanceUri = ProblemDetails.GetProblemNumber()
            };
    }

    public class ExtractNotFoundResponseExamples : IExamplesProvider<ProblemDetails>
    {
        public ProblemDetails GetExamples()
            => new ProblemDetails
            {
                HttpStatus = StatusCodes.Status404NotFound,
                Title = ProblemDetails.DefaultTitle,
                Detail = "Onbestaand testbestand.",
                ProblemInstanceUri = ProblemDetails.GetProblemNumber()
            };
    }
}
