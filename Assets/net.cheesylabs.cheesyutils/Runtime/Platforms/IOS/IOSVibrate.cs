using System.Runtime.InteropServices;
using UnityEngine;

namespace CheesyUtils.Platforms.IOS
{
#if UNITY_IOS
    public class IOSVibrate
    {
        [DllImport("__Internal")]
        private static extern void _VibrateForDuration(float duration, int strength);

        public static void Vibrate(float duration, HapticStrength strength = HapticStrength.Medium)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                _VibrateForDuration(duration, (int)strength);
            }
        }
        
        public static bool IsVibratorSupported()
        {
            return true;
        }
    }
#endif
}