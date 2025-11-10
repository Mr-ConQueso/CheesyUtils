using System.Collections;
using CheesyUtils.SavingLoading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace CheesyUtils.SceneManagement
{
    public class SceneSwapManager : Singleton<SceneSwapManager>
    {
        public static bool HasLoadedScene { get; private set; }
        private static AsyncOperationHandle<SceneInstance> _loadHandle;
        
        [SerializeField] private SceneTransitionManager _transitionManager;
        
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        /// <summary>
        /// Swaps the current scene with the one specified with a transition
        /// </summary>
        /// <param name="scene">The scene string name</param>
        public static void SwapScene(string scene)
        {
            SavedVariables.previousScene = SceneManager.GetActiveScene().name;
            Instance.StartCoroutine(TransitionTheSwapScene(scene));
        }
        
        /// <summary>
        /// Gets the scene from the addressable asset system and swaps to it with a transition
        /// </summary>
        /// <param name="key">The string key of the scene</param>
        public static void SwapAddressableScene(string key)
        {
            SavedVariables.previousScene = SceneManager.GetActiveScene().name;
            Instance.StartCoroutine(TransitionTheSwapAddressableScene(key));
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            StopAllCoroutines();
        }

        #region Transitions
        private static IEnumerator TransitionTheSwapScene(string scene)
        {
            // Start loading the scene asynchronously
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
            if (asyncLoad != null)
            {
                asyncLoad.allowSceneActivation = false;

                // Start the fade in animation
                Instance._transitionManager.StartAnimation();

                // Wait for fade in to complete
                while (Instance._transitionManager.isFadingIn)
                {
                    yield return null;
                }

                // Wait until the asynchronous scene load is almost complete
                while (!asyncLoad.isDone)
                {
                    if (asyncLoad.progress >= 0.9f && !Instance._transitionManager.isFadingIn)
                    {
                        // Allow the scene to be activated
                        asyncLoad.allowSceneActivation = true;
                        // SceneTransitionManager.Instance.EndAnimation();
                    }

                    yield return null;
                }
            }

            // Wait for fade out to complete
            while (Instance._transitionManager.isFadingOut)
            {
                yield return null;
            }
        }
        
        private static IEnumerator TransitionTheSwapAddressableScene(string key)
        {
            // Start loading the scene asynchronously
            _loadHandle = Addressables.LoadSceneAsync(key, LoadSceneMode.Single, false);
            yield return _loadHandle;

            // Start the fade in animation
            Instance._transitionManager.StartAnimation();

            // Wait for fade in to complete
            while (Instance._transitionManager.isFadingIn)
            {
                yield return null;
            }

            // Wait until the asynchronous scene load is almost complete
            while (_loadHandle.Status != AsyncOperationStatus.Succeeded)
            {
                if (_loadHandle.PercentComplete >= 0.9f && !Instance._transitionManager.isFadingIn)
                {
                    // Allow the scene to be activated
                    SceneInstance scene = _loadHandle.Result;
                    scene.ActivateAsync();
                }
                yield return null;
            }

            // Wait for fade out to complete
            while (Instance._transitionManager.isFadingOut)
            {
                yield return null;
            }
        }
        #endregion
    }
}