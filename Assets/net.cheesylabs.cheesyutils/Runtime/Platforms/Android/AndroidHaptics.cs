using System;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

namespace CheesyUtils.Platforms.Android
{
#if UNITY_ANDROID
    public static class AndroidHaptics
    {

        public static AndroidJavaClass UnityPlayer;
        public static AndroidJavaObject CurrentActivity;
        public static AndroidJavaObject Vibrator;
        public static AndroidJavaObject Context;
        public static AndroidJavaClass VibrationEffect;
        
        public static void Init()
        {
            UnityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
            CurrentActivity = UnityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
            Vibrator = CurrentActivity.Call<AndroidJavaObject> ("getSystemService", "vibrator");
            Context = CurrentActivity.Call<AndroidJavaObject> ("getApplicationContext");

            if (AndroidVersion >= 26)
            {
                VibrationEffect = new AndroidJavaClass ("android.os.VibrationEffect");
            }
        }

        ///<summary>
        /// Tiny pop vibration
        ///</summary>
        public static void VibratePop()
        {
            VibrateAndroid(50);
        }
        
        ///<summary>
        /// Small peek vibration
        ///</summary>
        public static void VibratePeek()
        {
            VibrateAndroid(100);
        }
        
        ///<summary>
        /// 3 small vibrations
        ///</summary>
        public static void VibrateNope()
        {
            long[] pattern = { 0, 50, 50, 50 };
            VibrateAndroid (pattern, -1);
        }
        
        /// <summary>
        /// https://developer.android.com/reference/android/os/Vibrator.html#vibrate(long)
        /// </summary>
        /// <param name="duration">duration in milliseconds</param>
        public static void VibrateAndroid(long duration)
        {
            if (AndroidVersion >= 26)
            {
                AndroidJavaObject createOneShot = VibrationEffect.CallStatic<AndroidJavaObject>("createOneShot", duration, - 1);
                Vibrator.Call("vibrate", createOneShot);

            }
            else
            {
                Vibrator.Call("vibrate", duration);
            }
        }

        /// <summary>
        /// https://proandroiddev.com/using-vibrate-in-android-b0e3ef5d5e07
        /// </summary>
        /// <param name="pattern">the pattern of vibration durations</param>
        /// <param name="repeat">1 to repeat forever, 0 to repeat until canceled</param>
        public static void VibrateAndroid(long[] pattern, int repeat)
        {
            if (AndroidVersion >= 26)
            {
                AndroidJavaObject createWaveform = VibrationEffect.CallStatic<AndroidJavaObject>("createWaveform", pattern, repeat);
                Vibrator.Call("vibrate", createWaveform);

            }
            else
            {
                Vibrator.Call("vibrate", pattern, repeat);
            }
        }
        
        public static void CancelAndroid()
        {
            Vibrator.Call("cancel");
        }

        public static bool HasVibrator()
        {
            AndroidJavaClass contextClass = new AndroidJavaClass ("android.content.Context");
            string Context_VIBRATOR_SERVICE = contextClass.GetStatic<string> ("VIBRATOR_SERVICE");
            AndroidJavaObject systemService = Context.Call<AndroidJavaObject> ("getSystemService", Context_VIBRATOR_SERVICE);
            
            if (systemService.Call<bool>("hasVibrator"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static int AndroidVersion
        {
            get
            {
                int iVersionNumber = 0;
                if (Application.platform == RuntimePlatform.Android)
                {
                    string androidVersion = SystemInfo.operatingSystem;
                    int sdkPos = androidVersion.IndexOf("API-", StringComparison.Ordinal);
                    iVersionNumber = int.Parse(androidVersion.Substring(sdkPos + 4, 2));
                }
                return iVersionNumber;
            }
        }
    }
#endif
}