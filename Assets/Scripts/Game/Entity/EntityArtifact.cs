using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[System.Serializable]
public class EntityArtifact : BaseEntity
{
    [SerializeField] public DataArtifact Data = new DataArtifact();
    private ScriptableEntityOther ConfigData => (ScriptableEntityOther)ScriptableData;
    public ScriptableAttributeArtifact ConfigAttribute => (ScriptableAttributeArtifact)ScriptableDataAttribute;

    public EntityArtifact(
        ScriptableEntityOther configData,
        ScriptableAttributeArtifact configArtifact,
        SaveDataUnit<DataArtifact> saveData = null)
    {
        base.Init();

        if (saveData == null)
        {
            ScriptableData = configData;
            SetData(configArtifact);
            _idObject = ScriptableData.idObject;
            ScriptableDataAttribute = configArtifact;
            Data.idPlayer = -1;
        }
        else
        {
            // ResourceSystem.Instance
            //     .GetEntityByType<ScriptableEntityArtifact>(TypeEntity.MapObject)
            //     .Where(t => t.idObject == saveData.idObject && t.TypeMapObject == TypeMapObject.Artifact)
            Data = saveData.data;
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntityMapObject>(TypeEntity.MapObject)
                .Where(t => t.TypeMapObject == TypeMapObject.Artifact
                && t.idObject == saveData.idObject).First();

            ScriptableDataAttribute = ResourceSystem.Instance
                .GetAttributesByType<ScriptableAttributeArtifact>(TypeAttribute.Artifact)
                .Where(t => t.idObject == Data.ida)
                .First();

            _idEntity = saveData.idEntity;
            _idObject = saveData.idObject;
            Effects = saveData.Effects;
        }
    }

    public void SetData(ScriptableAttributeArtifact configArtifact)
    {
        Data.ida = configArtifact.idObject;
    }

    public override void SetPlayer(Player player)
    {
        // ScriptableEntityArtifact configData = (ScriptableEntityArtifact)ScriptableData;
        // // configData.RunHero(ref player, this);
        // LevelManager.Instance.ActivePlayer.ActiveHero.AddArtifact(this);

        // if (configData.TypeWorkObject == TypeWorkObject.One)
        // {
        //     List<GridTileNode> nodes
        //         = GameManager.Instance.MapManager.gridTileHelper.GetNodeListAsNoPath(OccupiedNode, configData.RulesInput);
        //     foreach (var node in nodes)
        //     {
        //         node.RemoveStateNode(StateNode.Input);
        //     }
        //     // MapObjectGameObject.DestroyGameObject();
        //     DestroyEntity();
        // }

        // DestroyEntity();
    }

    #region InitData
    public void InitData(SaveDataUnit<DataArtifact> data)
    {
        Data = data.data;
    }
    #endregion

    #region SaveLoadData
    // public void LoadDataPlay(DataPlay data)
    // {
    //     throw new System.NotImplementedException();
    // }

    public override void SaveEntity(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.artifacts.Add(sdata);
    }
    #endregion
}

[System.Serializable]
public struct DataArtifact
{
    public int idPlayer;
    public string ida;
}