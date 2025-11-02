using UnityEditor;
using UnityEngine;

namespace UINavigation.Editor
{
    [CustomPropertyDrawer(typeof(ViewId))]
    public class ViewIdDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var viewIds = ViewIdProvider.GetAll();

            // UI: label + popup
            var rect = EditorGUI.PrefixLabel(position, label);

            var valueProp = property.FindPropertyRelative("value"); // serialized field from ViewId
            var current = valueProp?.stringValue ?? string.Empty;

            var index = ViewIdProvider.IndexOf(viewIds, current);
            var newIndex = EditorGUI.Popup(rect, index, viewIds);
            if (newIndex != index && valueProp != null)
            {
                valueProp.stringValue = viewIds[newIndex];
            }

            if (!string.IsNullOrEmpty(current) && !ViewIdProvider.Contains(viewIds, current))
            {
                var helpRect = new Rect(position.x, position.yMax + 2f, position.width, EditorGUIUtility.singleLineHeight * 1.2f);
                EditorGUI.HelpBox(helpRect, $"'{current}' does not exists in the list.", MessageType.Warning);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var baseH = EditorGUIUtility.singleLineHeight;

            var options =  ViewIdProvider.GetAll();
            var current = property.FindPropertyRelative("value")?.stringValue;

            if (!string.IsNullOrEmpty(current) && !ViewIdProvider.Contains(options, current))
                return baseH + EditorGUIUtility.singleLineHeight * 1.2f;

            return baseH;
        }
    }
}