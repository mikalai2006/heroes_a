using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;

public class GenerateIDEditor : Editor
{
    public SerializedProperty idObject;
    protected int _countClass;

    protected void OnEnable()
    {
        idObject = serializedObject.FindProperty("idObject");
    }
    public override void OnInspectorGUI()
    {
        // Debug.Log($"Get type {target.GetType()}={_countClass}");
        DrawDefaultInspector();
        serializedObject.Update();

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.green;

        if (idObject != null && idObject.stringValue == "")
        {
            string id = System.Guid.NewGuid().ToString("N");
            //EditorGUILayout.PropertyField(idObject, new GUIContent(id));
            idObject.stringValue = id;
            serializedObject.ApplyModifiedProperties();
        }

        if (idObject.stringValue == "")
        {
            style.normal.textColor = Color.red;
        }
        var r = new Rect(0, 0, 10, 10);
        GUI.Label(r, "ok", style);
    }

}

[CustomEditor(typeof(ScriptableMapObjectBase))]
//[CanEditMultipleObjects]
class GenerateIDUnitBaseEditor : GenerateIDEditor
{
    // public override void OnInspectorGUI()
    // {

    //     Type type = target.GetType();
    //     _countClass = UnityEngine.Object.FindObjectsOfType<typeof(type)>();

    //     base.OnInspectorGUI();
    // }
    //SerializedProperty idObject;

    //void OnEnable()
    //{
    //    idObject = serializedObject.FindProperty("idObject");
    //}
    //public override void OnInspectorGUI()
    //{
    //    Debug.Log("OnInspectorGUI");
    //    base.OnInspectorGUI();

    //    if (idObject.stringValue == "")
    //    {
    //        string id = System.Guid.NewGuid().ToString("N");
    //        //EditorGUILayout.PropertyField(idObject, new GUIContent(id));
    //        idObject.stringValue = id;
    //        serializedObject.ApplyModifiedProperties();
    //    }
    //    EditorGUILayout.LabelField(new GUIContent("Hello"));

    //    serializedObject.ApplyModifiedProperties();
    //}
}

[CustomEditor(typeof(ScriptableTown))]
class GenerateIDTownEditor : GenerateIDUnitBaseEditor
{

}

[CustomEditor(typeof(ScriptableWarriors))]
class GenerateIDWarriorEditor : GenerateIDUnitBaseEditor
{

}

[CustomEditor(typeof(ScriptableMapResource))]
class GenerateIDResourceEditor : GenerateIDUnitBaseEditor
{

}

[CustomEditor(typeof(ScriptableHero))]
class GenerateIDHeroEditor : GenerateIDUnitBaseEditor
{

}

[CustomEditor(typeof(ScriptableMapObject))]
class GenerateIDMapObjectEditor : GenerateIDUnitBaseEditor
{

}

[CustomEditor(typeof(ScriptableMine))]
class GenerateIDMineEditor : GenerateIDUnitBaseEditor
{

}

[CustomEditor(typeof(TileNature))]
class GenerateIDTileNatureEditor : GenerateIDEditor
{

}

[CustomEditor(typeof(TileLandscape))]
class GenerateIDTileLandscapeEditor : GenerateIDEditor
{

}


// [CustomEditor(typeof(ScriptableSkillSchool))]
// class GenerateIDScriptableSkillSchoolEditor : GenerateIDEditor
// {

// }

[CustomEditor(typeof(ScriptableArtifact))]
class GenerateIDScriptableArtifactEditor : GenerateIDEditor
{

}

[CustomEditor(typeof(ScriptableGameMode))]
class GenerateIDScriptableGameModeEditor : GenerateIDEditor
{

}

// [CustomEditor(typeof(ScriptableCollectionResource))]
// class GenerateIDScriptableCollectionResourceEditor : GenerateIDEditor
// {

// }
//[CustomEditor(typeof(TileNature))]
//[CanEditMultipleObjects]
//public class GenerateUIDNature : Editor
//{
//    SerializedProperty idObject;

//    void OnEnable()
//    {
//        idObject = serializedObject.FindProperty("idObject");
//        if (idObject != null && idObject.stringValue == "")
//        {
//            string id = System.Guid.NewGuid().ToString("N");
//            EditorGUILayout.PropertyField(idObject, new GUIContent(id));
//            serializedObject.ApplyModifiedProperties();
//        }
//    }
//    public override void OnInspectorGUI()
//    {
//        base.OnInspectorGUI();

//        serializedObject.ApplyModifiedProperties();
//    }
//}
