// using UnityEditor;
// using UnityEngine.UIElements;
// using UnityEditor.UIElements;

// [CustomEditor(typeof(ScriptableEntity), true)]
// public class ScriptableEntityEditor : Editor
// {
//     // public override void OnInspectorGUI() {
//     //     base.OnInspectorGUI();

//     // }
//     public override VisualElement CreateInspectorGUI()
//     {
//         var root = new VisualElement();

//         // Draw default inspector.
//         var test = new Label("Hello");
//         var foldout = new Foldout() { viewDataKey = "ListPath", text = "Full Inspector1" };

//         InspectorElement.FillDefaultInspector(root, serializedObject, this);

//         root.Add(test);
//         root.Add(foldout);

//         return root;
//         // return base.CreateInspectorGUI();
//     }
// }
