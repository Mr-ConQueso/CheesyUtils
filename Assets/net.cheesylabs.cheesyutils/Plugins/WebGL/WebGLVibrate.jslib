var WebGLVibrate = {
    vibrateInterval: null, // Store interval ID for persistent vibration

    Vibrate: function(duration) {
        if ("vibrate" in navigator) {
            navigator.vibrate(duration * 1000); // Convert seconds to milliseconds
        }
    },

    StopVibrate: function() {
        if ("vibrate" in navigator) {
            if (WebGLVibrate.vibrateInterval) {
                clearInterval(WebGLVibrate.vibrateInterval);
                WebGLVibrate.vibrateInterval = null;
            }
            navigator.vibrate(0); // Stop all ongoing vibrations
        }
    },

    IsVibratorSupported: function() {
        return "vibrate" in navigator ? 1 : 0; // Return 1 if supported, 0 if not
    }
};

mergeInto(LibraryManager.library, WebGLVibrate);