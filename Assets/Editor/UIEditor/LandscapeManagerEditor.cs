using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

public class LandscapeManagerEditor
{

    private static VisualTreeAsset m_ItemRowTemplate;
    private static List<TileLandscape> m_ItemDatabase = new List<TileLandscape>();

    private ListView m_ItemListView;
    private VisualElement m_ItemsTab;
    private float m_ItemHeight = 50;
    private TileLandscape m_activeItem;
    private ScrollView m_DetailSection;


    public VisualElement Init()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
            ("Assets/Editor/UIEditor/LandscapeManager.uxml");
        m_ItemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
            ("Assets/Editor/UIEditor/LandscapeItem.uxml");
        VisualElement rootFromUXML = visualTree.Instantiate();
        rootFromUXML.style.flexGrow = 1;
        //rootFromUXML.Q<VisualElement>("LandscapeContent").Add(rootFromUXML);


        LoadAllItems();

        m_ItemsTab = rootFromUXML.Q<VisualElement>("List");
        m_DetailSection = rootFromUXML.Q<ScrollView>("DetailsBox");


        GenerateListView();



        return rootFromUXML;
    }

    private void LoadAllItems()
    {
        m_ItemDatabase.Clear();
        string[] allPaths = Directory.GetFiles("Assets/ScriptableObjects/Landscape", "*.asset",
            SearchOption.AllDirectories);
        foreach (string path in allPaths)
        {
            string cleanedPath = path.Replace("\\", "/");
            m_ItemDatabase.Add((TileLandscape)AssetDatabase.LoadAssetAtPath(cleanedPath,
                typeof(TileLandscape)));
        }
    }

    private void GenerateListView()
    {
        Func<VisualElement> makeItem = () => m_ItemRowTemplate.CloneTree();

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            e.Q<VisualElement>("icon").style.backgroundImage = new StyleBackground(m_ItemDatabase[i].tileRule.m_DefaultSprite);
            //m_ItemDatabase[i] == null ? m_DefaultItemIcon.texture :
            //m_ItemDatabase[i].MenuSprite.texture; //.Icon.texture;
            e.Q<Label>("name").text = m_ItemDatabase[i].name; //.FriendlyName;
        };
        m_ItemListView = new ListView(m_ItemDatabase, m_ItemHeight, makeItem, bindItem);
        m_ItemListView.style.flexGrow = 1;
        m_ItemListView.selectionType = SelectionType.Single;
        m_ItemListView.style.height = m_ItemDatabase.Count * m_ItemHeight;
        m_ItemsTab.Add(m_ItemListView);

        m_ItemListView.selectionChanged += ListView_onSelectionChange;

        m_ItemListView.selectedIndex = 0;
    }



    private void ListView_onSelectionChange(IEnumerable<object> selectedItems)
    {
        m_activeItem = (TileLandscape)selectedItems.First();
        SerializedObject so = new SerializedObject(m_activeItem);
        m_DetailSection.Bind(so);
        m_DetailSection.style.visibility = Visibility.Visible;

    }

}
