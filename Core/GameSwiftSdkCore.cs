using System;
using System.Collections;
using System.Collections.Generic;
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
		/// Sends a GET request.
		/// </summary>
		/// <param name="data">Request data sent to the API</param>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		/// <typeparam name="T">Expected response class type.</typeparam>
		public static void SendGetRequest<T> (RequestData data, Action<T> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure)
		{
			SendRequest(handleFailure, GetCoroutine(data, handleSuccess, handleFailure));
		}

		private static void SendRequest (Action<BaseSdkFailResponse> handleFailure, IEnumerator requestCoroutine)
		{
			if (Instance != null)
			{
				try
				{
					Instance.StartCoroutine(requestCoroutine);
				}
				catch (Exception e)
				{
					handleFailure.Invoke(new SdkFailResponse($"Cannot start coroutine\n{e.Message}\n{e.StackTrace}"));
				}
			}
		}

		/// <summary>
		/// Sends a POST request.
		/// </summary>
		/// <param name="data">Request data sent to the API</param>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		/// <typeparam name="T">Expected response class type</typeparam>
		public static void SendPostRequest<T> (RequestData data, Action<T> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure)
		{
			SendRequest(handleFailure, PostCoroutine(data, handleSuccess, handleFailure));
		}

		private static IEnumerator GetCoroutine<T> (RequestData data, Action<T> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure)
		{
			using var request = UnityWebRequest.Get(data.uri);
			AddHeaders(request, data);

			yield return request.SendWebRequest();
			HandleRequestResult(handleSuccess, handleFailure, request);
		}

		private static IEnumerator PostCoroutine<T> (RequestData data, Action<T> handleSuccess,
			Action<BaseSdkFailResponse> handleFailure)
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
		/// Handles and deserializes Unity request's response class.
		/// </summary>
		/// <param name="handleSuccess">Success handler</param>
		/// <param name="handleFailure">Failure handler</param>
		/// <param name="request">Unity request that was sent</param>
		private static void HandleRequestResult<T> (Action<T> handleSuccess, Action<BaseSdkFailResponse> handleFailure,
			UnityWebRequest request)
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
			if (string.IsNullOrWhiteSpace(responseText))
			{
				handleFailure.Invoke(new SdkFailResponse("Cannot decode empty response text."));
			}
			else
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
		}

		/// <summary>
		/// Tries to get a cmdline argument value - returns true and sets output if found, otherwise returns false.
		/// </summary>
		/// <param name="argumentName">Name of argument to search for</param>
		/// <param name="argumentValue">Get redirect uri from cmd args</param>
		/// <returns>Value indicating whether reading was successful.</returns>
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

		/// <summary>
		/// Parses query string.
		/// </summary>
		/// <param name="body">Body with parameters to be parsed</param>
		/// <returns>Parsed query string to use in the request URL.</returns>
		public static string GetParsedQueryString (Dictionary<string, string> body)
		{
			return System.Text.Encoding.UTF8.GetString(UnityWebRequest.SerializeSimpleForm(body));
		}
	}
}