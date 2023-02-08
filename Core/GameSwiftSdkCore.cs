using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace GameSwiftSDK.Core
{
	/// <summary>
	/// Core of GameSwift SDK, responsible for sending requests through a singletonized, DontDestroyOnLoad instance of itself.
	/// </summary>
	public class GameSwiftSdkCore : MonoBehaviour
	{
		private static GameSwiftSdkCore _instance;

		/// <summary>
		/// Singleton instance used to send requests via Unity coroutines.
		/// </summary>
		public static GameSwiftSdkCore Instance
		{
			get
			{
				if (_instance == null)
				{
					var go = new GameObject("GameSwiftSDKCore");
					_instance = go.AddComponent<GameSwiftSdkCore>();
					DontDestroyOnLoad(go);
				}

				return _instance;
			}
		}

		/// <summary>
		/// Send command "GET" request 
		/// </summary>
		/// <param name="data">The object send custom headers to Api</param>
		/// <param name="handleSuccess">The method if get success request from the API</param>
		/// <param name="handleFailure">The method if get failure request from the API</param>
		/// <typeparam name="T">The object set result sent request</typeparam>
		public static void SendGetRequest<T> (
			RequestData data, Action<T> handleSuccess, Action<BaseSdkFailResponse> handleFailure)
		{
			SendRequest(handleFailure, GetCoroutine(data, handleSuccess, handleFailure));
		}

		private static void SendRequest (Action<BaseSdkFailResponse> handleFailure, IEnumerator requestCoroutine)
		{
			if (Validate() == false)
			{
				return;
			}

			try
			{
				Instance.StartCoroutine(requestCoroutine);
			}
			catch (Exception e)
			{
				handleFailure.Invoke(new SdkFailResponse($"Cannot start coroutine\n{e.Message}\n{e.StackTrace}"));
			}
		}

		private static bool Validate ()
		{
			return Instance != null;
		}

		/// <summary>
		/// Send command "Post" request 
		/// </summary>
		/// <param name="data">The object send custom headers to Api</param>
		/// <param name="handleSuccess">The method if get success request from the API</param>
		/// <param name="handleFailure">The method if get failure request from the API</param>
		/// <typeparam name="T">The object set result sent request</typeparam>
		public static void SendPostRequest<T> (
			RequestData data, Action<T> handleSuccess, Action<BaseSdkFailResponse> handleFailure)
		{
			SendRequest(handleFailure, PostCoroutine(data, handleSuccess, handleFailure));
		}

		private static IEnumerator GetCoroutine<T> (
			RequestData data, Action<T> handleSuccess, Action<BaseSdkFailResponse> handleFailure)
		{
			using var request = UnityWebRequest.Get(data.uri);
			AddHeaders(request, data);

			yield return request.SendWebRequest();
			HandleRequestResult(handleSuccess, handleFailure, request);
		}

		private static IEnumerator PostCoroutine<T> (
			RequestData data, Action<T> handleSuccess, Action<BaseSdkFailResponse> handleFailure)
		{
			using var request = UnityWebRequest.Put(data.uri, data.body);
			request.method = "POST";
			AddHeaders(request, data);

			yield return request.SendWebRequest();
			HandleRequestResult(handleSuccess, handleFailure, request);
		}

		private static void AddHeaders (UnityWebRequest request, RequestData data)
		{
			foreach (var header in data.Headers)
			{
				request.SetRequestHeader(header.Key, header.Value);
			}
		}

		/// <summary>
		/// Handle Unity request sent and deserialize its return value
		/// </summary>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		/// <param name="request">Unity request that was sent</param>
		private static void HandleRequestResult<T> (
			Action<T> handleSuccess, Action<BaseSdkFailResponse> handleFailure, UnityWebRequest request)
		{
			var responseText = request.downloadHandler.text;
			
			if (request.result == UnityWebRequest.Result.Success)
			{
				var responseData = JsonConvert.DeserializeObject<T>(responseText);
				handleSuccess?.Invoke(responseData);
			}
			else
			{
				HandleRequestFail(handleFailure, responseText);
			}
		}

		private static void HandleRequestFail (Action<BaseSdkFailResponse> handleFailure, string responseText)
		{
			try
			{
				var responseData = JsonConvert.DeserializeObject<SdkFailArrayResponse>(responseText);
				handleFailure.Invoke(responseData);
			}
			catch
			{
				try
				{
					var responseData = JsonConvert.DeserializeObject<SdkFailResponse>(responseText);
					handleFailure.Invoke(responseData);
				}
				catch (Exception exception)
				{
					var errorMessage = $"Cannot decode {responseText}\n{exception.Message}\n{exception.StackTrace}";
					var failureResponse = new SdkFailResponse(errorMessage);
					handleFailure.Invoke(failureResponse);
				}
			}
		}

		/// <summary>
		/// Try to get a cmdline argument value - returns true and sets output if found, otherwise returns false.
		/// </summary>
		/// <param name="argumentName">Name of argument to search for</param>
		/// <param name="argumentValue">Get redirect uri from cmd args</param>
		/// <returns>Value indicating whether reading was successful</returns>
		public static bool TryReadCmdArgument (string argumentName, out string argumentValue)
		{
			argumentValue = ReadCmdArg(argumentName);
			return string.IsNullOrEmpty(argumentValue) == false;
		}

		private static string ReadCmdArg (string argName)
		{
			var args = Environment.GetCommandLineArgs();

			for (int i = 0; i < args.Length; i++)
			{
				if (args[i] == argName && args.Length > i + 1)
				{
					return args[i + 1];
				}
			}

			return string.Empty;
		}
	}
}
