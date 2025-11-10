using System;
using UnityEngine;

namespace CheesyUtils.SavingLoading
{
    public class SavedSettings : MonoBehaviour
    {
        public static float masterVolume = 1;
        public static float musicVolume = 1;
        public static float sfxVolume = 1;
        public static AudioSpeakerMode audioMode = AudioSpeakerMode.Stereo;
        public static bool showSubtitles = false;
        
        public static float mouseSensibility = 100;
        public static int invertDirection = 1;
        
        public static int graphicsQuality = 0;
        public static int fullScreenMode = 0;

        public static string keybinds;
        
        public object CaptureState()
        {
            return new SaveData()
            {
                AudioSettings = new AudioSettings()
                {
                    masterVolume = masterVolume,
                    musicVolume = musicVolume,
                    sfxVolume = sfxVolume,
                    audioMode = GetAudioMode(audioMode),
                    showSubtitles = showSubtitles,
                },
                
                VideoSettings = new VideoSettings
                {
                    graphicsQuality = graphicsQuality,
                    fullScreenMode = fullScreenMode,
                },
                
                ControlsSettings = new ControlsSettings
                {
                    mouseSensibility = mouseSensibility,
                    invertDirection = invertDirection,
                    keybinds = keybinds,
                }
            };
        }

        public void RestoreState(object state)
        {
            var saveData = (SaveData)state;
            
            // Audio
            masterVolume = saveData.AudioSettings.masterVolume;
            musicVolume = saveData.AudioSettings.musicVolume;
            sfxVolume = saveData.AudioSettings.sfxVolume;
            audioMode = SetAudioMode(saveData.AudioSettings.audioMode);
            showSubtitles = saveData.AudioSettings.showSubtitles;
            
            // Video
            graphicsQuality = saveData.VideoSettings.graphicsQuality;
            fullScreenMode = saveData.VideoSettings.fullScreenMode;
            
            // Input
            mouseSensibility = saveData.ControlsSettings.mouseSensibility;
            invertDirection = saveData.ControlsSettings.invertDirection;

            keybinds = saveData.ControlsSettings.keybinds;
        }
        
        private int GetAudioMode(AudioSpeakerMode audioMode)
        {
            switch (audioMode)
            {
                case AudioSpeakerMode.Mono:
                    return 0;
                case AudioSpeakerMode.Stereo:
                    return 1;
                case AudioSpeakerMode.Quad: 
                    return 2;
                case AudioSpeakerMode.Surround: 
                    return 3;
                default:
                    return 1;
            }
        }

        private AudioSpeakerMode SetAudioMode(int audioMode)
        {
            switch (audioMode)
            {
                case 0:
                    return AudioSpeakerMode.Mono;
                case 1:
                    return AudioSpeakerMode.Stereo;
                case 2:
                    return AudioSpeakerMode.Quad;
                case 3:
                    return AudioSpeakerMode.Surround;
                default:
                    return AudioSpeakerMode.Stereo;
            }
        }
        
        [Serializable]
        private struct AudioSettings
        {
            public float masterVolume;
            public float musicVolume;
            public float sfxVolume;
            public int audioMode;
            public bool showSubtitles;
        }
        
        [Serializable]
        private struct VideoSettings
        {
            public int graphicsQuality;
            public int fullScreenMode;
        }
        
        [Serializable]
        private struct ControlsSettings
        {
            public float mouseSensibility;
            public int invertDirection;
            public string keybinds;
        }

        [Serializable]
        private struct SaveData
        {
            public AudioSettings AudioSettings;

            public VideoSettings VideoSettings;
            
            public ControlsSettings ControlsSettings;
        }
    }
}