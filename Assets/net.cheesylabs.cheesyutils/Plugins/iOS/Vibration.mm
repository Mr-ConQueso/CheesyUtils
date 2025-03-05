#import <Foundation/Foundation.h>
#import <AudioToolbox/AudioToolbox.h>
#import <UIKit/UIKit.h>

extern "C" {
    void _VibrateForDuration(float duration, int strength)
    {
        if (@available(iOS 10.0, *))
        {
            // Select haptic strength based on input
            UIImpactFeedbackStyle feedbackStyle;
            switch (strength) {
                case 1: feedbackStyle = UIImpactFeedbackStyleLight; break;
                case 2: feedbackStyle = UIImpactFeedbackStyleMedium; break;
                case 3: feedbackStyle = UIImpactFeedbackStyleHeavy; break;
                default: feedbackStyle = UIImpactFeedbackStyleMedium; break; // Default to medium
            }

            UIImpactFeedbackGenerator *impact = [[UIImpactFeedbackGenerator alloc] initWithStyle:feedbackStyle];
            [impact prepare];

            int repetitions = duration * 10; // Adjust loop count
            for (int i = 0; i < repetitions; i++)
            {
                [impact impactOccurred];
                [NSThread sleepForTimeInterval:0.1]; // Small delay between pulses
            }
        }
        else
        {
            // Fallback for older iOS versions (use standard vibration)
            int repetitions = duration * 5;
            for (int i = 0; i < repetitions; i++)
            {
                AudioServicesPlaySystemSound(kSystemSoundID_Vibrate);
                [NSThread sleepForTimeInterval:0.2];
            }
        }
    }
}
