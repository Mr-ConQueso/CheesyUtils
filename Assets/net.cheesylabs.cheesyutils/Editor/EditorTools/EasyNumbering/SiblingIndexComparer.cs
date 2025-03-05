using System.Collections;
using UnityEngine;

namespace CheesyUtils.EditorTools.EasyNumbering
{
    public class SiblingIndexComparer : IComparer
    {
        public int Compare(object obj, object compareTo)
        {
            GameObject go1 = (GameObject)obj;
            GameObject go2 = (GameObject)compareTo;

            int index1 = go1.transform.GetSiblingIndex();
            int index2 = go2.transform.GetSiblingIndex();

            return index1.CompareTo(index2);
        }
    }
}
