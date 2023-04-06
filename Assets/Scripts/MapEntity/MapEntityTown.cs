using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class MapEntityTown : BaseMapEntity
{
    public override void InitUnit(BaseEntity mapObjects)
    {

        base.InitUnit(mapObjects);

    }
    //private void Awake() => GameManager.OnBeforeStateChanged += OnStateChanged;

    //private void OnDestroy() => GameManager.OnBeforeStateChanged -= OnStateChanged;

    public override void OnAfterStateChanged(GameState newState)
    {
        //if (newState == GameState.HeroTurn) _canMove = true;
        base.OnAfterStateChanged(newState);
    }

    public void SetPlayer(Player player)
    {
        Transform flag = transform.Find("Flag");
        flag.GetComponent<SpriteRenderer>().color = player.DataPlayer.color;
    }

    public virtual void ExecuteMove()
    {
        // Override this to do some hero-specific logic, then call this base method to clean up the turn

        //_canMove = false;
    }

    //public override void OnSaveUnit()
    //{
    //    // SaveUnit(Data);
    //}

    // public void LoadDataPlay(DataPlay data)
    // {
    //     //throw new System.NotImplementedException();
    // }
    // public void SaveDataPlay(ref DataPlay data)
    // {
    //     var sdata = SaveUnit(Data);
    //     data.Units.towns.Add(sdata);
    // }
}