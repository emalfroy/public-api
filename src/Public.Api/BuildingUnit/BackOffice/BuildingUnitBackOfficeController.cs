namespace Public.Api.BuildingUnit.BackOffice
{
    using Autofac.Features.AttributeFilters;
    using Be.Vlaanderen.Basisregisters.Api;
    using Common.Infrastructure;
    using Common.Infrastructure.Controllers;
    using Common.Infrastructure.Controllers.Attributes;
    using FeatureToggle;
    using Infrastructure.Configuration;
    using Infrastructure.Swagger;
    using Infrastructure.Version;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using RestSharp;

    [ApiVisible]
    [ApiVersion(Version.V2)]
    [AdvertiseApiVersions(Version.V2)]
    [ApiRoute("")]
    [ApiExplorerSettings(GroupName = "Gebouweenheden")]
    [ApiConsumes(EndpointType.BackOffice)]
    [ApiProduces(EndpointType.BackOffice)]
    public partial class BuildingUnitBackOfficeController : RegistryApiController<BuildingUnitBackOfficeController>
    {
        protected override string NotFoundExceptionMessage => "Onbestaande gebouweenheid.";
        protected override string GoneExceptionMessage => "Verwijderde gebouweenheid.";

        public BuildingUnitBackOfficeController(
            [KeyFilter(RegistryKeys.BuildingBackOffice)]
            IRestClient restClient,
            [KeyFilter(RegistryKeys.BuildingBackOffice)]
            IFeatureToggle cacheToggle,
            ConnectionMultiplexerProvider redis,
            ILogger<BuildingUnitBackOfficeController> logger)
            : base(restClient, cacheToggle, redis, logger)
        { }

        private static ContentFormat DetermineFormat(ActionContext context)
            => ContentFormat.For(EndpointType.BackOffice, context);
    }
}
