namespace GameSwiftSDK.Id.Responses
{
    /// <summary>
    /// Response from "GET /api/auth/me" request
    /// </summary>
    public class AuthUserInfoResponse
    {
        /// <summary>
        /// Retrieved id
        /// </summary>
        public string id;
        /// <summary>
        /// Retrieved email
        /// </summary>
        public string email;
        /// <summary>
        /// Retrieved activate
        /// </summary>
        public string activate;
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