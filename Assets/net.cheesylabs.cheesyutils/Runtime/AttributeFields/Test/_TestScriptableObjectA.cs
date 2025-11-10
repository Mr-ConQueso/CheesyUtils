using UnityEngine;
using System.Collections.Generic;
using CheesyUtils.CheesyAttributes;

namespace CheesyUtils.CheesyAttributes.Test
{
    //[CreateAssetMenu(fileName = "TestScriptableObjectA", menuName = "NaughtyAttributes/TestScriptableObjectA")]
    public class _TestScriptableObjectA : ScriptableObject
    {
        [Expandable]
        public List<_TestScriptableObjectB> listB;
    }
}