using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace GameSwiftSDK.Id.LoginBlock
{
	/// <summary>
	/// Multiple account login attempts blocker configuration file.
	/// </summary>
	public class MultipleLoginsBlockerData : ScriptableObject
	{
		private const string BASE_RESOURCES_PATH = "Resources";
		private const string DIRECTORY_IN_RESOURCES = "GameSwiftSDK";
		private const string CONFIG_FILE_NAME = "MultipleLoginsBlockerData";

		/// <summary>
		/// Turns on/off multiple account login attempts blocker.
		/// </summary>
		[field: SerializeField]
		public bool CheckMultipleLoginAttempts { get; private set; } = true;

		/// <summary>
		/// Time rate, measured in seconds, at which "GET /api/{idVersion}/oauth/me" call will be made to maintain multiple account login attempts lock.
		/// </summary>
		[field: SerializeField, Range(5, 500)]
		public int BlockerHeartbeatRate { get; private set; } = 60;

		private static MultipleLoginsBlockerData _instance;

		internal static MultipleLoginsBlockerData Instance
		{
			get
			{
				if (_instance == null)
				{
					var assetPathInResources = Path.Combine(DIRECTORY_IN_RESOURCES, CONFIG_FILE_NAME);
					var loadedConfigFile = Resources.Load<MultipleLoginsBlockerData>(assetPathInResources);
					if (loadedConfigFile != null)
					{
						_instance = loadedConfigFile;
					}
					else
					{
#if UNITY_EDITOR
						_instance = CreateInstance<MultipleLoginsBlockerData>();

						var targetPathInAssets = Path.Combine(BASE_RESOURCES_PATH, DIRECTORY_IN_RESOURCES);
						Directory.CreateDirectory(Path.Combine(Application.dataPath, targetPathInAssets));
						var assetPath = Path.Combine("Assets", targetPathInAssets);

						Debug.Log($"Creating missing {CONFIG_FILE_NAME}.asset config file at {assetPath}");
						AssetDatabase.CreateAsset(_instance, Path.Combine(assetPath, $"{CONFIG_FILE_NAME}.asset"));
						AssetDatabase.SaveAssets();
						Debug.Log($"{CONFIG_FILE_NAME}.asset config file created successfully.");
#endif
					}
				}

				return _instance;
			}
		}

#if UNITY_EDITOR
		[InitializeOnLoadMethod]
		private static void InitializeMultipleLoginsBlockerData()
		{
			if (Instance == null)
			{
				Debug.LogWarning($"GameSwift SDK failed to create MultipleLoginsBlockerData.asset config file.");
			}
		}
#endif
	}
}
