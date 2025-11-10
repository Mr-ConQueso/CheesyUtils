using System.Collections;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace CheesyUtils.FMod
{
    /// <summary>
    /// Wrapper around an FMOD EventInstance used by the AudioManager pool.
    /// Handles playback, positional attachment, parameter setting and release.
    /// </summary>
    [RequireComponent(typeof(Transform))]
    public class SoundEmitter : MonoBehaviour
    {
        private EventInstance _instance;
        private SoundData _data;
        private Coroutine _watchCoroutine;

        /// <summary>Initializes this emitter for the given sound data.</summary>
        public void Initialize(SoundData data)
        {
            _data = data;
            _instance = RuntimeManager.CreateInstance(data.EventRef);

            // If the event is authored to loop in FMOD Studio, we simply keep the instance alive
            // until Stop() is called. For non-looping events we will wait for playback to end
            // and then release the instance in WaitForEnd().
        }

        /// <summary>Play the event. If a world position is provided, attach the instance to this GameObject.</summary>
        public void Play(Vector3? position = null)
        {
            // Attach for 3D spatialization if a position is provided.
            if (position.HasValue)
            {
                transform.position = position.Value;
                RuntimeManager.AttachInstanceToGameObject(_instance, gameObject, transform);
            }

            _instance.start();

            // Stop any previous watcher
            if (_watchCoroutine != null)
            {
                StopCoroutine(_watchCoroutine);
                _watchCoroutine = null;
            }

            // If the event is not a looping event, watch for it to finish and then return to pool.
            if (!_data.Loop)
            {
                _watchCoroutine = StartCoroutine(WaitForEnd());
            }
        }

        /// <summary>Stops the event immediately and returns this emitter to the pool.</summary>
        public void Stop()
        {
            // Stop playback
            _instance.stop(STOP_MODE.IMMEDIATE);

            // Ensure any coroutine monitoring playback is stopped
            if (_watchCoroutine != null)
            {
                StopCoroutine(_watchCoroutine);
                _watchCoroutine = null;
            }

            // Release the FMOD instance and return to pool
            _instance.release();
            AudioManager.Instance.ReturnToPool(this);
        }

        /// <summary>Set an FMOD parameter by name on the active instance.</summary>
        public void WithParameter(string name, float value)
        {
            if (_instance.isValid())
            {
                _instance.setParameterByName(name, value);
            }
        }

        /// <summary>Simple random pitch emulation: use an FMOD parameter if available, otherwise
        /// use pitch via the 'pitch' DSP parameter on the instance (not all events expose this).
        /// Use parameters authored in FMOD Studio for best results.</summary>
        public void WithRandomPitch(float min = -0.05f, float max = 0.05f)
        {
            // Prefer using a parameter named "Pitch" if the event exposes it.
            // Fallback: set a pitch parameter if present, otherwise do nothing.
            float delta = Random.Range(min, max);
            WithParameter("Pitch", 1f + delta);
        }

        /// <summary>Poll the instance until it stops, then release and return to pool.</summary>
        private IEnumerator WaitForEnd()
        {
            bool playing = true;
            while (playing)
            {
                // Query the playback state
                _instance.getPlaybackState(out PLAYBACK_STATE state);
                playing = state == PLAYBACK_STATE.PLAYING || state == PLAYBACK_STATE.SUSTAINING;
                yield return null;
            }

            // Release instance resources when finished
            _instance.release();
            _watchCoroutine = null;
            AudioManager.Instance.ReturnToPool(this);
        }
    }
}