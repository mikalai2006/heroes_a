using Cysharp.Threading.Tasks;

public interface IClickeredBuild
{
    UniTask<DataResultBuildDialog> OnClickToBuild();
}
