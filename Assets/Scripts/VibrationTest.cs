using System;
using System.Globalization;
using CheesyUtils.Inputs;
using TMPro;
using UnityEngine;

namespace DefaultNamespace
{
    public class VibrationTest : MonoBehaviour
    {
        [SerializeField] private TMP_Text _playPauseText;
        [SerializeField] private TMP_InputField _durationInputField;
        [SerializeField] private TMP_Dropdown _strengthTypeDropdown;
        [SerializeField] private VibrationSequence _vibrationSequence;
        [SerializeField] private ControllerVibration _controllerVibration;
        
        private bool _isPaused = false;
        private float _duration = 0.5f;
        private Vector2 _strength = new Vector2(0.5f, 0.5f);
        private HapticStrength _strengthType = HapticStrength.Medium;

        private void Start()
        {
            _durationInputField.text = _duration.ToString(CultureInfo.InvariantCulture);
            _strengthTypeDropdown.value = (int)_strengthType;
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
            _controllerVibration.VibrateWithDuration(_duration,_strengthType);
        }

        public void OnClick_VibrateOneShot()
        {
            _controllerVibration.VibrateOneShot(_strengthType);
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
        
        public void OnClick_SetStrengthType(Int32 value)
        {
            _strengthType = value switch
            {
                0 => HapticStrength.Light,
                1 => HapticStrength.Medium,
                2 => HapticStrength.Heavy,
                _ => _strengthType
            };

            _strength = _strengthType switch
            {
                HapticStrength.Light => new Vector2(0.1f, 0.1f),
                HapticStrength.Medium => new Vector2(0.5f, 0.5f),
                HapticStrength.Heavy => new Vector2(1f, 1f),
                _ => new Vector2(0.5f, 0.5f)
            };
        }
    }
}