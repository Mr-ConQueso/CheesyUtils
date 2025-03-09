using System;
using UnityEditor;

namespace CheesyUtils.Editor.BuildTools
{
    [InitializeOnLoad]
    public static class EditorApplicationCompilationUtil
    {
        public static event Action StartedCompiling = delegate {};
        public static event Action FinishedCompiling = delegate {};

        static EditorApplicationCompilationUtil()
        {
            EditorApplication.update += OnEditorUpdate;
        }


        private static bool StoredCompilingState
        {
            get => EditorPrefs.GetBool("EditorApplicationCompilationUtil::StoredCompilingState");
            set => EditorPrefs.SetBool("EditorApplicationCompilationUtil::StoredCompilingState", value);
        }

        private static void OnEditorUpdate()
        {
            switch (EditorApplication.isCompiling)
            {
                case true when StoredCompilingState == false:
                    StoredCompilingState = true;
                    StartedCompiling.Invoke();
                    break;
                case false when StoredCompilingState == true:
                    StoredCompilingState = false;
                    FinishedCompiling.Invoke();
                    break;
            }
        }
    }
}