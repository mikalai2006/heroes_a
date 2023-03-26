using Cysharp.Threading.Tasks;

using UnityEngine;

public class BaseSkillSchool : BaseMapObject, IDialogMapObjectOperation
{
    public override async void OnGoHero(Player player)
    {
        base.OnGoHero(player);

        DataResultDialog result = await OnTriggeredHero();

        if (result.isOk)
        {
            Debug.Log($"Check {result.keyVariant}");

        }
        else
        {
            // Click cancel.
        }
    }

    public async UniTask<DataResultDialog> OnTriggeredHero()
    {
        var dialogData = new DataDialog()
        {
            Description = this.ScriptableData.name,
            Header = this.name,
            Sprite = this.ScriptableData.MenuSprite,
        };

        var dialogWindow = new DialogMapObjectProvider(dialogData);
        return await dialogWindow.ShowAndHide();
    }
}
