using UnityEngine;
using UnityEngine.UI;

// Usage: attach this to UI Text component
// it displays current version number from Ios/Android player settings with Application.version
namespace CheesyUtils
{
    public class GetVersion : MonoBehaviour
    {
        private void Awake()
        {
            var t = GetComponent<Text>();
            if (t != null)
            {
                t.text = "v" + Application.version;
            }
        }
    }
}