using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
#if UNITY_ANDROID
using CheesyUtils.Platforms.Android;
#elif UNITY_WEBGL
using CheesyUtils.Platforms.Web;
#elif UNITY_IOS
using CheesyUtils.Platforms.IOS;
#endif

namespace CheesyUtils.Inputs
{
    public class ControllerVibration : MonoBehaviour
    {
        private List<IVibrationPart> _currentVibrateList = new List<IVibrationPart>();
        
        /// <summary>
        /// Pauses all active haptic feedback on supported input devices.
        /// </summary>
        public static void PauseVibration()
        {
            InputSystem.PauseHaptics();
        }
        
        /// <summary>
        /// Resumes paused haptic feedback on supported input devices.
        /// </summary>
        public static void ResumeVibration()
        {
            InputSystem.ResumeHaptics();
        }
        
        /// <summary>
        /// Stops and resets all haptic feedback on supported input devices.
        /// </summary>
        public static void ResetVibration()
        {
            InputSystem.ResetHaptics();

#if UNITY_ANDROID
            AndroidHaptics.StopVibration();
#elif UNITY_WEBGL
            WebGLVibrate.Stop();
#elif UNITY_IOS
            IOSVibrate.Stop();
#endif
        }
        
        /// <summary>
        /// Initiates a vibration sequence using a predefined list of vibration patterns.
        /// </summary>
        /// <param name="newList">The list of vibration patterns to execute sequentially.</param>
        public void VibrateSequence(List<IVibrationPart> newList)
        {
            if (CheckIfCanVibrate() == false) return;

            // DO NOT use `_currentVibrateList = newList` here as it only copies the reference
            _currentVibrateList = new List<IVibrationPart>(newList);
            StartCoroutine(VibrateSequenceManager());
        }
        
        /// <summary>
        /// Triggers a vibration for a specified duration and intensity.
        /// </summary>
        /// <param name="duration">The duration of the vibration in seconds.</param>
        /// <param name="strength">The intensity of the vibration, defaulting to medium.</param>
        public void VibrateWithDuration(float duration, HapticStrength strength = HapticStrength.Medium)
        {
            if (CheckIfCanVibrate() == false) return;

            StartCoroutine(Vibrate(duration, strength));
            VibrateHandHeld(duration, strength);
        }
        
        /// <summary>
        /// Triggers a short vibration with a specified intensity.
        /// </summary>
        /// <param name="strength">The intensity of the vibration, defaulting to medium.</param>
        public void VibrateOneShot(HapticStrength strength = HapticStrength.Medium)
        {
            if (CheckIfCanVibrate() == false) return;

            StartCoroutine(Vibrate(0f, strength));
            VibrateHandHeld(0f, strength);
        }

        private static bool CheckIfCanVibrate()
        {
#if UNITY_ANDROID
            return AndroidHaptics.IsVibratorSupported();
#elif UNITY_IOS
            return IOSVibrate.IsVibratorSupported();
#elif UNITY_WEBGL
            return WebGLVibrate.AreHapticsSupported();
#elif UNITY_STANDALONE
            return Gamepad.all.Count > 0;
#else
            return false;
#endif
        }
        
        private static Vector2 GetMotorStrength(HapticStrength strength)
        {
            return strength switch
            {
                HapticStrength.Light => new Vector2(0.1f, 0.1f),
                HapticStrength.Medium => new Vector2(0.5f, 0.5f),
                HapticStrength.Heavy => new Vector2(1f, 1f),
                _ => new Vector2(0.5f, 0.5f)
            };
        }

        private static int GetHapticStrength(HapticStrength strength)
        {
            return strength switch
            {
                HapticStrength.Light => 50,
                HapticStrength.Medium => 100,
                HapticStrength.Heavy => 250,
                _ => 100
            };
        }
        
        private static void VibrateHandHeld(float duration, HapticStrength strength)
        {
#if UNITY_ANDROID
            AndroidHaptics.Vibrate(duration, GetHapticStrength(strength));
#elif UNITY_IOS
            IOSVibrate.Vibrate(duration, strength);
#elif UNITY_WEBGL
            WebGLVibrate.Vibrate(duration, strength);
#endif
        }

        private IEnumerator VibrateSequenceManager()
        {
            foreach (IVibrationPart data in _currentVibrateList)
            { yield return StartCoroutine(VibrateSequence(data)); }

            //finishes the current list
            _currentVibrateList.Clear();
        }
        
        private static IEnumerator Vibrate(float duration, HapticStrength strength)
        {
            Vector2 motorStrength = GetMotorStrength(strength);
            Gamepad.current.SetMotorSpeeds(motorStrength.x, motorStrength.y);
            yield return new WaitForSeconds(duration);
            Gamepad.current.SetMotorSpeeds(0, 0);
        }

        /// <summary>
        /// Should only be called by vibrate manager
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static IEnumerator VibrateSequence(IVibrationPart data)
        {
            //initialises the vibration (required for animated curves)
            data.Activate();

            float duration = data.GetPartDataDuration();

            bool repeat = data.UpdateFrame();

            //if it needs to repeat check
            if (repeat)
            {
                while (data.UpdateFrame())
                {
                    Vector2 strength = data.GetPartDataStrength();
                    Gamepad.current.SetMotorSpeeds(strength.x, strength.y);
                    //continues to the next frame
                    yield return null;
                }
            }
            //if it only needs to happen once
            else
            {

                Vector2 strength = data.GetPartDataStrength();
                Gamepad.current.SetMotorSpeeds(strength.x, strength.y);
                //completes the vibration duration
                VibrateHandHeld(duration, HapticStrength.Medium);
                
                yield return new WaitForSeconds(duration);
            }
            //resets
            Gamepad.current.SetMotorSpeeds(0, 0);
        }
    }
}