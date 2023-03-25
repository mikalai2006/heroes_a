using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class BaseArtifact : BaseMapObject
{
    public Transform _model;

    protected override void Awake()
    {
        base.Awake();
        _model = transform.Find("Model");
    }

    public override void InitUnit(ScriptableUnitBase data, Vector3Int pos)
    {
        base.InitUnit(data, pos);

        ScriptableArtifact dataArtifact = (ScriptableArtifact)data;

        if (dataArtifact.sprite != null)
        {
            _model.GetComponent<SpriteRenderer>().sprite = dataArtifact.spriteMap;

        }

    }
}
