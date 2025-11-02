using UINavigation.Editor;
using TriInspector;
using UINavigation;
using UnityEditor;
using UnityEngine;

[assembly: RegisterTriValueDrawer(typeof( NavigationSystem.TriInspector.Editor.ViewIdTriDrawer), TriDrawerOrder.Drawer)]

namespace NavigationSystem.TriInspector.Editor
{
    public sealed class ViewIdTriDrawer : TriValueDrawer<ViewId>
    {
        public override float GetHeight(float width, TriValue<ViewId> propertyValue, TriElement next)
        {
            // Una sola línea; si quieres espacio para warnings, añade margen aquí
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, TriValue<ViewId> propertyValue, TriElement next)
        {
            // Field label (TriValue<T> expose the original TriProperty)
            var labelText = propertyValue.Property.DisplayName ?? propertyValue.Property.RawName;
            var label = new GUIContent(labelText);
            var rect = EditorGUI.PrefixLabel(position, label);
            
            var viewIds = ViewIdProvider.GetAll();
            var current = (string)propertyValue.SmartValue ?? string.Empty;
            var index = ViewIdProvider.IndexOf(viewIds, current);
            var newIndex = EditorGUI.Popup(rect, index, viewIds);
            
            if (newIndex != index)
            {
                propertyValue.SmartValue = (ViewId)viewIds[newIndex];
            }
        }
    }
}