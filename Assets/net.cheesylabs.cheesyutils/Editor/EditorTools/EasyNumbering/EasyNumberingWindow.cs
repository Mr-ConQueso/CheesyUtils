using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CheesyUtils.EditorTools.EasyNumbering
{
    public class EasyNumberingWindow : EditorWindow
    {
        private enum AffixType { Suffix, Prefix }

        #region Constants and read only fields

        #region GUIContent
        private static readonly GUIContent _useCurrentNameGUIContent = new GUIContent("Use current name", "Whether the numbering affix should be applied to the Objects current names or the names should be overwriten completly.");
        private static readonly GUIContent _baseNameGUIContent = new GUIContent("Base name", "The base name for the selected Objects. Numbering affix will be applied to this name.");
        private static readonly GUIContent _startValueGUIContent = new GUIContent("Start value", "The value from which numbering begins.");
        private static readonly GUIContent _useSelectionOrderGUIContent = new GUIContent("Use selection order", "Use the order in which the Objects were selected instead of their order in the scene hierarchy.\n\nNote: Only works for GameObjects in the scene hierarchy. Due to the way Unity orders the selection when selecting multiple GameObjects it only works if you 'CTLR+Click' select the GameObjects you want to apply numbering to!");
        private static readonly GUIContent _affixTypeGUIContent = new GUIContent("Affix type", "Whether numbers should be applied in front (prefix) or after (suffix) the base name.");
        private static readonly GUIContent _templateGUIContent = new GUIContent("Template", "The template used when numbering the Objects. Use zero (0) to indicate the number of digits. All zeroes will be considered part of the same number. \n\nFor example, an Object numbered 11:\n'_000' will show '_011'\n'_0_0_0' will show '_0_1_1'");
        private static readonly GUIContent _overflowGUIContent = new GUIContent("Overflow", "Whether the numbers should overflow the specified number of digits or just loop back to the start.");
        #endregion

        #region EditorPrefs keys
        private const string USE_CURRENT_NAME_KEY = "easy_numbering:use_current_name";
        private const string START_VALUE_KEY = "easy_numbering:start_value";
        private const string USE_SELECTION_ORDER_KEY = "easy_numbering:use_selection_order";
        private const string AFFIX_TYPE_KEY = "easy_numbering:affix_type";
        private const string TEMPLATE_KEY = "easy_numbering:template";
        private const string OVERFLOW_KEY = "easy_numbering:overflow";
        #endregion

        #region Default settings
        private const bool DEFAULT_USE_CURRENT_NAME = false;
        private const int DEFAULT_START_VALUE = 1;
        private const bool DEFAULT_USE_SELECTION_ORDER = false;
        private const AffixType DEFAULT_AFFIX_TYPE = AffixType.Suffix;
        private const string DEFAULT_TEMPLATE = " (0)";
        private const bool DEFAULT_OVERFLOW = true;
        #endregion

        #region Hotkeys
        // these hotkeys are for when the window is open only
        private const KeyCode APPLY_NUMBERING_HOTKEY = KeyCode.Return;
        private const KeyCode APPLY_NUMBERING_ALTERNATE_HOTKEY = KeyCode.KeypadEnter;
        private const KeyCode CANCEL_NUMBERING_HOTKEY = KeyCode.Escape;
        #endregion

        #region Other
        private const string PLUGIN_LOG_NAME = "Easy Numbering";
        private static readonly GUIStyle _italicLabelStyle = new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Italic };
        #endregion

        #endregion

        #region Private fields

        private static event Action _settingReset;

        private static Object[] _selection;
        private static string[] _selectionAssetGUIDs;

        private int _startValue = DEFAULT_START_VALUE;

        private bool _useCurrentName = DEFAULT_USE_CURRENT_NAME;
        private bool _useSelectionOrder = DEFAULT_USE_SELECTION_ORDER;
        private bool _overflow = DEFAULT_OVERFLOW;

        private string _baseName;
        private string _template = DEFAULT_TEMPLATE;

        private AffixType _affixType = DEFAULT_AFFIX_TYPE;

        #endregion

        #region Properties

        private bool SelectionIsAssets => _selectionAssetGUIDs.Length > 0;

        #endregion

        #region Menu item static methods

        /// <summary>
        /// Opens the numbering window.
        /// Hotkeys is defined with symbols at the end of the menu name. Default hotkeys is SHIFT + N.
        /// </summary>
        [MenuItem("Tools/Renaming/Apply numbering #n")]
        private static void OpenWindow()
        {
            if (Selection.count <= 0)
                return;

            _selection = Selection.objects;
            _selectionAssetGUIDs = Selection.assetGUIDs;

            var myWindow = GetWindow<EasyNumberingWindow>(true);
            myWindow.titleContent = new GUIContent("Numbering settings");
            myWindow.minSize = new Vector2(400, 235);
            myWindow.maxSize = new Vector2(400, 235);

            myWindow._useCurrentName = EditorPrefs.GetBool(USE_CURRENT_NAME_KEY, DEFAULT_USE_CURRENT_NAME);
            myWindow._startValue = EditorPrefs.GetInt(START_VALUE_KEY, DEFAULT_START_VALUE);
            myWindow._useSelectionOrder = EditorPrefs.GetBool(USE_SELECTION_ORDER_KEY, DEFAULT_USE_SELECTION_ORDER);
            myWindow._affixType = (AffixType)EditorPrefs.GetInt(AFFIX_TYPE_KEY, (int)DEFAULT_AFFIX_TYPE);
            myWindow._template = EditorPrefs.GetString(TEMPLATE_KEY, DEFAULT_TEMPLATE);
            myWindow._overflow = EditorPrefs.GetBool(OVERFLOW_KEY, DEFAULT_OVERFLOW);

            myWindow._baseName = _selection[0].name;

            _settingReset += myWindow.OnSettingsReset;

            myWindow.ShowUtility();
        }

        /// <summary>
        /// Validate method for the OpenWindow menu item.
        /// </summary>
        /// <returns>Returns true when selection has any items. Otherwise returns false.</returns>
        [MenuItem("Tools/Renaming/Apply numbering #n", true)]
        private static bool OpenWindowValidate()
        {
            return Selection.count > 0;
        }

        /// <summary>
        /// Resets all current and saved settings to their default values.
        /// </summary>
        [MenuItem("Tools/Renaming/Reset settings to default")]
        private static void ResetSettingsToDefault()
        {
            EditorPrefs.SetBool(USE_CURRENT_NAME_KEY, DEFAULT_USE_CURRENT_NAME);
            EditorPrefs.SetInt(START_VALUE_KEY, DEFAULT_START_VALUE);
            EditorPrefs.SetBool(USE_SELECTION_ORDER_KEY, DEFAULT_USE_SELECTION_ORDER);
            EditorPrefs.SetInt(AFFIX_TYPE_KEY, (int)DEFAULT_AFFIX_TYPE);
            EditorPrefs.SetString(TEMPLATE_KEY, DEFAULT_TEMPLATE);
            EditorPrefs.SetBool(OVERFLOW_KEY, DEFAULT_OVERFLOW);

            _settingReset?.Invoke();

            Debug.Log($"{PLUGIN_LOG_NAME}: Settings was reset to defaults.");
        }

        #endregion

        #region Unity methods

        private void OnGUI()
        {
            // cache the default colors of the GUI
            var oldColor = GUI.color;
            var oldBackgroundColor = GUI.backgroundColor;

            _useCurrentName = EditorGUILayout.Toggle(_useCurrentNameGUIContent, _useCurrentName);

            EditorGUI.BeginDisabledGroup(_useCurrentName);

            _baseName = EditorGUILayout.TextField(_baseNameGUIContent, _baseName);
            
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(10);

            _startValue = EditorGUILayout.IntField(_startValueGUIContent, _startValue);

            EditorGUI.BeginDisabledGroup(SelectionIsAssets);

            _useSelectionOrder = EditorGUILayout.Toggle(_useSelectionOrderGUIContent, _useSelectionOrder);

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.Space(10);

            _affixType = (AffixType)EditorGUILayout.EnumPopup(_affixTypeGUIContent, _affixType);
            _template = EditorGUILayout.TextField(_templateGUIContent, _template);

            EditorGUILayout.Space(5);

            #region Template preview

            // find out if the current template is valid
            string labelText;
            bool isTemplateValid = VerifyTemplate();

            // if the template is valid, show preview
            if (isTemplateValid)
            {
                GUI.color = new Color(0.75f, 0.75f, 0.75f);
                labelText = "PREVIEW: " + (_affixType == AffixType.Suffix ? $"GameObject{GetAffix(123)}" : $"{GetAffix(123)}GameObject");
            }
            // if the template was invalid, show error
            else
            {
                GUI.color = Color.red;
                labelText = "ERROR: The template cannot be empty!";
            }

            // preview or error label
            GUILayout.Label(labelText, _italicLabelStyle);
            GUI.color = oldColor;

            #endregion

            EditorGUILayout.Space(5);

            _overflow = EditorGUILayout.Toggle(_overflowGUIContent, _overflow);

            EditorGUILayout.Space(10);

            #region Buttons

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.Space();

            GUI.backgroundColor = new Color(0.75f, 0, 0);
            GUI.color = Color.red;

            if (GUILayout.Button("Cancel", GUILayout.MaxWidth(100)))
            {
                Close();
            }

            EditorGUILayout.Space();

            GUI.backgroundColor = new Color(0, 0.75f, 0);
            GUI.color = Color.green;

            EditorGUI.BeginDisabledGroup(!isTemplateValid);

            if (GUILayout.Button("Apply", GUILayout.MaxWidth(100)))
            {
                Apply();
            }

            EditorGUI.EndDisabledGroup();

            GUI.backgroundColor = oldBackgroundColor;
            GUI.color = oldColor;

            EditorGUILayout.Space();

            EditorGUILayout.EndHorizontal();
            
            #endregion

            #region Handle input

            // if the current event is null or not a key event, just return
            if (Event.current == null || !Event.current.isKey)
                return;

            // if the escape key was pressed, close the window without applying the numbering
            if (Event.current.keyCode == CANCEL_NUMBERING_HOTKEY)
                Close();

            // if the return/enter key was pressed and the template is valid, apply the numbering and close the window
            if ((Event.current.keyCode == APPLY_NUMBERING_HOTKEY || Event.current.keyCode == APPLY_NUMBERING_ALTERNATE_HOTKEY) && isTemplateValid)
                Apply();

            #endregion
        }

        private void OnLostFocus()
        {
            // if the window looses focus, close the it without applying the numbering
            Close();
        }

        private void OnDestroy()
        {
            _settingReset -= OnSettingsReset;
        }

        #endregion

        #region Event hanlders

        private void OnSettingsReset()
        {
            _useCurrentName = DEFAULT_USE_CURRENT_NAME;
            _startValue = DEFAULT_START_VALUE;
            _useSelectionOrder = DEFAULT_USE_SELECTION_ORDER;
            _affixType = DEFAULT_AFFIX_TYPE;
            _template = DEFAULT_TEMPLATE;
            _overflow = DEFAULT_OVERFLOW;

            Repaint();
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Hide the build in Close method and replace it with one that also saves the settings, before closing the window
        /// </summary>
        private new void Close()
        {
            SaveCurrentSettings();

            base.Close();
        }

        /// <summary>
        /// Applies the numbering to the selected objects.
        /// </summary>
        private void Apply()
        {
            if (SelectionIsAssets)
            {
                var success = RenameAssets();

                // if renaming was canceled, return
                if (!success)
                    return;
            }
            else
                RenameGameObjects();

            Close();
        }

        /// <summary>
        /// Method for renaming asset files. Does not support undo.
        /// </summary>
        /// <returns>Returns true if renaming was completed, and returns false if the user canceled.</returns>
        private bool RenameAssets()
        {
            // Warn the user that they cannot undo asset renaming
            if (!EditorUtility.DisplayDialog("Warning", "You are about to rename asset files. This action cannot be undone. Are you sure you want to continue?", "Yes", "No"))
                return false;

            // Capture the original base name once to prevent dynamic updates
            string originalBaseName = _useCurrentName ? _selection[0].name : _baseName;

            for (int i = 0; i < _selection.Length; i++)
            {
                // Use the original base name for all items to avoid duplication
                var newName = originalBaseName;
                var affix = GetAffix(_startValue + i);

                if (_affixType == AffixType.Prefix)
                    newName = affix + newName;
                else
                    newName = newName + affix;

                string assetPath = AssetDatabase.GUIDToAssetPath(_selectionAssetGUIDs[i]);
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);

                AssetDatabase.RenameAsset(assetPath, newName);
                EditorUtility.SetDirty(asset);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"{PLUGIN_LOG_NAME}: Applied numbering to '{_selection.Length}' assets.");
            return true;
        }

        /// <summary>
        /// Method for renaming GameObjects in the scene. Supports undo.
        /// </summary>
        private void RenameGameObjects()
        {
            if (!_useSelectionOrder)
                Array.Sort(_selection, new SiblingIndexComparer());

            // Capture the original base name once to prevent dynamic updates
            string originalBaseName = _useCurrentName ? _selection[0].name : _baseName;

            Undo.SetCurrentGroupName($"Applied numbering to '{_selection.Length}' objects.");
            int undoIndex = Undo.GetCurrentGroup();

            for (int i = 0; i < _selection.Length; i++)
            {
                // Use the original base name for all items to avoid duplication
                var newName = originalBaseName;
                var affix = GetAffix(_startValue + i);

                if (_affixType == AffixType.Prefix)
                    newName = affix + newName;
                else
                    newName = newName + affix;

                Undo.RecordObject(_selection[i], $"Renamed object from '{_selection[i].name}' to '{newName}'");
                _selection[i].name = newName;
            }

            Undo.CollapseUndoOperations(undoIndex);
            Debug.Log($"{PLUGIN_LOG_NAME}: Applied numbering to '{_selection.Length}' GameObjects.");
        }

        /// <summary>
        /// Constructs the affix that is to be added to the name.
        /// </summary>
        /// <param name="number">The number the affix shuould include</param>
        /// <returns>Returns the affix build from the template.</returns>
        private string GetAffix(int number)
        {
            if (string.IsNullOrEmpty(_template))
                _template = DEFAULT_TEMPLATE;

            // If template contains no numbers, return it as static text
            if (!_template.Contains("0"))
                return _template;

            string numberString = number.ToString();
            string affix = _template;
            int lastInsertIndex = -1;

            for (int i = _template.Length - 1; i >= 0; i--)
            {
                if (_template[i] == '0')
                {
                    if (numberString.Length > 0)
                    {
                        lastInsertIndex = i;

                        affix = affix.Remove(i, 1);
                        affix = affix.Insert(i, numberString[numberString.Length - 1].ToString());

                        numberString = numberString.Remove(numberString.Length - 1);
                    }
                }
            }

            if (_overflow && numberString.Length > 0 && lastInsertIndex != -1)
                affix = affix.Insert(lastInsertIndex, numberString);

            return affix;
        }

        /// <summary>
        /// Saves all the settings to EditorPrefs.
        /// </summary>
        private void SaveCurrentSettings()
        {
            EditorPrefs.SetBool(USE_CURRENT_NAME_KEY, _useCurrentName);
            EditorPrefs.SetInt(START_VALUE_KEY, _startValue);
            EditorPrefs.SetBool(USE_SELECTION_ORDER_KEY, _useSelectionOrder);
            EditorPrefs.SetInt(AFFIX_TYPE_KEY, (int)_affixType);
            // only save the template if it is valid
            if (VerifyTemplate())
                EditorPrefs.SetString(TEMPLATE_KEY, _template);
            EditorPrefs.SetBool(OVERFLOW_KEY, _overflow);
        }

        /// <summary>
        /// Returns wheter or not the current template is valid.
        /// </summary>
        /// <returns>Returns 'true' if the template is valid and 'false' otherwise.</returns>
        private bool VerifyTemplate()
        {
            return _template.Length > 0;
        }

        #endregion
    }
}
