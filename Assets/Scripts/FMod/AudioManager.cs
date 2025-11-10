using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

namespace CheesyUtils.FMod
{
    public class AudioManager : Singleton<AudioManager>
    {
        // ---- / Public Variables / ---- //
        public readonly Queue<SoundEmitter> FrequentSoundEmitters = new();

        // ---- / Serialized Variables / ---- //
        [SerializeField] private SoundEmitter _soundEmitterPrefab;
        [SerializeField] private bool _collectionCheck = true;
        [SerializeField] private int _defaultCapacity = 10;
        [SerializeField] private int _maxPoolSize = 100;
        [SerializeField] private int _maxSoundInstances = 30;
        
        private IObjectPool<SoundEmitter> _soundEmitterPool;
        private readonly List<SoundEmitter> _activeSoundEmitter = new();
        
        private void Start()
        {
            InitializePool();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        /// <summary>
        /// Creates a new SoundBuilder for configuring and playing sounds. Example usage:
        /// AudioManager.Instance.CreateSound().WithSoundData(shootSound)
        /// .WithRandomPitch().WithPosition(transform.position).Play();
        /// - WithSoundData: Specify the sound data to play.
        /// - WithRandomPitch: Optionally randomize the pitch.
        /// - WithPosition: Specify the position of the sound.
        /// - Play: Play the configured sound.
        /// </summary>
        /// <returns>A new instance of SoundBuilder.</returns>
        public SoundBuilder CreateSound() => new SoundBuilder(this);

        public bool CanPlaySound(SoundData data)
        {
            if (!data.FrequentSound) return true;

            if (FrequentSoundEmitters.Count >= _maxSoundInstances &&
                FrequentSoundEmitters.TryDequeue(out var soundEmitter))
            {
                try {
                    soundEmitter.Stop();
                    return true;
                } catch {
                    Debug.Log("SoundEmitter is already released", DLogType.Audio);
                }
                return false;
            }
            
            return true;
        }

        public SoundEmitter Get()
        {
            return _soundEmitterPool.Get();
        }

        public void ReturnToPool(SoundEmitter soundEmitter)
        {
            _soundEmitterPool.Release(soundEmitter);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            foreach (SoundEmitter soundEmitter in _activeSoundEmitter.Where(emitter => emitter.gameObject != null))
            {
                soundEmitter.gameObject.SetActive(false);
            }

            _activeSoundEmitter.Clear();
        }

        private static void OnDestroyPoolObject(SoundEmitter soundEmitter)
        {
            Destroy(soundEmitter.gameObject);
        }

        private void OnReturnedToPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(false);
            _activeSoundEmitter.Remove(soundEmitter);
        }

        private void OnTakeFromPool(SoundEmitter soundEmitter)
        {
            soundEmitter.gameObject.SetActive(true);
            _activeSoundEmitter.Add(soundEmitter);
        }

        private SoundEmitter CreateSoundEmitter()
        {
            SoundEmitter soundEmitter = Instantiate(_soundEmitterPrefab);
            _soundEmitterPrefab.gameObject.SetActive(false);
            return soundEmitter;
        }

        private void InitializePool()
        {
            _soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                _collectionCheck,
                _defaultCapacity,
                _maxPoolSize);
        }
    }
}
