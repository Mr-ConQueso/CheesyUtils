using System.Collections.Generic;
using AptabaseSDK;
using UnityEngine;

namespace CheesyUtils.Cloud
{
    public class AnalyticsTest : MonoBehaviour
    {
        private void Start()
        {
            Aptabase.TrackEvent("app_started", new Dictionary<string, object>
            {
                {"hello", "world"}
            });
        }

        private void OnApplicationQuit()
        {
            Aptabase.Flush();
        }
    }
}
