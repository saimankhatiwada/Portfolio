namespace Portfolio.Api.MediaTypes;

/// <summary>
/// Provides a collection of custom media type names used within the Portfolio API.
/// </summary>
/// <remarks>
/// This static class contains nested classes and constants that define various media type strings,
/// including standard JSON formats and custom HATEOAS-compliant media types.
/// These media types are utilized for content negotiation and response formatting in the API.
/// </remarks>
internal static class CustomMediaTypeNames
{
    /// <summary>
    /// Contains constants for application-specific media type names used in the Portfolio API.
    /// </summary>
    /// <remarks>
    /// This class defines various media type strings, including standard JSON formats and custom
    /// HATEOAS-compliant media types. These constants are used for content negotiation and response
    /// formatting within the API.
    /// </remarks>
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
