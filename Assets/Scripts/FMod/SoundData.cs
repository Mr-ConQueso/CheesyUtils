using System;
using FMODUnity;

namespace CheesyUtils.FMod
{
    /// <summary>Configurable data for a sound event.</summary>
    [Serializable]
    public class SoundData
    {
        public EventReference EventRef;
        public bool Loop;
        public bool PlayOnAwake;
        public bool FrequentSound;
    }

    /// <summary> Defines an FMod parameter.</summary>
    [Serializable]
    public struct FModParameter
    {
        public string Name;
        public float Value;
    }
}