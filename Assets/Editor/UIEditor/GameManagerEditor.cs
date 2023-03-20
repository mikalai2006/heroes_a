using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GameManagerEditor : EditorWindow
{
    private Sprite m_DefaultItemIcon;

    private static List<TileNature> m_ListNature = new List<TileNature>();
    private static VisualTreeAsset m_ItemRowTemplate;
    private ListView m_CornerListView;

    private ScrollView m_DetailTileSection;
    private VisualElement m_LargeDisplayIcon;
    private TileNature m_activeCorner;
    
    private VisualElement root;

    [MenuItem("Game/Landscape Manager")]
    public static void Init()
    {
        GameManagerEditor wnd = GetWindow<GameManagerEditor>();
        wnd.titleContent = new GUIContent("Game Object Manager");
        //Vector2 size = new Vector2(800, 475);
        //wnd.minSize = size;
        //wnd.maxSize = size;
    }
    public void CreateGUI()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
            ("Assets/Editor/UIEditor/GameManagerEditor.uxml");
        //m_ItemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
        //    ("Assets/UIEditor/LandscapeItem.uxml");
        VisualElement rootFromUXML = visualTree.Instantiate();
        rootFromUXML.style.flexGrow = 1;
        rootVisualElement.Add(rootFromUXML);
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>
            ("Assets/Editor/UIEditor/LandscapeManager.uss");
        rootVisualElement.styleSheets.Add(styleSheet);
        m_DefaultItemIcon = (Sprite)AssetDatabase.LoadAssetAtPath(
            "Assets/Sprites/UnknownIcon.png", typeof(Sprite));
        
        root = rootVisualElement;

        var tabController = new TabController(root);
        tabController.RegisterTabCallbacks();

        var Landscape = new LandscapeManagerEditor();
        var tabLandscape = Landscape.Init();
        //root.Q<VisualElement>("LandscapeContent").Clear();
        root.Q<VisualElement>("LandscapeContent").Add(tabLandscape);

        var Nature = new NatureManagerEditor();
        var tabNature = Nature.Init();
        //root.Q<VisualElement>("NatureContent").Clear();
        root.Q<VisualElement>("NatureContent").Add(tabNature);

        var Unit = new UnitManagerEditor();
        var tabUnit = Unit.Init();
        //root.Q<VisualElement>("NatureContent").Clear();
        root.Q<VisualElement>("UnitContent").Add(tabUnit);
    }

    

    //private void ListView_onSelectionChange(IEnumerable<object> selectedItems)
    //{
    //    m_activeItem = (TileLandscape)selectedItems.First();
    //    SerializedObject so = new SerializedObject(m_activeItem);
    //    m_DetailSection.Bind(so);
    //    //if (m_activeItem.tileRule.m_DefaultSprite != null)
    //    //{
    //    //    m_LargeDisplayIcon.style.backgroundImage = m_activeItem.tileRule.m_DefaultSprite.texture;
    //    //}
    //    m_DetailSection.style.visibility = Visibility.Visible;


    //    var corners = m_DetailSection.Q<VisualElement>("corners");
    //    corners.Clear();
    //    m_ListNature.Clear();

    //    string[] paths = Directory.GetFiles("Assets/Resources/Nature", "*.asset", SearchOption.AllDirectories);
    //    foreach (string path in paths)
    //    {
    //        string cleanedPath = path.Replace("\\", "/");
    //        m_ListNature.Add((TileNature)AssetDatabase.LoadAssetAtPath(cleanedPath, typeof(TileNature)));
    //    }

    //    Func<VisualElement> makeCorner = () => m_ItemRowTemplate.CloneTree();

    //    Action<VisualElement, int> bindCorner = (e, i) =>
    //    {
    //        if (m_ListNature[i] != null)
    //        {
    //            e.Q<VisualElement>("icon").style.backgroundImage = m_ListNature[i].m_DefaultSprite.texture;
    //            e.Q<Label>("name").text = m_ListNature[i].name;
    //        }
    //    };


    //    m_CornerListView = new ListView(m_ListNature, 50, makeCorner, bindCorner);
    //    m_CornerListView.style.flexGrow = 1;
    //    corners.Add(m_CornerListView);
    //    m_CornerListView.selectionChanged += ListCorner_onSelectionChange;


    //}
    private void ListCorner_onSelectionChange(IEnumerable<object> selectedItems)
    {
        //m_activeCorner = (TileNature)selectedItems.First();
        //SerializedObject so = new SerializedObject(m_activeCorner);
        //m_DetailTileSection.Bind(so);
        ////if (m_activeItem.tileRule.m_DefaultSprite != null)
        ////{
        ////    m_LargeDisplayIcon.style.backgroundImage = m_activeItem.tileRule.m_DefaultSprite.texture;
        ////}
        //m_DetailTileSection.style.visibility = Visibility.Visible;

    }
}