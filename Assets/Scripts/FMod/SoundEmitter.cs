using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace CheesyUtils.FMod
{
    /// <summary>
    /// Wrapper around an FMOD StudioEventEmitter used by the AudioManager pool.
    /// Handles playback, positional attachment, parameter setting and release.
    /// </summary>
    [RequireComponent(typeof(Transform))]
    [RequireComponent(typeof(StudioEventEmitter))]
    public class SoundEmitter : MonoBehaviour
    {
        private SoundData _data;
        private Coroutine _watchCoroutine;
        private StudioEventEmitter _emitter;

        /// <summary>Initializes this emitter for the given sound data.</summary>
        public void Initialize(SoundData data)
        {
            _data = data;
            _emitter = GetComponent<StudioEventEmitter>();
            _emitter.EventReference = data.EventRef;

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
            }

            _emitter.Play();

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
            _emitter.Stop();

            // Ensure any coroutine monitoring playback is stopped
            if (_watchCoroutine != null)
            {
                StopCoroutine(_watchCoroutine);
                _watchCoroutine = null;
            }

            // Release the FMOD instance and return to pool
            _emitter.EventInstance.release();
            AudioManager.Instance.ReturnToPool(this);
        }

        /// <summary>Set a FMOD parameter by name on the active instance.</summary>
        public void WithParameter(FModParameter parameter)
        {
            if (!_emitter) return;

            _emitter.SetParameter(parameter.Name, parameter.Value);
        }

        /// <summary>Set a list of FMOD parameters by name on the active instance.</summary>
        public void WithParameters(List<FModParameter> parameters)
        {
            if (!_emitter) return;

            foreach (var param in parameters)
            {
                _emitter.SetParameter(param.Name, param.Value);
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
            WithParameter(new FModParameter
            {
                Name = "Pitch",
                Value = 1.2f
            });
        }

        /// <summary>Poll the instance until it stops, then release and return to pool.</summary>
        private IEnumerator WaitForEnd()
        {
            bool playing = true;
            while (playing)
            {
                // Query the playback state
                _emitter.EventInstance.getPlaybackState(out PLAYBACK_STATE state);
                playing = state is PLAYBACK_STATE.PLAYING or PLAYBACK_STATE.SUSTAINING;
                yield return null;
            }

            // Release instance resources when finished
            _emitter.EventInstance.release();
            _watchCoroutine = null;
            AudioManager.Instance.ReturnToPool(this);
        }
    }
}