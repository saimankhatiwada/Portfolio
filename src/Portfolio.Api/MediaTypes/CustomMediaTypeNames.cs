namespace Portfolio.Api.MediaTypes;

internal static class CustomMediaTypeNames
{
    public static class Application
    {
        public const string JsonV1 = "application/json;v=1";
        public const string JsonV2 = "application/json;v=2";
        public const string HateoasJson = "application/vnd.portfolio.hateoas+json";
        public const string HateoasJsonV1 = "application/vnd.portfolio.hateoas.1+json";
        public const string HateoasJsonV2 = "application/vnd.portfolio.hateoas.2+json";

        public const string HateoasSubType = "hateoas";
    }
}
