using System.Collections.Generic;

using UnityEngine;

[System.Serializable]
public class EntityBook
{
    [SerializeField] public DataBook Data = new DataBook();

    public EntityBook(EntityHero hero)
    {
        Data.idPlayer = hero.Data.idPlayer;
        Data.Spells = new List<ScriptableAttributeSpell>();

        if (hero.Data.spells != null && hero.Data.spells.Count != 0)
        {
            foreach (var spell in hero.Data.spells)
            {
                var spellData = ResourceSystem.Instance
                    .GetAttributesByType<ScriptableAttributeSpell>(TypeAttribute.Spell)
                    .Find(t => t.idObject == spell);
                Data.Spells.Add(spellData);
            }
        }
    }

}

[System.Serializable]
public struct DataBook
{
    public int idPlayer;
    public List<ScriptableAttributeSpell> Spells;
}