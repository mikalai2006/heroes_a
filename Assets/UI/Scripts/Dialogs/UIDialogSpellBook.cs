using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Events;
using System.Collections.Generic;

public class UIDialogSpellBook : UIDialogBaseWindow
{
    [SerializeField] private VisualTreeAsset _templateCostItem;
    [SerializeField] private VisualTreeAsset _templateButton;
    [SerializeField] private VisualTreeAsset _templateButtonCheckItem;
    private readonly string _nameSpriteElement = "Sprite";
    private readonly string _nameValueLabel = "Value";
    private readonly string _nameBoxVariants = "BoxVariants";
    private readonly string _nameSpriteObject = "SpriteObject";

    private Button _buttonOk;
    private Button _buttonCancel;
    private VisualElement _spellList1;
    private VisualElement _spellList2;
    private VisualElement _boxSpriteObject;

    private TaskCompletionSource<DataResultDialogSpellBook> _processCompletionSource;

    public UnityEvent processAction;

    private DataDialogSpellBook _dataDialog;
    private DataResultDialogSpellBook _dataResultDialog;

    public override void Start()
    {
        base.Start();

        Panel.AddToClassList("w-full");
        Panel.AddToClassList("h-full");
        Title.style.display = DisplayStyle.None;

        _buttonOk = root.Q<Button>("Close");
        _buttonOk.clickable.clicked += OnClickClose;

        _spellList1 = root.Q<VisualElement>("SpellList1");
        // _spellList2 = root.Q<VisualElement>("SpellList2");

        // base.Localize(root);
    }

    public async Task<DataResultDialogSpellBook> ProcessAction(DataDialogSpellBook dataDialog)
    {
        base.Init();

        _dataDialog = dataDialog;
        _dataResultDialog = new DataResultDialogSpellBook()
        {
        };


        // if (_dataDialog.TypeCheck == TypeCheck.OnlyOk)
        // {
        //     _buttonCancel.style.display = DisplayStyle.None;
        // }

        // Title.text = _dataDialog.Header;
        // Panel.Q<Label>("Description").text = _dataDialog.Description;
        // if (_dataDialog.Sprite != null)
        // {
        //     _boxSpriteObject.style.backgroundImage = new StyleBackground(_dataDialog.Sprite);
        //     _boxSpriteObject.style.width = new StyleLength(new Length(
        //         _dataDialog.Sprite.bounds.size.x * _dataDialog.Sprite.pixelsPerUnit,
        //         LengthUnit.Pixel
        //     ));
        //     _boxSpriteObject.style.height = new StyleLength(new Length(
        //         _dataDialog.Sprite.bounds.size.y * _dataDialog.Sprite.pixelsPerUnit,
        //         LengthUnit.Pixel
        //     ));
        // }

        // for (int i = 0; i < _dataDialog.Groups.Count; i++)
        // {
        //     for (int j = 0; j < _dataDialog.Groups[i].Values.Count; j++)
        //     {
        //         VisualElement item = _templateCostItem.Instantiate();
        //         if (_dataDialog.TypeWorkEffect == TypeWorkAttribute.One)
        //         {
        //             var index = j;
        //             item = _templateButtonCheckItem.Instantiate();
        //             var btn = item.Q<Button>();
        //             btn.RegisterCallback<ClickEvent>((ClickEvent evt) =>
        //             {
        //                 ResetClassButton();
        //                 btn.AddToClassList("button_checked");
        //                 btn.AddToClassList("border-color");
        //                 btn.RemoveFromClassList("button_bordered");
        //             });
        //         }
        //         var _spriteElement = item.Q<VisualElement>(_nameSpriteElement);
        //         var _valueLabel = item.Q<Label>(_nameValueLabel);
        //         var sprite = _dataDialog.Groups[i].Values[j].Sprite;

        //         _spriteElement.style.backgroundImage = new StyleBackground(sprite);
        //         _spriteElement.style.width = new StyleLength(
        //             new Length(sprite.bounds.size.x * sprite.pixelsPerUnit, LengthUnit.Pixel)
        //         );
        //         _spriteElement.style.height = new StyleLength(
        //             new Length(sprite.bounds.size.y * sprite.pixelsPerUnit, LengthUnit.Pixel)
        //         );

        //         var val = _dataDialog.Groups[i].Values[j].Value;
        //         _valueLabel.text = val != 0 ? val.ToString() : "";

        //         _boxVariantsElement.Add(item);
        //     }
        // }
        // ResetClassButton();

        _processCompletionSource = new TaskCompletionSource<DataResultDialogSpellBook>();

        return await _processCompletionSource.Task;
    }

    private void OnClickClose()
    {
        _dataResultDialog.isOk = true;
        _processCompletionSource.SetResult(_dataResultDialog);

        processAction?.Invoke();
    }
}

