using System;
using UnityEngine;

namespace CheesyUtils.CheesyAttributes.Test
{
    public class ExpandableTest : MonoBehaviour
    {
        // See #294
        public int precedingField = 5;

        [Expandable]
        public ScriptableObject obj0;

        public ExpandableScriptableObjectNest1 nest1;
    }

    [Serializable]
    public class ExpandableScriptableObjectNest1
    {
        [Expandable]
        public ScriptableObject obj1;

        public ExpandableScriptableObjectNest2 nest2;
    }

    [Serializable]
    public class ExpandableScriptableObjectNest2
    {
        [Expandable]
        public ScriptableObject obj2;
    }
}
