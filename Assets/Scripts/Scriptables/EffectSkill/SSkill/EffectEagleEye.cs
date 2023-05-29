using System.Collections.Generic;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization;

using Random = System.Random;

[CreateAssetMenu(fileName = "EagleEyeEffect", menuName = "Game/EffectSkill/EffectEagleEye")]
public class EffectEagleEye : BaseEffectSkill
{
    public async override UniTask RunEffect(Player player, BaseEntity entity)
    {
        var hero = ((EntityHero)entity);

        // var secondarySkill = ResourceSystem.Instance
        //     .GetAttributesByType<ScriptableAttributeSecondarySkill>(TypeAttribute.SecondarySkill)
        //     .Find(t => t.TypeTwoSkill == TypeSecondarySkill.EagleEye);

        // hero.Data.Qualities.scout = secondarySkill.Levels[hero.Data.SSkills[secondarySkill.TypeTwoSkill].level].value;
        // Effect eagle eye
        var valueEagleEye = hero.Data.SSkills[TypeSecondarySkill.EagleEye];
        Random random = new Random();
        int rand = random.Next(0, 100);
        if (rand <= valueEagleEye.value)
        {
            // Get Wisdom level.
            int maxlevel = 2;
            if (hero.Data.SSkills.ContainsKey(TypeSecondarySkill.Wisdom))
            {
                maxlevel = hero.Data.SSkills[TypeSecondarySkill.Wisdom].level + 1;
            }

            int levelEagleEye = valueEagleEye.level + 1;
            var spells = hero.SpellBook.GenerateSpells(
                TypeSpell.Combat,
                levelEagleEye <= maxlevel ? levelEagleEye : maxlevel,
                2);
            if (spells.Count > 0)
            {
                var indexChoose = UnityEngine.Random.Range(0, spells.Count);
                if (player.DataPlayer.playerType != PlayerType.Bot)
                {

                    var _dialogData = new DataDialogMapObjectGroup()
                    {
                        Values = new List<DataDialogMapObjectGroupItem>()
                    };

                    foreach (var spell in spells)
                    {
                        _dialogData.Values.Add(new DataDialogMapObjectGroupItem()
                        {
                            Sprite = spell.MenuSprite,
                            title = spell.Text.title.GetLocalizedString()
                        });
                    }
                    var groups = new List<DataDialogMapObjectGroup>();

                    groups.Add(_dialogData);

                    var dialogData = new DataDialogMapObject()
                    {
                        // Header = configData.Text.title.GetLocalizedString(),
                        Description = new LocalizedString(Constants.LanguageTable.LANG_TABLE_ADVENTURE, "eagleeyeffect_t").GetLocalizedString(),
                        TypeCheck = TypeCheck.Choose,
                        TypeWorkEffect = TypeWorkAttribute.One,
                        Groups = groups
                    };

                    var dialogWindow = new DialogMapObjectProvider(dialogData);
                    var result = await dialogWindow.ShowAndHide();
                    indexChoose = result.keyVariant;
                }

                hero.SpellBook.AddSpell(spells[indexChoose]);
            }
        }
    }
}
