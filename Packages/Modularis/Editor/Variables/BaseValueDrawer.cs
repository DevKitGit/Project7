using Devkit.Modularis.Variables;
using UnityEditor;


namespace DevKit.Modularis.Editor.Variables
{
    [CustomEditor(typeof(BaseVariable),true)]
    public class BaseValueDrawer : UnityEditor.Editor 
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            var changed =EditorGUI.EndChangeCheck();
            if (changed)
            {
                ((BaseVariable)target).InvokeCallback();
            }
        }
    }
}