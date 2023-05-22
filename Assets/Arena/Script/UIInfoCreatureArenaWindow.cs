using System.Linq;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine.UIElements;

public class UIInfoCreatureArenaWindow : UIDialogBaseWindow
{
    private readonly string _nameButtonClose = "ButtonClose";
    private Button _buttonClose;
    protected TaskCompletionSource<DataResultBuildDialog> _processCompletionSource;
    protected DataResultBuildDialog _dataResultDialog;
    private ArenaEntity _arenaEntity;

    public override void Start()
    {
        base.Start();

        _buttonClose = DialogApp.rootVisualElement.Q<TemplateContainer>(_nameButtonClose).Q<Button>("Btn");
        _buttonClose.clickable.clicked += OnClickClose;

    }

    private void OnClickClose()
    {
        _dataResultDialog.isOk = false;
        _processCompletionSource.SetResult(_dataResultDialog);
    }

    public async Task<DataResultBuildDialog> ProcessAction(ArenaEntity arenaEntity)
    {
        base.Init();
        _arenaEntity = arenaEntity;

        _dataResultDialog = new DataResultBuildDialog();
        _processCompletionSource = new TaskCompletionSource<DataResultBuildDialog>();

        DrawCreatureInfo();

        return await _processCompletionSource.Task;
    }

    private void DrawCreatureInfo()
    {
        var creature = _arenaEntity.Entity;
        var creatureData = ((EntityCreature)_arenaEntity.Entity).ConfigAttribute;
        var parameters = ((ScriptableAttributeCreature)_arenaEntity.Entity.ScriptableDataAttribute).CreatureParams;
        root.Q<Label>("AttackValue").text
            = string.Format("{0}({1})", parameters.Attack, parameters.Attack + _arenaEntity.Data.AttackModificators.Values.Sum());
        //  = parameters.Attack.ToString();
        root.Q<Label>("DefenseValue").text = parameters.Defense.ToString();
        if (parameters.Shoots != 0)
        {
            root.Q<Label>("AmmountValue").text = parameters.Shoots.ToString();
        }
        root.Q<Label>("DamageValue").text
            = string.Format("{0}-{1}", parameters.DamageMin, parameters.DamageMax);
        root.Q<Label>("HPValue").text = parameters.HP.ToString();
        root.Q<Label>("HPAValue").text
            = (_arenaEntity.Data.totalHP - _arenaEntity.Data.HP * _arenaEntity.Data.quantity).ToString();
        root.Q<Label>("SpeedValue").text
            = string.Format("{0}({1})", parameters.Speed, _arenaEntity.Speed);

        VisualElement elementSprite = root.Q<VisualElement>("AnimationBlok");
        elementSprite.style.backgroundImage
            = new StyleBackground(creatureData.MenuSprite);

        root.Q<Label>("Quantity").text = _arenaEntity.Data.quantity.ToString();

        VisualElement activeSpell = root.Q<VisualElement>("ActiveSpell");
        activeSpell.Clear();
        foreach (var spellItem in _arenaEntity.Data.SpellsState)
        {
            Button btn = new Button();
            btn.AddToClassList("button");
            btn.AddToClassList("bg-transparent");
            btn.AddToClassList("button_bordered");
            btn.AddToClassList("m-05");
            btn.AddToClassList("p-0");
            btn.style.width = new StyleLength(new Length(80, LengthUnit.Pixel));
            btn.style.height = new StyleLength(new Length(60, LengthUnit.Pixel));
            VisualElement imgSpell = new VisualElement();
            imgSpell.AddToClassList("w-full");
            imgSpell.AddToClassList("h-full");
            imgSpell.style.backgroundImage
                = new StyleBackground(spellItem.Key.MenuSprite);
            btn.Add(imgSpell);
            activeSpell.Add(btn);
        }
    }
}

