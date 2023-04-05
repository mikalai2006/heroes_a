using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

public class UnitManagerEditor
{

    private static VisualTreeAsset m_ItemRowTemplate, m_NoPathTemplate;

    private static List<ScriptableEntity> m_UnitDB = new List<ScriptableEntity>();

    private ListView m_ListUnit;
    private VisualElement m_Tabs, m_SectionNoPath;
    private readonly float m_ItemHeight = 50, m_NoPathsize = 30;
    private readonly int countNoPath = 2;
    private ScriptableEntity m_activeItem;
    private ScrollView m_SectionDetails;

    public VisualElement Init()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
            ("Assets/Editor/UIEditor/UnitManagerEditor.uxml");
        m_ItemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
            ("Assets/Editor/UIEditor/LandscapeItem.uxml");
        m_NoPathTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
            ("Assets/Editor/UIEditor/TypeNoPathTemplate.uxml");
        VisualElement rootFromUXML = visualTree.Instantiate();
        rootFromUXML.style.flexGrow = 1;


        LoadAllItems();

        m_Tabs = rootFromUXML.Q<VisualElement>("UnitListTab");
        m_SectionDetails = rootFromUXML.Q<ScrollView>("UnitDetails");
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
        //Undo.RecordObject(m_activeItem, "Changed NoPath");
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
        m_UnitDB.Clear();
        string[] allPaths = Directory.GetFiles("Assets/Units/Scriptables", "*.asset",
            SearchOption.AllDirectories);
        foreach (string path in allPaths)
        {
            string cleanedPath = path.Replace("\\", "/");
            m_UnitDB.Add((ScriptableEntity)AssetDatabase.LoadAssetAtPath(cleanedPath,
                typeof(ScriptableEntity)));
        }

    }

    private void GenerateListView()
    {
        Func<VisualElement> makeItem = () => m_ItemRowTemplate.CloneTree();

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            // if (m_UnitDB[i].MenuSprite != null)
            // {
            //     e.Q<VisualElement>("icon").style.backgroundImage = new StyleBackground(m_UnitDB[i].MenuSprite);

            // }
            // else
            // {
            //     // SpriteRenderer[] sprites = m_UnitDB[i].Prefab.gameObject.GetComponentsInChildren<SpriteRenderer>();
            //     e.Q<VisualElement>("icon").style.backgroundImage = new StyleBackground(m_UnitDB[i].MenuSprite);
            // }
            // new StyleBackground(m_UnitDB[i].MenuSprite);
            //m_ItemDatabase[i] == null ? m_DefaultItemIcon.texture :
            //m_ItemDatabase[i].MenuSprite.texture; //.Icon.texture;
            e.Q<Label>("name").text = m_UnitDB[i].name; //.FriendlyName;
        };
        m_ListUnit = new ListView(m_UnitDB, m_ItemHeight, makeItem, bindItem);
        //m_ListNature.style.flexGrow = 1;
        m_ListUnit.selectionType = SelectionType.Single;
        //m_ListNature.style.height = new StyleLength(new Length(100, LengthUnit.Percent));// m_NatureDB.Count * m_ItemHeight;
        m_Tabs.Add(m_ListUnit);

        m_ListUnit.selectionChanged += ListView_onSelectionChange;

        m_ListUnit.selectedIndex = 0;
    }



    private void ListView_onSelectionChange(IEnumerable<object> selectedItems)
    {

        //Debug.Log($"Typeof selectedItem {selectedItems.First().GetType()}");

        m_activeItem = (ScriptableEntity)selectedItems.First();
        SerializedObject so = new SerializedObject(m_activeItem);
        m_SectionDetails.Bind(so);


        // if (m_activeItem.MenuSprite != null)
        // {
        //     m_SectionDetails.Q<VisualElement>("Sprite").style.backgroundImage = new StyleBackground(m_activeItem.MenuSprite);
        // }
        // else
        // {
        //     // SpriteRenderer[] sprites = m_activeItem.Prefab.gameObject.GetComponentsInChildren<SpriteRenderer>();
        //     m_SectionDetails.Q<VisualElement>("Sprite").style.backgroundImage = new StyleBackground(m_activeItem.MenuSprite);
        // }

        m_SectionDetails.style.visibility = Visibility.Visible;

        GenerateNoPath();
    }
    private void AddItem_OnClick()
    {
        //Create an instance of the scriptable object and set the default parameters
        ScriptableEntity newItem = ScriptableObject.CreateInstance<ScriptableEntity>();
        string path = EditorUtility.SaveFilePanelInProject("Save nature", "NewNature", "Asset", "Save new nature", "Assets/Resources/Nature");
        if (path == "") return;
        //newItem.FriendlyName = $"New Item";
        //newItem.Icon = m_DefaultItemIcon;
        //Create the asset, using the unique ID for the name
        AssetDatabase.CreateAsset(newItem, path); // $"Assets/Data/{newItem.ID}.asset"
        //Add it to the item list
        m_UnitDB.Add(newItem);
        //Refresh the ListView so everything is redrawn again
        m_ListUnit.Rebuild();
        //m_ListNature.style.height = m_ItemDatabase.Count * m_ItemHeight;
    }

    private void DeleteItem_OnClick()
    {
        string path = AssetDatabase.GetAssetPath(m_activeItem);
        AssetDatabase.DeleteAsset(path);
        m_UnitDB.Remove(m_activeItem);
        m_ListUnit.Rebuild();
        //Nothing is selected, so hide the details section
        m_SectionDetails.style.visibility = Visibility.Hidden;
    }


}
