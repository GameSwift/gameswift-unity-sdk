using System;

namespace GameSwiftSDK.Id.Responses
{
	/// <summary>
	/// Response given after successful <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_getMe">GET /api/{idVersion}/oauth/me</a> request (including login from launcher).
	/// </summary>
	public class OauthUserInfoResponse
	{
		/// <summary>
		/// Access token expiration date.
		/// </summary>
		public DateTime accessTokenExpiresAt;

		/// <summary>
		/// Refresh token expiration date.
		/// </summary>
		public DateTime refreshTokenExpiresAt;

		/// <summary>
		/// [Temporarily unsupported] Scope is used to restrict specific token's permissions.
		/// </summary>
		public string[] scope;

		/// <summary>
		/// Unique user's ID.
		/// </summary>
		public string userId;

		/// <summary>
		/// Oauth client ID.
		/// </summary>
		public string clientId;

		/// <summary>
		/// User's roles.
		/// </summary>
		public string[] roles;

		/// <summary>
		/// Game branches user has access to.
		/// </summary>
		public string[] branches;

		/// <summary>
		/// User's platform nickname. It can be empty.
		/// </summary>
		public string nickname;

		/// <summary>
		/// User's email address.
		/// </summary>
		public string email;

		/// <summary>
		/// User's avatar url.
		/// </summary>
		public string avatarUrl;

		/// <summary>
		/// User's country.
		/// </summary>
		public string country;

		/// <summary>
		/// Information about the user.
		/// </summary>
		public string about;

		/// <summary>
		/// Information if user's account is public or private.
		/// </summary>
		public bool isPublic;
	}
}