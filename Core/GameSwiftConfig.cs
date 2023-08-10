using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GameSwiftSDK.Core
{
    /// <summary>
    /// Multiple account login attempts blocker configuration file.
    /// </summary>
    public class GameSwiftConfig : ScriptableObject
    {
        private const string DIRECTORY_IN_RESOURCES = "GameSwiftSDK";
        private const string CONFIG_FILE_NAME = "GameSwiftConfig";

        /// <summary>
        /// Mandatory API authentication secret, distributed by GameSwift. Is is unique for every game using the SDK.
        /// </summary>
        [field: SerializeField]
        private string ClientAuthenticationSecret { get; set; }

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

        private static GameSwiftConfig _instance;

        public string GetClientAuthenticationSecret()
        {
            if (string.IsNullOrEmpty(ClientAuthenticationSecret))
            {
                Debug.LogError($"{nameof(ClientAuthenticationSecret)} is not filled! All sdk calls will fail!");
            }

            return ClientAuthenticationSecret;
        }

        public static GameSwiftConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    var pathInResources = Path.Combine(DIRECTORY_IN_RESOURCES, CONFIG_FILE_NAME);
                    _instance = Resources.Load<GameSwiftConfig>(pathInResources);
                }

                return _instance;
            }
        }

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        private static void AttachAssetCheckToEditorUpdate()
        {
            EditorApplication.update += InitializeMultipleLoginsBlockerData;
        }

        private static void InitializeMultipleLoginsBlockerData()
        {
            var isRefreshing = EditorApplication.isCompiling || EditorApplication.isUpdating;
            if (isRefreshing == false)
            {
                
                EditorApplication.update -= InitializeMultipleLoginsBlockerData;
                if (Instance == null)
                {
                    var createdConfig = CreateInstance<GameSwiftConfig>();

                    var relativeResourcesPath = Path.Combine("Resources", DIRECTORY_IN_RESOURCES);
                    Directory.CreateDirectory(Path.Combine(Application.dataPath, relativeResourcesPath));
                    
                    var resourcesPath = Path.Combine("Assets", relativeResourcesPath);
                    var createdConfigPath = Path.Combine(resourcesPath, $"{CONFIG_FILE_NAME}.asset");
                    AssetDatabase.CreateAsset(createdConfig, createdConfigPath);
                    
                    var oldAsset = Path.Combine(resourcesPath, "MultipleLoginsBlockerData.asset");
                    AssetDatabase.DeleteAsset(oldAsset);
                    
                    AssetDatabase.SaveAssets();
                    Debug.Log($"Config file created successfully at {createdConfigPath}.");
                }
            }
        }
#endif
    }
}