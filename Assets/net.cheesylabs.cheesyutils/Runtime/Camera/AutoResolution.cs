using UnityEngine;

namespace CheesyUtils
{
    public class AutoResolution : MonoBehaviour
    {
        public int SetWidth = 1440;
        public int SetHeight = 2560;

        private Camera _mainCamera;
        private Rect _rect;
        
        private void Awake()
        {
            _mainCamera = Camera.main;
            if (_mainCamera != null) _rect = _mainCamera.rect;
        }

        private void Start()
        {
            // Calculate the scale height and width of the screen
            float scaleHeight = ((float)Screen.width / Screen.height) / ((float)9 / 16);
            float scaleWidth = 1f / scaleHeight;

            // Adjust the camera's dimensions based on the scale height and width
            if (scaleHeight < 1)
            {
                _rect.height = scaleHeight;
                _rect.y = (1f - scaleHeight) / 2f;
            }
            else
            {
                _rect.width = scaleWidth;
                _rect.x = (1f - scaleWidth) / 2f;
            }

            _mainCamera.rect = _rect;

            SetResolution();
        }

        /// <summary>
        /// Sets the resolution of the screen to the desired dimensions, while maintaining aspect ratio.
        /// </summary>
        public void SetResolution()
        {
            // Get the current device's screen dimensions
            int deviceWidth = Screen.width;
            int deviceHeight = Screen.height;

            // Set the screen resolution to the desired dimensions, while maintaining aspect ratio
            Screen.SetResolution(SetWidth, (int)(((float)deviceHeight / deviceWidth) * SetWidth), true);

            // Adjust the camera's dimensions based on the new resolution
            if ((float)SetWidth / SetHeight < (float)deviceWidth / deviceHeight)
            {
                float newWidth = ((float)SetWidth / SetHeight) / ((float)deviceWidth / deviceHeight);
                _mainCamera.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
            }
            else
            {
                float newHeight = ((float)deviceWidth / deviceHeight) / ((float)SetWidth / SetHeight);
                _mainCamera.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight); // 새로운 Rect 적용
            }
        }
    }
}
