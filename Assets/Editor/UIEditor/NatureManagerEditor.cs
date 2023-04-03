using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

public class NatureManagerEditor
{

    private static VisualTreeAsset m_ItemRowTemplate, m_NoPathTemplate;

    private static List<TileNature> m_NatureDB = new List<TileNature>();

    private ListView m_ListNature;
    private VisualElement m_TabsNature, m_SectionNoPath;
    private readonly float m_ItemHeight = 50, m_NoPathsize = 30;
    private readonly int countNoPath = 2;
    private TileNature m_activeItem;
    private ScrollView m_SectionDetails;

    public VisualElement Init()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
            ("Assets/Editor/UIEditor/NatureManagerEditor.uxml");
        m_ItemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
            ("Assets/Editor/UIEditor/LandscapeItem.uxml");
        m_NoPathTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
            ("Assets/Editor/UIEditor/TypeNoPathTemplate.uxml");
        VisualElement rootFromUXML = visualTree.Instantiate();
        rootFromUXML.style.flexGrow = 1;


        LoadAllItems();

        m_TabsNature = rootFromUXML.Q<VisualElement>("NatureListTab");
        m_SectionDetails = rootFromUXML.Q<ScrollView>("NatureDetails");
        m_SectionNoPath = rootFromUXML.Q<VisualElement>("BoxNoPath");


        GenerateListView();

        rootFromUXML.Q<Button>("NewNature").clicked += AddItem_OnClick;
        rootFromUXML.Q<Button>("RemoveNature").clicked += DeleteItem_OnClick;


        return rootFromUXML;
    }


    private void GenerateNoPath()
    {
        //m_SectionNoPath.style.marginLeft = countNoPath * m_NoPathsize;
        //m_SectionNoPath.style.marginTop = countNoPath * m_NoPathsize;
        m_SectionNoPath.Clear();
        int row = 0;
        int col = 0;
        for (int x = 1; x < Mathf.Pow(countNoPath + 1, 3) - 1; x++)
        {
            if (col == countNoPath && row == countNoPath)
            {
                col++;
                continue;
            };

            var newNode = m_NoPathTemplate.CloneTree();

            newNode.style.position = Position.Absolute;
            newNode.style.top = row * m_NoPathsize;
            newNode.style.left = col * m_NoPathsize; // newNode.Q<VisualElement>("node").style.height.value.value;

            newNode.Q<Label>("NodeValue").text = x.ToString();
            if ((TypeNoPath)x > 0)
            {
                newNode.RegisterCallback<ClickEvent>(NoPathOnClick);
                if (IsExistsRuleNoPath((TypeNoPath)x))
                {
                    newNode.Q<VisualElement>("NodeCheck").RemoveFromClassList("node_check_hidden");
                }
                else
                {
                    newNode.Q<VisualElement>("NodeCheck").AddToClassList("node_check_hidden");
                }
            }

            m_SectionNoPath.Add(newNode);

            col++;
            if (x % ((countNoPath * 2) + 1) == 0)
            {
                row++;
                col = 0;
            }
        }
    }

    private bool IsExistsRuleNoPath(TypeNoPath typeNoPath)
    {
        return m_activeItem.listTypeNoPath.Contains(typeNoPath);
    }

    private void NoPathOnClick(ClickEvent evt)
    {
        VisualElement clickedTab = evt.currentTarget as VisualElement;

        Label valueEl = clickedTab.Q<Label>("NodeValue");
        int index = Int32.Parse(valueEl.text);
        TypeNoPath val = (TypeNoPath)index;
        //Debug.Log($"click nopath {clickedTab.name} {val}");
        if (IsExistsRuleNoPath((TypeNoPath)index))
        {
            m_activeItem.listTypeNoPath.Remove((TypeNoPath)index);
            clickedTab.Q<VisualElement>("NodeCheck").AddToClassList("node_check_hidden");
        }
        else
        {
            m_activeItem.listTypeNoPath.Add((TypeNoPath)index);
            clickedTab.Q<VisualElement>("NodeCheck").RemoveFromClassList("node_check_hidden");
        }
        EditorUtility.SetDirty(m_activeItem);
        //if (!TabIsCurrentlySelected(clickedTab))
        //{
        //    GetAllTabs().Where(
        //        (tab) => tab != clickedTab && TabIsCurrentlySelected(tab)
        //    ).ForEach(UnselectTab);
        //    SelectTab(clickedTab);
        //}
    }

    private void LoadAllItems()
    {
        m_NatureDB.Clear();
        string[] allPaths = Directory.GetFiles("Assets/Landscape/Scriptables/Nature", "*.asset",
            SearchOption.AllDirectories);
        foreach (string path in allPaths)
        {
            string cleanedPath = path.Replace("\\", "/");
            m_NatureDB.Add((TileNature)AssetDatabase.LoadAssetAtPath(cleanedPath,
                typeof(TileNature)));
        }
    }

    private void GenerateListView()
    {
        Func<VisualElement> makeItem = () => m_ItemRowTemplate.CloneTree();

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            e.Q<VisualElement>("icon").style.backgroundImage = new StyleBackground(m_NatureDB[i].m_DefaultSprite);
            //m_ItemDatabase[i] == null ? m_DefaultItemIcon.texture :
            //m_ItemDatabase[i].MenuSprite.texture; //.Icon.texture;
            e.Q<Label>("name").text = m_NatureDB[i].name; //.FriendlyName;
        };
        m_ListNature = new ListView(m_NatureDB, m_ItemHeight, makeItem, bindItem);
        //m_ListNature.style.flexGrow = 1;
        m_ListNature.selectionType = SelectionType.Single;
        //m_ListNature.style.height = new StyleLength(new Length(100, LengthUnit.Percent));// m_NatureDB.Count * m_ItemHeight;
        m_TabsNature.Add(m_ListNature);

        m_ListNature.selectionChanged += ListView_onSelectionChange;

        m_ListNature.selectedIndex = 0;
    }



    private void ListView_onSelectionChange(IEnumerable<object> selectedItems)
    {
        m_activeItem = (TileNature)selectedItems.First();
        SerializedObject so = new SerializedObject(m_activeItem);
        m_SectionDetails.Bind(so);

        m_SectionDetails.Q<VisualElement>("Sprite").style.backgroundImage = new StyleBackground(m_activeItem.m_DefaultSprite);

        m_SectionDetails.style.visibility = Visibility.Visible;

        GenerateNoPath();
    }

    private void AddItem_OnClick()
    {
        //Create an instance of the scriptable object and set the default parameters
        TileNature newItem = ScriptableObject.CreateInstance<TileNature>();
        string path = EditorUtility.SaveFilePanelInProject("Save nature", "NewNature", "Asset", "Save new nature", "Assets/Resources/Nature");
        if (path == "") return;
        //newItem.FriendlyName = $"New Item";
        //newItem.Icon = m_DefaultItemIcon;
        //Create the asset, using the unique ID for the name
        AssetDatabase.CreateAsset(newItem, path); // $"Assets/Data/{newItem.ID}.asset"
        //Add it to the item list
        m_NatureDB.Add(newItem);
        //Refresh the ListView so everything is redrawn again
        m_ListNature.Rebuild();
        //m_ListNature.style.height = m_ItemDatabase.Count * m_ItemHeight;
    }

    private void DeleteItem_OnClick()
    {
        string path = AssetDatabase.GetAssetPath(m_activeItem);
        AssetDatabase.DeleteAsset(path);
        m_NatureDB.Remove(m_activeItem);
        m_ListNature.Rebuild();
        //Nothing is selected, so hide the details section
        m_SectionDetails.style.visibility = Visibility.Hidden;
    }


}
