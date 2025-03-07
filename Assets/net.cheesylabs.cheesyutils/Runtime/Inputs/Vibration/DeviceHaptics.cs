using UnityEngine;

#if UNITY_ANDROID
using CheesyUtils.Platforms.Android;
#elif UNITY_WEBGL
using CheesyUtils.Platforms.Web;
#elif UNITY_IOS
using CheesyUtils.Platforms.IOS;
#endif

namespace CheesyUtils.Inputs
{
    public static class DeviceHaptics
    {
        private static bool _initialized = false;
        
        public static void Init()
        {
            if (_initialized) return;
            
            AndroidHaptics.Init();
            
            _initialized = true;
        }
        
        public static void StopHaptics()
        {
            if (Application.isMobilePlatform)
            {
#if UNITY_ANDROID
                AndroidHaptics.CancelAndroid();
#elif UNITY_WEBGL
                WebGLHaptics.Stop();
#endif
            }
        }
        
        public static bool HasVibrator()
        {
            if (Application.isMobilePlatform)
            {
#if UNITY_ANDROID
                return AndroidHaptics.HasVibrator();
#elif UNITY_IOS
                return IOSHaptics.HasVibrator();
#elif UNITY_WEBGL
                return WebGLHaptics.AreHapticsSupported();
#else
                return false;
#endif
            }
            else
            {
                return false;
            }
        }
        
        public static void VibratePop()
        {
            if (Application.isMobilePlatform && HasVibrator())
            {
#if UNITY_ANDROID
                AndroidHaptics.VibratePop();
#elif UNITY_IOS
                IOSHaptics.VibratePop();
#elif UNITY_WEBGL
                WebGLHaptics.Vibrate(duration);
#endif
            }
        }
        
        public static void VibratePeek()
        {
            if (Application.isMobilePlatform && HasVibrator())
            {
#if UNITY_ANDROID
                AndroidHaptics.VibratePeek();
#elif UNITY_IOS
                IOSHaptics.VibratePeek();
#elif UNITY_WEBGL
                WebGLHaptics.VibratePeek(duration);
#endif
            }
        }
        
        public static void VibrateNope()
        {
            if (Application.isMobilePlatform && HasVibrator())
            {
#if UNITY_ANDROID
                AndroidHaptics.VibrateNope();
#elif UNITY_IOS
                IOSHaptics.VibrateNope();
#elif UNITY_WEBGL
                WebGLHaptics.VibrateNope(duration);
#endif
            }
        }
        
        public static void Vibrate()
        {
            Handheld.Vibrate();
        }
    }
}