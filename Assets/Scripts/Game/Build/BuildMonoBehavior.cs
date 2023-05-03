using System.Collections;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.EventSystems;

public class BuildMonoBehavior : MonoBehaviour, IPointerClickHandler
{
    public UITown UITown;
    [SerializeField] protected BaseBuild Build;
    public BaseBuild GetBuild => Build;

    #region Events GameState
    #endregion

    public void GoPulse()
    {
        StartCoroutine(Pulse());
    }

    public IEnumerator Pulse()
    {
        foreach (Transform transform in Helpers.GetDeepChildren<SpriteRenderer>(gameObject, true))
        {
            var sprite = transform.GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                sprite.gameObject.AddComponent<Pulsable>();
            }
        }
        yield return new WaitForSeconds(3f);
        foreach (Pulsable pulsable in GameObject.FindObjectsOfType<Pulsable>())
        {
            pulsable.Reset();
            Destroy(pulsable);
        }
    }

    public virtual void Init(UITown uiTown)
    {
        UITown = uiTown;
    }

    public virtual void InitGameObject(BaseBuild buildObject)
    {
        Build = buildObject;
        for (int y = 0; y < transform.childCount; y++)
        {
            // obj.TypeBuild = build.BuildLevels[i].TypeBuild;

            Transform child = transform.GetChild(y);
            if (null == child)
                continue;

            // obj.LevelBuild = y;
            if (Build.level == y)
            {
                child.gameObject.SetActive(true);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }
        // if (TypeCreateBuild == build.BuildLevels[i].TypeBuild)
        // {
        //     // Debug.Log($"Pulse {build.name}");
        //     StartCoroutine(obj.Pulse());
        // }
    }

    public async void OnPointerClick(PointerEventData eventData)
    {
        var b = Build.ConfigData;
        switch (b.TypeBuild)
        {
            case TypeBuild.Town:
                await OnClickToHall();
                break;
            case TypeBuild.Tavern:
                await OnClickToTavern();
                break;
            case TypeBuild.Army:
                await OnClickArmy();
                break;
        }
    }

    public async UniTask<DataResultBuildDialog> OnClickToHall()
    {
        var dialogWindow = new UITownListBuildOperation(new DataDialogMapObject());
        return await dialogWindow.ShowAndHide();
    }

    public async UniTask<DataResultBuildDialog> OnClickToTavern()
    {
        var dialogWindow = new UITavernOperation(Build);
        return await dialogWindow.ShowAndHide();
    }

    public async UniTask<DataResultDialogDwelling> OnClickArmy()
    {
        var dwelling = ((BuildArmy)Build).Dwelling;
        Debug.Log($"GetBuild.Town.Data.HeroinTown={GetBuild.Town.Data.HeroinTown != null}");
        var creatures = GetBuild.Town.Data.HeroinTown != null
            ? ((EntityHero)UnitManager.Entities[GetBuild.Town.Data.HeroinTown]).Data.Creatures
            : GetBuild.Town.Data.Creatures;
        var dialogWindow = new DialogDwellingProvider(new DataDialogDwelling()
        {
            Creatures = creatures,
            dwelling = dwelling
        });
        return await dialogWindow.ShowAndHide();
    }

    protected virtual void OnDestroy()
    {
        Build.DestroyGameObject();
        // Build = null;
    }


}
