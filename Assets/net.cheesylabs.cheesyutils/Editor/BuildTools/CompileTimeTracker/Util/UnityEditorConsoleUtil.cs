using System;
using System.Reflection;
using UnityEditor;

namespace CheesyUtils.Editor.BuildTools
{
    public struct UnityConsoleCountsByType
    {
        public int ErrorCount;
        public int WarningCount;
        public int LOGCount;
    }

    public static class UnityEditorConsoleUtil
    {
        private static readonly MethodInfo ClearMethod;
        private static readonly MethodInfo GetCountMethod;
        private static readonly MethodInfo GetCountsByTypeMethod;

        static UnityEditorConsoleUtil()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
#if UNITY_2017_1_OR_NEWER
            Type logEntriesType = assembly.GetType("UnityEditor.LogEntries");
#else 
            logEntriesType  = assembly.GetType("UnityEditorInternal.LogEntries");
#endif

            ClearMethod = logEntriesType.GetMethod("Clear");
            GetCountMethod = logEntriesType.GetMethod("GetCount");
            GetCountsByTypeMethod = logEntriesType.GetMethod("GetCountsByType");
        }

        public static void Clear()
        {
            if (ClearMethod == null)
            {
                Debug.LogError("Failed to find LogEntries.Clear method info!");
                return;
            }

            ClearMethod.Invoke(null, null);
        }

        public static int GetCount()
        {
            if (GetCountMethod != null) return (int)GetCountMethod.Invoke(null, null);
            
            Debug.LogError("Failed to find LogEntries.GetCount method info!");
            return 0;

        }

        public static UnityConsoleCountsByType GetCountsByType()
        {
            UnityConsoleCountsByType countsByType = new UnityConsoleCountsByType();

            if (GetCountsByTypeMethod == null)
            {
                Debug.LogError("Failed to find LogEntries.GetCountsByType method info!");
                return countsByType;
            }

            object[] arguments = new object[] { 0, 0, 0 };
            GetCountsByTypeMethod.Invoke(null, arguments);

            countsByType.ErrorCount = (int)arguments[0];
            countsByType.WarningCount = (int)arguments[1];
            countsByType.LOGCount = (int)arguments[2];

            return countsByType;
        }
    }
}
