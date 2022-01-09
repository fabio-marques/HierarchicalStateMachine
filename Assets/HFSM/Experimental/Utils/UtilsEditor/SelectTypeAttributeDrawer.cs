//By Leonidas85 at https://forum.unity.com/threads/how-to-set-serializedproperty-managedreferencevalue-to-null.746645/

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEditor;
using Object = UnityEngine.Object;
#endif
using UnityEngine;

namespace HFSM.Experimental.Utils
{
    /// <summary>
    /// Add <see cref="SelectTypeAttribute"/> to a field that has a [SerializeReference] attribute to draw a dropdown with suitable types to switch between.
    /// </summary>
    public class SelectTypeAttribute : PropertyAttribute
    {
    }


#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SelectTypeAttribute))]
    public class SelectTypeAttributeDrawer : PropertyDrawer
    {
        private List<Type> _cachedInstantiableTypes;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SelectTypeAttribute selectTypeAttribute = attribute as SelectTypeAttribute;

            // type is returned as "{assembly name} {type name}" from property.managedReferenceFieldTypename, needs to be "{assembly name}, {type name}" for Type.GetType()
            string[] baseTypeAndAssemblyName = property.managedReferenceFieldTypename.Split(' ');
            string baseTypeString = $"{baseTypeAndAssemblyName[1]}, {baseTypeAndAssemblyName[0]}";
            Type baseType = Type.GetType(baseTypeString);

            List<Type> instantiableTypes = GetAllInstantiableTypesDerivedFrom(baseType);
            GUIContent[] options = new GUIContent[instantiableTypes.Count + 1];
            options[0] = new GUIContent("None");
            int selectedIndex = 0;
            for (int i = 0; i < instantiableTypes.Count; i++)
            {
                Type type = instantiableTypes[i];
                options[i + 1] = new GUIContent(type.Name);
                string typeAndAssemblyName = $"{type.Assembly.GetName().Name} {type.FullName}";
                if (property.managedReferenceFullTypename == typeAndAssemblyName)
                {
                    selectedIndex = i + 1;
                }
            }

            int newSelectedIndex =
                EditorGUI.Popup(
                    new Rect(position.x + position.width - 130, position.y, 130, EditorGUIUtility.singleLineHeight),
                    selectedIndex, options);
            if (selectedIndex != newSelectedIndex)
            {
                Undo.RegisterCompleteObjectUndo(property.serializedObject.targetObject, "selected type change");
                if (newSelectedIndex == 0)
                {
                    property.managedReferenceValue = null;
                }
                else
                {
                    Type selectedType = instantiableTypes[newSelectedIndex - 1];
                    property.managedReferenceValue = FormatterServices.GetUninitializedObject(selectedType);
                }
            }

            EditorGUI.PropertyField(
                new Rect(
                    position.x,
                    position.y,
                    position.width,
                    position.height),
                property,
                new GUIContent(property.displayName),
                true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }

        private List<Type> GetAllInstantiableTypesDerivedFrom(Type targetType)
        {
            //TypeCache.GetTypesDerivedFrom<Type>() //TODO test
            if (_cachedInstantiableTypes != null)
            {
                return _cachedInstantiableTypes;
            }

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _cachedInstantiableTypes = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (type != targetType &&
                        targetType.IsAssignableFrom(type) &&
                        CheckInstantiable(type))
                    {
                        _cachedInstantiableTypes.Add(type);
                    }
                }
            }

            if (CheckInstantiable(targetType))
            {
                _cachedInstantiableTypes.Add(targetType);
            }

            return _cachedInstantiableTypes;
        }

        private static bool CheckInstantiable(Type type)
        {
            if (typeof(Component).IsAssignableFrom(type) ||
                typeof(Object).IsAssignableFrom(type))
            {
                return false;
            }

            if (type == typeof(string) || type.IsValueType)
            {
                return true;
            }

            return !type.IsInterface &&
                   !type.IsGenericTypeDefinition &&
                   !type.IsAbstract &&
                   type.IsVisible;
        }
    }
#endif
}