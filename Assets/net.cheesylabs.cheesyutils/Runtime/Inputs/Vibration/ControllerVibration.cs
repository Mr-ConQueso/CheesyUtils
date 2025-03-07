using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CheesyUtils.Inputs
{
    public class ControllerVibration : MonoBehaviour
    {
        private List<IVibrationPart> _currentVibrateList = new List<IVibrationPart>();

        private void Awake()
        {
            DeviceHaptics.Init();
        }

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
            DeviceHaptics.StopHaptics();
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
        public void VibrateWithDuration(float duration)
        {
            if (CheckIfCanVibrate() == false) return;

            StartCoroutine(Vibrate(duration));
            DeviceHaptics.VibratePop();
        }
        
        /// <summary>
        /// Triggers a short vibration with a specified intensity.
        /// </summary>
        /// <param name="strength">The intensity of the vibration, defaulting to medium.</param>
        public void VibrateOneShot()
        {
            if (CheckIfCanVibrate() == false) return;

            StartCoroutine(Vibrate(0f));
            DeviceHaptics.VibratePop();
        }
        
        private static bool CheckIfCanVibrate()
        {
            if (Application.isMobilePlatform)
            {
                return DeviceHaptics.HasVibrator();
            }
            return Gamepad.all.Count > 0;
        }

        private IEnumerator VibrateSequenceManager()
        {
            foreach (IVibrationPart data in _currentVibrateList)
            { yield return StartCoroutine(VibrateSequence(data)); }

            //finishes the current list
            _currentVibrateList.Clear();
        }
        
        private static IEnumerator Vibrate(float duration)
        {
            Vector2 motorStrength = new Vector2(0.5f, 0.5f);
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
                DeviceHaptics.VibratePop();
                
                yield return new WaitForSeconds(duration);
            }
            //resets
            Gamepad.current.SetMotorSpeeds(0, 0);
        }
    }
}