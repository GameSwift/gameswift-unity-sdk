using GameSwiftSDK.Id.LoginBlock;
using UnityEngine;

namespace GameSwiftSDK.Id
{
	/// <summary>
	/// Core of GameSwift SDK integration with ID. It attaches to already exiting GameSwiftSdkCore singleton.
	/// </summary>
	public partial class GameSwiftSdkId : MonoBehaviour
	{
		private const string ID_VERSION = "";
		private const string API_ADDRESS = "https://id.gameswift.io/api" + ID_VERSION;

		private static GameSwiftSdkId _instance;

		public static GameSwiftSdkId Instance
		{
			get
			{
				if (_instance == null)
				{
					Instantiate();
				}

				return _instance;
			}
		}

		/// <summary>
		/// Multiple Logins Blocker component.
		/// </summary>
		public MultipleLoginsBlocker MultipleLoginsBlocker { get; private set; }

		/// <summary>
		/// Stored API Command Line Access Token. Should be used only in a
		/// <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_getAuthorize">GET /api/{idVersion}/oauth/authorize</a> request.
		/// </summary>
		public string CmdAccessToken { get; private set; }

		/// <summary>
		/// Stored API Access Token.
		/// </summary>
		public string AccessToken { get; private set; }

		/// <summary>
		/// Stored API Refresh Token.
		/// </summary>
		public string RefreshToken { get; private set; }

		private bool _accessTokenRetrieved;

		private static void Instantiate ()
		{
			_instance = Core.GameSwiftSdkCore.Instance.gameObject.AddComponent<GameSwiftSdkId>();

			if (MultipleLoginsBlockerData.Instance.CheckMultipleLoginAttempts)
			{
				Instance.MultipleLoginsBlocker = Instance.gameObject.AddComponent<MultipleLoginsBlocker>();
			}
		}
	}
}