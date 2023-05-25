using System;
using System.Threading.Tasks;


using UnityEngine.UIElements;

public class UIArenaEndStatWindow : UIDialogBaseWindow
{
    private readonly string _nameButtonClose = "Cancel";
    private Button _buttonClose;
    public static event Action OnCloseStat;
    protected TaskCompletionSource<ArenaStatResult> _processCompletionSource;
    protected ArenaStatResult _dataResultDialog;
    private ArenaStat _stat;

    public override void Start()
    {
        base.Start();

        _buttonClose = DialogApp.rootVisualElement.Q<TemplateContainer>(_nameButtonClose).Q<Button>("Btn");
        _buttonClose.clickable.clicked += OnClickClose;

    }

    private async void OnClickClose()
    {
        await AudioManager.Instance.Click();
        _dataResultDialog.isOk = false;
        _processCompletionSource.SetResult(_dataResultDialog);
        OnCloseStat?.Invoke();
    }

    public async Task<ArenaStatResult> ProcessAction(ArenaStat stat)
    {
        base.Init();
        _stat = stat;

        _dataResultDialog = new ArenaStatResult();
        _processCompletionSource = new TaskCompletionSource<ArenaStatResult>();

        DrawStat();

        return await _processCompletionSource.Task;
    }

    private void DrawStat()
    {
        // var creature = _stat.Entity;
        // var creatureData = ((EntityCreature)_stat.Entity).ConfigAttribute;
        // var parameters = ((ScriptableAttributeCreature)_stat.Entity.ScriptableDataAttribute).CreatureParams;
        // root.Q<Label>("AttackValue").text
        //     = string.Format("{0}({1})", parameters.Attack, parameters.Attack + _stat.Data.AttackModificators.Values.Sum());
        // //  = parameters.Attack.ToString();
        // root.Q<Label>("DefenseValue").text
        //     = string.Format("{0}({1})", parameters.Defense, parameters.Defense + _stat.Data.DefenseModificators.Values.Sum());
        // // = parameters.Defense.ToString();
        // if (parameters.Shoots != 0)
        // {
        //     root.Q<Label>("AmmountValue").text = parameters.Shoots.ToString();
        // }
        // root.Q<Label>("DamageValue").text
        //     = string.Format("{0}-{1}", parameters.DamageMin, parameters.DamageMax);
        // root.Q<Label>("HPValue").text = parameters.HP.ToString();
        // int currentHP = _stat.Data.totalHP - (_stat.Data.HP * (_stat.Data.quantity - 1));
        // root.Q<Label>("HPAValue").text
        //     = (currentHP == 0 ? _stat.Data.HP : currentHP).ToString();
        // root.Q<Label>("SpeedValue").text
        //     = string.Format("{0}({1})", parameters.Speed, _stat.Speed);

        // VisualElement elementSprite = root.Q<VisualElement>("AnimationBlok");
        // elementSprite.style.backgroundImage
        //     = new StyleBackground(creatureData.MenuSprite);

        // root.Q<Label>("Quantity").text = _stat.Data.quantity.ToString();

        // VisualElement activeSpell = root.Q<VisualElement>("ActiveSpell");
        // activeSpell.Clear();
        // foreach (var spellItem in _stat.Data.SpellsState)
        // {
        //     Button btn = new Button();
        //     btn.AddToClassList("button");
        //     btn.AddToClassList("bg-transparent");
        //     btn.AddToClassList("button_bordered");
        //     btn.AddToClassList("m-05");
        //     btn.AddToClassList("p-0");
        //     btn.style.width = new StyleLength(new Length(80, LengthUnit.Pixel));
        //     btn.style.height = new StyleLength(new Length(60, LengthUnit.Pixel));
        //     VisualElement imgSpell = new VisualElement();
        //     imgSpell.AddToClassList("w-full");
        //     imgSpell.AddToClassList("h-full");
        //     imgSpell.style.backgroundImage
        //         = new StyleBackground(spellItem.Key.MenuSprite);
        //     btn.Add(imgSpell);
        //     activeSpell.Add(btn);
        // }
    }
}

