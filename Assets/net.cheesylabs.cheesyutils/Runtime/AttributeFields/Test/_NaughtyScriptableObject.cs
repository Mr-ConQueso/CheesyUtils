using System.Collections.Generic;
using CheesyUtils.CheesyAttributes;
using UnityEngine;

namespace CheesyUtils.CheesyAttributes.Test
{
    //[CreateAssetMenu(fileName = "NaughtyScriptableObject", menuName = "NaughtyAttributes/_NaughtyScriptableObject")]
    public class _NaughtyScriptableObject : ScriptableObject
    {
        [Expandable]
        public List<_TestScriptableObjectA> listA;
    }
}
