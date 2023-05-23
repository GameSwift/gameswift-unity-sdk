using System;
using Newtonsoft.Json;

namespace GameSwiftSDK.Id.Responses
{
	/// <summary>
	/// Response given after successful <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postToken">POST /api/{idVersion}/oauth/token</a> request.
	/// </summary>
	public class TokenResponse
	{
		[Obsolete("Use accessToken instead."), JsonIgnore] public string access_token => accessToken;
		[Obsolete("Use refreshToken instead."), JsonIgnore] public string refresh_token => refreshToken;
		[Obsolete("Use idToken instead."), JsonIgnore] public string id_token => idToken;
		[Obsolete("Use expiresIn instead."), JsonIgnore] public int expires_in => expiresIn;
		[Obsolete("Use tokenType instead."), JsonIgnore] public string token_type => tokenType;

		/// <summary>
		/// Access token used for project authorization.
		/// </summary>
		[JsonProperty("access_token")] public string accessToken;

		/// <summary>
		/// Refresh token used for project authorization.
		/// </summary>
		[JsonProperty("refresh_token")] public string refreshToken;

		/// <summary>
		/// Access token ID.
		/// </summary>
		[JsonProperty("id_token")] public string idToken;

		/// <summary>
		/// Access token expiration time.
		/// </summary>
		[JsonProperty("expires_in")] public int expiresIn;

		/// <summary>
		/// Access token type.
		/// </summary>
		[JsonProperty("token_type")] public string tokenType;
	}
}