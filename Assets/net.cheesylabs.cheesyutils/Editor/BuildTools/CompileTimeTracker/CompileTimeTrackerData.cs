using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CheesyUtils.Editor.BuildTools
{
    public class CompileTimeTrackerData
    {
        private const int KHistoryKeyframeMaxCount = 100;

        public int StartTime
        {
            get => this._startTime;
            set
            {
                this._startTime = value;
                this.Save();
            }
        }

        public void AddCompileTimeKeyframe(CompileTimeKeyframe keyframe)
        {
            this._compileTimeHistory.Add(keyframe);
            this.Save();
        }

        public IList<CompileTimeKeyframe> GetCompileTimeHistory()
        {
            return this._compileTimeHistory;
        }

        public CompileTimeTrackerData(string editorPrefKey)
        {
            this._editorPrefKey = editorPrefKey;
            this.Load();
        }


        [SerializeField] private int _startTime;
        [SerializeField] private List<CompileTimeKeyframe> _compileTimeHistory;

        private readonly string _editorPrefKey;

        private void Save()
        {
            while (this._compileTimeHistory.Count > KHistoryKeyframeMaxCount)
            {
                this._compileTimeHistory.RemoveAt(0);
            }

            EditorPrefs.SetInt(this._editorPrefKey + "._startTime", this._startTime);
            EditorPrefs.SetString(this._editorPrefKey + "._compileTimeHistory", CompileTimeKeyframe.SerializeList(this._compileTimeHistory));
        }

        private void Load()
        {
            this._startTime = EditorPrefs.GetInt(this._editorPrefKey + "._startTime");
            string key = this._editorPrefKey + "._compileTimeHistory";
            
            this._compileTimeHistory = EditorPrefs.HasKey(key) ? CompileTimeKeyframe.DeserializeList(EditorPrefs.GetString(key)) : new List<CompileTimeKeyframe>();
        }
    }
}