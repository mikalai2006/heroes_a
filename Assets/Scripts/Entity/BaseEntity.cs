// using System;
// using System.Collections;
// using System.Collections.Generic;

// using UnityEngine;
// using UnityEngine.AddressableAssets;
// using UnityEngine.ResourceManagement.AsyncOperations;

// public class BaseCharacter
// {
//     [NonSerialized] private UnitBase _asset;
//     [NonSerialized] public GridTileNode OccupiedNode = null;
//     [NonSerialized] public GridTileNode ProtectedNode = null;
//     [NonSerialized] public TypeInput typeInput;
//     [NonSerialized] public TypeUnit typeUnit;
//     [NonSerialized] public Vector3Int Position;
//     [NonSerialized] public ScriptableUnitBase ScriptableData;
//     protected string idUnit;
//     protected string idObject;
//     protected string idWar;

//     public BaseCharacter(ScriptableUnitBase data, Vector3Int pos)
//     {
//         Debug.Log($"Spawn UNIT::: {data.TypeUnit}");
//         InitUnit(data, pos);
//     }

//     public async virtual void InitUnit(ScriptableUnitBase data, Vector3Int pos)
//     {
//         ScriptableData = data;
//         if (data.Prefab2.RuntimeKeyIsValid())
//         {
//             AsyncOperationHandle<GameObject> operationHandle = Addressables.InstantiateAsync(
//                 data.Prefab2,
//                 pos,
//                 Quaternion.identity,
//                 GameManager.Instance.MapManager.UnitManager._tileMapUnits.transform
//                 );
//             await operationHandle.Task;
//             if (operationHandle.Status == AsyncOperationStatus.Succeeded)
//             {
//                 // Addressables.Release(operationHandle);
//                 var r_asset = operationHandle.Result;
//                 _asset = r_asset.GetComponent<UnitBase>();
//                 _asset.InitUnit(data, pos);
//                 Debug.Log("Load prefab!");
//             }
//             else
//             {
//                 Debug.LogError($"Error Load prefab: {operationHandle.Status}");

//             }
//         }

//         typeUnit = data.TypeUnit;
//         typeInput = data.typeInput;
//         Position = pos;
//         idUnit = System.Guid.NewGuid().ToString("N");
//         idObject = data.idObject;
//     }

//     protected virtual void Awake()
//     {
//         GameManager.OnBeforeStateChanged += OnBeforeStateChanged;
//         GameManager.OnAfterStateChanged += OnAfterStateChanged;
//     }

//     protected virtual void OnDestroy()
//     {
//         if (_asset != null)
//         {
//             ScriptableData.Prefab2.ReleaseInstance(_asset.gameObject);
//         }
//         GameManager.OnBeforeStateChanged -= OnBeforeStateChanged;
//         GameManager.OnAfterStateChanged -= OnAfterStateChanged;
//     }

//     public virtual void OnBeforeStateChanged(GameState newState)
//     {
//         switch (newState)
//         {
//             case GameState.SaveGame:
//                 // OnSaveUnit();
//                 break;
//         }
//     }

//     public virtual void OnAfterStateChanged(GameState newState)
//     {
//     }

// }
