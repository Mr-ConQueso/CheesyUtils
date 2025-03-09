using System;
using System.Collections.Generic;
using UnityEditor;

namespace CheesyUtils.Editor.BuildTools
{
    [InitializeOnLoad]
    public static class CompileTimeTracker
    {
        public static event Action<CompileTimeKeyframe> KeyframeAdded = delegate {};

        public static IList<CompileTimeKeyframe> GetCompileTimeHistory()
        {
            return Data.GetCompileTimeHistory();
        }

        static CompileTimeTracker()
        {
            EditorApplicationCompilationUtil.StartedCompiling += HandleEditorStartedCompiling;
            EditorApplicationCompilationUtil.FinishedCompiling += HandleEditorFinishedCompiling;
        }


        private const string KCompileTimeTrackerKey = "CompileTimeTracker::_data";
        private static CompileTimeTrackerData _data = null;

        private static CompileTimeTrackerData Data => _data ??= new CompileTimeTrackerData(KCompileTimeTrackerKey);

        private static int StoredErrorCount
        {
            get => EditorPrefs.GetInt("CompileTimeTracker::StoredErrorCount");
            set => EditorPrefs.SetInt("CompileTimeTracker::StoredErrorCount", value);
        }

        private static void HandleEditorStartedCompiling()
        {
            Data.StartTime = TrackingUtil.GetMilliseconds();

            UnityConsoleCountsByType countsByType = UnityEditorConsoleUtil.GetCountsByType();
            StoredErrorCount = countsByType.ErrorCount;
        }

        private static void HandleEditorFinishedCompiling()
        {
            int elapsedTime = TrackingUtil.GetMilliseconds() - Data.StartTime;

            UnityConsoleCountsByType countsByType = UnityEditorConsoleUtil.GetCountsByType();
            bool hasErrors = (countsByType.ErrorCount - StoredErrorCount) > 0;

            CompileTimeKeyframe keyframe = new CompileTimeKeyframe(elapsedTime, hasErrors);
            Data.AddCompileTimeKeyframe(keyframe);
            KeyframeAdded.Invoke(keyframe);
        }
    }
}