using FMODUnity;
using UnityEngine;

namespace CheesyUtils.FMod
{
    public class FModEvents : MonoBehaviour
    {
        [Header("Sound Effects")]
        [field: SerializeField] public EventReference CoinCollected { get; private set; }
        [field: SerializeField] public EventReference PlayerFootsteps { get; private set; }
    }
}