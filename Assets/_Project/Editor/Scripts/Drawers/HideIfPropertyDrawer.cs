using UnityEditor;
using UnityEngine;

namespace DeveloperConsole
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfPropertyDrawer : PropertyDrawer
    {
        private bool _isFieldDraw;
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _isFieldDraw = false;
            
            HideIfAttribute hideIfAttribute = (HideIfAttribute)attribute;
            
            SerializedProperty targetProperty = property.serializedObject.FindProperty(hideIfAttribute.memberName);
            if (targetProperty is { propertyType: SerializedPropertyType.Boolean})
            {
                if (!targetProperty.boolValue)
                {
                    DrawPropertyField();
                }
                
                return;
            }

            DrawPropertyField();
            return;
            
            
            
            // --- Local methods ---
            void DrawPropertyField()
            {
                _isFieldDraw = true;
                EditorGUI.PropertyField(position, property, label);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (_isFieldDraw) return base.GetPropertyHeight(property, label);
            
            return 0;
        }
    }
}