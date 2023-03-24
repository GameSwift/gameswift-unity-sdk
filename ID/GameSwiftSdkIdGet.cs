using System;
using GameSwiftSDK.Core;
using GameSwiftSDK.Id.Responses;

namespace GameSwiftSDK.Id
{
	/// <summary>
	/// Send custom request to <a href="https://id.gameswift.io/swagger">GameSwift ID</a>.
	/// </summary>
	public partial class GameSwiftSdkId
	{
		/// <summary>
		/// Send 1 request to get information login using launcher. Reads access_token as cmd arg to call 
		/// </summary>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void ReadUserInfoFromLauncher (
			Action<OauthUserInfoResponse> handleSuccess, Action<BaseSdkFailResponse> handleFailure)
		{
			var isTokenAvailable = GameSwiftSdkCore.TryReadCmdArgument("-access_token", out var accessToken);

			if (isTokenAvailable)
			{
				Instance.OauthAccessToken = accessToken;
				GetOauthUserInformation(accessToken, handleSuccess, handleFailure);
			}
			else
			{
				var errorMessage = "GameSwift ID cannot read cmdline arguments. Access token was not provided.";
				handleFailure.Invoke(new SdkFailResponse(errorMessage));
			}
		}

		/// <summary>
		/// Send 2 requests to get user data from launcher. Reads 3 cmd arguments and calls /api/oauth/token
		/// and then (after caching access and refresh token) /api/oauth/me.
		/// </summary>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void ReadTokenAndUserInfoFromLauncher (
			Action<OauthUserInfoResponse> handleSuccess, Action<BaseSdkFailResponse> handleFailure)
		{
			var isCodeAvailable = GameSwiftSdkCore.TryReadCmdArgument("-authorization_code", out var authorizationCode);
			var isClientIdAvailable = GameSwiftSdkCore.TryReadCmdArgument("-client_id", out var clientId);
			var isRedirectUriAvailable = GameSwiftSdkCore.TryReadCmdArgument("-redirect_uri", out var redirectUri);

			if (isCodeAvailable && isClientIdAvailable && isRedirectUriAvailable)
			{
				RetrieveOauthToken(authorizationCode, clientId, redirectUri, HandleOauthTokenRetrieved, handleFailure);

				void HandleOauthTokenRetrieved (TokenResponse response)
				{
					Instance.OauthAccessToken = response.access_token;
					Instance.RefreshToken = response.refresh_token;
					GetOauthUserInformation(response.access_token, handleSuccess, handleFailure);
				}
			}
			else
			{
				var errorMessage = "GameSwift ID cannot read cmdline arguments. " +
				                   $"Authorization code: {isCodeAvailable}. " +
				                   $"Client id: {isClientIdAvailable}. " +
				                   $"Redirect uri: {isRedirectUriAvailable}.";

				handleFailure.Invoke(new SdkFailResponse(errorMessage));
			}
		}

		/// <summary>
		/// Send request <a href="https://id.gameswift.io/swagger/#/default/HealthController_checkHealth">GET /api/health</a> to GameSwift ID.
		/// </summary>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void GetApiHealth (
			Action<HealthResponse> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure = null)
		{
			var apiUri = $"{API_ADDRESS}/health";
			var request = new RequestData(apiUri);
			GameSwiftSdkCore.SendGetRequest(request, handleSuccess, handleFailure);
		}

		/// <summary>
		/// Send request <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_getMe">GET /api/oauth/me</a> to GameSwift ID.
		/// </summary>
		/// <param name="accessToken">Access Token</param>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void GetOauthUserInformation (
			string accessToken, Action<OauthUserInfoResponse> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure)
		{
			var apiUri = $"{API_ADDRESS}/oauth/me";
			var request = new RequestData(apiUri);
			request.SetupHeaders(CustomHeader.AccessToken, accessToken);

			GameSwiftSdkCore.SendGetRequest(request, handleSuccess, handleFailure);
		}

		/// <summary>
		/// Send request <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_getMe">GET /api/oauth/me</a> to GameSwift ID.
		/// </summary>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void GetOauthUserInformation (
			Action<OauthUserInfoResponse> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure)
		{
			if (string.IsNullOrEmpty(Instance.OauthAccessToken))
			{
				var failMessage = "No authorization code cached from launcher, cannot retrieve user info";
				handleFailure.Invoke(new FailResponse(failMessage));
			}
			else
			{
				var apiUri = $"{API_ADDRESS}/oauth/me";
				var request = new RequestData(apiUri);
				request.SetupHeaders(CustomHeader.AccessToken, Instance.OauthAccessToken);

				GameSwiftSdkCore.SendGetRequest(request, handleSuccess, handleFailure);
			}
		}

		/// <summary>
		/// Send request <a href="https://id.gameswift.io/swagger/#/default/AuthController_getMe">GET /api/auth/me</a> to GameSwift ID.
		/// </summary>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void GetAuthUserInformation (
			Action<AuthUserInfoResponse> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure)
		{
			var apiUri = $"{API_ADDRESS}/auth/me";
			var request = new RequestData(apiUri);
			request.SetupHeaders(CustomHeader.AccessToken, Instance.AccessToken);

			GameSwiftSdkCore.SendGetRequest(request, handleSuccess, handleFailure);
		}

		/// <summary>
		/// Send request <a href="https://id.gameswift.io/swagger/#/default/WalletController_getWallets">GET /api/wallet/{userId}</a> to GameSwift ID.
		/// </summary>
		/// <param name="userId">UserId for wallet to get</param>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void GetWallet (
			string userId, Action<WalletResponse[]> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure)
		{
			var apiUri = $"{API_ADDRESS}/wallet/{userId}";
			var request = new RequestData(apiUri);
			request.SetupHeaders(CustomHeader.AccessToken, Instance.AccessToken);

			GameSwiftSdkCore.SendGetRequest(request, handleSuccess, handleFailure);
		}
	}
}
