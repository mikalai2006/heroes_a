using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Cysharp.Threading.Tasks;

using UnityEngine;
using UnityEngine.Localization;

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

        var streightCreaturesHeroLeft = _arenaManager.heroLeft.Data.ArenaCreatures.Select(t => t.Value.totalAI).Sum() * _arenaManager.heroLeft.Entity.Streight;
        var streightCreaturesHeroRight = _arenaManager.heroRight.Data.ArenaCreatures.Select(t => t.Value.totalAI).Sum() * (_arenaManager.heroRight.Entity != null ? _arenaManager.heroRight.Entity.Streight : 1);

        Debug.Log($"streightCreaturesHero={streightCreaturesHeroLeft}, streightCreaturesEnemy={streightCreaturesHeroRight}");

        // Capitulation bot hero.
        if (
            streightCreaturesHeroLeft > streightCreaturesHeroRight
            && _arenaHeroEntity == _arenaManager.heroRight
            && _arenaManager.ArenaTown == null
            && _arenaHeroEntity.Entity != null
            )
        {
            _arenaHeroEntity.typearenaHeroStatus = TypearenaHeroStatus.Runned;
            _arenaManager.heroLeft.typearenaHeroStatus = TypearenaHeroStatus.Victorious;
            await _arenaManager.CalculateStat();
            return;
        }

        // Capitulation user hero.
        if (streightCreaturesHeroLeft < streightCreaturesHeroRight && _arenaHeroEntity == _arenaManager.heroLeft)
        {
            _arenaManager.DisableInputSystem();
            var dataSmart = new Dictionary<string, string> {
                    { "name", _arenaHeroEntity.Entity.Data.name },
                    { "gender", _arenaHeroEntity.Entity.ConfigData.TypeGender.ToString() }
                };
            var arguments = new[] { dataSmart };
            var descriptionSmart = Helpers.GetLocalizedPluralString(
                new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "hero_askrun"),
                arguments,
                dataSmart
                );

            var dialogData = new DataDialogHelp()
            {
                Header = new LocalizedString(Constants.LanguageTable.LANG_TABLE_UILANG, "Help").GetLocalizedString(),
                Description = descriptionSmart,
                showCancelButton = true,
            };

            var dialogWindow = new DialogHelpProvider(dialogData);
            var result = await dialogWindow.ShowAndHide();
            if (result.isOk)
            {
                _arenaManager.EnableInputSystem();
                _arenaHeroEntity.typearenaHeroStatus = TypearenaHeroStatus.Runned;
                _arenaManager.heroLeft.typearenaHeroStatus = TypearenaHeroStatus.Victorious;
                await _arenaManager.CalculateStat();
                return;
            }
        }


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
