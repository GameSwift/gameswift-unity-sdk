namespace GameSwiftSDK.Id.Responses
{
    /// <summary>
    /// Success response from "GET /api/oauth/token" request
    /// </summary>
    public class TokenResponse
    {
        /// <summary>
        /// Retrieved access Token
        /// </summary>
        public string access_token;
        /// <summary>
        /// Retrieved refresh token
        /// </summary>
        public string refresh_token;
        /// <summary>
        /// Retrieved id token
        /// </summary>
        public string id_token;
        /// <summary>
        /// Retrieved token type
        /// </summary>
        public string token_type;
    }
}