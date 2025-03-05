using System.Runtime.InteropServices;
using UnityEngine;

namespace CheesyUtils.Platforms.Web
{
#if UNITY_WEBGL
    public class WebGLVibrate
    {
        [DllImport("__Internal")]
        private static extern void Vibrate(float duration);

        [DllImport("__Internal")]
        private static extern void StopVibrate();

        [DllImport("__Internal")]
        private static extern int IsVibratorSupported();

        public static void Vibrate(float duration, HapticStrength strength = HapticStrength.Medium)
        {
                Vibrate(duration); // Call JavaScript vibration API
        }

        public static void Stop()
        {
            StopVibrate(); // Call JavaScript function to stop vibration
        }

        public static bool AreHapticsSupported()
        {
            return IsVibratorSupported() == 1;
        }
    }
#endif
}