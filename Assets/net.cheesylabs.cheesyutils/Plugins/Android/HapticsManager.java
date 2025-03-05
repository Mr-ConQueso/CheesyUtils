package net.cheesylabs.haptics;

import android.content.Context;
import android.os.Build;
import android.os.VibrationEffect;
import android.os.Vibrator;
import android.os.VibratorManager;
import android.util.Log;

public class HapticsManager {
    private Vibrator vibrator;
    private static final String TAG = "HapticsManager";

    public HapticsManager(Context context) {
        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.S) {
            VibratorManager vibratorManager = (VibratorManager) context.getSystemService(Context.VIBRATOR_MANAGER_SERVICE);
            if (vibratorManager != null) {
                vibrator = vibratorManager.getDefaultVibrator();
            }
        } else {
            vibrator = (Vibrator) context.getSystemService(Context.VIBRATOR_SERVICE);
        }

        if (vibrator == null) {
            Log.e(TAG, "Vibrator service not available.");
        }
    }

    // Check if amplitude control is supported
    public boolean hasAmplitudeControl() {
        return vibrator != null && vibrator.hasAmplitudeControl();
    }

    // Check if primitives are supported
    public boolean hasPrimitiveSupport(int primitive) {
        return Build.VERSION.SDK_INT >= Build.VERSION_CODES.S && vibrator != null && vibrator.areAllPrimitivesSupported(primitive);
    }

    // Vibrate with duration and strength (adjusted for device capabilities)
    public void vibrate(long duration, int strength) {
        if (vibrator == null) return;

        if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.O) {
            if (hasAmplitudeControl()) {
                // Use amplitude control for fine-grained vibration
                vibrator.vibrate(VibrationEffect.createOneShot(duration, strength));
            } else {
                // Fallback to basic ON/OFF vibration
                vibrator.vibrate(VibrationEffect.createOneShot(duration, VibrationEffect.DEFAULT_AMPLITUDE));
            }
        } else {
            // Legacy vibration API for older devices
            vibrator.vibrate(duration);
        }
    }

    // Vibrate using a predefined effect (e.g., tick, click)
    public void vibrateEffect(int effectType) {
        if (vibrator == null || Build.VERSION.SDK_INT < Build.VERSION_CODES.Q) return;

        vibrator.vibrate(VibrationEffect.createPredefined(effectType));
    }

    // Advanced vibration using primitives (e.g., slow rise, thud)
    public void vibrateWithPrimitives() {
        if (vibrator == null || Build.VERSION.SDK_INT < Build.VERSION_CODES.S) return;

        if (hasPrimitiveSupport(VibrationEffect.Composition.PRIMITIVE_SLOW_RISE) &&
            hasPrimitiveSupport(VibrationEffect.Composition.PRIMITIVE_CLICK)) {
            vibrator.vibrate(
                VibrationEffect.startComposition()
                    .addPrimitive(VibrationEffect.Composition.PRIMITIVE_SLOW_RISE, 0.8f)
                    .addPrimitive(VibrationEffect.Composition.PRIMITIVE_CLICK, 1.0f)
                    .compose()
            );
        } else {
            // Fallback to a simple vibration
            vibrate(100, VibrationEffect.DEFAULT_AMPLITUDE);
        }
    }

    // Stop vibration
    public void stopVibration() {
        if (vibrator != null) {
            vibrator.cancel();
        }
    }
}