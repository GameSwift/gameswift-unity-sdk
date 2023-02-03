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
		/// Send request to get information login using launcher.
		/// </summary>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void GetUserInformationFromLauncher (
			Action<OauthUserInfoResponse> handleSuccess, Action<BaseSdkFailResponse> handleFailure)
		{
			var isCodeAvailable = GameSwiftSdkCore.TryReadCmdArgument("-authorization_code", out var authorizationCode);
			var isClientIdAvailable = GameSwiftSdkCore.TryReadCmdArgument("-client_id", out var clientId);
			var isRedirectUriAvailable = GameSwiftSdkCore.TryReadCmdArgument("-redirect_uri", out var redirectUri);

			if (isCodeAvailable && isClientIdAvailable && isRedirectUriAvailable)
			{
				RetrieveOauthToken(authorizationCode, clientId, redirectUri, HandleTokenRetrieved, handleFailure);

				void HandleTokenRetrieved (TokenResponse response)
				{
					Instance._oauthAccessToken = response.access_token;
					Instance._refreshToken = response.refresh_token;
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
		/// <param name="body">Access Token</param>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void GetOauthUserInformation (
			string body, Action<OauthUserInfoResponse> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure)
		{
			var apiUri = $"{API_ADDRESS}/oauth/me";
			var request = new RequestData(apiUri);
			request.SetupHeaders(CustomHeader.AccessToken, body);

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
			if (string.IsNullOrEmpty(Instance._oauthAccessToken))
			{
				var failMessage = "No authorization code cached from launcher, cannot retrieve user info";
				handleFailure.Invoke(new FailResponse(failMessage));
			}
			else
			{
				var apiUri = $"{API_ADDRESS}/oauth/me";
				var request = new RequestData(apiUri);
				request.SetupHeaders(CustomHeader.AccessToken, Instance._oauthAccessToken);

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
			request.SetupHeaders(CustomHeader.AccessToken, Instance._accessToken);

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
			request.SetupHeaders(CustomHeader.AccessToken, Instance._accessToken);

			GameSwiftSdkCore.SendGetRequest(request, handleSuccess, handleFailure);
		}
	}
}
