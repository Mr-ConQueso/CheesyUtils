using UnityEngine;

#if UNITY_IOS
using System.Collections;
using System.Runtime.InteropServices;
#endif

namespace CheesyUtils.Platforms.IOS
{
#if UNITY_IOS
    public static class IOSHaptics
    {
        [DllImport ( "__Internal" )]
        private static extern bool _HasVibrator ();

        [DllImport ( "__Internal" )]
        private static extern void _Vibrate ();

        [DllImport ( "__Internal" )]
        private static extern void _VibratePop ();

        [DllImport ( "__Internal" )]
        private static extern void _VibratePeek ();

        [DllImport ( "__Internal" )]
        private static extern void _VibrateNope ();

        [DllImport("__Internal")]
        private static extern void _impactOccurred(string style);

        [DllImport("__Internal")]
        private static extern void _notificationOccurred(string style);

        [DllImport("__Internal")]
        private static extern void _selectionChanged();

        private static bool _initialized = false;

        public static void VibrateIOS(ImpactFeedbackStyle style)
        {
            _impactOccurred(style.ToString());
        }

        public static void VibrateIOS(NotificationFeedbackStyle style)
        {
            _notificationOccurred(style.ToString());
        }

        public static void VibrateIOS_SelectionChanged()
        {
            _selectionChanged();
        }    

        ///<summary>
        /// Tiny pop vibration
        ///</summary>
        public static void VibratePop()
        {
            _VibratePop();
        }
        
        ///<summary>
        /// Small peek vibration
        ///</summary>
        public static void VibratePeek()
        {
            _VibratePeek();
        }
        
        ///<summary>
        /// 3 small vibrations
        ///</summary>
        public static void VibrateNope()
        {
            _VibrateNope();
        }

        public static bool HasVibrator()
        {
            return _HasVibrator();
        }
    }
#endif
    
    public enum ImpactFeedbackStyle
    {
        Heavy,
        Medium,
        Light,
        Rigid,
        Soft
    }
    
    public enum NotificationFeedbackStyle
    {
        Error,
        Success,
        Warning
    }
}