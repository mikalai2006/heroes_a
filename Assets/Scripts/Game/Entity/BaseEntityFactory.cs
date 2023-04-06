// // public interface IEntityFactory
// // {
// //     BaseEntity CreateEntity(ScriptableEntity data, GridTileNode node);
// // }
// public abstract class BaseEntityFactory
// {
//     public BaseEntity CreateEntity(TypeEntity typeEntity, GridTileNode node)
//     {
//         var config = GetEntityConfig(typeEntity);
//         switch (typeEntity)
//         {
//             case TypeEntity.Hero:
//                 return new EntityHero((ScriptableEntityHero)config, node);
//             case TypeEntity.Town:
//                 return new EntityTown((ScriptableEntityTown)config, node);
//             case TypeEntity.Artifact:
//                 return new EntityArtifact((ScriptableEntityArtifact)config, node);
//             case TypeEntity.Creature:
//                 return new EntityCreature((ScriptableEntityCreature)config, node);
//             case TypeEntity.Resource:
//                 return new EntityResource(config, node);
//             // case TypeEntity.MapObject:
//             //     return new EntityResource((ScriptableEntityResource)data, node);
//             default:
//                 return null;
//         }
//     }
//     protected abstract ScriptableEntity GetEntityConfig(TypeEntity typeEntity);
// }