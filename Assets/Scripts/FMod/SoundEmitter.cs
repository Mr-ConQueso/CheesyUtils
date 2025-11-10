using System.Collections;
using UnityEngine;

namespace CheesyUtils.FMod
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        public SoundData data { get; private set; }
        private AudioSource _audioSource;
        private Coroutine _playingCoroutine;

        private void Awake()
        {
            _audioSource = gameObject.GetOrAdd<AudioSource>();
        }
        
        public void Initialize(SoundData incomingData)
        {
            this.data = incomingData;
            _audioSource.clip = incomingData.Clip;
            _audioSource.outputAudioMixerGroup = incomingData.MixerGroup;
            _audioSource.loop = incomingData.Loop;
            _audioSource.playOnAwake = incomingData.PlayOnAwake;
        }

        public void Play()
        {
            if (_playingCoroutine != null)
            {
                StopCoroutine(_playingCoroutine);
            }

            _audioSource.Play();
            _playingCoroutine = StartCoroutine(WaitForSoundToEnd());
        }

        public void Stop()
        {
            if (_playingCoroutine != null)
            {
                StopCoroutine(_playingCoroutine);
                _playingCoroutine = null;
            }

            _audioSource.Stop();
            AudioManager.Instance.ReturnToPool(this);
        }

        public void WithRandomPitch(float min = -0.05f, float max = 0.05f)
        {
            _audioSource.pitch += Random.Range(min, max);
        }

        private IEnumerator WaitForSoundToEnd()
        {
            yield return new WaitWhile(() => _audioSource.isPlaying);
            AudioManager.Instance.ReturnToPool(this);
        }
    }
}