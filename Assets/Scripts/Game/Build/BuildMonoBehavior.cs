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

        StartCoroutine(Pulse());
        // if (TypeCreateBuild == build.BuildLevels[i].TypeBuild)
        // {
        //     // Debug.Log($"Pulse {build.name}");
        //     StartCoroutine(obj.Pulse());
        // }
    }

    public async void OnPointerClick(PointerEventData eventData)
    {
        var b = Build.ConfigData;
        Debug.Log($"TypeBuild={b.TypeBuild}");
        switch (b.TypeBuild)
        {
            case TypeBuild.Town:
                Debug.Log("Click town");
                await OnClickToHall();
                break;
            case TypeBuild.Tavern:
                Debug.Log("Click tavern");
                await OnClickToTavern();
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

    protected virtual void OnDestroy()
    {
        Build.DestroyGameObject();
        // Build = null;
    }


}
