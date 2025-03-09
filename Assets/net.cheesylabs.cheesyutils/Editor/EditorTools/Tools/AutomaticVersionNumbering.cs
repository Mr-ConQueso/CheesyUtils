using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Build.Profile;
using UnityEditor.Callbacks;
using UnityEngine;

namespace CheesyUtils.EditorTools
{
    public enum VersionType
    {
        Alpha,
        Beta,
        Development,
        Release
    }

    [InitializeOnLoad]
    public static class AutomaticVersionNumbering
    {
        #region Constants

        internal const string IS_ENABLED_KEY = "automatic_version_numbering:is_enabled";
        internal const string VERSION_TYPE_KEY = "automatic_version_numbering:version_type";
        private const string PLUGIN_LOG_NAME = "Automatic Version Numbering";
        #endregion

        #region Menu items
        [MenuItem("Tools/Build Tools/Automatic Version Numbering")]
        private static void ShowSettingsWindow()
        {
            var window = EditorWindow.GetWindow<VersionSettingsWindow>();
            window.titleContent = new GUIContent("Version Settings");
            window.Show();
        }
        #endregion

        #region Callbacks
        [PostProcessBuild]
        private static void IncrementVersionNumber(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (!EditorPrefs.GetBool(IS_ENABLED_KEY, false))
                return;

            string currentVersion = PlayerSettings.bundleVersion;
            string newVersion = ConstructNewVersionString(currentVersion);

            if (string.IsNullOrEmpty(newVersion))
            {
                Debug.LogWarning($"{PLUGIN_LOG_NAME}: Version number was not increased. Invalid format in: '{currentVersion}'.");
                return;
            }

            // Update global version
            PlayerSettings.bundleVersion = newVersion;

            // Update all build profiles
            BuildProfile buildProfile = BuildProfile.GetActiveBuildProfile();
            
            // foreach (BuildProfile profile in buildProfiles)
            // {
            //     profile.bundleVersion = newVersion;
            //     EditorUtility.SetDirty(profile);
            // }

            AssetDatabase.SaveAssets();
        }
        #endregion

        #region Private methods
        private static string ConstructNewVersionString(string oldVersionString)
        {
            Match match = Regex.Match(oldVersionString, @"(\d+)\.(\d+)(?!.*\d+\.\d+)");
            if (!match.Success) return null;

            int major = int.Parse(match.Groups[1].Value);
            int minor = int.Parse(match.Groups[2].Value);

            if (minor >= 19)
            {
                major++;
                minor = 0;
            }
            else
            {
                minor++;
            }

            VersionType versionType = (VersionType)EditorPrefs.GetInt(VERSION_TYPE_KEY, (int)VersionType.Alpha);
            return $"{versionType.ToString().ToLower()}-v{major}.{minor}";
        }
        #endregion
    }

    public class VersionSettingsWindow : EditorWindow
    {
        private void OnGUI()
        {
            bool isEnabled = EditorPrefs.GetBool(AutomaticVersionNumbering.IS_ENABLED_KEY, false);
            VersionType versionType = (VersionType)EditorPrefs.GetInt(AutomaticVersionNumbering.VERSION_TYPE_KEY, (int)VersionType.Alpha);

            EditorGUI.BeginChangeCheck();
            
            isEnabled = EditorGUILayout.Toggle("Enable Automatic Versioning", isEnabled);
            versionType = (VersionType)EditorGUILayout.EnumPopup("Version Type", versionType);

            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetBool(AutomaticVersionNumbering.IS_ENABLED_KEY, isEnabled);
                EditorPrefs.SetInt(AutomaticVersionNumbering.VERSION_TYPE_KEY, (int)versionType);
            }

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox("Version format: [type]-v[major].[minor]\nExample: beta-v1.19 → beta-v2.0", MessageType.Info);
        }
    }
}