using Cysharp.Threading.Tasks;

public interface IClickeredBuild
{
    UniTask<DataResultDialog> OnClickToBuild();
}
