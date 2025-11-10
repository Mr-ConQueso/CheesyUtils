using UnityEngine;

namespace CheesyUtils.FMod
{
    public class SoundBuilder
    {
        // ---- / Private Variables / ---- //
        private readonly AudioManager _audioManager;
        private SoundData _soundData;
        private Vector3 _position = Vector3.zero;
        private bool _randomPitch;

        public SoundBuilder(AudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        public SoundBuilder WithSoundData(SoundData soundData)
        {
            _soundData = soundData;
            return this;
        }
        
        public SoundBuilder WithRandomPitch(bool useRandomPitch = true)
        {
            _randomPitch = useRandomPitch;
            return this;
        }
        
        public SoundBuilder WithPosition(Vector3 position)
        {
            _position = position;
            return this;
        }

        public void Play()
        {
            if (!_audioManager.CanPlaySound(_soundData)) return;

            SoundEmitter soundEmitter = _audioManager.Get();
            soundEmitter.Initialize(_soundData);
            soundEmitter.transform.position = _position;
            soundEmitter.transform.parent = AudioManager.Instance.transform;

            if (_randomPitch)
            {
                soundEmitter.WithRandomPitch();
            }

            if (_soundData.FrequentSound)
            {
                _audioManager.FrequentSoundEmitters.Enqueue(soundEmitter);
            }
            
            soundEmitter.Play();
        }
    }
}