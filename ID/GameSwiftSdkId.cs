using UnityEngine;

namespace GameSwiftSDK.Id
{
	/// <summary>
	/// Core of GameSwift SDK integration with ID. It attaches to already exiting GameSwiftSdkCore singleton. 
	/// </summary>
	public partial class GameSwiftSdkId : MonoBehaviour
	{
		private const string API_ADDRESS = "https://id.gameswift.io/api";

		private static GameSwiftSdkId _instance;

		private string _loginAccessToken;
		private string _refreshToken;
		private bool _accessTokenRetrieved;

		private static GameSwiftSdkId Instance
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

		private static void Instantiate ()
		{
			_instance = Core.GameSwiftSdkCore.Instance.gameObject.AddComponent<GameSwiftSdkId>();
		}
	}
}
