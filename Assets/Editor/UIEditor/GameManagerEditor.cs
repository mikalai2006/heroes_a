using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

public class GameManagerEditor : EditorWindow
{
    private VisualElement _root;

    [MenuItem("Game/Landscape Manager")]
    public static void Init()
    {
        GameManagerEditor wnd = GetWindow<GameManagerEditor>();
        wnd.titleContent = new GUIContent("Game Object Manager");
    }
    public void CreateGUI()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
            ("Assets/Editor/UIEditor/GameManagerEditor.uxml");
        VisualElement rootFromUXML = visualTree.Instantiate();
        rootFromUXML.style.flexGrow = 1;
        rootVisualElement.Add(rootFromUXML);
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>
            ("Assets/Editor/UIEditor/LandscapeManager.uss");
        rootVisualElement.styleSheets.Add(styleSheet);

        _root = rootVisualElement;

        var tabController = new TabController(_root);
        tabController.RegisterTabCallbacks();

        var landscape = new LandscapeManagerEditor();
        var tabLandscape = landscape.Init();
        //root.Q<VisualElement>("LandscapeContent").Clear();
        _root.Q<VisualElement>("LandscapeContent").Add(tabLandscape);

        var nature = new NatureManagerEditor();
        var tabNature = nature.Init();
        //root.Q<VisualElement>("NatureContent").Clear();
        _root.Q<VisualElement>("NatureContent").Add(tabNature);

        var unit = new UnitManagerEditor();
        var tabUnit = unit.Init();
        //root.Q<VisualElement>("NatureContent").Clear();
        _root.Q<VisualElement>("UnitContent").Add(tabUnit);
    }
}