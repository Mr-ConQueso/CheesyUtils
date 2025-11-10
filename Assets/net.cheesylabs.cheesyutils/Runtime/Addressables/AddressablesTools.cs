using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CheesyUtils.PackagedAssets
{
    public static class AddressablesTools
    {
        public static string ItemIconsBundleName { get; private set; } = "pt_items";

        private static AssetBundle _assetBundle;

        public static object GetAddressableObject(string objectName)
        {
            return Addressables.LoadAssetAsync<Sprite>($"Assets/Textures/PawnTown/UI/Items/{objectName}.png").Result;
        }
        
        public static object GetObjectsFromAssetBundle(Type type, string objectName)
        {
            var allAssetBundles = AssetBundle.GetAllLoadedAssetBundles();
            
            Debug.Log("All asset bundles:");
            foreach (AssetBundle assetBundle in allAssetBundles)
            {
                Debug.Log(assetBundle.name);
                if (assetBundle.name == ItemIconsBundleName)
                {
                    return assetBundle.LoadAllAssets(type).FirstOrDefault(x => x.name == objectName);
                }
            }
            return null;
            // string assetBundlePath = Application.streamingAssetsPath + "/AssetBundles/Windows/";
            // AssetBundle theBundle = AssetBundle.LoadFromFile(assetBundlePath + "objectsbundle1");
            // GameObject[] allObjectsBundle1Prefabs = theBundle.LoadAllAssets<GameObject>();
        }
        
        public static object LoadAssetBundle(Type type, string objectName)
        {
            if (_assetBundle) return _assetBundle.LoadAllAssets(type).FirstOrDefault(x => x.name == objectName);
            
            return null;
            
            /*
            AssetBundle assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.dataPath, "AssetBundles", ItemIconsBundleName));
            if (!assetBundle)
            {
                Debug.Log("Failed to load AssetBundle!");
                return null;
            }
            Debug.Log("Loaded AssetBundle: " + assetBundle.name);

            // var prefab = myLoadedAssetBundle.LoadAsset<GameObject>("MyObject");
            //
            // assetBundle.Unload(false);
            return assetBundle.LoadAllAssets(type).FirstOrDefault(x => x.name == objectName);
            */
        }
    }
}