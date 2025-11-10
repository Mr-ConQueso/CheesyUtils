using System;
using UnityEngine;
using UnityEngine.Audio;

namespace CheesyUtils.FMod
{
    [Serializable]
    public class SoundData
    {
        public AudioClip Clip;
        public AudioMixerGroup MixerGroup;
        public bool Loop;
        public bool PlayOnAwake;
        public bool FrequentSound;
    }
}