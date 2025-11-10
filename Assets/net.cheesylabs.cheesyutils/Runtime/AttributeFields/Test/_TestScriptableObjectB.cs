using UnityEngine;
using System.Collections.Generic;
using CheesyUtils.CheesyAttributes;

namespace CheesyUtils.CheesyAttributes.Test
{
    //[CreateAssetMenu(fileName = "TestScriptableObjectB", menuName = "NaughtyAttributes/TestScriptableObjectB")]
    public class _TestScriptableObjectB : ScriptableObject
    {
        [MinMaxSlider(0, 10)]
        public Vector2Int slider;
    }
}