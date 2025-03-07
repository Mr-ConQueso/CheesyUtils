// pixel perfect camera helpers, from old unity 2D tutorial videos
// source: https://www.youtube.com/watch?v=rMCLWt1DuqI

using UnityEngine;

namespace CheesyUtils
{
    [ExecuteInEditMode]
    public class PixelPerfectCamera : MonoBehaviour
    {
        public float PixelsToUnits = 100;
        private Camera _cam;

        private void Start()
        {
            _cam = GetComponent<Camera>();
            if (_cam == null)
            {
                Debug.LogError("Camera not found..", gameObject);
                this.enabled = false;
                return;
            }
            SetScale();
        }

        // in editor need to update in a loop, in case of game window resizes
#if UNITY_EDITOR
        private void Update()
        {
            SetScale();
        }
#endif

        private void SetScale()
        {
            _cam.orthographicSize = Screen.height / PixelsToUnits / 2;
        }
    }
}
