using System.Collections.Generic;
using System.IO;
using CheesyUtils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Debug = CheesyUtils.Debug;

namespace CheesyUtils.PackagedAssets
{
    public class AddressablesManager : Singleton<AddressablesManager>
    {
        private const string MeshFolder = "Assets/Meshes/PawnTown/Items/";
        private const string IconsFolder = "Assets/Textures/PawnTown/UI/Items/";
        
        [SerializeField] private List<AssetReference> _addressablesToLoad = new List<AssetReference>();
        
        public Dictionary<string, object> LoadedAddressables { get; private set; } = new Dictionary<string, object>();
        private int _totalAssetsToLoad;
        private int _loadedAssetsCount;

        private void Start()
        {
            LoadAllAddressables();
        }
        
        private void OnApplicationQuit()
        {
            UnloadAllAddressables();
        }

        public void UnloadAllAddressables()
        {
            Addressables.CleanBundleCache();
            LoadedAddressables.Clear();
        }

        public void LoadAllAddressables()
        {
            // LoadAddressables();
        }

        private void LoadAddressables(IEnumerable<string> assetKeys)
        {
            if (LoadedAddressables.Count >= 0)
            {
                UnloadAllAddressables();
            }
            var keysList = new List<string>(assetKeys);
            _totalAssetsToLoad = keysList.Count;
            _loadedAssetsCount = 0;
            LoadedAddressables.Clear();

            if (_totalAssetsToLoad == 0)
            {
                Debug.LogWarning("No asset keys provided for loading", DLogType.Content);
                return;
            }

            foreach (string key in keysList)
            {
                string currentKey = key;
                Addressables.LoadAssetAsync<object>(currentKey).Completed += handle =>
                {
                    HandleAssetLoaded(currentKey, handle);
                    CheckAllAssetsLoaded();
                };
            }
        }

        private void HandleAssetLoaded(string originalKey, AsyncOperationHandle<object> handle)
        {
            _loadedAssetsCount++;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                string fileName = Path.GetFileNameWithoutExtension(originalKey);
                if (!LoadedAddressables.ContainsKey(fileName))
                {
                    LoadedAddressables.Add(fileName, handle.Result);
                    Debug.Log($"Successfully loaded: {fileName}");
                }
                else
                {
                    Debug.LogWarning($"Duplicate asset filename detected: {fileName}. Skipping.");
                }
            }
            else
            {
                Debug.LogError($"Failed to load asset at path: {originalKey}");
            }
        }

        private void CheckAllAssetsLoaded()
        {
            if (_loadedAssetsCount >= _totalAssetsToLoad)
            {
                if (LoadedAddressables.Count > 0)
                {
                    Debug.Log($"Successfully loaded {LoadedAddressables.Count}/{_totalAssetsToLoad} assets");
                }
                else
                {
                    Debug.LogError("Failed to load any assets");
                }
            }
        }
    }
}