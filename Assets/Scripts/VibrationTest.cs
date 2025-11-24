using System.Globalization;
using CheesyUtils.Inputs;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class IOSHapticsTest : MonoBehaviour
    {
        [SerializeField] private TMP_Text _playPauseText;
        [SerializeField] private TMP_InputField _durationInputField;
        [SerializeField] private TMP_Dropdown _strengthTypeDropdown;
        [SerializeField] private VibrationSequence _vibrationSequence;
        [SerializeField] private ControllerVibration _controllerVibration;
        
        private bool _isPaused = false;
        private float _duration = 0.5f;
        private Vector2 _strength = new Vector2(0.5f, 0.5f);

        private void Start()
        {
            _durationInputField.text = _duration.ToString(CultureInfo.InvariantCulture);
        }

        public void OnClick_VibrateUnity()
        {
#if UNITY_ANDROID
            Handheld.Vibrate();
#elif UNITY_IOS
            Handheld.Vibrate();
#endif
        }

        public void OnClick_VibrateSequence()
        {
            _controllerVibration.VibrateSequence(_vibrationSequence.sequence);
        }
        
        public void OnClick_VibrateDuration()
        {
            _controllerVibration.VibrateWithDuration(_duration);
        }

        public void OnClick_VibrateOneShot()
        {
            _controllerVibration.VibrateOneShot();
        }
        
        public void OnClick_PlayPauseVibration()
        {
            if (_isPaused)
            {
                ControllerVibration.ResumeVibration();
                _playPauseText.text = "||";
                _isPaused = false;
            }
            else
            {
                ControllerVibration.PauseVibration();
                _playPauseText.text = ">";
                _isPaused = true;
            }
        }
        
        public void OnClick_ResetVibration()
        {
            ControllerVibration.ResetVibration();
        }
        
        public void OnClick_SetDuration(string text)
        {
            float duration = float.Parse(text);
            _duration = duration;
        }
    }
}