namespace GameSwiftSDK.Id.Responses
{
    /// <summary>
    /// Get Success response's request - login from laucher  
    /// </summary>
    public class OauthUserInfoResponse
    {
        /// <summary>
        /// Retrieved nickname
        /// </summary>
        public string nickname;
        /// <summary>
        /// Retrieved email address
        /// </summary>
        public string email;
        /// <summary>
        /// Retrieved access token
        /// </summary>
        public string accessToken;
        /// <summary>
        /// Retrieved refresh token
        /// </summary>
        public string refreshToken;
        /// <summary>
        /// Retrieved userId
        /// </summary>
        public string userId;
        /// <summary>
        /// Retrieved client Id 
        /// </summary>
        public string clientId;
        /// <summary>
        /// Retrieved roles
        /// </summary>
        public string[] roles;
        /// <summary>
        /// Retrieved branches
        /// </summary>
        public string[] branches;
    }
}
