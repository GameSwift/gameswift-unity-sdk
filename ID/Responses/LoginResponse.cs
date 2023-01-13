namespace GameSwiftSDK.Id.Responses
{
    /// <summary>
    /// Response from "GET /api/auth/login" request
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// Retrieved user's Id
        /// </summary>
        public string userId;
        /// <summary>
        /// Retrieved email
        /// </summary>
        public string email;
        /// <summary>
        /// Retrieved nickname
        /// </summary>
        public string nickname;
        /// <summary>
        /// Retrieved access token
        /// </summary>
        public string accessToken;
        /// <summary>
        /// Retrieved refresh Token
        /// </summary>
        public string refreshToken;
        /// <summary>
        /// Retrieved expires
        /// </summary>
        public string expires_in;
        /// <summary>
        /// Retrieved refresh token expires
        /// </summary>
        public string refresh_token_expires_in;
    }
}