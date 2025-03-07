// pixel perfect camera helpers, from old unity 2D tutorial videos
// source: https://www.youtube.com/watch?v=rMCLWt1DuqI

using UnityEngine;

namespace CheesyUtils
{
    [ExecuteInEditMode]
    public class ScaleCamera : MonoBehaviour
    {
        public int TargetWidth = 640;
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
            int height = Mathf.RoundToInt(TargetWidth / (float)Screen.width * Screen.height);
            _cam.orthographicSize = height / PixelsToUnits / 2;
        }
    }
}
