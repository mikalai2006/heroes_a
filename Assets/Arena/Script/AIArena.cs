using System;
using System.Linq;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;

public class AIArena
{
    private ArenaManager _arenaManager;
    private ArenaHeroEntity _arenaHeroEntity;

    public void Init(ArenaManager arenaManager, ArenaHeroEntity arenaHeroEntity)
    {
        _arenaManager = arenaManager;
        _arenaHeroEntity = arenaHeroEntity;
    }

    public async UniTask Run()
    {
        // var allowFightingNodes = await _arenaManager.ArenaQueue.activeEntity.arenaEntity.GetFightingNodes();

        if (_arenaManager.FightingOccupiedNodes.Count > 0)
        {
            await DoAttack();
            return;
        }
        else if (_arenaManager.AllowPathNodes.Count > 0)
        {
            await DoMove();
            return;
        };
    }

    private async UniTask DoAttack()
    {
        var choosedForActionNode = _arenaManager.FightingOccupiedNodes[UnityEngine.Random.Range(0, _arenaManager.FightingOccupiedNodes.Count - 1)];

        await choosedForActionNode.OccupiedUnit.ClickCreature(choosedForActionNode.position);

        await _arenaManager.ArenaQueue.activeEntity.arenaEntity.ClickButtonAction();
    }

    private async UniTask DoMove()
    {
        var activeCreature = _arenaManager.ArenaQueue.activeEntity.arenaEntity;

        var allowNodes = _arenaManager.AllowPathNodes
            .Where(t => t != activeCreature.OccupiedNode && t != activeCreature.RelatedNode)
            .ToList();
        var nodeForMove = allowNodes[UnityEngine.Random.Range(0, allowNodes.Count)];

        _arenaManager.ClearAttackNode();
        await _arenaManager.DrawPath(nodeForMove);
        _arenaManager.DrawButtonAction();

        await _arenaManager.ArenaQueue.activeEntity.arenaEntity.ClickButtonAction();
    }

}
