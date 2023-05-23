using System;
using Newtonsoft.Json;

namespace GameSwiftSDK.Id.Responses
{
	/// <summary>
	/// Response given after successful <a href="https://id.gameswift.io/swagger/#/default/AuthController_login">POST /api/{idVersion}/auth/login</a> request.
	/// </summary>
	public class LoginResponse
	{
		[Obsolete("Use expiresIn instead."), JsonIgnore] public string expires_in => expiresIn;
		[Obsolete("Use refreshTokenExpiresIn instead."), JsonIgnore] public string refresh_token_expires_in => refreshTokenExpiresIn;

		/// <summary>
		/// Unique user's ID.
		/// </summary>
		public string userId;

		/// <summary>
		/// User's email.
		/// </summary>
		public string email;

		/// <summary>
		/// User's platform nickname. It can be empty.
		/// </summary>
		public string nickname;

		/// <summary>
		/// User's roles.
		/// </summary>
		public string[] roles;

		/// <summary>
		/// Game branches user has access to.
		/// </summary>
		public string[] branches;

		/// <summary>
		/// Retrieved access token used for authorization.
		/// </summary>
		public string accessToken;

		/// <summary>
		/// Retrieved refresh token used to maintain current authorization session.
		/// </summary>
		public string refreshToken;

		/// <summary>
		/// Access token expiration time.
		/// </summary>
		[JsonProperty("expires_in")] public string expiresIn;

		/// <summary>
		/// Refresh token expiration time.
		/// </summary>
		[JsonProperty("refresh_token_expires_in")] public string refreshTokenExpiresIn;
	}
}