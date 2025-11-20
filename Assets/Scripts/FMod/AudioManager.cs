using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;
using FMOD.Studio;
using FMODUnity;

namespace CheesyUtils.FMod
{
    /// <summary>
    /// Central FMOD audio controller. Manages sound playback, pooling,
    /// bus volumes, and one-shot event creation.
    /// </summary>
    [DefaultExecutionOrder(-90)]
    public class AudioManager : Singleton<AudioManager>
    {
        // --- Serialized Fields ---
        [Header("Bus Volumes")]
        [Range(0f, 1f)] [SerializeField] private float _masterVolume = 1f;
        [Range(0f, 1f)] [SerializeField] private float _musicVolume = 1f;
        [Range(0f, 1f)] [SerializeField] private float _ambienceVolume = 1f;
        [Range(0f, 1f)] [SerializeField] private float _sfxVolume = 1f;

        [Header("Pooling")]
        [SerializeField] private SoundEmitter _soundEmitterPrefab;
        [SerializeField] private bool _collectionCheck = true;
        [SerializeField] private int _defaultCapacity = 10;
        [SerializeField] private int _maxPoolSize = 100;
        [SerializeField] private int _maxSoundInstances = 30;

        // --- Private Fields ---
        private IObjectPool<SoundEmitter> _soundEmitterPool;
        private readonly List<SoundEmitter> _activeEmitters = new();
        private readonly Queue<SoundEmitter> _frequentEmitters = new();

        private Bus _masterBus;
        private Bus _musicBus;
        private Bus _ambienceBus;
        private Bus _sfxBus;

        private readonly List<EventInstance> _eventInstances = new();

        // --- Unity Lifecycle ---
        protected override void Awake()
        {
            base.Awake();
            InitializeBuses();
            InitializePool();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void Update()
        {
            _masterBus.setVolume(_masterVolume);
            _musicBus.setVolume(_musicVolume);
            _ambienceBus.setVolume(_ambienceVolume);
            _sfxBus.setVolume(_sfxVolume);
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            CleanUp();
        }

        // --- Public API ---

        /// <summary>Creates a builder for configuring and playing a sound.</summary>
        public SoundBuilder CreateSound() => new SoundBuilder(this);

        /// <summary>Plays a one-shot sound at a given world position.</summary>
        public void PlayOneShot(EventReference sound, Vector3 position)
        {
            RuntimeManager.PlayOneShot(sound, position);
        }

        /// <summary>Returns true if another frequent sound may be played.</summary>
        public bool CanPlaySound(SoundData data)
        {
            if (!data.FrequentSound) return true;

            if (_frequentEmitters.Count >= _maxSoundInstances &&
                _frequentEmitters.TryDequeue(out var oldEmitter))
            {
                oldEmitter.Stop();
                return true;
            }

            return true;
        }

        public SoundEmitter Get() => _soundEmitterPool.Get();
        public void ReturnToPool(SoundEmitter emitter) => _soundEmitterPool.Release(emitter);

        /// <summary>Creates and tracks an FMOD event instance.</summary>
        public EventInstance CreateInstance(EventReference reference)
        {
            EventInstance eventInstance = RuntimeManager.CreateInstance(reference);
            _eventInstances.Add(eventInstance);
            return eventInstance;
        }

        // --- Private Helpers ---

        private void InitializeBuses()
        {
            _masterBus = RuntimeManager.GetBus("bus:/");
            _musicBus = RuntimeManager.GetBus("bus:/Music");
            _ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
            _sfxBus = RuntimeManager.GetBus("bus:/SFX");
        }

        private void InitializePool()
        {
            _soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyEmitter,
                _collectionCheck,
                _defaultCapacity,
                _maxPoolSize);
        }

        private SoundEmitter CreateEmitter()
        {
            var emitter = Instantiate(_soundEmitterPrefab, transform);
            emitter.gameObject.SetActive(false);
            return emitter;
        }

        private void OnTakeFromPool(SoundEmitter emitter)
        {
            emitter.gameObject.SetActive(true);
            _activeEmitters.Add(emitter);
        }

        private void OnReturnedToPool(SoundEmitter emitter)
        {
            emitter.gameObject.SetActive(false);
            _activeEmitters.Remove(emitter);
        }

        private static void OnDestroyEmitter(SoundEmitter emitter)
        {
            if (emitter)
                Destroy(emitter.gameObject);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            foreach (SoundEmitter emitter in _activeEmitters)
                if (emitter) emitter.Stop();

            _activeEmitters.Clear();
        }

        private void CleanUp()
        {
            foreach (EventInstance inst in _eventInstances)
            {
                inst.stop(STOP_MODE.IMMEDIATE);
                inst.release();
            }
        }
    }
}