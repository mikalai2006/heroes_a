using System.Collections.Generic;

using Cysharp.Threading.Tasks;


public abstract class BaseExplore : BaseMapObject, IDialogMapObjectOperation
{
    public async override void OnGoHero(Player player)
    {
        base.OnGoHero(player);

        DataResultDialog result = await OnTriggeredHero();

        if (result.isOk)
        {
            List<GridTileNode> noskyNodes = GameManager.Instance.MapManager.DrawSky(OccupiedNode, 10);

            player.SetNosky(noskyNodes);
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
