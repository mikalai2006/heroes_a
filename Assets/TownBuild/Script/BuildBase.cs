using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public abstract class BuildBase : MonoBehaviour
{
    public UITown UITown;

    // public List<GameObject> Childrens;

    // public ScriptableBuildBase ScriptableData;
    // public TypeBuild TypeBuild;
    // public int LevelBuild;
    public virtual void Start()
    {
        // foreach (Transform child in gameObject.transform)
        // {
        //     if (null == child)
        //         continue;
        //     Childrens.Add(child.gameObject);
        // }
    }

    public virtual void Update()
    {

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
        // Player player = LevelManager.Instance.ActivePlayer;
        // for (int i = 0; i < ScriptableData.BuildLevels.Count; i++)
        // {
        //     var bl = ScriptableData.BuildLevels[i];
        //     if ((player.ActiveTown.Data.ProgressBuilds & bl.TypeBuild) == bl.TypeBuild)
        //     {
        //         SetActiveLevel(i);
        //     }
        // }
        // Debug.Log($"Init {name}");
    }

    // private void SetActiveLevel(int i)
    // {
    //     for (int j = 0; j < Childrens.Count; j++)
    //     {
    //         Childrens[j].SetActive(j == i ? true : false);
    //     }
    // }
}
