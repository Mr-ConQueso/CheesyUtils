using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace CheesyUtils.SceneManagement
{
    public class SceneBootstrapper : MonoBehaviour
    {
        private const string Key = "Assets/Scenes/SplashScreen.unity";
        private static AsyncOperationHandle<SceneInstance> _loadHandle;
        
        private IEnumerator Start()
        {
            _loadHandle = Addressables.LoadSceneAsync(Key, LoadSceneMode.Single, false);
            yield return _loadHandle;

            if (_loadHandle.Status != AsyncOperationStatus.Succeeded) yield break;
            SceneInstance scene = _loadHandle.Result;
            scene.ActivateAsync();
        }

        private void OnDestroy()
        {
            Addressables.UnloadSceneAsync(_loadHandle);
        }
    }
}