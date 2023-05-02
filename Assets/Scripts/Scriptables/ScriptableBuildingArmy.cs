using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewBuildingArmy", menuName = "Game/Building/Army Building")]
public class ScriptableBuildingArmy : ScriptableBuilding
{
    [SerializeField] public List<ScriptableAttributeCreature> Creatures;
    [SerializeField] public ScriptableEntityDwelling Dwelling;
}
