

// using System;
// using System.Collections;

// using UnityEditor;

// using UnityEngine;

// [CustomPropertyDrawer(typeof(EnumTypeBuildAttribute))]
// public class TypeBuildsAttributeDrawer : PropertyDrawer
// {
//     private BitArray arr = new BitArray(44);
//     private const float FOLDOOUT_HEIGHT = 16;
//     private SerializedProperty content;
//     private SerializedProperty enumType;

//     private SerializedProperty array;

//     void OnColorSelected(object entry)
//     {
//         int val = (int)Enum.Parse<TestTypeBuild>(entry.ToString());
//         arr[val] = true;
//         Debug.Log($"entry::: {val} | {Helpers.ToBitString(arr)}");
//         // arr[]
//     }
//     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
//     {

//         // // EditorGUI.BeginProperty(position, label, property);
//         // // Rect foldoutRect = new Rect(position.x, position.y, position.width, FOLDOOUT_HEIGHT);
//         // // property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, label);
//         EnumTypeBuildAttribute enumData = (EnumTypeBuildAttribute)attribute;
//         // string path = property.propertyPath;
//         // if (array == null)
//         // {
//         //     array = property.serializedObject.FindProperty(path.Substring(0, path.LastIndexOf('.')));
//         //     if (array == null)
//         //     {
//         //         EditorGUI.LabelField(position, "Use EnumTypeBuildAttribute on arrays.");
//         //         return;
//         //     }
//         // }
//         // if (array.arraySize != enumData.names.Length)
//         // {
//         //     array.arraySize = enumData.names.Length;
//         // }
//         // int index = System.Convert.ToInt32(path.Substring(path.IndexOf('[') + 1).Replace("]", ""));
//         // label.text = enumData.names[index];
//         EditorGUI.PropertyField(position, property, label, true);

//         string[] entries = enumData.names;
//         // if (GUI.Button(position, "Hello", EditorStyles.toolbarButton))
//         // {

//         //     GenericMenu toolsMenu = new GenericMenu();
//         //     foreach (string entry in entries)
//         //     {
//         //         toolsMenu.AddItem(new GUIContent(entry), false, OnColorSelected, entry);
//         //         toolsMenu.AddItem(new GUIContent(entry), false, OnColorSelected, entry);
//         //     }
//         //     toolsMenu.ShowAsContext();
//         // }

//         // for (int i = 0; i < arr.Length; i++)
//         // {
//         //     if (arr[i]) GUI.Button(position, entries[i]);
//         // }

//         // EditorGUI.EndProperty();
//     }

//     public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//     {
//         if (content == null)
//         {
//             content = property.FindPropertyRelative("content");
//         }
//         // if (enumType == null)
//         // {
//         //     enumType = property.FindPropertyRelative("enumType");
//         // }

//         // float height = FOLDOOUT_HEIGHT;
//         // if (property.isExpanded)
//         // {
//         //     if (content.arraySize != enumType.enumNames.Length)
//         //     {
//         //         content.arraySize = enumType.enumNames.Length;
//         //     }
//         // }

//         // for (int i = 0; i < content.arraySize; i++)
//         // {
//         //     height += EditorGUI.GetPropertyHeight(content.GetArrayElementAtIndex(i));
//         // }

//         return EditorGUI.GetPropertyHeight(property);
//     }
// }