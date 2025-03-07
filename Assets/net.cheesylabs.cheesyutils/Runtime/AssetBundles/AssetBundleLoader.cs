using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace CheesyUtils
{
    public class AssetBundleLoader : MonoBehaviour
    {
        public static string AssetBundleURL = "http://localhost/bundle";
        public static string BundleName = "bundle";

        private void Start()
        {
            StartCoroutine(DownloadAndCache(AssetBundleURL));
        }

        public IEnumerator DownloadAndCache(string bundleURL, string assetName = "")
        {
            while (!Caching.ready)
            {
                yield return null;
            }

            // Clear cache for previous versions of the asset bundle
            Caching.ClearOtherCachedVersions(BundleName, Hash128.Parse("0"));

            UnityWebRequest www = UnityWebRequest.Get(bundleURL + ".manifest?r=" + (Random.value * 9999999));
            Debug.Log("Loading manifest: " + bundleURL + ".manifest");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                Debug.LogError("www error: " + www.error);
                www.Dispose();
                www = null;
                yield break;
            }

            Hash128 hashString;

            if (www.downloadHandler.text.Contains("ManifestFileVersion"))
            {
                var hashRow = www.downloadHandler.text.ToString().Split("\n".ToCharArray())[5];
                hashString = Hash128.Parse(hashRow.Split(':')[1].Trim());

                if (hashString.isValid)
                {
                    if (Caching.IsVersionCached(bundleURL, hashString))
                    {
                        Debug.Log("Bundle with this hash is already cached!");
                    }
                    else
                    {
                        Debug.Log("No cached version found for this hash..");
                    }
                }
                else
                {
                    Debug.LogError("Invalid hash: " + hashString);
                    yield break;
                }
            }
            else
            {
                Debug.LogError("Manifest doesn't contain string 'ManifestFileVersion': " + bundleURL + ".manifest");
                yield break;
            }

            www = UnityWebRequestAssetBundle.GetAssetBundle(bundleURL + "?r=" + (Random.value * 9999999), hashString, 0);

            yield return www.SendWebRequest();

            if (www.error != null)
            {
                Debug.LogError("www error: " + www.error);
                www.Dispose();
                www = null;
                yield break;
            }

            AssetBundle bundle = ((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle;
            GameObject bundlePrefab = null;

            if (assetName == "")
            {
                bundlePrefab = (GameObject)bundle.LoadAsset(bundle.GetAllAssetNames()[0]);
            }
            else
            {
                bundlePrefab = (GameObject)bundle.LoadAsset(assetName);
            }

            if (bundlePrefab != null)
            {
                Instantiate(bundlePrefab, Vector3.zero, Quaternion.identity);
            }

            www.Dispose();
            www = null;

            Resources.UnloadUnusedAssets();
            bundle.Unload(false);
            bundle = null;
        }
    }
}
