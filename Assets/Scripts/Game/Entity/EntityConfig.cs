using System;

using UnityEngine;

[Serializable]
public class EntityConfig
{
    [NonSerialized] private BaseMapEntity _asset;
    [NonSerialized] public GridTileNode OccupiedNode = null;
    [NonSerialized] public GridTileNode ProtectedNode = null;
    [NonSerialized] public ScriptableEntity ScriptableData;
    [NonSerialized] public Vector3Int Position;
    [NonSerialized] public BaseMapEntity MapObjectGameObject;
    protected string idUnit;
    protected string idObject;
}