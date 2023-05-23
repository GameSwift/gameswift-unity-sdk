namespace GameSwiftSDK.Id.Responses
{
	/// <summary>
	/// Response given after successful <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_getAuthorize">GET /api/{idVersion}/oauth/authorize</a> request.
	/// </summary>
	public class AuthorizeResponse
	{
		/// <summary>
		/// Authorization code used in <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postToken">POST /api/{idVersion}/oauth/token</a> request.
		/// </summary>
		public string code;

		/// <summary>
		/// Authorization state. It can be empty.
		/// </summary>
		public string state;
	}
}