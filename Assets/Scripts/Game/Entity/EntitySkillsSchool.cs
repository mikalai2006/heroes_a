using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

[Serializable]
public class EntitySkillSchool : BaseEntity, ISaveDataPlay
{
    public ScriptableEntitySkillSchool ConfigData => (ScriptableEntitySkillSchool)ScriptableData;
    [SerializeField] public DataSkillSchool Data = new DataSkillSchool();

    public EntitySkillSchool(GridTileNode node, SaveDataUnit<DataSkillSchool> saveData = null)
    {
        if (saveData == null)
        {
            List<ScriptableEntitySkillSchool> list = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntitySkillSchool>(TypeEntity.SkillSchool)
                .ToList();
            ScriptableData = list[UnityEngine.Random.Range(0, list.Count)];

            // Data.Skills = new List<ItemSkill>();
            Data.TypeWork = ConfigData.TypeWorkPerk;

            // for (int i = 0; i < ConfigData.PrimarySkills.Count; i++)
            // {
            //     List<ItemSkill> ListVariant = ConfigData.PrimarySkills[i].ListVariant;
            //     for (int j = 0; j < ListVariant.Count; j++)
            //     {
            //         Data.Skills.Add(ListVariant[j]);
            //     }

            // }
        }
        else
        {
            ScriptableData = ResourceSystem.Instance
                .GetEntityByType<ScriptableEntitySkillSchool>(TypeEntity.SkillSchool)
                .Where(t => t.idObject == saveData.idObject)
                .First();
            Data = saveData.data;
            idUnit = saveData.idUnit;
        }
        base.Init(ScriptableData, node);
    }

    public override void SetPlayer(Player player)
    {
        ScriptableEntitySkillSchool configData = (ScriptableEntitySkillSchool)ScriptableData;
        configData.OnDoHero(ref player, this);
        // for (int i = 0; i < Data.Skills.Count; i++)
        // {
        //     // Change skill for hero.
        //     // player.ChangeResource(Data.Skills[i].Skill.TypeSkill, Data.Skills[i].Value);
        // }
        if (Data.TypeWork == TypeWorkPerk.One)
        {
            //ScriptableData.MapPrefab.ReleaseInstance(gameObject);
            MapObjectGameObject.DestroyGameObject();
        }
    }

    #region SaveLoadData
    // public void LoadDataPlay(DataPlay data)
    // {
    //     throw new System.NotImplementedException();
    // }

    public void SaveDataPlay(ref DataPlay data)
    {
        var sdata = SaveUnit(Data);
        data.entity.skillSchools.Add(sdata);
    }
    #endregion
}

[System.Serializable]
public struct DataSkillSchool
{
    // public List<ItemSkill> Skills;
    public TypeWorkPerk TypeWork;
}