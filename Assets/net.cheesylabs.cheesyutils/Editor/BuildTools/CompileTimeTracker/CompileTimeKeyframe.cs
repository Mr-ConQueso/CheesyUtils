using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace CheesyUtils.Editor.BuildTools
{
    [Serializable]
    public class CompileTimeKeyframe
    {
        private const string KKeyframeDelimiter = "@";
        private const string KListDelimiter = "#";

        private static readonly string[] KKeyframeDelimiterArray = new string[] { KKeyframeDelimiter };
        private static readonly string[] KListDelimiterArray = new string[] { KListDelimiter };

        public static CompileTimeKeyframe Deserialize(string serialized)
        {
            string[] tokens = serialized.Split(KKeyframeDelimiterArray, StringSplitOptions.None);
            if (tokens.Length != 3)
            {
                Debug.LogError("Failed to deserialize CompileTimeKeyframe because splitting by " + KKeyframeDelimiter + " did not result in 3 tokens!");
                return null;
            }
            
            CompileTimeKeyframe keyframe = new CompileTimeKeyframe();
            keyframe.ElapsedCompileTimeInMS = Convert.ToInt32(tokens[0]);
            keyframe.SerializedDate = tokens[1];
            keyframe.HadErrors = Convert.ToBoolean(tokens[2]);

            return keyframe;
        }

        public static string Serialize(CompileTimeKeyframe keyframe)
        {
            return keyframe == null ? "" : string.Format("{1}{0}{2}{0}{3}", KKeyframeDelimiter, keyframe.ElapsedCompileTimeInMS, keyframe.SerializedDate, keyframe.HadErrors);
        }

        public static List<CompileTimeKeyframe> DeserializeList(string serialized)
        {
            if (string.IsNullOrEmpty(serialized))
            {
                return new List<CompileTimeKeyframe>();
            }

            string[] serializedKeyframes = serialized.Split(KListDelimiterArray, StringSplitOptions.None);

            return serializedKeyframes.Select(s => Deserialize(s)).Where(k => k != null).ToList();
        }

        public static string SerializeList(List<CompileTimeKeyframe> keyframes)
        {
            string[] serializedKeyframes = keyframes.Where(k => k != null).Select(k => Serialize(k)).ToArray();
            return string.Join(KListDelimiter, serializedKeyframes);
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static string ToCSV(CompileTimeKeyframe keyframe)
        {
            return keyframe == null ? "" : string.Format("{0},{1},{2}", keyframe._computedDate, keyframe.ElapsedCompileTimeInMS, keyframe.HadErrors);
        }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public static string ToCSV(List<CompileTimeKeyframe> keyframes)
        {
            string[] serializedKeyframes = keyframes.Where(k => k != null).Select(k => ToCSV(k)).ToArray();
            string fields = "date,compile_time,had_errors\n";
            return fields + string.Join("\n", serializedKeyframes);
        }


        // PRAGMA MARK - Public Interface
        public DateTime Date
        {
            get
            {
                if (this._computedDate != null) return this._computedDate.Value;
                this._computedDate = string.IsNullOrEmpty(this.SerializedDate) ? DateTime.MinValue : DateTime.Parse(this.SerializedDate);

                return this._computedDate.Value;
            }
        }

        public int ElapsedCompileTimeInMS = 0;
        public string SerializedDate = "";
        public bool HadErrors = false;

        private CompileTimeKeyframe() {}

        public CompileTimeKeyframe(int elapsedCompileTimeInMS, bool hadErrors)
        {
            this.ElapsedCompileTimeInMS = elapsedCompileTimeInMS;
            this.SerializedDate = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            this.HadErrors = hadErrors;
        }

        [NonSerialized] private DateTime? _computedDate;
    }
}
