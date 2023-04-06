using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class EntitySkillSchool : BaseEntity, IDataPlay
{
    public ScriptableEntitySkillSchool ConfigData => (ScriptableEntitySkillSchool)ScriptableData;
    public DataSkillSchool Data;

    public EntitySkillSchool(GridTileNode node)
    {
        List<ScriptableEntitySkillSchool> list = ResourceSystem.Instance
            .GetEntityByType<ScriptableEntitySkillSchool>(TypeEntity.SkillSchool)
            .ToList();
        ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];

        Data = new DataSkillSchool();
        Data.Skills = new List<ItemSkill>();
        Data.TypeWork = ConfigData.TypeWorkPerk;

        for (int i = 0; i < ConfigData.PrimarySkills.Count; i++)
        {
            List<ItemSkill> ListVariant = ConfigData.PrimarySkills[i].ListVariant;
            for (int j = 0; j < ListVariant.Count; j++)
            {
                Data.Skills.Add(ListVariant[j]);
            }

        }
        base.Init(ScriptableData, node);
    }

    public void SetPlayer(PlayerData data)
    {
        //Debug.Log($"Town SetPlayer::: id{data.id}-idArea{data.idArea}");

    }

    public void LoadDataPlay(DataPlay data)
    {
        //throw new System.NotImplementedException();
    }

    public void SaveDataPlay(ref DataPlay data)
    {
        // var sdata = SaveUnit(Data);
        // data.Units.warriors.Add(sdata);
    }
}

[System.Serializable]
public struct DataSkillSchool
{
    public List<ItemSkill> Skills;
    public TypeWorkPerk TypeWork;
}