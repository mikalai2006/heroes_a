using System.Collections.Generic;
using System.Linq;

using Cysharp.Threading.Tasks;

using UnityEngine;


[CreateAssetMenu(fileName = "Catapult", menuName = "Game/Attribute/WarMachine/Catapult")]
public class WarMachineCatapult : ScriptableAttributeWarMachine
{
    public async override UniTask<ArenaResultChoose> ChooseTarget(ArenaManager arenaManager, EntityHero hero, Player player = null)
    {
        // Check type run.
        int levelSSkill = 0;
        if (arenaManager.ArenaQueue.ActiveHero != null)
        {
            levelSSkill = arenaManager.ArenaQueue.ActiveHero.Data.SSkills.ContainsKey(TypeSecondarySkill.Ballistics)
                ? arenaManager.ArenaQueue.ActiveHero.Data.SSkills[TypeSecondarySkill.Ballistics].level + 1
                : 0;
        }
        var dataActiveHero = arenaManager.ArenaQueue.ActiveHero.ArenaHeroEntity;
        ArenaTypeRunEffect typeRunEffect = ArenaTypeRunEffect.AutoChoose;

        arenaManager.ArenaTown.ArenaEntityTownMB.SetStatusColliders(true);
        switch (levelSSkill)
        {
            case 0:
                typeRunEffect = ArenaTypeRunEffect.AutoChoose;
                var fortification = arenaManager.ArenaTown.FortificationsGameObject;
                var keyGameObjectForAction = fortification.Keys.ElementAt(Random.Range(0, fortification.Keys.Count));
                arenaManager.clickedFortification = keyGameObjectForAction.gameObject;
                break;
            case 1:
                //set params ballistic.
                dataActiveHero.Data.ballisticShoot = 1;
                dataActiveHero.Data.ballisticChanceHitKeep = 7;
                dataActiveHero.Data.ballisticChanceHitTower = 15;
                dataActiveHero.Data.ballisticChanceHitBridge = 30;
                dataActiveHero.Data.ballisticChanceHitIntendedWall = 60;
                dataActiveHero.Data.ballisticChanceNoDamage = 0;
                dataActiveHero.Data.ballisticChance1Damage = 50;
                dataActiveHero.Data.ballisticChance2Damage = 50;
                typeRunEffect = ArenaTypeRunEffect.Choosed;
                break;
            case 2:
                //set params ballistic.
                dataActiveHero.Data.ballisticShoot = 2;
                dataActiveHero.Data.ballisticChanceHitKeep = 7;
                dataActiveHero.Data.ballisticChanceHitTower = 15;
                dataActiveHero.Data.ballisticChanceHitBridge = 30;
                dataActiveHero.Data.ballisticChanceHitIntendedWall = 60;
                dataActiveHero.Data.ballisticChanceNoDamage = 0;
                dataActiveHero.Data.ballisticChance1Damage = 50;
                dataActiveHero.Data.ballisticChance2Damage = 50;
                typeRunEffect = ArenaTypeRunEffect.Choosed;
                break;
            case 3:
                //set params ballistic.
                dataActiveHero.Data.ballisticShoot = 2;
                dataActiveHero.Data.ballisticChanceHitKeep = 10;
                dataActiveHero.Data.ballisticChanceHitTower = 20;
                dataActiveHero.Data.ballisticChanceHitBridge = 40;
                dataActiveHero.Data.ballisticChanceHitIntendedWall = 75;
                dataActiveHero.Data.ballisticChanceNoDamage = 0;
                dataActiveHero.Data.ballisticChance1Damage = 0;
                dataActiveHero.Data.ballisticChance2Damage = 100;
                typeRunEffect = ArenaTypeRunEffect.Choosed;
                break;
        }

        await UniTask.Delay(1);
        return new ArenaResultChoose()
        {
            ChoosedNodes = null,
            TypeRunEffect = typeRunEffect
        };
    }

    public async override UniTask RunEffectByGameObject(ArenaManager arenaManager, GridArenaNode node, GameObject gameObject)
    {
        // Debug.Log($"{name}:::RunEffectByGameObject | Target={gameObject.name}");
        var ArenaWarMachineMonoBehavior = ((ArenaWarMachine)node.OccupiedUnit).ArenaWarMachineMonoBehavior;

        // Calculate probability and damage.
        var wallsObjects = arenaManager.ArenaTown.FortificationsGameObject.Where(t => t.Key.name.IndexOf("Wall") != -1 && t.Value > 0).ToList();

        var objectGorAttack = gameObject.transform;

        var dataActiveHero = arenaManager.ArenaQueue.ActiveHero.ArenaHeroEntity.Data;
        for (int i = 0; i < dataActiveHero.ballisticShoot; i++)
        {
            var valueHPobjectGorAttack = arenaManager.ArenaTown.FortificationsGameObject[gameObject.transform];
            int chance = Helpers.GenerateChance();
            var randomObject = wallsObjects.Count > 0
                ? wallsObjects[UnityEngine.Random.Range(0, wallsObjects.Count)].Key.gameObject.transform
                : null;
            if (randomObject == null)
            {
                var allowRandomObject = arenaManager.ArenaTown.FortificationsGameObject
                    .Where(t => t.Value != 0);
                if (allowRandomObject.Count() != 0)
                {
                    randomObject = allowRandomObject.First().Key;
                }
                else
                {
                    return;
                }
            }
            if (valueHPobjectGorAttack == 0)
            {
                objectGorAttack = randomObject;
            }

            // check double damage.
            int valueDamage = 0;
            if (chance < dataActiveHero.ballisticChance1Damage) valueDamage = 1;
            if (chance < dataActiveHero.ballisticChance2Damage) valueDamage = 2;

            // Check chance nodamage.
            if (chance < dataActiveHero.ballisticChanceNoDamage) valueDamage = 0;
            bool isHitShoot = false;

            if (gameObject.name.IndexOf("Wall") >= 0)
            {
                if (chance < dataActiveHero.ballisticChanceHitIntendedWall)
                {
                    await ArenaWarMachineMonoBehavior.RunAttackShoot(null, objectGorAttack);
                    await arenaManager.ArenaTown.AttackFortification(objectGorAttack, valueDamage);
                    isHitShoot = true;
                }
            }
            else if (gameObject.name.IndexOf("Tower") >= 0)
            {
                if (chance < dataActiveHero.ballisticChanceHitTower)
                {
                    await ArenaWarMachineMonoBehavior.RunAttackShoot(null, objectGorAttack);
                    await arenaManager.ArenaTown.AttackFortification(objectGorAttack, valueDamage);
                    isHitShoot = true;
                }
            }
            else if (gameObject.name.IndexOf("Bridge") >= 0)
            {
                if (chance < dataActiveHero.ballisticChanceHitBridge)
                {
                    await ArenaWarMachineMonoBehavior.RunAttackShoot(null, objectGorAttack);
                    await arenaManager.ArenaTown.AttackFortification(objectGorAttack, valueDamage);
                    isHitShoot = true;
                }
            }
            else if (gameObject.name.IndexOf("Keep") >= 0)
            {
                if (chance < dataActiveHero.ballisticChanceHitKeep)
                {
                    await ArenaWarMachineMonoBehavior.RunAttackShoot(null, objectGorAttack);
                    await arenaManager.ArenaTown.AttackFortification(objectGorAttack, valueDamage);
                    isHitShoot = true;
                }
            }

            if (randomObject != null && !isHitShoot)
            {
                // var transform = wallsObjects[UnityEngine.Random.Range(0, wallsObjects.Count)].Key.gameObject.transform;
                await ArenaWarMachineMonoBehavior.RunAttackShoot(null, randomObject);
                await arenaManager.ArenaTown.AttackFortification(randomObject, valueDamage);
                Debug.Log($"{name}:::RunEffectByGameObject randomObject| objectForAttack={randomObject.name} / damage={valueDamage}[chance={chance}");
            }
            else
            {
                Debug.Log($"{name}:::RunEffectByGameObject isHitShoot| objectForAttack={objectGorAttack.name} / damage={valueDamage}[chance={chance}");
            }
            arenaManager.ArenaTown.ArenaEntityTownMB.SetStatusColliders(false);
        }
        await UniTask.Delay(1);
    }
}

