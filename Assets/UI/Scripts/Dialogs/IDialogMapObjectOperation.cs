using Cysharp.Threading.Tasks;

public interface IDialogMapObjectOperation
{
    UniTask<DataResultDialog> OnTriggeredHero();

    // void SetPlayer(Player player);

}
