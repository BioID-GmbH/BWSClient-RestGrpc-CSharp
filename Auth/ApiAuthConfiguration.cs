namespace BioID.RestGrpcForwarder.Auth
{
    /// <summary>
    /// The ApiAuthConfiguration class is used to configure authentication for an API.
    /// </summary>
    public class ApiAuthConfiguration
    {
        /// <summary>
        /// The name of the header used for authentication.
        /// </summary>
        public string HeaderName { get; set; } = string.Empty;

        /// <summary>
        /// The API key used for authentication.
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;
    }
}
