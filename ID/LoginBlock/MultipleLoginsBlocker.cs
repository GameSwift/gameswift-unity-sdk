using System;
using System.Collections;
using GameSwiftSDK.Core;
using GameSwiftSDK.Id.Responses;
using UnityEngine;

namespace GameSwiftSDK.Id.LoginBlock
{
	/// <summary>
	/// Component used to prevent multiple account login attempts. If multiple logins blocker is turned on this component
	/// will be added to the GameSwiftSdkId.Instance and will be sending <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_getMe">GET /api/{idVersion}/oauth/me</a>
	/// heartbeats to maintain the lock. You can setup MultipleLoginsBlocker in its config ScriptableObject file.
	/// </summary>
	public class MultipleLoginsBlocker : MonoBehaviour
	{
		/// <summary>
		/// Event triggered after successful <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_getMe">GET /api/{idVersion}/oauth/me</a>
		/// heartbeat that blocks multiple login attempts.
		/// </summary>
		public event Action OnBlockerHeartbeatSuccess = delegate {};

		/// <summary>
		/// Event triggered after failed <a href="https://id.gameswift.io/swagger/#/oauth/OauthController_getMe">GET /api/{idVersion}/oauth/me</a>
		/// heartbeat that blocks multiple login attempts.
		/// </summary>
		public event Action OnBlockerHeartbeatFail = delegate {};

		private void Start ()
		{
			StartCoroutine(CheckForMultipleLoginAttempts());
		}

		private IEnumerator CheckForMultipleLoginAttempts ()
		{
			for (;;)
			{
				yield return new WaitForSeconds(MultipleLoginsBlockerData.Instance.BlockerHeartbeatRate);
				SendBlockerHeartbeat();
			}
		}

		private void SendBlockerHeartbeat ()
		{
			GameSwiftSdkId.GetOauthUserInformation(HandleUserInformationSuccess, HandleUserInformationFailure);

			void HandleUserInformationSuccess (OauthUserInfoResponse response)
			{
				OnBlockerHeartbeatSuccess.Invoke();
			}

			void HandleUserInformationFailure (BaseSdkFailResponse sdkFailResponse)
			{
				OnBlockerHeartbeatFail.Invoke();
			}
		}
	}
}