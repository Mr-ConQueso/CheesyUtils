using UnityEngine;

namespace CheesyUtils.Platforms.Android
{
#if UNITY_ANDROID
    public static class AndroidHaptics
    {
        private static AndroidJavaObject hapticsManager;

        static AndroidHaptics()
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    hapticsManager = new AndroidJavaObject("net.cheesylabs.haptics.HapticsManager", activity);
                }
            }
        }

        /// <summary>
        /// Checks if the device supports amplitude control for haptic feedback.
        /// </summary>
        /// <returns>True if amplitude control is available, otherwise false.</returns>
        public static bool HasAmplitudeControl()
        {
            return hapticsManager?.Call<bool>("hasAmplitudeControl") ?? false;
        }

        /// <summary>
        /// Triggers a vibration with the specified duration and intensity.
        /// </summary>
        /// <param name="duration">Duration of the vibration in seconds.</param>
        /// <param name="strength">Intensity of the vibration (0-255).</param>
        public static void Vibrate(float duration, int strength)
        {
            duration *= 1000;
            hapticsManager?.Call("vibrate", (long)duration, Mathf.Clamp(strength, 0, 255));
        }

        /// <summary>
        /// Triggers a predefined haptic effect.
        /// </summary>
        /// <param name="effectType">The type of haptic effect (tick, click, etc.).</param>
        public static void VibrateEffect(int effectType)
        {
            hapticsManager?.Call("vibrateEffect", effectType);
        }

        /// <summary>
        /// Triggers an advanced vibration using haptic primitives.
        /// </summary>
        public static void VibrateWithPrimitives()
        {
            hapticsManager?.Call("vibrateWithPrimitives");
        }

        /// <summary>
        /// Stops any ongoing vibration.
        /// </summary>
        public static void StopVibration()
        {
            hapticsManager?.Call("stopVibration");
        }

        /// <summary>
        /// Checks if the current device supports haptic feedback.
        /// </summary>
        /// <returns>Returns true if the device supports haptic feedback, otherwise false.</returns>
        public static bool IsVibratorSupported()
        {
            return true;
        }
#endif
    }
}