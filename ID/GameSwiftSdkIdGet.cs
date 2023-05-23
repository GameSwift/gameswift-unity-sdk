using System;
using System.Collections.Generic;
using GameSwiftSDK.Core;
using GameSwiftSDK.Id.Responses;

namespace GameSwiftSDK.Id
{
	/// <summary>
	/// Setup and send requests to <a href="https://id.gameswift.io/swagger">GameSwift ID</a>.
	/// </summary>
	public partial class GameSwiftSdkId
	{
		/// <summary>
		/// Reads user's access_token cmd argument from the launcher. Stores CmdAccessToken in the GameSwiftSdkId.Instance to be used
		/// only in a <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_getAuthorize">GET /api/{idVersion}/oauth/authorize</a> request.
		/// Then it sends a <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_getMe">GET /api/{idVersion}/oauth/me</a> request.
		/// </summary>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void ReadUserInfoFromLauncher (Action<OauthUserInfoResponse> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure)
		{
			var isTokenAvailable = GameSwiftSdkCore.TryReadCmdArgument("-access_token", out var cmdAccessToken);

			if (isTokenAvailable)
			{
				GetOauthUserInformation(cmdAccessToken, HandleOAuthMeSuccess, handleFailure);

				void HandleOAuthMeSuccess (OauthUserInfoResponse oauthMeResponse)
				{
					Instance.CmdAccessToken = cmdAccessToken;
					handleSuccess.Invoke(oauthMeResponse);
				}
			}
			else
			{
				var errorMessage = "GameSwift ID cannot read cmdline arguments. Access token was not provided.";
				handleFailure.Invoke(new SdkFailResponse(errorMessage));
			}
		}

		/// <summary>
		/// Generates a one time use authorization code which is needed in a <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postToken">POST /api/{idVersion}/oauth/token</a> request.
		/// Sends a <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_getAuthorize">GET /api/{idVersion}/oauth/authorize</a> request.
		/// </summary>
		/// <param name="accessToken">Access Token received from <a href="https://id.gameswift.io/swagger/#/default/AuthController_login">POST /api/{idVersion}/auth/login</a> request</param>
		/// <param name="clientId">OAuth client ID received from <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postClient">POST /api/{idVersion}/oauth/client</a> request</param>
		/// <param name="redirectUri">Redirect uri received from <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postClient">POST /api/{idVersion}/oauth/client</a> request</param>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void GetAuthorizationCode (string accessToken, string clientId, string redirectUri,
			Action<AuthorizeResponse> handleSuccess, Action<BaseSdkFailResponse> handleFailure)
		{
			Dictionary<string, string> body = new Dictionary<string, string>()
			{
				{ "response_type", "code" },
				{ "client_id", clientId },
				{ "redirect_uri", redirectUri },
				{ "code_challenge_method", "S256" },
				{ "state", "empty" }
			};
			var queryString = GameSwiftSdkCore.GetParsedQueryString(body);

			var apiUri = $"{API_ADDRESS}/oauth/authorize?{queryString}";
			var request = new RequestData(apiUri);
			request.SetupHeaders(CustomHeader.AccessToken, accessToken);

			GameSwiftSdkCore.SendGetRequest(request, handleSuccess, handleFailure);
		}

		/// <summary>
		/// Receives logged and authorized user's information. Also maintains multiple account login attempts lock.
		/// Sends a <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_getMe">GET /api/{idVersion}/oauth/me</a> request.
		/// </summary>
		/// <param name="accessToken">Access Token received from <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postToken">POST /api/{idVersion}/oauth/token</a> request</param>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void GetOauthUserInformation (string accessToken, Action<OauthUserInfoResponse> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure)
		{
			var apiUri = $"{API_ADDRESS}/oauth/me";
			var request = new RequestData(apiUri);
			request.SetupHeaders(CustomHeader.AccessToken, accessToken);

			GameSwiftSdkCore.SendGetRequest(request, handleSuccess, handleFailure);
		}

		/// <summary>
		/// Retrieves logged and authorized user's information. Also maintains multiple account login attempts lock.
		/// Uses stored Access Token in it's request call.
		/// Sends a <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_getMe">GET /api/{idVersion}/oauth/me</a> request.
		/// </summary>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void GetOauthUserInformation (Action<OauthUserInfoResponse> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure)
		{
			if (string.IsNullOrEmpty(Instance.AccessToken))
			{
				var failMessage = "No authorization code cached from launcher, cannot retrieve user info";
				handleFailure.Invoke(new SdkFailResponse(failMessage));
			}
			else
			{
				GetOauthUserInformation(Instance.AccessToken, handleSuccess, handleFailure);
			}
		}

		/// <summary>
		/// Retrieves API status. Sends a <a href="https://id.gameswift.io/swagger/#/default/HealthController_checkHealth">GET /api/{idVersion}/health</a> request.
		/// </summary>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void GetApiHealth (Action<HealthResponse> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure = null)
		{
			var apiUri = $"{API_ADDRESS}/health";
			var request = new RequestData(apiUri);
			GameSwiftSdkCore.SendGetRequest(request, handleSuccess, handleFailure);
		}

		/// <summary>
		/// Retrieves user's wallets.
		/// Sends a <a href="https://id.gameswift.io/swagger/#/default/WalletController_getWallets">GET /api/{idVersion}/wallet</a> request.
		/// </summary>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void GetWallet (Action<WalletResponse[]> handleSuccess, Action<BaseSdkFailResponse> handleFailure)
		{
			var apiUri = $"{API_ADDRESS}/wallet";
			var request = new RequestData(apiUri);
			request.SetupHeaders(CustomHeader.AccessToken, Instance.AccessToken);

			GameSwiftSdkCore.SendGetRequest(request, handleSuccess, handleFailure);
		}

		/// <summary>
		/// Retrieves user's specific wallet balance.
		/// Sends a <a href="https://id.gameswift.io/swagger/#/default/WalletController_getWalletBalance">GET /api/{idVersion}/wallet/{walletId}/balance</a> request.
		/// </summary>
		/// <param name="walletId">Wallet ID</param>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void GetWalletBalance (string walletId, Action<WalletBalanceResponse> handleSuccess, Action<BaseSdkFailResponse> handleFailure)
		{
			var apiUri = $"{API_ADDRESS}/wallet/{walletId}/balance";
			var request = new RequestData(apiUri);
			request.SetupHeaders(CustomHeader.AccessToken, Instance.AccessToken);

			GameSwiftSdkCore.SendGetRequest(request, handleSuccess, handleFailure);
		}
	}
}