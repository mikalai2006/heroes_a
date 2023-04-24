using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UIElements;

public class MapObjectsManagerEditor
{

    private static VisualTreeAsset m_ItemRowTemplate, m_NoPathTemplate, m_ItemEffect;

    private struct PropertyCreationParams
    {
        public string Path;
    }
    private static List<ScriptableEntityMapObject> m_UnitDB = new List<ScriptableEntityMapObject>();

    private ListView m_ListUnit;
    private VisualElement m_Tabs, m_SectionNoPath, m_SectionEffects, m_SectionRulesInput;
    private readonly float m_ItemHeight = 50, m_NoPathsize = 30;
    private readonly int countNoPath = 2;
    private ScriptableEntityMapObject m_activeItem;
    private ScrollView m_SectionDetails;

    public VisualElement Init()
    {
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
            ("Assets/Editor/UIEditor/MapObjectManagerEditor.uxml");
        m_ItemRowTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
            ("Assets/Editor/UIEditor/LandscapeItem.uxml");
        m_NoPathTemplate = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
            ("Assets/Editor/UIEditor/TypeNoPathTemplate.uxml");
        m_ItemEffect = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>
            ("Assets/Editor/UIEditor/Item/ItemEffect.uxml");
        VisualElement rootFromUXML = visualTree.Instantiate();
        rootFromUXML.style.flexGrow = 1;


        LoadAllItems();

        m_Tabs = rootFromUXML.Q<VisualElement>("UnitListTab");
        m_SectionDetails = rootFromUXML.Q<ScrollView>("UnitDetails");
        m_SectionNoPath = rootFromUXML.Q<VisualElement>("BoxNoPath");
        m_SectionRulesInput = rootFromUXML.Q<VisualElement>("RulesInput");
        m_SectionEffects = rootFromUXML.Q<VisualElement>("EffectList");


        GenerateListView();

        // rootFromUXML.Q<Button>("NewNature").clicked += AddItem_OnClick;
        // rootFromUXML.Q<Button>("RemoveNature").clicked += DeleteItem_OnClick;


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


    private void GenerateRulesInput()
    {
        m_SectionRulesInput.Clear();
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
                newNode.RegisterCallback<ClickEvent>(OnClickRuleInput);
                if (IsExistsRuleInput((TypeNoPath)x))
                {
                    newNode.Q<VisualElement>("NodeCheck").RemoveFromClassList("node_check_hidden");
                }
                else
                {
                    newNode.Q<VisualElement>("NodeCheck").AddToClassList("node_check_hidden");
                }
            }

            m_SectionRulesInput.Add(newNode);

            col++;
            if (x % ((countNoPath * 2) + 1) == 0)
            {
                row++;
                col = 0;
            }
        }
    }

    private bool IsExistsRuleInput(TypeNoPath typeNoPath)
    {
        return m_activeItem.RulesInput.Contains(typeNoPath);
    }

    private void OnClickRuleInput(ClickEvent evt)
    {
        VisualElement clickedTab = evt.currentTarget as VisualElement;

        Label valueEl = clickedTab.Q<Label>("NodeValue");
        int index = Int32.Parse(valueEl.text);
        TypeNoPath val = (TypeNoPath)index;
        //Undo.RecordObject(m_activeItem, "Changed NoPath");
        if (IsExistsRuleInput((TypeNoPath)index))
        {
            m_activeItem.listRuleInput.Remove((TypeNoPath)index);
            clickedTab.Q<VisualElement>("NodeCheck").AddToClassList("node_check_hidden");
        }
        else
        {
            m_activeItem.listRuleInput.Add((TypeNoPath)index);
            clickedTab.Q<VisualElement>("NodeCheck").RemoveFromClassList("node_check_hidden");
        }
        EditorUtility.SetDirty(m_activeItem);
    }

    private void LoadAllItems()
    {
        m_UnitDB.Clear();
        string[] allPaths = Directory.GetFiles("Assets/Entity/MapObject", "*.asset",
            SearchOption.AllDirectories);
        foreach (string path in allPaths)
        {
            string cleanedPath = path.Replace("\\", "/");
            m_UnitDB.Add((ScriptableEntityMapObject)AssetDatabase.LoadAssetAtPath(cleanedPath,
                typeof(ScriptableEntity)));
        }

    }

    private void GenerateListView()
    {
        Func<VisualElement> makeItem = () => m_ItemRowTemplate.CloneTree();

        Action<VisualElement, int> bindItem = (e, i) =>
        {
            if (m_UnitDB[i].MenuSprite != null)
            {
                e.Q<VisualElement>("icon").style.backgroundImage = new StyleBackground(m_UnitDB[i].MenuSprite);

            }
            else
            {
                // SpriteRenderer[] sprites = m_UnitDB[i].Prefab.gameObject.GetComponentsInChildren<SpriteRenderer>();
                e.Q<VisualElement>("icon").style.backgroundImage = new StyleBackground(m_UnitDB[i].MenuSprite);
            }
            new StyleBackground(m_UnitDB[i].MenuSprite);
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

        m_activeItem = (ScriptableEntityMapObject)selectedItems.First();
        SerializedObject so = new SerializedObject(m_activeItem);
        m_SectionDetails.Bind(so);


        if (m_activeItem.MenuSprite != null)
        {
            m_SectionDetails.Q<VisualElement>("Sprite").style.backgroundImage = new StyleBackground(m_activeItem.MenuSprite);
        }
        else
        {
            // SpriteRenderer[] sprites = m_activeItem.Prefab.gameObject.GetComponentsInChildren<SpriteRenderer>();
            m_SectionDetails.Q<VisualElement>("Sprite").style.backgroundImage = new StyleBackground(m_activeItem.MenuSprite);
        }

        m_SectionDetails.style.visibility = Visibility.Visible;

        GenerateNoPath();
        GenerateRulesInput();
        DrawEffects();
    }

    private void DrawEffects()
    {
        m_SectionEffects.Clear();
        var sObj = new SerializedObject(m_activeItem);
        m_SectionEffects.Bind(sObj);
        for (int x = 0; x < m_activeItem.Effects.Count; x++)
        {
            // var elementEffect = m_ItemEffect.CloneTree();
            var index = x;
            var blokItem = new VisualElement();
            blokItem.AddToClassList("p-2");
            var item = m_activeItem.Effects[x];
            var probability = new DoubleField();
            probability.label = nameof(item.probability);
            // probability.value = item.probability;
            var propertyEffects = sObj.FindProperty("Effects");
            probability.BindProperty(propertyEffects.GetArrayElementAtIndex(x).FindPropertyRelative("probability"));
            // probability.bindingPath = nameof(item.probability);
            // probability.RegisterValueChangedCallback((ChangeEvent<double> evt) =>
            // {
            //     item.probability = evt.newValue;
            //     EditorUtility.SetDirty(m_activeItem);
            //     // DrawEffects();
            // });
            blokItem.Add(probability);

            var descriptionProperty = propertyEffects.GetArrayElementAtIndex(x).FindPropertyRelative("Item").FindPropertyRelative("description");
            var description = new PropertyField(descriptionProperty);
            description.label = nameof(item.Item.description);
            description.BindProperty(descriptionProperty);

            blokItem.Add(description);
            // var obj = new ObjectField();
            // obj.value = item;
            // m_SectionEffects.Add(obj);
            for (int y = 0; y < item.Item.items.Count(); y++)
            {
                var blokListEffects = new VisualElement();
                blokListEffects.style.flexDirection = FlexDirection.Row;
                blokListEffects.AddToClassList("p-1");
                blokListEffects.AddToClassList("box_shadow");
                blokListEffects.AddToClassList("m-px");

                var listEffects = new VisualElement();
                listEffects.AddToClassList("w-75");
                var listButtons = new VisualElement();
                listButtons.AddToClassList("w-25");

                var effect = item.Item.items[y];

                var labelEffect = new Label();
                labelEffect.text = effect.name;
                labelEffect.AddToClassList("text-primary");
                listEffects.Add(labelEffect);

                if (effect != null)
                {
                    var box = CreateUIElementInspector(effect, null);
                    listEffects.Add(box);
                    var b = new Button();
                    b.text = "Remove effect";
                    b.clickable.clicked += () =>
                    {
                        RemoveEffect(effect, item);
                    };
                    listButtons.Add(b);
                }

                blokListEffects.Add(listEffects);
                blokListEffects.Add(listButtons);
                blokItem.Add(blokListEffects);
            }

            var addButton = new Button();
            addButton.text = "Add effect";
            addButton.clickable.clicked += () =>
            {
                GenericMenu menu = new GenericMenu();
                var guids = AssetDatabase.FindAssets("Effect t:script", new[] { "Assets/Scripts/Scriptables" });
                //AssetDatabase.FindAssets("t:script");

                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var type = AssetDatabase.LoadAssetAtPath(path, typeof(System.Object));

                    // get the current assembly and look through all classes to find the fullname of the asset
                    bool typeFound = false;
                    Assembly assembly = typeof(BaseEffect).Assembly;

                    // make sure that incoming guid type inherits the type of the list
                    foreach (System.Type t in assembly.GetTypes())
                    {
                        if (t.IsSubclassOf(typeof(BaseEffect)) && t.Name == type.name)
                        {
                            typeFound = true;
                            break;
                        }
                    }

                    // if the type isn't found then skip
                    if (!typeFound) { continue; }

                    menu.AddItem(
                        new GUIContent(Path.GetFileNameWithoutExtension(path)),
                        false,
                        (object dataobj) =>
                        {
                            var data = (PropertyCreationParams)dataobj;

                            // create new sub property
                            var type = AssetDatabase.LoadAssetAtPath(data.Path, typeof(System.Object));
                            var newProperty = ScriptableObject.CreateInstance(type.name);
                            newProperty.name = type.name;

                            AssetDatabase.AddObjectToAsset(newProperty, m_activeItem);
                            // AssetDatabase.CreateAsset(newProperty, "Assets/Data/" + newProperty.name + ".asset");
                            AssetDatabase.SaveAssets();
                            item.Item.items.Add((BaseEffect)newProperty);
                            // element.objectReferenceValue = newProperty;
                            // // make room in list
                            // var index = item.Item.Count;
                            // item.Item.Count++;
                            // listx.index = index;
                            // var element = listx.serializedProperty.GetArrayElementAtIndex(index);
                            EditorUtility.SetDirty(m_activeItem);
                            DrawEffects();
                        },
                        new PropertyCreationParams() { Path = path });
                }

                menu.ShowAsContext();
            };
            blokItem.Add(addButton);

            var removeEffectItemButton = new Button();
            removeEffectItemButton.text = "Remove probability";
            removeEffectItemButton.clickable.clicked += () =>
            {
                // SerializedProperty removeItem = propertyEffects.GetArrayElementAtIndex(x);
                if (m_activeItem.Effects[index].Item.items.Count > 0)
                {
                    foreach (var effect in m_activeItem.Effects[index].Item.items.ToList())
                    {
                        RemoveEffect(effect, item);
                        // Debug.Log($"Remove effect {effect.name}");
                    }
                }
                m_activeItem.Effects.Remove(item);
                EditorUtility.SetDirty(m_activeItem);
                DrawEffects();
            };
            blokItem.Add(removeEffectItemButton);
            m_SectionEffects.Add(blokItem);
        }
        var buttonNewEffect = new Button();
        buttonNewEffect.text = "New effect probability";
        buttonNewEffect.clickable.clicked += () =>
        {
            m_activeItem.Effects.Add(new ItemProbabiliti<ItemEffect>()
            {
                Item = new ItemEffect()
                {
                    items = new List<BaseEffect>()
                },
                probability = 1
            });
            EditorUtility.SetDirty(m_activeItem);
            DrawEffects();
        };
        m_SectionEffects.Add(buttonNewEffect);
    }

    private void RemoveEffect(BaseEffect effect, ItemProbabiliti<ItemEffect> item)
    {
        item.Item.items.Remove(effect);
        AssetDatabase.RemoveObjectFromAsset(effect);

        UnityEngine.Object.DestroyImmediate(effect, true);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // m_activeItem.Effects[index].Item.Remove(effect);
        EditorUtility.SetDirty(m_activeItem);
        DrawEffects();
    }

    private VisualElement CreateUIElementInspector(UnityEngine.Object target, List<string> propertiesToExclude)
    {
        var container = new VisualElement();

        var serializedObject = new SerializedObject(target);

        var fields = GetVisibleSerializedFields(target.GetType());

        for (int i = 0; i < fields.Length; ++i)
        {
            var field = fields[i];
            // Do not draw HideInInspector fields or excluded properties.
            if (propertiesToExclude != null && propertiesToExclude.Contains(field.Name))
            {
                continue;
            }

            var serializedProperty = serializedObject.FindProperty(field.Name);

            var propertyField = new PropertyField(serializedProperty);

            container.Add(propertyField);
        }

        container.Bind(serializedObject);


        return container;
    }

    private FieldInfo[] GetVisibleSerializedFields(Type T)
    {
        List<FieldInfo> infoFields = new List<FieldInfo>();

        var publicFields = T.GetFields(BindingFlags.Instance | BindingFlags.Public);
        for (int i = 0; i < publicFields.Length; i++)
        {
            if (publicFields[i].GetCustomAttribute<HideInInspector>() == null)
            {
                infoFields.Add(publicFields[i]);
            }
        }

        var privateFields = T.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
        for (int i = 0; i < privateFields.Length; i++)
        {
            if (privateFields[i].GetCustomAttribute<SerializeField>() != null)
            {
                infoFields.Add(privateFields[i]);
            }
        }

        return infoFields.ToArray();
    }
    // private void AddItem_OnClick()
    // {
    //     //Create an instance of the scriptable object and set the default parameters
    //     ScriptableEntity newItem = ScriptableObject.CreateInstance<ScriptableEntity>();
    //     string path = EditorUtility.SaveFilePanelInProject("Save nature", "NewNature", "Asset", "Save new nature", "Assets/Resources/Nature");
    //     if (path == "") return;
    //     //newItem.FriendlyName = $"New Item";
    //     //newItem.Icon = m_DefaultItemIcon;
    //     //Create the asset, using the unique ID for the name
    //     AssetDatabase.CreateAsset(newItem, path); // $"Assets/Data/{newItem.ID}.asset"
    //     //Add it to the item list
    //     m_UnitDB.Add(newItem);
    //     //Refresh the ListView so everything is redrawn again
    //     m_ListUnit.Rebuild();
    //     //m_ListNature.style.height = m_ItemDatabase.Count * m_ItemHeight;
    // }

    // private void DeleteItem_OnClick()
    // {
    //     string path = AssetDatabase.GetAssetPath(m_activeItem);
    //     AssetDatabase.DeleteAsset(path);
    //     m_UnitDB.Remove(m_activeItem);
    //     m_ListUnit.Rebuild();
    //     //Nothing is selected, so hide the details section
    //     m_SectionDetails.style.visibility = Visibility.Hidden;
    // }


}
