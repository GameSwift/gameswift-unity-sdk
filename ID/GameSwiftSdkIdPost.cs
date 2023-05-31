using System;
using System.Collections.Generic;
using GameSwiftSDK.Core;
using GameSwiftSDK.Id.Responses;
using Newtonsoft.Json;

namespace GameSwiftSDK.Id
{
	/// <summary>
	/// Setup and send requests to <a href="https://id.gameswift.io/swagger">GameSwift ID</a>.
	/// </summary>
	public partial class GameSwiftSdkId
	{
		/// <summary>
		/// Logins to GameSwift ID. It's response contains an Access Token needed and used ONLY in
		/// a <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_getAuthorize">GET /api/{idVersion}/oauth/authorize</a> request.
		/// This method doesn't include further authorization steps so they need to be executed manually to retrieve and store Access and Refresh Tokens.
		/// If there is no need of a specific login treatment use "LoginAndAuthorize" method instead.
		/// Sends a <a href="https://id.gameswift.io/swagger/#/default/AuthController_login">POST /api/{idVersion}/auth/login</a> request.
		/// </summary>
		/// <param name="emailOrNickname">User's email or nickname</param>
		/// <param name="password">User's password</param>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void Login (string emailOrNickname, string password, Action<LoginResponse> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure)
		{
			Dictionary<string, string> credentials = new Dictionary<string, string>()
			{
				{ "emailOrNickname", emailOrNickname },
				{ "password", password }
			};
			var requestBody = JsonConvert.SerializeObject(credentials, Formatting.Indented);

			var apiUri = $"{API_ADDRESS}/auth/login";
			var request = new RequestData(apiUri, requestBody);
			request.SetupHeaders(CustomHeader.None, "");

			GameSwiftSdkCore.SendPostRequest<LoginResponse>(request, HandleLoginSuccess, handleFailure);

			void HandleLoginSuccess (LoginResponse loginResponse)
			{
				handleSuccess.Invoke(loginResponse);
			}
		}

		/// <summary>
		/// Logins to GameSwift ID and authorizes the user. Stores Access Token and Refresh Token in the GameSwiftSdkId.Instance.
		/// To achieve that multiple requests are sent in sequence:
		/// 1. <a href="https://id.gameswift.io/swagger/#/default/AuthController_login">POST /api/{idVersion}/auth/login</a>
		/// 2. <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_getAuthorize">GET /api/{idVersion}/oauth/authorize</a>
		/// 3. <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postToken">POST /api/{idVersion}/oauth/token</a>
		/// 4. <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_getMe">GET /api/{idVersion}/oauth/me</a>
		/// </summary>
		/// <param name="emailOrNickname">User's email or nickname</param>
		/// <param name="password">User's password</param>
		/// <param name="clientId">OAuth client ID received from <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postClient">POST /api/{idVersion}/oauth/client</a> request</param>
		/// <param name="redirectUri">Redirect uri received from <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postClient">POST /api/{idVersion}/oauth/client</a> request</param>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void LoginAndAuthorize (string emailOrNickname, string password, string clientId, string redirectUri,
			Action<OauthUserInfoResponse> handleSuccess, Action<BaseSdkFailResponse> handleFailure)
		{
			Login(emailOrNickname, password, HandleLoginSuccess, handleFailure);

			void HandleLoginSuccess (LoginResponse response)
			{
				Authorize(response.accessToken, clientId, redirectUri, handleSuccess, handleFailure);
			}
		}

		/// <summary>
		/// Authorizes the user. Stores Access Token and Refresh Token in the GameSwiftSdkId.Instance.
		/// To achieve that multiple requests are sent in sequence:
		/// 1. <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_getAuthorize">GET /api/{idVersion}/oauth/authorize</a>
		/// 2. <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postToken">POST /api/{idVersion}/oauth/token</a>
		/// 3. <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_getMe">GET /api/{idVersion}/oauth/me</a>
		/// </summary>
		/// <param name="accessToken">Access Token retrieved from a <a href="https://id.gameswift.io/swagger/#/default/AuthController_login">POST /api/{idVersion}/auth/login</a> request
		/// or a Cmd Access Token stored in the GameSwiftSdkId.Instance after a successful "ReadUserInfoFromLauncher" method execution (in case of logging in from the launcher)</param>
		/// <param name="clientId">OAuth client ID received from <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postClient">POST /api/{idVersion}/oauth/client</a> request</param>
		/// <param name="redirectUri">Redirect uri received from <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postClient">POST /api/{idVersion}/oauth/client</a> request</param>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void Authorize (string accessToken, string clientId, string redirectUri,
			Action<OauthUserInfoResponse> handleSuccess, Action<BaseSdkFailResponse> handleFailure)
		{
			GetAuthorizationCode(accessToken, clientId, redirectUri, HandleAuthorizationSuccess, handleFailure);

			void HandleAuthorizationSuccess (AuthorizeResponse response)
			{
				RetrieveOauthToken(response.code, clientId, redirectUri, HandleRequestTokenSuccess, handleFailure);
			}

			void HandleRequestTokenSuccess (TokenResponse response)
			{
				GetOauthUserInformation(response.accessToken, HandleOAuthMeSuccess, handleFailure);

				void HandleOAuthMeSuccess (OauthUserInfoResponse oauthMeResponse)
				{
					Instance.AccessToken = response.accessToken;
					Instance.RefreshToken = response.refreshToken;
					Instance._accessTokenRetrieved = true;
					handleSuccess.Invoke(oauthMeResponse);
				}
			}
		}

		/// <summary>
		/// Refreshes current token.
		/// Sends a <a href="https://id.gameswift.io/swagger/#/default/AuthController_postRefresh">POST /api/{idVersion}/auth/refresh</a> request.
		/// </summary>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void RefreshApiToken (Action<MessageResponse> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure)
		{
			var apiUri = $"{API_ADDRESS}/auth/refresh";
			var request = new RequestData(apiUri);
			request.SetupHeaders(CustomHeader.RefreshToken, Instance.RefreshToken);

			GameSwiftSdkCore.SendPostRequest(request, handleSuccess, handleFailure);
		}

		/// <summary>
		/// Retrieves access and refresh tokens with a use of authorization code.
		/// Sends a <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postToken">POST /api/{idVersion}/oauth/token</a> request.
		/// </summary>
		/// <param name="authorizationCode">Authorization code received from <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_getAuthorize">GET /api/{idVersion}/oauth/authorize</a> request</param>
		/// <param name="clientId">OAuth client ID received from <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postClient">POST /api/{idVersion}/oauth/client</a> request</param>
		/// <param name="redirectUri">Redirect uri received from <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postClient">POST /api/{idVersion}/oauth/client</a> request</param>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void RetrieveOauthToken (string authorizationCode, string clientId, string redirectUri,
			Action<TokenResponse> handleSuccess, Action<BaseSdkFailResponse> handleFailure)
		{
			Dictionary<string, string> body = new Dictionary<string, string>()
			{
				{ "client_id", clientId },
				{ "grant_type", "authorization_code" },
				{ "code", authorizationCode },
				{ "redirect_uri", redirectUri }
			};
			var queryString = GameSwiftSdkCore.GetParsedQueryString(body);

			var apiUri = $"{API_ADDRESS}/oauth/token";
			var request = new RequestData(apiUri, queryString);
			request.SetupHeaders(CustomHeader.WwwContentType, "");

			GameSwiftSdkCore.SendPostRequest(request, handleSuccess, handleFailure);
		}

		/// <summary>
		/// Retrieves access and refresh tokens with a use of refresh token.
		/// Sends a <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postToken">POST /api/{idVersion}/oauth/token</a> request.
		/// </summary>
		/// <param name="clientId">OAuth client ID received from <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postClient">POST /api/{idVersion}/oauth/client</a> request</param>
		/// <param name="redirectUri">Redirect uri received from <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postClient">POST /api/{idVersion}/oauth/client</a> request</param>
		/// <param name="refreshToken">Refresh token received from <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postToken">POST /api/{idVersion}/oauth/token</a> request.
		/// If the user is correctly logged in and authorized with /oauth/token it should be already stored in GameSwiftSdkId.Instance.RefreshToken</param>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		private static void RefreshOauthToken (string clientId, string redirectUri, string refreshToken,
			Action<TokenResponse> handleSuccess, Action<BaseSdkFailResponse> handleFailure)
		{
			Dictionary<string, string> body = new Dictionary<string, string>()
			{
				{ "client_id", clientId },
				{ "grant_type", "refresh_token" },
				{ "redirect_uri", redirectUri },
				{ "refresh_token", refreshToken }
			};
			var queryString = GameSwiftSdkCore.GetParsedQueryString(body);

			var apiUri = $"{API_ADDRESS}/oauth/token";
			var request = new RequestData(apiUri, queryString);
			request.SetupHeaders(CustomHeader.WwwContentType, "");

			GameSwiftSdkCore.SendPostRequest(request, handleSuccess, handleFailure);
		}
	}
}