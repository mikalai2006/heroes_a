using UnityEditor;

using UnityEngine;

[CustomEditor(typeof(ScriptableBuildBase))]
public class ScriptableBuildBaseEditor : Editor
{
    private const float FOLDOOUT_HEIGHT = 16;
    private SerializedProperty content;
    private SerializedProperty enumType;
    private SerializedProperty array;

}