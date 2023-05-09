using System;
using System.Linq;
using System.Reflection;
using DevKit.Modularis.Editor.Variables;
using Devkit.Modularis.References;
using Devkit.Modularis.Variables;
using Modularis.Editor.Helpers;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DevKit.Modularis.Editor.References
{
    [CustomPropertyDrawer(typeof(BaseReference), true)]
    public class BaseReferenceDrawer : PropertyDrawer
    {
        private readonly string[] _popupOptions = { "Use Constant", "Use Variable" };
        private readonly string[] _popup2Options = { "Save Constant to Variable", "Load Constant from Variable", "Reset Constant"};

        private GUIStyle _popupStyleDropdown, _popupStyleButton, _foldout;
        private SerializedProperty _targetProperty;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _targetProperty = property;
            _popupStyleDropdown ??= new GUIStyle(GUI.skin.GetStyle("PaneOptions")) { imagePosition = ImagePosition.ImageOnly };
            _popupStyleButton ??= new GUIStyle(GUI.skin.GetStyle("miniButton"));
            _foldout ??= new GUIStyle(GUI.skin.GetStyle("FoldoutHeader"));

            label = EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, label);

            EditorGUI.BeginChangeCheck();
            
            var useConstantP = property.FindPropertyRelative("useConstant");
            var constantValueP = property.FindPropertyRelative("constantValue");
            var variableP = property.FindPropertyRelative("variable");
            var foldoutP = property.FindPropertyRelative("foldout");
            
            position.height = 16f + EditorGUIUtility.standardVerticalSpacing;
            var configButton = new Rect(position);
            configButton.yMin += _popupStyleDropdown.margin.top;
            configButton.width = _popupStyleDropdown.fixedWidth + _popupStyleDropdown.margin.right;
            configButton.x -= 16f;
            position.xMin = configButton.xMax;
            
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var result = EditorGUI.Popup(configButton,useConstantP.boolValue ? 0 : 1, _popupOptions, _popupStyleDropdown);
            useConstantP.boolValue = result == 0;
            var baseType = variableP.GetUnderlyingType().GetTypeInfo().BaseType;
            if (baseType != null)
            {
                var genericType = baseType.GenericTypeArguments[0];
                var genericTypeSpecificName = genericType.ToString().Split(".").Last();
                if (useConstantP.boolValue)
                {
                    if (constantValueP.propertyType == SerializedPropertyType.Generic)
                    {

                        foldoutP.boolValue = EditorGUI.BeginFoldoutHeaderGroup(
                            position, 
                            foldoutP.boolValue, 
                            genericTypeSpecificName,
                            menuAction:ShowHeaderContextMenu);
                        
                        if (foldoutP.boolValue)
                        {
                            var newpos = new Rect(position);
                            newpos.y += 16f + EditorGUIUtility.standardVerticalSpacing;
                            var offset = newpos.x;
                            newpos.x = 0f;
                            newpos.width += offset;
                            EditorGUI.indentLevel += 1;
                            var startingDepth = constantValueP.depth;
                            // Move into the first child of constantValue
                            constantValueP.NextVisible(true);
                            do
                            {
                                EditorGUILayout.PropertyField(constantValueP, true);
                                constantValueP.NextVisible(false);
                                // Quit iterating when you are back at the original depth (you've drawn all children)
                            } while (constantValueP.depth > startingDepth);
                        }
                        EditorGUI.EndFoldoutHeaderGroup();
                    }
                    else
                    {
                        position.xMax -= 22f;
                        EditorGUI.PropertyField(position, constantValueP,new GUIContent(""),includeChildren:false);
                        configButton.x += position.width + 17;
                        var result2 = EditorGUI.Popup(configButton,-1, _popup2Options, _popupStyleDropdown);
                        /*switch (result2)
                        {
                            case 0:
                                OnSaveClicked();
                                break;
                            case 1:
                                OnResetToVariableClicked();
                                break;
                            case 2:
                                OnResetClicked();
                                break;
                        }*/
                    }
                }
                else
                {
                    EditorGUI.PropertyField(position, variableP,new GUIContent(""),includeChildren:false);
                }
            }


            var baseVariable = variableP.objectReferenceValue as object as BaseVariable;

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
                if (!useConstantP.boolValue)
                {
                    if (baseVariable != null)
                    {
                        baseVariable.InvokeCallback();
                    }
                }
                else
                {
                    var myDataClass = PropertyDrawerUtility.GetActualObjectForSerializedProperty<BaseReference>(fieldInfo, property);
                    myDataClass?.ConstantValueChanged?.Invoke();
                }
            }
            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }
        void ShowHeaderContextMenu(Rect position)
        {
            /*var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Save Constant to Variable"), false, OnSaveClicked);
            menu.AddItem(new GUIContent("Load Constant from Variable"), false, OnResetToVariableClicked);
            menu.AddItem(new GUIContent("Reset Constant"), false, OnResetClicked);
            menu.DropDown(position);*/
        }

        void OnSaveClicked()
        {
            /*var constantValueP = _targetProperty.FindPropertyRelative("constantValue");
            var variableP = _targetProperty.FindPropertyRelative("variable");
            var baseVariable = variableP.objectReferenceValue as object as BaseVariable;
            if (baseVariable == null)
            {
                Debug.LogError("Cannot save to variable without variable assigned",_targetProperty.serializedObject.targetObject);
                return;
            }
            Undo.RecordObject(baseVariable, "Save Constant to Variable");
            baseVariable.SetValue(constantValueP.boxedValue);
            variableP.serializedObject.ApplyModifiedPropertiesWithoutUndo();*/
        }

        private void OnResetClicked()
        {
            /*var constantValueP = _targetProperty.FindPropertyRelative("constantValue");
            Undo.RecordObject(constantValueP.serializedObject.targetObject, "Reset Constant");
            
            var baseReference = PropertyDrawerUtility.GetActualObjectForSerializedProperty<BaseReference>(fieldInfo, _targetProperty);
            baseReference.ResetConstant();*/
        }
        
        private void OnResetToVariableClicked()
        {
            /*var constantValueP = _targetProperty.FindPropertyRelative("constantValue");
            var variableP = _targetProperty.FindPropertyRelative("variable");
            
            var baseVariable = variableP.objectReferenceValue as object as BaseVariable;
            var prop = new SerializedObject(variableP.objectReferenceValue);
            if (baseVariable == null)
            {
                Debug.LogError("Cannot save to variable without variable assigned",_targetProperty.serializedObject.targetObject);
                return;
            }
            var baseReference = PropertyDrawerUtility.GetActualObjectForSerializedProperty<BaseReference>(fieldInfo, _targetProperty);
            var value = variableP.FindPropertyRelative("value");
            Undo.RecordObject(baseVariable, "Save Constant to Variable");
            constantValueP.serializedObject.CopyFromSerializedProperty(variableP);
            constantValueP.serializedObject.ApplyModifiedPropertiesWithoutUndo();*/
        }

 
    }
    
}