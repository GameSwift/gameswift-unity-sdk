﻿using System;
using System.Collections.Generic;
using GameSwiftSDK.Core;
using GameSwiftSDK.Id.Responses;
using Newtonsoft.Json;

namespace GameSwiftSDK.Id
{
	/// <summary>
	/// Setup and send requests to GameSwift ID.
	/// </summary>
	public partial class GameSwiftSdkId
	{
		/// <summary>
		/// Send request to get information login using launcher.
		/// </summary>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void RefreshTokenFromLauncher (
			Action<TokenResponse> handleSuccess, Action<BaseSdkFailResponse> handleFailure)
		{
			var isClientIdAvailable = GameSwiftSdkCore.TryReadCmdArgument("-client_id", out var clientId);
			var isRedirectUriAvailable = GameSwiftSdkCore.TryReadCmdArgument("-redirect_uri", out var redirectUri);

			if (isClientIdAvailable && isRedirectUriAvailable)
			{
				RefreshOauthToken(clientId, redirectUri, Instance.RefreshToken,
				                  HandleTokenRetrieved, handleFailure);

				void HandleTokenRetrieved (TokenResponse response)
				{
					Instance.RefreshToken = response.refresh_token;
					Instance.OauthAccessToken = response.access_token;
					handleSuccess.Invoke(response);
				}
			}
			else
			{
				var errorMessage = "GameSwift ID cannot read cmdline arguments. " +
				                   $"Client id: {isClientIdAvailable}. " +
				                   $"Redirect uri: {isRedirectUriAvailable}.";

				handleFailure.Invoke(new SdkFailResponse(errorMessage));
			}
		}

		/// <summary>
		/// Send command <a href="https://id.gameswift.io/swagger/#/default/AuthController_login">POST /api/auth/login</a> to GameSwift ID.
		/// </summary>
		/// <param name="emailOrNickname">Email or nickname to login</param>
		/// <param name="password">Login's password</param>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void Login (
			string emailOrNickname, string password, Action<LoginResponse> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure)
		{
			Dictionary<string, string> credentials = new Dictionary<string, string>()
			                                         {
				                                         { "emailOrNickname", emailOrNickname },
				                                         { "password", password }
			                                         };

			var apiUri = $"{API_ADDRESS}/auth/login";

			var requestBody = JsonConvert.SerializeObject(credentials, Formatting.Indented);
			var request = new RequestData(apiUri, requestBody);
			request.SetupHeaders(CustomHeader.None, "");

			GameSwiftSdkCore.SendPostRequest<LoginResponse>(request, HandleSuccessInternal, handleFailure);

			void HandleSuccessInternal (LoginResponse loginResponse)
			{
				Instance.AccessToken = loginResponse.accessToken;
				Instance._accessTokenRetrieved = true;
				Instance.RefreshToken = loginResponse.refreshToken;
				handleSuccess.Invoke(loginResponse);
			}
		}

		/// <summary>
		/// Send request <a href="https://id.gameswift.io/swagger/#/default/AuthController_logout">POST /api/auth/logout</a> to GameSwift ID.
		/// </summary>
		/// <param name="body">Request body to send</param>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void Logout (
			string body, Action<MessageResponse> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure)
		{
			if (Instance._accessTokenRetrieved)
			{
				var apiUri = $"{API_ADDRESS}/auth/logout";
				var request = new RequestData(apiUri, body);
				request.SetupHeaders(CustomHeader.AccessToken, Instance.AccessToken);

				GameSwiftSdkCore.SendPostRequest(request, handleSuccess, handleFailure);
				Instance._accessTokenRetrieved = false;
			}
			else
			{
				FailResponse response =
					new FailResponse("Cannot use logout endpoint without retrieving access token from login!");

				handleFailure.Invoke(response);
			}
		}

		/// <summary>
		/// Send request <a href="https://id.gameswift.io/swagger/#/default/AuthController_postRefresh">POST /api/auth/refresh</a> to GameSwift ID.
		/// </summary>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void RefreshApiToken (
			Action<MessageResponse> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure)
		{
			var apiUri = $"{API_ADDRESS}/auth/refresh";
			var request = new RequestData(apiUri);
			request.SetupHeaders(CustomHeader.RefreshToken, Instance.RefreshToken);

			GameSwiftSdkCore.SendPostRequest(request, handleSuccess, handleFailure);
		}

		/// <summary>
		/// Send request <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postToken">POST /api/oauth/token</a> to GameSwift ID.
		/// The generated authorization code can only used once
		/// </summary>
		/// <param name="authorizationCode">Authorization code which get response request "GET /api/oauth/authorize"</param>
		/// <param name="clientId">Client id which get response request "GET /api/oauth/client" </param>
		/// <param name="redirectUri">Redirect uri which get response request "Get /api/oauth/client"</param>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		public static void RetrieveOauthToken (
			string authorizationCode, string clientId, string redirectUri, Action<TokenResponse> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure)
		{
			var body = System.Web.HttpUtility.ParseQueryString(string.Empty);
			body.Add("client_id", clientId);
			body.Add("client_secret", "");
			body.Add("grant_type", "authorization_code");
			body.Add("code", authorizationCode);
			body.Add("redirect_uri", redirectUri);

			var apiUri = $"{API_ADDRESS}/oauth/token";
			var request = new RequestData(apiUri, body.ToString());
			request.SetupHeaders(CustomHeader.WwwContentType, "");
			GameSwiftSdkCore.SendPostRequest(request, handleSuccess, handleFailure);
		}

		/// <summary>
		/// Send request <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_postToken">POST /api/oauth/token</a> to GameSwift ID.
		/// </summary>
		/// <param name="clientId">Client id which get response request "GET /api/oauth/client" </param>
		/// <param name="redirectUri">Redirect uri which get response request "Get /api/oauth/client"</param>
		/// <param name="refreshToken">Previous refresh token that needs refreshing</param>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		private static void RefreshOauthToken (
			string clientId, string redirectUri, string refreshToken,
			Action<TokenResponse> handleSuccess, Action<BaseSdkFailResponse> handleFailure)
		{
			var body = System.Web.HttpUtility.ParseQueryString(string.Empty);
			body.Add("client_id", clientId);
			body.Add("grant_type", "refresh_token");
			body.Add("redirect_uri", redirectUri);
			body.Add("refresh_token", refreshToken);

			var apiUri = $"{API_ADDRESS}/oauth/token";
			var request = new RequestData(apiUri, body.ToString());
			request.SetupHeaders(CustomHeader.WwwContentType, "");
			GameSwiftSdkCore.SendPostRequest(request, handleSuccess, handleFailure);
		}
	}
}
