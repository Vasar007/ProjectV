namespace ProjectV.CommonWebApi.Models
{
    public static class RestApiStrings
    {
        public static string InternalErrorMessage { get; } = "Internal Server Error from the custom middleware.";
        public static string AccessViolationErrorMessage { get; } = "Access violation error from the custom middleware";
    }
}
