using System.Collections.Generic;

using UnityEngine;

[CreateAssetMenu(fileName = "NewEntitySkillSchool", menuName = "Game/Entity/SkillSchool")]
public class ScriptableEntitySkillSchool : ScriptableEntity
{
    [Space(10)]
    [Header("Options Perks")]
    public List<EntitySkillSchoolItem> Skills;
    public List<EntityResource> Cost;

    public void OnDoHero(ref Player player, BaseEntity entity)
    {
        foreach (var primarySkill in Skills)
        {
            Debug.Log($"Increment hero skills {primarySkill.PrimarySkill.name}[{primarySkill.value}]");
        }
    }
}

[System.Serializable]
public struct EntitySkillSchoolItem
{
    public string id;
    public ScriptableAttributePrimarySkill PrimarySkill;
    public int value;
}