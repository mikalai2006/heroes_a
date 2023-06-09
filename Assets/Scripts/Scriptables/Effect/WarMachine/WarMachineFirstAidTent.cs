using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "FirstAidTent", menuName = "Game/Attribute/WarMachine/FirstAidTent")]
public class WarMachineFirstAidTent : ScriptableAttributeWarMachine
{
    public AssetReferenceGameObject AnimatePrefab;
    public async override UniTask<ArenaResultChoose> ChooseTarget(ArenaManager arenaManager, ArenaHeroEntity hero, Player player = null)
    {
        List<GridArenaNode> nodes = arenaManager
            .GridArenaHelper
            .GetAllGridNodes()
            .Where(t =>
                t.OccupiedUnit != null
                && !t.StateArenaNode.HasFlag(StateArenaNode.Excluded)
                && t.StateArenaNode.HasFlag(StateArenaNode.Occupied)
                && t.OccupiedUnit.TypeArenaPlayer == arenaManager.ArenaQueue.activeEntity.arenaEntity.TypeArenaPlayer
                // && (((EntityCreature)t.OccupiedUnit.Entity).ConfigAttribute.TypeAttribute != TypeAttribute.WarMachine)
                && t.OccupiedUnit is ArenaCreature
            )
            .ToList();
        // arenaManager.ArenaQueue.activeEntity.arenaEntity.Data.typeAttack = TypeAttack.AttackWarMachine;

        // Checktype run.
        int levelSSkill = 0;
        if (arenaManager.ArenaQueue.ActiveHero != null)
        {
            levelSSkill = arenaManager.ArenaQueue.ActiveHero.Entity.Data.SSkills.ContainsKey(TypeSecondarySkill.FirstAid)
                ? arenaManager.ArenaQueue.ActiveHero.Entity.Data.SSkills[TypeSecondarySkill.FirstAid].level + 1
                : 0;
        }
        ArenaTypeRunEffect typeRunEffect = ArenaTypeRunEffect.AutoChoose;
        switch (levelSSkill)
        {
            case 0:
                typeRunEffect = ArenaTypeRunEffect.AutoChoose;
                break;
            case 1:
            case 2:
            case 3:
                typeRunEffect = ArenaTypeRunEffect.Choosed;
                break;
        }

        await UniTask.Delay(1);
        return new ArenaResultChoose()
        {
            ChoosedNodes = nodes,
            TypeRunEffect = typeRunEffect
        };
    }

    public async override UniTask RunEffect(ArenaManager arenaManager, GridArenaNode node, GridArenaNode nodeToAction, Player player = null)
    {
        // Run effect.
        if (AnimatePrefab.RuntimeKeyIsValid())
        {
            var asset = Addressables.InstantiateAsync(
                       AnimatePrefab,
                       new Vector3(0, 1, 0),
                       Quaternion.identity,
               nodeToAction.OccupiedUnit is ArenaCreature ?
                ((ArenaCreature)nodeToAction.OccupiedUnit).ArenaMonoBehavior.transform
                : ((ArenaWarMachine)nodeToAction.OccupiedUnit).ArenaWarMachineMonoBehavior.transform
                   );
            var obj = await asset.Task;
            obj.gameObject.transform.localPosition = new Vector3(0, 1, 0);
            await UniTask.Delay(1000);
            Addressables.Release(asset);
        }

        // Calculate cure.
        int recoveryHP = Random.Range(CreatureParams.DamageMin, CreatureParams.DamageMax);
        var levelSSkill = arenaManager.ArenaQueue.ActiveHero.Entity.Data.SSkills.ContainsKey(TypeSecondarySkill.FirstAid)
            ? arenaManager.ArenaQueue.ActiveHero.Entity.Data.SSkills[TypeSecondarySkill.FirstAid].level + 1
            : 0;
        if (levelSSkill > 0)
        {
            var pSkill = arenaManager.ArenaQueue.ActiveHero.Entity.Data.SSkills[TypeSecondarySkill.FirstAid];
            recoveryHP = Random.Range(1, pSkill.value);
        }

        var entity = nodeToAction.OccupiedUnit;
        Debug.Log($"{name}::: Before cure: {entity.Data.totalHP}");
        entity.SetDamage(-recoveryHP);
        Debug.Log($"{name}::: After cure: {entity.Data.totalHP}[{recoveryHP}]");

        await UniTask.Yield();
    }
}

