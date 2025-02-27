using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace EnlitGames.ScriptableObjectTable
{
    public class ScriptableObjectPreview : EditorWindow
    {
        private static readonly Color GreyBackgroundColor = new Color(0.16f, 0.16f, 0.16f);
        private static readonly Color DefaultBackgroundColor = new Color(0.2f, 0.2f, 0.2f);
        private static ScriptableObject _selectedScriptableObject;
        private static bool _showWarningForUndisplayedFields = false;
        private static bool _hideReadOnlyFields = false;
        private bool _scaleSwap = true;
        
        [MenuItem("Window/Scriptable Object Table")]
        public static void ShowExample()
        {
            var wnd = GetWindow<ScriptableObjectPreview>();
            wnd.titleContent = new GUIContent("Scriptable Object Table");
        }

        public void CreateGUI()
        {
            _showWarningForUndisplayedFields = false;
            VisualElement root = rootVisualElement;
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Plugins/ScriptableObjectTable/Editor/ScriptableObjectPreview.uxml");
            VisualElement scriptableObjectTable = visualTree.Instantiate();
            root.Add(scriptableObjectTable);
            

            ObjectField scriptableObjectSelection = root.Query<ObjectField>("ScriptableObjectSelection");
            scriptableObjectSelection.RegisterValueChangedCallback((evt) => { PopulateTable((ScriptableObject)evt.newValue); });
            scriptableObjectSelection.value = _selectedScriptableObject;

            Toggle hideReadOnlyFields = root.Query<Toggle>("HideReadOnlyFields");
            hideReadOnlyFields.RegisterValueChangedCallback((evt) => { HideReadOnlyFieldsToggled(evt.newValue); });
            hideReadOnlyFields.value = _hideReadOnlyFields;
        }

        [RuntimeInitializeOnLoadMethod]
        private static void OnLoad()
        {
            _selectedScriptableObject = null;
            _showWarningForUndisplayedFields = false;
            _hideReadOnlyFields = false;
        }

        private void HideReadOnlyFieldsToggled(bool newValue)
        {
            _hideReadOnlyFields = newValue;
            PopulateTable(_selectedScriptableObject);
            ForceUpdateScrollViewScale();
        }

        private void PopulateTable(ScriptableObject newSelectedScriptableObject)
        {
            VisualElement root = rootVisualElement;
            VisualElement scrollview = root.Query<ScrollView>("scroll-view-wrap-example");
            scrollview.Clear();
            if (newSelectedScriptableObject != null)
            {
                ScriptableObject scriptableObject = (ScriptableObject)newSelectedScriptableObject;
                _selectedScriptableObject = scriptableObject;
                ShowSelectedScriptableObject(scriptableObject, scrollview);
            }
        }

        private void ShowSelectedScriptableObject(ScriptableObject scriptableObject, VisualElement scrollview)
        {
            List<ScriptableObjectData> scriptableObjectDataList = GetScriptableObjectDataList(scriptableObject);

            float pathColumnWidth = ColumnWidthCalculator.FindScriptableObjectPathColumnWidth(scriptableObjectDataList);
            List<float> columnWidths = ColumnWidthCalculator.FindColumnWidths(scriptableObjectDataList);
            
            if(_showWarningForUndisplayedFields)
            {
                ShowWarningForUndisplayedFields();
            }
            else HideWarningForUndisplayedFields();

            ShowHeader(scriptableObjectDataList[0], scrollview, pathColumnWidth, columnWidths);
            for(int i = 0; i < scriptableObjectDataList.Count; i++)
            {
                bool colorRowGrey = i % 2 == 0;
                ShowScriptableObjectInstance(scriptableObjectDataList[i], scrollview, pathColumnWidth, columnWidths, colorRowGrey);
            }
        }

        private void ShowWarningForUndisplayedFields()
        {
            Label warning = rootVisualElement.Query<Label>("Warning");
            warning.text = "Some fields are not displayed because they are not serializable. You can make them serializable by adding the [SerializeField] attribute to them.";
        }

        private void HideWarningForUndisplayedFields()
        {
            Label warning = rootVisualElement.Query<Label>("Warning");
            warning.text = "";
        }

        private void ShowHeader(ScriptableObjectData scriptableObjectData, VisualElement scrollview, float pathColumnWidth, List<float> columnWidths)
        {
            VisualElement headerRow = new VisualElement();
            headerRow.style.flexDirection = FlexDirection.Row;
            scrollview.Add(headerRow);
            Label pathHeader = new Label("File Path");
            pathHeader.style.width = pathColumnWidth;
            headerRow.Add(pathHeader);
            for(int i = 0; i < scriptableObjectData.fields.Count; i++)
            {
                
                Label fieldHeader = new Label(scriptableObjectData.fields[i].Name);
                fieldHeader.style.width = columnWidths[i];

                headerRow.Add(fieldHeader);
            }
        }

        private void ShowScriptableObjectInstance(ScriptableObjectData scriptableObjectData, VisualElement scrollview, float columnWidth, List<float> columnWidths, bool colorRowGrey)
        {
            VisualElement scriptableObjectInstanceRow = new VisualElement();
            scriptableObjectInstanceRow.style.flexDirection = FlexDirection.Row;
            scrollview.Add(scriptableObjectInstanceRow);
            Label pathLabel = new Label(scriptableObjectData.path)
            {
                style =
                {
                    backgroundColor = colorRowGrey ? GreyBackgroundColor : DefaultBackgroundColor,
                    width = columnWidth
                }
            };
            pathLabel.RegisterCallback<MouseUpEvent>((evt) => { Selection.activeObject = scriptableObjectData.scriptableObjectInstance; });

            scriptableObjectInstanceRow.Add(pathLabel);
            for(int i = 0; i < scriptableObjectData.fields.Count; i++)
            {
                VisualElement element = MakeVisualElementForValue(scriptableObjectData.fields[i].GetValue(scriptableObjectData.scriptableObjectInstance));
                string fieldName = scriptableObjectData.fields[i].Name;
                if(element is Label)
                {
                    element.RegisterCallback<MouseUpEvent>((evt) => { Selection.activeObject = scriptableObjectData.scriptableObjectInstance; });
                }
                element.tooltip = scriptableObjectData.fields[i].Name;
                
                SerializedObject so = new SerializedObject(scriptableObjectData.scriptableObjectInstance);

                if(element is IBindable bindable)
                {
                    SerializedProperty property = so.FindProperty(scriptableObjectData.fields[i].Name);
                    if(property != null)
                        bindable.BindProperty(property);
                    else 
                        Debug.LogWarning("Could not find property " + scriptableObjectData.fields[i].Name + " on " + scriptableObjectData.scriptableObjectInstance.name);
                }
                
                element.style.width = columnWidths[i];

                //set background of every second column to grey
                if(colorRowGrey)
                {
                    element.style.backgroundColor = GreyBackgroundColor;
                    //get children and change their background color
                    foreach(VisualElement child in element.Children())
                    {
                        child.style.backgroundColor = GreyBackgroundColor;
                        //get children and change their background color
                        foreach(VisualElement grandChild in child.Children())
                        {
                            grandChild.style.backgroundColor = GreyBackgroundColor;
                        }
                    }
                }
                else 
                {
                    element.style.backgroundColor = DefaultBackgroundColor;
                    //get children and change their background color
                    foreach(VisualElement child in element.Children())
                    {
                        child.style.backgroundColor = DefaultBackgroundColor;
                        //get children and change their background color
                        foreach(VisualElement grandChild in child.Children())
                        {
                            grandChild.style.backgroundColor = DefaultBackgroundColor;
                        }
                    }
                }
                
                scriptableObjectInstanceRow.Add(element);
            }
        }

        VisualElement MakeVisualElementForValue(dynamic value)
        {
            VisualElement visualElement;
            if ((object)value != null)
                visualElement = new Label(value.ToString());
            else 
            {
                value = null;
                visualElement = new Label("null");
            }
            if(value != null && value is Color or Color32)
            {
                visualElement = new ColorField();
                ((ColorField)visualElement).SetValueWithoutNotify(value);
            }
            if(value is Vector2)
            {
                visualElement = new Vector2Field();
                ((Vector2Field)visualElement).SetValueWithoutNotify(value);
            }
            if(value is Vector3)
            {
                visualElement = new Vector3Field();
                ((Vector3Field)visualElement).SetValueWithoutNotify(value);
            }
            if(value is Vector4)
            {
                visualElement = new Vector4Field();
                ((Vector4Field)visualElement).SetValueWithoutNotify(value);
            }
            if(value is Rect)
            {
                visualElement = new RectField();
                ((RectField)visualElement).SetValueWithoutNotify(value);
            }
            if(value is Bounds)
            {
                visualElement = new BoundsField();
                ((BoundsField)visualElement).SetValueWithoutNotify(value);
            }
            if(value != null && (value.GetType() == typeof(Transform) || value.GetType() == typeof(Object) || 
                                 value is GameObject || value.GetType() == typeof(Component) || 
                                 value.GetType().IsSubclassOf(typeof(ScriptableObject)) || value is Sprite))
            {
                visualElement = new ObjectField();
                ((ObjectField)visualElement).objectType = value.GetType();
                ((ObjectField)visualElement).SetValueWithoutNotify(value);
                ((ObjectField)visualElement).allowSceneObjects = false;
            }
            if(value != null && value.GetType() == typeof(AnimationCurve))
            {
                visualElement = new CurveField();
                ((CurveField)visualElement).SetValueWithoutNotify(value);
            }
            if(value != null && value.GetType() == typeof(Gradient))
            {
                visualElement = new GradientField();
                ((GradientField)visualElement).SetValueWithoutNotify(value);
            }
            if(value is LayerMask)
            {
                visualElement = new LayerMaskField();
                ((LayerMaskField)visualElement).SetValueWithoutNotify(value);
            }
            if(value is RectInt)
            {
                visualElement = new RectIntField();
                ((RectIntField)visualElement).SetValueWithoutNotify(value);
            }
            if(value is BoundsInt)
            {
                visualElement = new BoundsIntField();
                ((BoundsIntField)visualElement).SetValueWithoutNotify(value);
            }
            if(value.GetType() == typeof(Enum) || value.GetType().IsEnum)
            {
                visualElement = new EnumField();
                ((EnumField)visualElement).SetValueWithoutNotify(value);
            }
            if(value is bool)
            {
                visualElement = new Toggle();
                ((Toggle)visualElement).SetValueWithoutNotify(value);
            }
            if(value is int)
            {
                visualElement = new IntegerField();
                ((IntegerField)visualElement).SetValueWithoutNotify(value);
            }
            if(value is float)
            {
                visualElement = new FloatField();
                ((FloatField)visualElement).SetValueWithoutNotify(value);
            }
            if(value is double)
            {
                visualElement = new DoubleField();
                ((DoubleField)visualElement).SetValueWithoutNotify(value);
            }
            if(value is string or String or char)
            {
                visualElement = new TextField();
                ((TextField)visualElement).SetValueWithoutNotify(value);
            }
            
            return visualElement;
        }
        
        private static Type GetTypeFromName(string name)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var tt = assembly.GetType(name);
                if (tt != null)
                {
                    return tt;
                }
            }
            Debug.LogError("Type not found: " + name);

            return null;
        }

        private List<ScriptableObjectData> GetScriptableObjectDataList(ScriptableObject scriptableObject)
        {
            Type scriptableObjectType = GetTypeFromName(scriptableObject.GetType().FullName);
            var fields = GetFieldsToDisplay(scriptableObject);
            
            List<ScriptableObjectData> scriptableObjectDataList = new List<ScriptableObjectData>();
            string[] scriptableObjectPaths = AssetDatabase.FindAssets("t:" + scriptableObject.GetType().FullName);
            foreach (string scriptableObjectPath in scriptableObjectPaths)
            {
                ScriptableObjectData scriptableObjectData = new ScriptableObjectData
                {
                    name = AssetDatabase.GUIDToAssetPath(scriptableObjectPath),
                    type = scriptableObjectType.ToString(),
                    path = AssetDatabase.GUIDToAssetPath(scriptableObjectPath),
                    scriptableObjectInstance = (ScriptableObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(scriptableObjectPath), scriptableObjectType)
                };

                scriptableObjectData.fields.AddRange(fields);
                
                scriptableObjectDataList.Add(scriptableObjectData);
            }

            return scriptableObjectDataList;
        }

        private List<FieldInfo> GetFieldsToDisplay(ScriptableObject scriptableObject)
        {
            Type scriptableObjectType = GetTypeFromName(scriptableObject.GetType().FullName);
            var scriptableObjectPaths = AssetDatabase.FindAssets("t:" + scriptableObject.GetType().FullName);
            var scriptableObjectInstance = (ScriptableObject)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(scriptableObjectPaths[0]), scriptableObjectType);
            
            List<FieldInfo> fields = new List<FieldInfo>(scriptableObjectType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));
            //remove fields that are not supported
            List<FieldInfo> invalidFieldsRemoved = new List<FieldInfo>(fields);
            _showWarningForUndisplayedFields = false;
            foreach(var field in fields)
            {
                SerializedObject so = new SerializedObject(scriptableObjectInstance);
                SerializedProperty property = so.FindProperty(field.Name);
                if(property == null)
                {
                    _showWarningForUndisplayedFields = true;
                    invalidFieldsRemoved.Remove(field);
                }
                else if(_hideReadOnlyFields)
                {
                    if(field.IsInitOnly)
                    {
                        invalidFieldsRemoved.Remove(field);
                    }
                    if(MakeVisualElementForValue(field.GetValue(scriptableObjectInstance)) is Label)
                    {
                        invalidFieldsRemoved.Remove(field);
                    }
                }
            }
                
            return invalidFieldsRemoved;

        }

        private void ForceUpdateScrollViewScale()
        {
            //this is a hack-fix for a bug in the scrollview where it doesn't update the scrollbars when the scale changes.
            //https://forum.unity.com/threads/how-to-refresh-scrollview-scrollbars-to-reflect-changed-content-width-and-height.1260920/
            //https://issuetracker.unity3d.com/issues/uitoolkit-scrollview-scroll-bars-arent-refreshed-after-changing-its-size
            var newLen = new StyleLength(Length.Percent(_scaleSwap ? 99.9f : 100f));
            ScrollView scrollView = rootVisualElement.Q<ScrollView>();
            scrollView.style.width = newLen;
            scrollView.style.height = newLen;
            _scaleSwap = !_scaleSwap;
        }
    }
}