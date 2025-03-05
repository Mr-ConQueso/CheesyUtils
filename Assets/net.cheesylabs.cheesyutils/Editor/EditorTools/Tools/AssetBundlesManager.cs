using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CheesyUtils.EditorTools
{
    public class AssetBundlesManager
    {
        [MenuItem("Assets/Create Asset Bundles")]
        public static void CreateAssetBundles()
        {
            string assetBundlePath = Path.Combine(Application.dataPath, "AssetBundles");
            try
            {
                BuildPipeline.BuildAssetBundles(assetBundlePath, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }
    }
}