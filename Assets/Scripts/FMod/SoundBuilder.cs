using System.Collections.Generic;
using UnityEngine;

namespace CheesyUtils.FMod
{
    /// <summary>
    /// Fluent builder for configuring and playing FMOD sounds.
    /// </summary>
    public class SoundBuilder
    {
        private readonly AudioManager _manager;
        private SoundData _data;
        private Vector3 _position;
        private bool _useRandomPitch;
        private List<FModParameter> _parameters =  new();

        public SoundBuilder(AudioManager manager) => _manager = manager;

        public SoundBuilder WithSoundData(SoundData data)
        {
            _data = data;
            return this;
        }

        public SoundBuilder WithParameter(string name, float value)
        {
            _parameters.Add(new FModParameter
            {
                Name = name,
                Value = value
            });
            return this;
        }

        public SoundBuilder WithPosition(Vector3 position)
        {
            _position = position;
            return this;
        }

        public SoundBuilder WithRandomPitch(bool enable = true)
        {
            _useRandomPitch = enable;
            return this;
        }

        public void Play()
        {
            if (_data == null) return;
            if (!_manager.CanPlaySound(_data)) return;

            var emitter = _manager.Get();
            emitter.Initialize(_data);
            emitter.transform.position = _position;

            if (_useRandomPitch) emitter.WithRandomPitch();
            if (_parameters.Count > 0) emitter.WithParameters(_parameters);

            if (_data.FrequentSound)
                _manager.CreateSound().WithSoundData(_data); // track if needed

            emitter.Play(_position);
        }
    }
}