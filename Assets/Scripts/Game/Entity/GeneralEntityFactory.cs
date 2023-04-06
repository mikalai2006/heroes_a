// using System;
// using System.Collections.Generic;
// using System.Linq;

// using Random = UnityEngine.Random;

// public class GeneralEntityFactory : BaseEntityFactory
// {
//     protected override ScriptableEntity GetEntityConfig(TypeEntity typeEntity)
//     {
//         switch (typeEntity)
//         {
//             case TypeEntity.Creature:
//                 List<ScriptableEntityCreature> list = ResourceSystem.Instance
//                     .GetEntityByType<ScriptableEntityCreature>(TypeEntity.Creature)
//                     .ToList();
//                 if (list.Count == 0) return null;
//                 return list[Random.Range(0, list.Count)];
//             case TypeEntity.Artifact:
//                 List<ScriptableEntityArtifact> listArtifact = ResourceSystem.Instance
//                     .GetEntityByType<ScriptableEntityArtifact>(TypeEntity.Artifact)
//                     .ToList();
//                 if (listArtifact.Count == 0) return null;
//                 return listArtifact[Random.Range(0, listArtifact.Count)];
//             case TypeEntity.Town:
//                 List<ScriptableEntityTown> listTown = ResourceSystem.Instance
//                     .GetEntityByType<ScriptableEntityTown>(TypeEntity.Artifact)
//                     .ToList();
//                 if (listTown.Count == 0) return null;
//                 return listTown[Random.Range(0, listTown.Count)];
//             case TypeEntity.Hero:
//                 List<ScriptableEntityHero> listHero = ResourceSystem.Instance
//                     .GetEntityByType<ScriptableEntityHero>(TypeEntity.Artifact)
//                     .ToList();
//                 if (listHero.Count == 0) return null;
//                 return listHero[Random.Range(0, listHero.Count)];
//             default:
//                 return null;
//         }
//     }
// }