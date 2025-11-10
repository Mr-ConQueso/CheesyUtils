using UnityEngine;

namespace CheesyUtils.Audio
{
    public class MusicController : MonoBehaviour
    {
        // ---- / Serialized Variables / ---- //
        [SerializeField] private SoundData _musicData;

        private void Start()
        {
            AudioManager.Instance.CreateSound()
                .WithSoundData(_musicData)
                .WithRandomPitch(false)
                .WithPosition(transform.position)
                .Play();
        }
    }
}
