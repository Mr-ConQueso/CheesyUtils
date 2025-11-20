using System;
using UnityEngine;

namespace CheesyUtils.CheesyAttributes.Test
{
    public class ReadOnlyTest : MonoBehaviour
    {
        [ReadOnly]
        public int readOnlyInt = 5;

        public ReadOnlyNest1 nest1;
    }

    [Serializable]
    public class ReadOnlyNest1
    {
        [ReadOnly]
        [AllowNesting]
        public float readOnlyFloat = 3.14f;

        public ReadOnlyNest2 nest2;
    }

    [Serializable]
    public struct ReadOnlyNest2
    {
        [ReadOnly]
        [AllowNesting]
        public string readOnlyString;
    }
}
